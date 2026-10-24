/***************************************************************************
 * MultipartFormBuilder - curl's -F / --form-string parsing and multipart body layout
 *
 * Parsing follows src/tool_formparse.c (name=value, name=@file, name=<file, quoted words with
 * \\ and \" escapes, ;type= ;filename= ;headers= parameters). The body follows lib/mime.c:
 * boundary of 24 dashes plus 22 random alphanumerics, Content-Disposition with %22-escaped
 * names, no Content-Type on plain text parts, file parts typed from the file extension with
 * curl's table (default application/octet-stream).
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    internal static class MultipartFormBuilder
    {
        private const string BoundaryDashes = "------------------------";
        private const string BoundaryAlphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Parses one <c>-F</c> argument (or <c>--form-string</c> when <paramref name="literal"/>) into a part.
        /// Returns null when the argument has no <c>=</c>.
        /// </summary>
        public static CurlFormPart? Parse(string field, bool literal)
        {
            var eq = field.IndexOf('=');
            if (eq < 0)
            {
                return null;
            }

            var part = new CurlFormPart { Name = field.Substring(0, eq) };
            var contents = field.Substring(eq + 1);

            if (literal)
            {
                part.Value = contents;
                return part;
            }

            if (contents.StartsWith("@", StringComparison.Ordinal))
            {
                var pos = 1;
                var data = ParseParams(contents, ref pos, part, allowFilename: true);
                part.FilePath = data;
                part.IsFileUpload = true;
                return part;
            }

            var p = 0;
            var value = ParseParams(contents, ref p, part, allowFilename: false);
            if (value.StartsWith("<", StringComparison.Ordinal))
            {
                part.FilePath = value.Substring(1);
                part.IsFileContent = true;
            }
            else
            {
                part.Value = value;
            }
            return part;
        }

        /// <summary>curl's get_param_part: the data word followed by ;type= ;filename= ;headers= parameters.</summary>
        private static string ParseParams(string s, ref int pos, CurlFormPart part, bool allowFilename)
        {
            SkipSpaces(s, ref pos);
            var data = ReadWord(s, ref pos);

            while (pos < s.Length && s[pos] == ';')
            {
                pos++;
                SkipSpaces(s, ref pos);
                if (StartsWith(s, pos, "type="))
                {
                    pos += 5;
                    SkipSpaces(s, ref pos);
                    var start = pos;
                    while (pos < s.Length && "()<>@,;:\\\"[]?=\r\n ".IndexOf(s[pos]) < 0)
                    {
                        pos++;
                    }
                    part.ContentType = s.Substring(start, pos - start);
                    while (pos < s.Length && s[pos] != ';')
                    {
                        pos++;
                    }
                }
                else if (allowFilename && StartsWith(s, pos, "filename="))
                {
                    pos += 9;
                    SkipSpaces(s, ref pos);
                    part.FileName = ReadWord(s, ref pos);
                }
                else if (StartsWith(s, pos, "headers="))
                {
                    pos += 8;
                    SkipSpaces(s, ref pos);
                    var header = ReadWord(s, ref pos);
                    if (header.Length > 0)
                    {
                        part.Headers.Add(header);
                    }
                }
                else
                {
                    ReadWord(s, ref pos); // unknown parameter (curl warns and skips it)
                }
            }
            return data;
        }

        /// <summary>curl's get_param_word: a double-quoted word with \\ and \" escapes, or text up to ';'.</summary>
        private static string ReadWord(string s, ref int pos)
        {
            if (pos < s.Length && s[pos] == '"')
            {
                var sb = new StringBuilder();
                var i = pos + 1;
                while (i < s.Length)
                {
                    var c = s[i];
                    if (c == '\\' && i + 1 < s.Length && (s[i + 1] == '\\' || s[i + 1] == '"'))
                    {
                        sb.Append(s[i + 1]);
                        i += 2;
                        continue;
                    }
                    if (c == '"')
                    {
                        i++;
                        while (i < s.Length && s[i] != ';')
                        {
                            i++; // trailing data after the closing quote is ignored
                        }
                        pos = i;
                        return sb.ToString();
                    }
                    sb.Append(c);
                    i++;
                }
                // no closing quote: treat it as an unquoted word
            }

            var start = pos;
            while (pos < s.Length && s[pos] != ';')
            {
                pos++;
            }
            return s.Substring(start, pos - start).TrimEnd(' ', '\t');
        }

        private static void SkipSpaces(string s, ref int pos)
        {
            while (pos < s.Length && (s[pos] == ' ' || s[pos] == '\t'))
            {
                pos++;
            }
        }

        private static bool StartsWith(string s, int pos, string prefix) =>
            string.Compare(s, pos, prefix, 0, prefix.Length, StringComparison.OrdinalIgnoreCase) == 0;

        /// <summary>Parts for options filled through the API (FormData/Files) rather than -F.</summary>
        public static List<CurlFormPart> PartsFor(CurlOptions options)
        {
            if (options.FormParts.Count > 0)
            {
                return options.FormParts;
            }

            var parts = new List<CurlFormPart>();
            foreach (var field in options.FormData)
            {
                parts.Add(new CurlFormPart { Name = field.Key, Value = field.Value });
            }
            foreach (var file in options.Files)
            {
                parts.Add(new CurlFormPart { Name = file.Key, FilePath = file.Value, IsFileUpload = true });
            }
            return parts;
        }

        /// <summary>
        /// Builds the multipart/form-data body exactly as curl lays it out: a
        /// <see cref="MultipartFormDataContent"/> whose parts carry curl's raw part headers and whose
        /// Content-Type names the boundary unquoted.
        /// </summary>
        /// <exception cref="CurlReadErrorException">A referenced file cannot be read (exit code 26).</exception>
        public static HttpContent Build(IReadOnlyList<CurlFormPart> parts, CurlOptions options)
        {
            var boundary = NewBoundary();
            var content = new MultipartFormDataContent(boundary);
            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);

            foreach (var part in parts)
            {
                var disposition = new StringBuilder("form-data; name=\"").Append(Escape(part.Name)).Append('"');
                byte[] data;
                string? contentType = part.ContentType;

                if (part.IsFileUpload || part.IsFileContent)
                {
                    data = ReadFile(part, options);
                    if (part.IsFileUpload)
                    {
                        var fileName = part.FileName ?? Path.GetFileName(part.FilePath ?? string.Empty);
                        disposition.Append("; filename=\"").Append(Escape(fileName)).Append('"');
                        contentType ??= ContentTypeFor(part.FilePath ?? fileName);
                    }
                }
                else
                {
                    // command-line text is sent as UTF-8, like the -d data
                    data = Encoding.UTF8.GetBytes(part.Value ?? string.Empty);
                }

                var partContent = new ByteArrayContent(data);
                partContent.Headers.TryAddWithoutValidation("Content-Disposition", disposition.ToString());
                if (!string.IsNullOrEmpty(contentType))
                {
                    // plain text fields get no Content-Type at all, exactly like curl
                    partContent.Headers.TryAddWithoutValidation("Content-Type", contentType);
                }
                foreach (var header in part.Headers)
                {
                    var colon = header.IndexOf(':');
                    if (colon > 0)
                    {
                        partContent.Headers.TryAddWithoutValidation(header.Substring(0, colon).Trim(), header.Substring(colon + 1).Trim());
                    }
                }
                content.Add(partContent);
            }

            return content;
        }

        private static byte[] ReadFile(CurlFormPart part, CurlOptions options)
        {
            var path = part.FilePath ?? string.Empty;
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException || ex is NotSupportedException)
            {
                var how = part.IsFileUpload ? $"-F \"{part.Name}=@{path}\"" : $"-F \"{part.Name}=<{path}\"";
                throw new CurlReadErrorException(path,
                    $"Failed to open/read local data from file/application: {how} names a file that cannot be read ({ex.Message}). " +
                    "Check the path (it is relative to the current directory) and its permissions; use --form-string to send a value that starts with @ or < literally." +
                    "\nFor file errors, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md",
                    options.OriginalCommand);
            }
        }

        /// <summary>curl's form-data escaping of names and file names: " CR LF become %22 %0D %0A.</summary>
        private static string Escape(string value) =>
            value.Replace("\"", "%22").Replace("\r", "%0D").Replace("\n", "%0A");

        /// <summary>curl's Curl_mime_contenttype table.</summary>
        internal static string ContentTypeFor(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            switch (ext)
            {
                case ".gif": return "image/gif";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".svg": return "image/svg+xml";
                case ".txt": return "text/plain";
                case ".htm":
                case ".html": return "text/html";
                case ".pdf": return "application/pdf";
                case ".xml": return "application/xml";
                default: return "application/octet-stream";
            }
        }

        private static string NewBoundary()
        {
            var bytes = new byte[22];
#if NETSTANDARD2_0 || NET472 || NET48
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
#else
            RandomNumberGenerator.Fill(bytes);
#endif
            var sb = new StringBuilder(BoundaryDashes, BoundaryDashes.Length + 22);
            foreach (var b in bytes)
            {
                sb.Append(BoundaryAlphabet[b % BoundaryAlphabet.Length]);
            }
            return sb.ToString();
        }
    }
}
