/***************************************************************************
 * WriteOutFormatter - curl's -w / --write-out (src/tool_writeout.c)
 *
 * Supports every transfer variable curl documents that applies to HTTP: %{http_code},
 * %{response_code}, %{size_*}, %{speed_*}, %{time_*} (seconds with six decimals),
 * %{url}, %{url_effective}, %{redirect_url}, %{num_*}, %{method}, %{scheme},
 * %{local_ip}/%{local_port}/%{remote_ip}/%{remote_port}, %{exitcode}, %{errormsg},
 * %{filename_effective}, %{url.*}/%{urle.*} URL parts, %header{name}, %{json},
 * %{header_json}, %{stderr}/%{stdout} output switches, %output{file} / %output{>>file},
 * %{onerror}, %% and the \n \r \t escapes. A format starting with @ is read from a file.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CurlDotNet.Core
{
    /// <summary>
    /// The values a <c>-w</c> format can print, collected at the end of a transfer.
    /// </summary>
    internal sealed class WriteOutContext
    {
        public int ResponseCode;
        public string HttpVersion = "0";
        public string? ContentType;
        public long SizeDownload;
        public long SizeDelivered;
        public long SizeUpload;
        public long SizeHeader;
        public long SizeRequest;
        public double TimeNameLookup;
        public double TimeConnect;
        public double TimeAppConnect;
        public double TimePreTransfer;
        public double TimePostTransfer;
        public double TimeRedirect;
        public double TimeStartTransfer;
        public double TimeTotal;
        public string Url = string.Empty;
        public string UrlEffective = string.Empty;
        public string RedirectUrl = string.Empty;
        public string Method = "GET";
        public string Scheme = string.Empty;
        public string Referer = string.Empty;
        public int NumRedirects;
        public int NumConnects;
        public int NumHeaders;
        public int NumRetries;
        public int UrlNum;
        public int ExitCode;
        public string ErrorMessage = string.Empty;
        public string FilenameEffective = string.Empty;
        public string LocalIp = string.Empty;
        public int LocalPort;
        public string RemoteIp = string.Empty;
        public int RemotePort;
        public bool ProxyUsed;
        public List<KeyValuePair<string, string>> Headers = new List<KeyValuePair<string, string>>();
    }

    /// <summary>What a -w format produced for each destination.</summary>
    internal sealed class WriteOutResult
    {
        /// <summary>Text for standard output (appended to <see cref="CurlResult.Body"/>).</summary>
        public string Stdout = string.Empty;

        /// <summary>Text for standard error (written to --stderr FILE or the console).</summary>
        public string Stderr = string.Empty;
    }

    internal static class WriteOutFormatter
    {
        /// <summary>
        /// The variables <c>%{json}</c> prints, in curl's order (tool_writeout.c's variable table, alphabetical,
        /// including the <c>url.*</c>/<c>urle.*</c> parts), followed by <c>curl_version</c>.
        /// </summary>
        private static readonly string[] JsonVariables =
        {
            "certs", "conn_id", "content_type", "errormsg", "exitcode", "filename_effective", "ftp_entry_path",
            "http_code", "http_connect", "http_version", "local_ip", "local_port", "method", "num_certs",
            "num_connects", "num_headers", "num_redirects", "num_retries", "proxy_ssl_verify_result", "proxy_used",
            "redirect_url", "referer", "remote_ip", "remote_port", "response_code", "scheme", "size_delivered",
            "size_download", "size_header", "size_request", "size_upload", "speed_download", "speed_upload",
            "ssl_verify_result", "time_appconnect", "time_connect", "time_namelookup", "time_posttransfer",
            "time_pretransfer", "time_queue", "time_redirect", "time_starttransfer", "time_total", "tls_earlydata",
            "url", "url.fragment", "url.host", "url.options", "url.password", "url.path", "url.port", "url.query",
            "url.scheme", "url.user", "url.zoneid", "url_effective", "urle.fragment", "urle.host", "urle.options",
            "urle.password", "urle.path", "urle.port", "urle.query", "urle.scheme", "urle.user", "urle.zoneid",
            "urlnum", "xfer_id",
        };

        /// <summary>
        /// The variables curl writes as JSON numbers (writeLong, writeOffset and writeTime in tool_writeout.c);
        /// every other variable is a JSON string, or null when it has no value.
        /// </summary>
        private static readonly HashSet<string> JsonNumbers = new HashSet<string>(StringComparer.Ordinal)
        {
            "conn_id", "exitcode", "http_code", "http_connect", "local_port", "num_certs", "num_connects", "num_headers",
            "num_redirects", "num_retries", "proxy_ssl_verify_result", "proxy_used", "remote_port", "response_code",
            "size_delivered", "size_download", "size_header", "size_request", "size_upload", "speed_download",
            "speed_upload", "ssl_verify_result", "time_appconnect", "time_connect", "time_namelookup",
            "time_posttransfer", "time_pretransfer", "time_queue", "time_redirect", "time_starttransfer", "time_total",
            "tls_earlydata", "urlnum", "xfer_id",
        };

        /// <summary>
        /// Reads the format: <c>@file</c> loads it from a file (<c>@-</c> would be standard input, which a
        /// library has none of, so it is empty).
        /// </summary>
        public static string LoadFormat(string format)
        {
            if (format.StartsWith("@", StringComparison.Ordinal))
            {
                var path = format.Substring(1);
                if (path == "-")
                {
                    return string.Empty;
                }
                try
                {
                    return File.ReadAllText(path);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    return string.Empty; // curl warns and prints nothing
                }
            }
            return format;
        }

        /// <summary>Expands <paramref name="format"/> for one finished transfer.</summary>
        public static WriteOutResult Format(string format, WriteOutContext ctx)
        {
            var result = new WriteOutResult();
            var stdout = new StringBuilder();
            var stderr = new StringBuilder();
            var current = stdout;
            string? fileTarget = null;
            var fileAppend = false;
            var fileText = new StringBuilder();
            var fmt = LoadFormat(format);

            void Flush()
            {
                if (fileTarget != null)
                {
                    WriteFile(fileTarget, fileText.ToString(), fileAppend);
                    fileText.Clear();
                    fileTarget = null;
                }
            }

            var i = 0;
            while (i < fmt.Length)
            {
                var c = fmt[i];
                if (c == '%' && i + 1 < fmt.Length)
                {
                    if (fmt[i + 1] == '%')
                    {
                        current.Append('%');
                        i += 2;
                        continue;
                    }

                    if (fmt[i + 1] == '{')
                    {
                        var end = fmt.IndexOf('}', i + 2);
                        if (end > 0)
                        {
                            var name = fmt.Substring(i + 2, end - i - 2);
                            i = end + 1;
                            switch (name)
                            {
                                case "stdout":
                                    Flush();
                                    current = stdout;
                                    continue;
                                case "stderr":
                                    Flush();
                                    current = stderr;
                                    continue;
                                case "onerror":
                                    if (ctx.ExitCode == 0)
                                    {
                                        i = fmt.Length; // the rest is only for failed transfers
                                    }
                                    continue;
                            }
                            current.Append(Variable(name, ctx));
                            continue;
                        }
                    }

                    if (string.CompareOrdinal(fmt, i + 1, "header{", 0, 7) == 0)
                    {
                        var end = fmt.IndexOf('}', i + 8);
                        if (end > 0)
                        {
                            var name = fmt.Substring(i + 8, end - i - 8);
                            var values = ctx.Headers.Where(h => string.Equals(h.Key, name, StringComparison.OrdinalIgnoreCase)).Select(h => h.Value).ToList();
                            current.Append(string.Join(", ", values));
                            i = end + 1;
                            continue;
                        }
                    }

                    if (string.CompareOrdinal(fmt, i + 1, "output{", 0, 7) == 0)
                    {
                        var end = fmt.IndexOf('}', i + 8);
                        if (end > 0)
                        {
                            Flush();
                            var target = fmt.Substring(i + 8, end - i - 8);
                            fileAppend = target.StartsWith(">>", StringComparison.Ordinal);
                            fileTarget = fileAppend ? target.Substring(2) : target;
                            current = fileText;
                            i = end + 1;
                            continue;
                        }
                    }

                    current.Append(c);
                    i++;
                    continue;
                }

                if (c == '\\' && i + 1 < fmt.Length)
                {
                    switch (fmt[i + 1])
                    {
                        case 'n': current.Append('\n'); break;
                        case 'r': current.Append('\r'); break;
                        case 't': current.Append('\t'); break;
                        default: current.Append(c).Append(fmt[i + 1]); break;
                    }
                    i += 2;
                    continue;
                }

                current.Append(c);
                i++;
            }

            Flush();
            result.Stdout = stdout.ToString();
            result.Stderr = stderr.ToString();
            return result;
        }

        private static void WriteFile(string path, string text, bool append)
        {
            try
            {
                if (append)
                {
                    File.AppendAllText(path, text);
                }
                else
                {
                    File.WriteAllText(path, text);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // curl reports "could not open output file" as a warning; the transfer itself succeeded
            }
        }

        private static string Seconds(double value) => value.ToString("0.000000", CultureInfo.InvariantCulture);

        private static string Int(long value) => value.ToString(CultureInfo.InvariantCulture);

        /// <summary>Expands one <c>%{name}</c>; unknown names expand to nothing, as in curl.</summary>
        internal static string Variable(string name, WriteOutContext ctx)
        {
            if (name.StartsWith("url.", StringComparison.Ordinal))
            {
                return UrlPart(ctx.Url, name.Substring(4));
            }
            if (name.StartsWith("urle.", StringComparison.Ordinal))
            {
                return UrlPart(ctx.UrlEffective, name.Substring(5));
            }

            switch (name)
            {
                case "content_type": return ctx.ContentType ?? string.Empty;
                case "errormsg": return ctx.ErrorMessage;
                case "exitcode": return Int(ctx.ExitCode);
                case "filename_effective": return ctx.FilenameEffective;
                case "ftp_entry_path": return string.Empty;
                case "http_code":
                case "response_code": return ctx.ResponseCode.ToString("000", CultureInfo.InvariantCulture);
                case "http_connect": return "000";
                case "http_version": return ctx.HttpVersion;
                case "local_ip": return ctx.LocalIp;
                case "local_port": return ctx.LocalPort == 0 ? string.Empty : Int(ctx.LocalPort);
                case "method": return ctx.Method;
                case "num_certs": return "0";
                case "num_connects": return Int(ctx.NumConnects);
                case "num_headers": return Int(ctx.NumHeaders);
                case "num_redirects": return Int(ctx.NumRedirects);
                case "num_retries": return Int(ctx.NumRetries);
                case "proxy_used": return ctx.ProxyUsed ? "1" : "0";
                case "proxy_ssl_verify_result": return "0";
                case "redirect_url": return ctx.RedirectUrl;
                case "referer": return ctx.Referer;
                case "remote_ip": return ctx.RemoteIp;
                case "remote_port": return ctx.RemotePort == 0 ? string.Empty : Int(ctx.RemotePort);
                case "scheme": return ctx.Scheme;
                case "size_delivered": return Int(ctx.SizeDelivered);
                case "size_download": return Int(ctx.SizeDownload);
                case "size_header": return Int(ctx.SizeHeader);
                case "size_request": return Int(ctx.SizeRequest);
                case "size_upload": return Int(ctx.SizeUpload);
                case "speed_download": return Int(ctx.TimeTotal > 0 ? (long)(ctx.SizeDownload / ctx.TimeTotal) : ctx.SizeDownload);
                case "speed_upload": return Int(ctx.TimeTotal > 0 ? (long)(ctx.SizeUpload / ctx.TimeTotal) : ctx.SizeUpload);
                case "ssl_verify_result": return "0";
                case "time_appconnect": return Seconds(ctx.TimeAppConnect);
                case "time_connect": return Seconds(ctx.TimeConnect);
                case "time_namelookup": return Seconds(ctx.TimeNameLookup);
                case "time_posttransfer": return Seconds(ctx.TimePostTransfer);
                case "time_pretransfer": return Seconds(ctx.TimePreTransfer);
                case "time_queue": return Seconds(0);
                case "time_redirect": return Seconds(ctx.TimeRedirect);
                case "time_starttransfer": return Seconds(ctx.TimeStartTransfer);
                case "time_total": return Seconds(ctx.TimeTotal);
                case "url": return ctx.Url;
                case "url_effective": return ctx.UrlEffective;
                case "urlnum": return Int(ctx.UrlNum);
                case "conn_id":
                case "xfer_id":
                case "tls_earlydata": return "0";
                case "certs": return string.Empty;
                case "json": return Json(ctx);
                case "header_json": return HeaderJson(ctx);
                default: return string.Empty;
            }
        }

        private static string UrlPart(string url, string part)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || url.IndexOf(' ') >= 0)
            {
                return string.Empty;
            }

            var userInfo = uri.UserInfo;
            var colon = userInfo.IndexOf(':');
            switch (part)
            {
                case "scheme": return uri.Scheme;
                case "user": return colon < 0 ? userInfo : userInfo.Substring(0, colon);
                case "password": return colon < 0 ? string.Empty : userInfo.Substring(colon + 1);
                case "options": return string.Empty;
                case "host": return uri.Host;
                case "port": return ExplicitPort(url) ?? (uri.IsDefaultPort ? string.Empty : Int(uri.Port));
                case "path": return uri.AbsolutePath;
                case "query": return uri.Query.Length > 0 ? uri.Query.Substring(1) : string.Empty;
                case "fragment": return uri.Fragment.Length > 0 ? uri.Fragment.Substring(1) : string.Empty;
                case "zoneid": return string.Empty;
                default: return string.Empty;
            }
        }

        private static string? ExplicitPort(string url)
        {
            var schemeEnd = url.IndexOf("://", StringComparison.Ordinal);
            if (schemeEnd < 0) return null;
            var authority = url.Substring(schemeEnd + 3);
            var slash = authority.IndexOfAny(new[] { '/', '?', '#' });
            if (slash >= 0) authority = authority.Substring(0, slash);
            var at = authority.LastIndexOf('@');
            if (at >= 0) authority = authority.Substring(at + 1);
            var close = authority.LastIndexOf(']');
            var portColon = authority.LastIndexOf(':');
            if (portColon > close && portColon >= 0 && portColon + 1 < authority.Length)
            {
                return authority.Substring(portColon + 1);
            }
            return null;
        }

        /// <summary>
        /// <c>%{json}</c>: every variable as one JSON object, like curl: counters, sizes, speeds and times are
        /// numbers (<c>"http_code":200</c>, <c>"time_total":0.012345</c>), text is a string, and text without a
        /// value is <c>null</c> (<c>"errormsg":null</c> for a transfer that worked).
        /// </summary>
        private static string Json(WriteOutContext ctx)
        {
            var sb = new StringBuilder("{");
            foreach (var name in JsonVariables)
            {
                sb.Append(JsonString(name)).Append(':');
                if (JsonNumbers.Contains(name))
                {
                    sb.Append(JsonNumber(name, ctx));
                }
                else
                {
                    var value = Variable(name, ctx);
                    sb.Append(value.Length == 0 && name != "certs" ? "null" : JsonString(value));
                }
                sb.Append(',');
            }
            sb.Append("\"curl_version\":").Append(JsonString("libcurl/" + "8.7.1 (CurlDotNet)"));
            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>A numeric variable as curl's JSON writes it: plain digits (no <c>000</c> padding), ports as 0 when unknown.</summary>
        private static string JsonNumber(string name, WriteOutContext ctx)
        {
            switch (name)
            {
                case "http_code":
                case "response_code": return Int(ctx.ResponseCode);
                case "http_connect": return "0";
                case "local_port": return Int(ctx.LocalPort);
                case "remote_port": return Int(ctx.RemotePort);
                default:
                    var value = Variable(name, ctx);
                    return value.Length == 0 ? "0" : value;
            }
        }

        private static string HeaderJson(WriteOutContext ctx)
        {
            var groups = new List<KeyValuePair<string, List<string>>>();
            foreach (var header in ctx.Headers)
            {
                var key = header.Key.ToLowerInvariant();
                var index = groups.FindIndex(g => g.Key == key);
                if (index < 0)
                {
                    groups.Add(new KeyValuePair<string, List<string>>(key, new List<string> { header.Value }));
                }
                else
                {
                    groups[index].Value.Add(header.Value);
                }
            }

            var sb = new StringBuilder("{");
            for (var g = 0; g < groups.Count; g++)
            {
                if (g > 0) sb.Append(",\n");
                sb.Append(JsonString(groups[g].Key)).Append(":[");
                sb.Append(string.Join(",", groups[g].Value.Select(JsonString)));
                sb.Append(']');
            }
            sb.Append("\n}"); // tool_writeout_json.c headerJSON(): the closing brace is on a line of its own
            return sb.ToString();
        }

        private static string JsonString(string value)
        {
            var sb = new StringBuilder("\"");
            foreach (var c in value)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    default:
                        if (c < 0x20) sb.Append("\\u").Append(((int)c).ToString("x4"));
                        else sb.Append(c);
                        break;
                }
            }
            return sb.Append('"').ToString();
        }
    }
}
