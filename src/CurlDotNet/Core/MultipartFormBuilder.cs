/***************************************************************************
 * MultipartFormBuilder - curl's -F / --form-string parsing and multipart body layout
 *
 * Parsing follows src/tool_formparse.c (name=value, name=@file[,file2], name=<file, quoted
 * words with \\ and \" escapes, ;type= (which absorbs following unknown parameters such as
 * ;charset=), ;filename= and ;headers= parameters). The body follows lib/mime.c: boundary of
 * 24 dashes plus 22 random alphanumerics, Content-Disposition with %22-escaped names (or
 * backslash escapes with --form-escape), no Content-Type on plain text parts, file parts typed
 * from the file extension with curl's table (default application/octet-stream), and several
 * files in one field sent as a nested multipart/mixed part.
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

            var name = field.Substring(0, eq);
            var contents = field.Substring(eq + 1);

            if (literal)
            {
                return new CurlFormPart { Name = name, Value = contents };
            }

            if (contents.StartsWith("@", StringComparison.Ordinal))
            {
                // one or more files separated by ',' each with its own ;type= / ;filename=
                var files = new List<CurlFormPart>();
                var pos = 1;
                while (true)
                {
                    var file = new CurlFormPart { Name = name, IsFileUpload = true };
                    file.FilePath = ParseParams(contents, ref pos, file, ',');
                    files.Add(file);
                    if (pos < contents.Length && contents[pos] == ',')
                    {
                        pos++;
                        continue;
                    }
                    break;
                }

                if (files.Count == 1)
                {
                    return files[0];
                }

                return new CurlFormPart
                {
                    Name = name,
                    IsFileUpload = true,
                    FilePath = files[0].FilePath,
                    MixedFiles = files,
                };
            }

            var part = new CurlFormPart { Name = name };
            var p = 0;
            var value = ParseParams(contents, ref p, part, '\0');
            // curl tests the unparsed value: "name=<file" reads a file, "name= <b>" is the text "<b>"
            if (contents.StartsWith("<", StringComparison.Ordinal))
            {
                part.FilePath = value.Length > 0 && value[0] == '<' ? value.Substring(1) : value;
                part.IsFileContent = true;
            }
            else
            {
                part.Value = value;
            }
            return part;
        }

        /// <summary>
        /// curl's get_param_part: the data word followed by ;type= ;filename= ;headers= parameters,
        /// ending at <paramref name="endChar"/> (',' between files) or the end of the argument.
        /// </summary>
        private static string ParseParams(string s, ref int pos, CurlFormPart part, char endChar)
        {
            SkipSpaces(s, ref pos);
            var data = ReadWord(s, ref pos, endChar);

            // While a type= is open (curl's "endct"), unknown parameters such as charset=x belong to it.
            var typeStart = -1;
            var typeEnd = -1;
            void CloseType()
            {
                if (typeStart >= 0)
                {
                    part.ContentType = s.Substring(typeStart, typeEnd - typeStart);
                    typeStart = -1;
                }
            }

            while (pos < s.Length && s[pos] == ';')
            {
                pos++;
                SkipSpaces(s, ref pos);
                if (typeStart < 0 && StartsWith(s, pos, "type="))
                {
                    pos += 5;
                    SkipSpaces(s, ref pos);
                    typeStart = pos;
                    while (pos < s.Length && "()<>@,;:\\\"[]?=\r\n ".IndexOf(s[pos]) < 0)
                    {
                        pos++;
                    }
                    typeEnd = pos;
                    while (pos < s.Length && s[pos] != ';' && s[pos] != endChar)
                    {
                        pos++;
                    }
                }
                else if (StartsWith(s, pos, "filename="))
                {
                    CloseType();
                    pos += 9;
                    SkipSpaces(s, ref pos);
                    part.FileName = ReadWord(s, ref pos, endChar);
                }
                else if (StartsWith(s, pos, "headers="))
                {
                    CloseType();
                    pos += 8;
                    SkipSpaces(s, ref pos);
                    var header = ReadWord(s, ref pos, endChar);
                    if (header.Length > 0)
                    {
                        part.Headers.Add(header);
                    }
                }
                else if (typeStart >= 0)
                {
                    // part of the content type, e.g. ";charset=utf-8"
                    while (pos < s.Length && s[pos] != ';' && s[pos] != endChar)
                    {
                        if (s[pos] != ' ' && s[pos] != '\t')
                        {
                            typeEnd = pos + 1;
                        }
                        pos++;
                    }
                }
                else
                {
                    ReadWord(s, ref pos, endChar); // unknown parameter (curl warns and skips it)
                }
            }
            CloseType();
            return data;
        }

        /// <summary>
        /// curl's get_param_word: a double-quoted word with \\ and \" escapes, or text up to ';' (or
        /// <paramref name="endChar"/>), with trailing blanks removed from unquoted words.
        /// </summary>
        private static string ReadWord(string s, ref int pos, char endChar)
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
                        while (i < s.Length && s[i] != ';' && s[i] != endChar)
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
            while (pos < s.Length && s[pos] != ';' && s[pos] != endChar)
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
        /// <remarks>
        /// Like lib/http.c and lib/mime.c, a <c>-H "Content-Type: ..."</c> given with <c>-F</c> becomes the type of
        /// the whole body, parameters included, with <c>; boundary=...</c> appended. The parts are then
        /// <c>form-data</c> only when that type is <c>multipart/form-data</c>; under any other type (such as
        /// <c>text/info</c>) curl labels named parts <c>attachment</c>. A part without a name (<c>-F =value</c>)
        /// gets no <c>name</c> parameter.
        /// </remarks>
        /// <exception cref="CurlReadErrorException">A referenced file cannot be read (exit code 26).</exception>
        public static HttpContent Build(IReadOnlyList<CurlFormPart> parts, CurlOptions options)
        {
            var boundary = NewBoundary();
            var content = new MultipartFormDataContent(boundary);
            var rootType = UserContentType(options) ?? "multipart/form-data";
            SetMultipartType(content, rootType, boundary);
            // mime_prepare_headers: subparts are "form-data" only under multipart/form-data
            var rootDisposition = ContentTypeMatch(rootType, "multipart/form-data") ? "form-data" : null;

            foreach (var part in parts)
            {
                HttpContent partContent;

                if (part.MixedFiles.Count > 1)
                {
                    // several files in one field: a nested multipart/mixed, each file an attachment
                    var innerBoundary = NewBoundary();
                    var mixed = new MultipartContent("mixed", innerBoundary);
                    SetMultipartType(mixed, "multipart/mixed", innerBoundary);
                    foreach (var file in part.MixedFiles)
                    {
                        mixed.Add(FilePart(null, file, options, withName: false)); // multipart/mixed is not form-data: "attachment"
                    }
                    var mixedDisposition = Disposition(rootDisposition, part.Name, null, "multipart/mixed", options);
                    if (mixedDisposition != null)
                    {
                        mixed.Headers.TryAddWithoutValidation("Content-Disposition", mixedDisposition);
                    }
                    partContent = mixed;
                }
                else if (part.IsFileUpload)
                {
                    partContent = FilePart(rootDisposition, part, options);
                }
                else
                {
                    var data = part.IsFileContent
                        ? ReadFile(part, options)
                        : Encoding.UTF8.GetBytes(part.Value ?? string.Empty); // command-line text is UTF-8, like -d
                    // plain text fields get no Content-Type unless ;type= asks for one, or ;filename= names a
                    // known type (-F 'name=a;filename=a.pdf' is application/pdf, like curl)
                    var type = !string.IsNullOrEmpty(part.ContentType)
                        ? part.ContentType
                        : (part.FileName != null && !part.IsFileContent ? KnownContentTypeFor(part.FileName) : null);
                    partContent = new ByteArrayContent(data);
                    var disposition = Disposition(rootDisposition, part.Name, part.FileName, type, options);
                    if (disposition != null)
                    {
                        partContent.Headers.TryAddWithoutValidation("Content-Disposition", disposition);
                    }
                    if (!string.IsNullOrEmpty(type))
                    {
                        partContent.Headers.TryAddWithoutValidation("Content-Type", type);
                    }
                    AddPartHeaders(partContent, part);
                }

                content.Add(partContent);
            }

            return content;
        }

        /// <summary>
        /// A file part: the <paramref name="disposition"/> (null means curl's default, <c>attachment</c>) with the
        /// name (when <paramref name="withName"/>) and filename parameters, typed from the extension.
        /// </summary>
        private static HttpContent FilePart(string? disposition, CurlFormPart file, CurlOptions options, bool withName = true)
        {
            var data = ReadFile(file, options);
            var fileName = file.FileName ?? Path.GetFileName(file.FilePath ?? string.Empty);
            var type = file.ContentType ?? ContentTypeFor(file.FilePath ?? fileName);
            // files inside a multipart/mixed field carry only the filename, like curl's subparts
            var name = withName ? file.Name : null;
            var partContent = new ByteArrayContent(data);
            var header = Disposition(disposition, name, fileName, type, options);
            if (header != null)
            {
                partContent.Headers.TryAddWithoutValidation("Content-Disposition", header);
            }
            partContent.Headers.TryAddWithoutValidation("Content-Type", type);
            AddPartHeaders(partContent, file);
            return partContent;
        }

        /// <summary>
        /// curl's add_content_disposition (lib/mime.c): without a disposition from the enclosing multipart, a part
        /// with a name, a filename or a non-multipart type is an <c>attachment</c>; an <c>attachment</c> with
        /// neither name nor filename gets no header at all (null). An empty name is no name (tool_formparse.c).
        /// </summary>
        private static string? Disposition(string? disposition, string? name, string? fileName, string? contentType, CurlOptions options)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = null;
            }
            if (disposition == null &&
                (fileName != null || name != null || (contentType != null && !contentType.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase))))
            {
                disposition = "attachment";
            }
            if (disposition != null && disposition.Equals("attachment", StringComparison.OrdinalIgnoreCase) && name == null && fileName == null)
            {
                disposition = null;
            }
            if (disposition == null)
            {
                return null;
            }
            var sb = new StringBuilder(disposition);
            if (name != null)
            {
                sb.Append("; name=\"").Append(Escape(name, options)).Append('"');
            }
            if (fileName != null)
            {
                sb.Append("; filename=\"").Append(Escape(fileName, options)).Append('"');
            }
            return sb.ToString();
        }

        /// <summary>
        /// curl's content_type_match: <paramref name="contentType"/> is <paramref name="target"/>, case-insensitively,
        /// followed by the end, a blank or a <c>;</c> parameter.
        /// </summary>
        private static bool ContentTypeMatch(string contentType, string target)
        {
            if (!contentType.StartsWith(target, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (contentType.Length == target.Length)
            {
                return true;
            }
            var next = contentType[target.Length];
            return next == ' ' || next == '\t' || next == '\r' || next == '\n' || next == ';';
        }

        /// <summary>
        /// The <c>-H "Content-Type: ..."</c> value to use as the type of the whole multipart body (lib/http.c skips
        /// the spaces after the colon and keeps the rest verbatim), or null when none was given.
        /// </summary>
        private static string? UserContentType(CurlOptions options)
        {
            foreach (var header in options.Headers)
            {
                if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(header.Value))
                {
                    var value = header.Value.TrimStart(' ');
                    return value.Length > 0 ? value : null;
                }
            }
            return null;
        }

        private static void AddPartHeaders(HttpContent content, CurlFormPart part)
        {
            foreach (var header in part.Headers)
            {
                var colon = header.IndexOf(':');
                if (colon > 0)
                {
                    content.Headers.TryAddWithoutValidation(header.Substring(0, colon).Trim(), header.Substring(colon + 1).Trim());
                }
            }
        }

        private static void SetMultipartType(HttpContent content, string mediaType, string boundary)
        {
            // HttpClient quotes the boundary; curl does not
            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", mediaType + "; boundary=" + boundary);
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
                var (reason, fix) = RequestShaping.DescribeReadFailure(ex);
                throw new CurlReadErrorException(path,
                    CurlException.Compose(
                        $"Failed to open/read local data from file/application: {how} names a file that cannot be read: {reason}.",
                        fix + " To send a value that starts with @ or < literally, use --form-string instead of -F.",
                        DocLinks.HowToUpload),
                    options.OriginalCommand, ex);
            }
        }

        /// <summary>
        /// curl's escaping of form names and file names: by default " CR LF become %22 %0D %0A;
        /// with --form-escape a backslash and a double quote get a backslash in front instead.
        /// </summary>
        private static string Escape(string value, CurlOptions options)
        {
            if (options.FormEscape)
            {
                return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
            }
            return value.Replace("\"", "%22").Replace("\r", "%0D").Replace("\n", "%0A");
        }

        /// <summary>curl's Curl_mime_contenttype table, defaulting to application/octet-stream (file uploads).</summary>
        internal static string ContentTypeFor(string fileName) => KnownContentTypeFor(fileName) ?? "application/octet-stream";

        /// <summary>curl's Curl_mime_contenttype table: the type for a known extension, or null.</summary>
        internal static string? KnownContentTypeFor(string fileName)
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
                default: return null;
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
