/***************************************************************************
 * FileHandler - file:// protocol handler
 *
 * Based on curl's lib/file.c by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Handler for file:// protocol.
    /// </summary>
    internal class FileHandler : IProtocolHandler
    {
        public async Task<CurlResult> ExecuteAsync(CurlOptions options, CancellationToken cancellationToken)
        {
            var started = Stopwatch.StartNew();
            var uri = new Uri(options.Url);
            var filePath = Uri.UnescapeDataString(uri.LocalPath);

            // Check if file exists
            if (!File.Exists(filePath))
            {
                var missing = new CurlResult
                {
                    StatusCode = 404,
                    Body = $"File not found: {filePath}",
                    Command = options.OriginalCommand
                };
                // curl fails this transfer with exit 37 (CURLE_FILE_COULDNT_READ_FILE); -w still runs and says so
                AppendWriteOut(missing, options, BuildWriteOutContext(options, 0, null, started, 37, $"Couldn't open file {filePath}"));
                return missing;
            }

            try
            {
                var result = new CurlResult
                {
                    StatusCode = 200,
                    Command = options.OriginalCommand
                };

                var fileInfo = new FileInfo(filePath);
                result.Headers["Content-Length"] = fileInfo.Length.ToString();
                result.Headers["Last-Modified"] = fileInfo.LastWriteTimeUtc.ToString("R");

                string? textContent = null;
                byte[]? binaryContent = null;

                // Determine if binary or text
                if (IsBinaryFile(filePath))
                {
#if NETSTANDARD2_0 || NET472 || NET48
                    binaryContent = await Task.Run(() => File.ReadAllBytes(filePath), cancellationToken).ConfigureAwait(false);
#else
                    binaryContent = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
#endif
                    result.BinaryData = binaryContent;
                }
                else
                {
#if NETSTANDARD2_0 || NET472 || NET48
                    textContent = await Task.Run(() => File.ReadAllText(filePath), cancellationToken).ConfigureAwait(false);
#else
                    textContent = await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
#endif
                    result.Body = textContent;
                }

                // Handle output file (-o)
                if (!string.IsNullOrEmpty(options.OutputFile))
                {
                    await WriteOutputAsync(options.OutputFile, textContent, binaryContent, options, "-o/--output", cancellationToken).ConfigureAwait(false);
                    result.OutputFiles.Add(options.OutputFile);
                }
                else if (options.UseRemoteFileName)
                {
                    var remoteName = Path.GetFileName(filePath);
                    if (string.IsNullOrWhiteSpace(remoteName))
                    {
                        remoteName = "curl-download";
                    }
                    var destination = Path.Combine(Directory.GetCurrentDirectory(), remoteName);
                    await WriteOutputAsync(destination, textContent, binaryContent, options, "-O/--remote-name", cancellationToken).ConfigureAwait(false);
                    result.OutputFiles.Add(destination);
                }

                var size = binaryContent?.LongLength ?? (textContent == null ? 0 : fileInfo.Length);
                var written = result.OutputFiles.Count > 0 ? result.OutputFiles[result.OutputFiles.Count - 1] : null;
                if (written != null)
                {
                    result.Body = string.Empty; // like the HTTP handler: -o/-O leave Body empty, -w text still lands there
                }
                AppendWriteOut(result, options, BuildWriteOutContext(options, size, written, started, 0, string.Empty));
                return result;
            }
            catch (UnauthorizedAccessException)
            {
                // the message of this exception type names the file and says to check that it is readable
                throw new CurlFileCouldntReadException(filePath, options.OriginalCommand);
            }
            catch (IOException ex)
            {
                throw new CurlReadErrorException(filePath, CurlException.Compose(
                    $"Couldn't read the file '{filePath}' named by {ErrorText.SafeUrl(options.Url)} ({ex.GetType().Name}: {ex.Message})",
                    "Check that no other program holds the file locked and that the disk or network share is still available, then retry.",
                    DocLinks.ErrorReference), options.OriginalCommand, ex);
            }
        }

        /// <summary>
        /// The <c>-w</c>/<c>--write-out</c> values for a file:// transfer, as curl reports them (lib/file.c):
        /// no response code (<c>%{response_code}</c> and <c>%{http_code}</c> print 000), no connection, no
        /// headers, scheme <c>file</c>, and the file's byte count as <c>%{size_download}</c>.
        /// </summary>
        private static WriteOutContext BuildWriteOutContext(CurlOptions options, long size, string? writtenPath, Stopwatch started,
            int exitCode, string errorMessage)
        {
            var seconds = started.Elapsed.TotalSeconds;
            return new WriteOutContext
            {
                ResponseCode = 0,
                SizeDownload = size,
                SizeDelivered = size,
                Url = options.Url,
                UrlEffective = options.Url,
                Method = "GET",
                Scheme = "file",
                UrlNum = options.TransferNumber,
                NumRetries = options.RetryNumber,
                FilenameEffective = writtenPath ?? string.Empty,
                ExitCode = exitCode,
                ErrorMessage = errorMessage,
                TimePreTransfer = seconds,
                TimeStartTransfer = seconds,
                TimeTotal = seconds,
            };
        }

        /// <summary>
        /// Expands <c>-w</c> for this transfer: standard-output text is appended to <see cref="CurlResult.Body"/>
        /// (what curl would print after the file's content), standard-error text goes to <c>--stderr FILE</c> or
        /// the console.
        /// </summary>
        private static void AppendWriteOut(CurlResult result, CurlOptions options, WriteOutContext context)
        {
            if (string.IsNullOrEmpty(options.WriteOut))
            {
                return;
            }

            var expanded = WriteOutFormatter.Format(options.WriteOut!, context);
            result.Body = (result.Body ?? string.Empty) + expanded.Stdout;
            if (string.IsNullOrEmpty(expanded.Stderr))
            {
                return;
            }
            if (!string.IsNullOrEmpty(options.StderrFile) && options.StderrFile != "-")
            {
                try
                {
                    File.AppendAllText(options.StderrFile!, expanded.Stderr);
                    return;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    // curl falls back to the terminal when --stderr cannot be opened
                }
            }
            Console.Error.Write(expanded.Stderr);
        }

        public bool SupportsProtocol(string protocol)
        {
            return protocol == "file";
        }

        private bool IsBinaryFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            var textExtensions = new[] { ".txt", ".json", ".xml", ".html", ".htm", ".css", ".js",
                ".csv", ".log", ".md", ".yml", ".yaml", ".ini", ".cfg", ".conf" };

            return !Array.Exists(textExtensions, ext => ext == extension);
        }

        /// <summary>
        /// Writes the file:// content to the output file. A failure is a write error (exit 23) that names
        /// the output path, never a read error of the source file.
        /// </summary>
        private static async Task WriteOutputAsync(string destination, string? textContent, byte[]? binaryContent, CurlOptions options, string what,
            CancellationToken cancellationToken)
        {
            try
            {
                await WriteOutputCoreAsync(destination, textContent, binaryContent, cancellationToken).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CurlWriteErrorException(destination, CurlException.Compose(
                    $"Failed writing '{destination}' given with {what}: access denied ({ex.Message})",
                    "Write somewhere this process may write (for example Path.GetTempPath()), or clear the file's read-only flag.",
                    DocLinks.HowToDownload), options.OriginalCommand, ex);
            }
            catch (IOException ex)
            {
                throw new CurlWriteErrorException(destination, CurlException.Compose(
                    $"Failed writing '{destination}' given with {what}: {ex.Message}",
                    "Check that the disk is not full, that no other program holds the file open, and that the path names a file rather than a folder.",
                    DocLinks.HowToDownload), options.OriginalCommand, ex);
            }
        }

        private static async Task WriteOutputCoreAsync(string destination, string? textContent, byte[]? binaryContent, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(destination);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (binaryContent != null)
            {
#if NETSTANDARD2_0 || NET472 || NET48
                await Task.Run(() => File.WriteAllBytes(destination, binaryContent), cancellationToken).ConfigureAwait(false);
#else
                await File.WriteAllBytesAsync(destination, binaryContent, cancellationToken).ConfigureAwait(false);
#endif
            }
            else
            {
                var content = textContent ?? string.Empty;
#if NETSTANDARD2_0 || NET472 || NET48
                await Task.Run(() => File.WriteAllText(destination, content), cancellationToken).ConfigureAwait(false);
#else
                await File.WriteAllTextAsync(destination, content, cancellationToken).ConfigureAwait(false);
#endif
            }
        }
    }
}