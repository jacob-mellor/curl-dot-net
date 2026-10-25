/***************************************************************************
 * TransferOutput - where curl writes a transfer: -o, -O, -J, --output-dir, --create-dirs,
 * --no-clobber, -R, -D and --remove-on-error (src/tool_cb_wrt.c, tool_operate.c, tool_cb_hdr.c)
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    internal static class TransferOutput
    {
        /// <summary>The page that explains the option a file belongs to (-o/-O downloads, -c cookies, everything else).</summary>
        private static string DocFor(string what) =>
            what.StartsWith("-o", StringComparison.Ordinal) || what.StartsWith("-O", StringComparison.Ordinal) ? DocLinks.HowToDownload
            : what.StartsWith("-c", StringComparison.Ordinal) ? DocLinks.CookbookCookies
            : DocLinks.ErrorReference;

        /// <summary>
        /// The file the body goes to, or null for standard output (no -o/-O, or -o -).
        /// </summary>
        public static string? ResolveOutputPath(CurlOptions options, HttpTransfer transfer, RawResponseHead? finalHead, string effectiveUrl)
        {
            string? name;
            if (!string.IsNullOrEmpty(options.OutputFile))
            {
                if (options.OutputFile == "-")
                {
                    return null;
                }
                name = options.OutputFile!;
            }
            else if (options.UseRemoteFileName)
            {
                string? fromHeader = null;
                if (options.RemoteHeaderName && finalHead != null)
                {
                    fromHeader = ContentDispositionFileName(finalHead.GetHeader("Content-Disposition"));
                }
                name = fromHeader ?? RemoteFileName(options.RemoteHeaderName ? effectiveUrl : transfer.OriginalUrl);
            }
            else
            {
                return null;
            }

            if (!string.IsNullOrEmpty(options.OutputDir) && !Path.IsPathRooted(name))
            {
                name = Path.Combine(options.OutputDir!, name);
            }
            return name;
        }

        /// <summary>
        /// The file name curl's -O picks from a URL: the last non-empty path segment, or
        /// <c>curl_response</c> when the path has none.
        /// </summary>
        internal static string RemoteFileName(string url)
        {
            string path;
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                path = uri.AbsolutePath;
            }
            else
            {
                path = url;
                var q = path.IndexOfAny(new[] { '?', '#' });
                if (q >= 0) path = path.Substring(0, q);
            }

            var segment = path.Split('/').LastOrDefault(s => s.Length > 0);
            return string.IsNullOrEmpty(segment) ? "curl_response" : segment!;
        }

        /// <summary>
        /// curl's parse_filename for -J: the <c>filename=</c> parameter, unquoted, without any
        /// directory part. Returns null when there is no usable name.
        /// </summary>
        internal static string? ContentDispositionFileName(string? header)
        {
            if (string.IsNullOrEmpty(header))
            {
                return null;
            }

            var s = header!;
            var p = 0;
            while (p < s.Length)
            {
                while (p < s.Length && !char.IsLetter(s[p]))
                {
                    p++;
                }
                if (string.CompareOrdinal(s, p, "filename=", 0, 9) != 0)
                {
                    while (p < s.Length && s[p] != ';') p++;
                    continue;
                }

                var value = s.Substring(p + 9);
                char stop = ';';
                if (value.Length > 0 && (value[0] == '"' || value[0] == '\''))
                {
                    stop = value[0];
                    value = value.Substring(1);
                }
                var end = value.IndexOf(stop);
                if (end >= 0) value = value.Substring(0, end);

                var slash = value.LastIndexOf('/');
                if (slash >= 0) value = value.Substring(slash + 1);
                var backslash = value.LastIndexOf('\\');
                if (backslash >= 0) value = value.Substring(backslash + 1);
                value = value.TrimEnd('\r', '\n');
                return value.Length == 0 ? null : value;
            }
            return null;
        }

        /// <summary>
        /// Applies --no-clobber, the -J overwrite guard and --create-dirs, returning the path to
        /// write to.
        /// </summary>
        public static string PrepareTarget(string path, CurlOptions options, bool fromHeaderName)
        {
            EnsureDirectory(path, options, "-o/--output");

            if (options.NoClobber && File.Exists(path))
            {
                // curl keeps the extension: report.txt -> report-1.txt; without one: save -> save.1
                var fileName = Path.GetFileName(path);
                var dot = fileName.LastIndexOf('.');
                var stem = dot > 0 ? path.Substring(0, path.Length - (fileName.Length - dot)) : path;
                var extension = dot > 0 ? fileName.Substring(dot) : string.Empty;
                for (var n = 1; n <= 100; n++)
                {
                    var number = n.ToString(CultureInfo.InvariantCulture);
                    var candidate = dot > 0 ? stem + "-" + number + extension : path + "." + number;
                    if (!File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
                throw new CurlWriteErrorException(path, CurlException.Compose(
                    $"--no-clobber found '{path}' and all 100 numbered alternatives (such as '{(dot > 0 ? stem + "-1" + extension : path + ".1")}') already taken",
                    "Delete or move the old downloads, or choose another name with -o FILE (or another folder with --output-dir).",
                    DocLinks.HowToDownload), options.OriginalCommand);
            }

            if (fromHeaderName && File.Exists(path))
            {
                throw new CurlWriteErrorException(path, CurlException.Compose(
                    $"Refusing to overwrite '{path}': with -O -J/--remote-header-name the file name comes from the response (Content-Disposition, else the URL) and curl never replaces an existing file that way",
                    "Delete or move the existing file, or choose the name yourself with -o FILE.",
                    DocLinks.HowToDownload), options.OriginalCommand);
            }

            return path;
        }

        /// <summary>Creates the parent directory with --create-dirs, otherwise fails with exit code 23.</summary>
        public static void EnsureDirectory(string path, CurlOptions options, string what)
        {
            string? dir;
            try
            {
                dir = Path.GetDirectoryName(Path.GetFullPath(path));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                throw new CurlWriteErrorException(path, CurlException.Compose(
                    $"'{path}' given with {what} is not a valid file name ({ex.GetType().Name}: {ex.Message})",
                    "Use a path without characters the file system rejects (such as : * ? \" < > | on Windows) and within the length limit; quote paths that contain spaces.",
                    DocFor(what)), options.OriginalCommand, ex);
            }

            if (string.IsNullOrEmpty(dir) || Directory.Exists(dir))
            {
                return;
            }

            if (options.CreateDirs)
            {
                try
                {
                    Directory.CreateDirectory(dir);
                    return;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    throw new CurlWriteErrorException(path, CurlException.Compose(
                        $"--create-dirs could not create the folder '{dir}' for '{path}' given with {what} ({ex.GetType().Name}: {ex.Message})",
                        "Check that this process may create folders there (permissions, read-only drive) and that no file has the same name as a folder in the path.",
                        DocFor(what)), options.OriginalCommand, ex);
                }
            }

            throw new CurlWriteErrorException(path, CurlException.Compose(
                $"Failed to open '{path}' given with {what}: the directory '{dir}' does not exist",
                "Create the directory first, or add --create-dirs so curl creates it.",
                DocFor(what)), options.OriginalCommand);
        }

        /// <summary>Writes bytes to a file, turning I/O failures into exit code 23.</summary>
        public static void WriteFile(string path, byte[] data, bool append, CurlOptions options, string what)
        {
            try
            {
                if (append)
                {
                    using var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
                    stream.Write(data, 0, data.Length);
                }
                else
                {
                    File.WriteAllBytes(path, data);
                }
            }
            catch (DirectoryNotFoundException)
            {
                throw new CurlWriteErrorException(path, CurlException.Compose(
                    $"Failed to open '{path}' given with {what}: its directory does not exist (it was removed while the transfer ran, or the path is on a drive that is gone)",
                    "Create the directory first, or add --create-dirs so curl creates it.",
                    DocFor(what)), options.OriginalCommand);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CurlWriteErrorException(path, CurlException.Compose(
                    $"Failed writing '{path}' given with {what}: access denied ({ex.Message})",
                    "Write somewhere this process may write (for example Path.GetTempPath()), clear the file's read-only flag, or run with the needed permissions; " +
                    "on Windows a file open in another program cannot be replaced either.",
                    DocFor(what)), options.OriginalCommand, ex);
            }
            catch (IOException ex)
            {
                throw new CurlWriteErrorException(path, CurlException.Compose(
                    $"Failed writing '{path}' given with {what}: {ex.Message}",
                    "Check that the disk is not full, that no other program holds the file open, and that the path points at a file rather than a folder.",
                    DocFor(what)), options.OriginalCommand, ex);
            }
        }

        /// <summary>-R/--remote-time: stamps the file with the server's Last-Modified time.</summary>
        public static void ApplyRemoteTime(string path, string? lastModified)
        {
            if (string.IsNullOrEmpty(lastModified))
            {
                return;
            }

            var seconds = CurlDate.Parse(lastModified!);
            if (!seconds.HasValue)
            {
                return;
            }

            try
            {
                var time = DateTimeOffset.FromUnixTimeSeconds(seconds.Value).UtcDateTime;
                File.SetLastWriteTimeUtc(path, time);
                File.SetLastAccessTimeUtc(path, time);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentOutOfRangeException)
            {
                // curl only warns when the time cannot be set
            }
        }

        /// <summary>
        /// -D/--dump-header FILE: appends the header blocks (the file is created on first use). <c>-D %</c> sends
        /// them to standard error instead (tool_operate.c's setup_headerfile), which is <c>--stderr FILE</c> when
        /// one is given.
        /// </summary>
        public static void DumpHeaders(CurlOptions options, string headerText)
        {
            var path = options.DumpHeaderFile!;
            if (path == DumpHeadersToStderr)
            {
                WriteStderr(options, headerText);
                return;
            }
            EnsureDirectory(path, options, "-D/--dump-header");
            // the first transfer of a --next group creates the file; later transfers of the group append,
            // so "-D heads url1 url2" keeps both header blocks, while a -D in the next group starts over
            var key = options.GroupNumber.ToString(CultureInfo.InvariantCulture) + "|" + Path.GetFullPath(path);
            var append = options.Session != null && !options.Session.DumpHeaderFiles.Add(key);
            WriteFile(path, WireText.Latin1.GetBytes(headerText), append, options, "-D/--dump-header");
        }

        /// <summary>The <c>-D</c> argument curl reads as "standard error".</summary>
        internal const string DumpHeadersToStderr = "%";

        /// <summary>
        /// Writes tool output meant for standard error: appended to <c>--stderr FILE</c> when one is set (falling
        /// back to the console when it cannot be opened, like curl), otherwise <see cref="Console.Error"/>.
        /// </summary>
        internal static void WriteStderr(CurlOptions options, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (!string.IsNullOrEmpty(options.StderrFile) && options.StderrFile != "-")
            {
                try
                {
                    using var file = new FileStream(options.StderrFile!, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    var bytes = WireText.Latin1.GetBytes(text);
                    file.Write(bytes, 0, bytes.Length);
                    return;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    // curl falls back to the terminal when --stderr cannot be opened
                }
            }
            Console.Error.Write(text);
        }
    }
}
