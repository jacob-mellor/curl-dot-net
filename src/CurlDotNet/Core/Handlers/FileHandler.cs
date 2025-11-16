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
            var uri = new Uri(options.Url);
            var filePath = Uri.UnescapeDataString(uri.LocalPath);

            // Check if file exists
            if (!File.Exists(filePath))
            {
                return new CurlResult
                {
                    StatusCode = 404,
                    Body = $"File not found: {filePath}",
                    Command = options.OriginalCommand
                };
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
#if NETSTANDARD2_0
                    binaryContent = await Task.Run(() => File.ReadAllBytes(filePath), cancellationToken);
#else
                    binaryContent = await File.ReadAllBytesAsync(filePath, cancellationToken);
#endif
                    result.BinaryData = binaryContent;
                }
                else
                {
#if NETSTANDARD2_0
                    textContent = await Task.Run(() => File.ReadAllText(filePath), cancellationToken);
#else
                    textContent = await File.ReadAllTextAsync(filePath, cancellationToken);
#endif
                    result.Body = textContent;
                }

                // Handle output file (-o)
                if (!string.IsNullOrEmpty(options.OutputFile))
                {
                    await WriteOutputAsync(options.OutputFile, textContent, binaryContent, cancellationToken);
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
                    await WriteOutputAsync(destination, textContent, binaryContent, cancellationToken);
                    result.OutputFiles.Add(destination);
                }

                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CurlFileCouldntReadException($"Permission denied: {filePath}");
            }
            catch (IOException ex)
            {
                throw new CurlReadErrorException(filePath, ex.Message);
            }
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

        private static async Task WriteOutputAsync(string destination, string? textContent, byte[]? binaryContent, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(destination);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (binaryContent != null)
            {
#if NETSTANDARD2_0
                await Task.Run(() => File.WriteAllBytes(destination, binaryContent), cancellationToken);
#else
                await File.WriteAllBytesAsync(destination, binaryContent, cancellationToken);
#endif
            }
            else
            {
                var content = textContent ?? string.Empty;
#if NETSTANDARD2_0
                await Task.Run(() => File.WriteAllText(destination, content), cancellationToken);
#else
                await File.WriteAllTextAsync(destination, content, cancellationToken);
#endif
            }
        }
    }
}