/***************************************************************************
 * RequestRewrite - the request-head details HttpClient cannot express
 *
 * HttpClient always sends a Host header, writes "Name: " (with a space) for an empty header and
 * derives the request target from the URI. curl can drop Host (-H "Host:"), send blank headers
 * (-H "Name;" -> "Name:") and use any request target (--request-target "*"). For HTTP/1.x the
 * recording connection layer (WireRecording.RecordingStream) applies these rules to the request
 * head on its way to the socket; nothing else about the request changes.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Lower-cases the hex digits of chunk-size lines in a chunked request body as it is written,
    /// following the chunk framing across writes (sizes, data, CRLF, trailers).
    /// </summary>
    internal sealed class ChunkSizeLowercaser
    {
        private enum State
        {
            Size,
            Extension,
            Data,
            DataEnd,
            Done,
        }

        private State _state = State.Size;
        private long _size;
        private long _remaining;

        /// <summary>Rewrites <paramref name="data"/> in place.</summary>
        public void Transform(byte[] data, int offset, int count)
        {
            for (var i = offset; i < offset + count; i++)
            {
                var b = data[i];
                switch (_state)
                {
                    case State.Size:
                        if (b >= (byte)'A' && b <= (byte)'F')
                        {
                            data[i] = (byte)(b + 32);
                            b = data[i];
                        }
                        if (b >= (byte)'0' && b <= (byte)'9')
                        {
                            _size = _size * 16 + (b - '0');
                        }
                        else if (b >= (byte)'a' && b <= (byte)'f')
                        {
                            _size = _size * 16 + (b - 'a' + 10);
                        }
                        else if (b == (byte)'\n')
                        {
                            EndOfSizeLine();
                        }
                        else if (b != (byte)'\r')
                        {
                            _state = State.Extension;
                        }
                        break;
                    case State.Extension:
                        if (b == (byte)'\n')
                        {
                            EndOfSizeLine();
                        }
                        break;
                    case State.Data:
                        var take = (int)Math.Min(_remaining, offset + count - i);
                        _remaining -= take;
                        i += take - 1;
                        if (_remaining == 0)
                        {
                            _state = State.DataEnd;
                        }
                        break;
                    case State.DataEnd:
                        if (b == (byte)'\n')
                        {
                            _state = State.Size;
                            _size = 0;
                        }
                        break;
                    case State.Done:
                        return; // trailers and anything after: unchanged
                }
            }
        }

        private void EndOfSizeLine()
        {
            if (_size == 0)
            {
                _state = State.Done;
                return;
            }
            _remaining = _size;
            _state = State.Data;
        }
    }

    /// <summary>Byte-level changes to one HTTP/1.x request head.</summary>
    internal sealed class RequestRewrite
    {
        private RequestRewrite()
        {
        }

        /// <summary><c>--request-target</c>: replaces the target of the request line.</summary>
        public string? Target { get; private set; }

        /// <summary><c>-H "Host:"</c>: no Host header at all.</summary>
        public bool DropHost { get; private set; }

        /// <summary>Header names sent without a value (<c>-H "Name;"</c>), written <c>Name:</c> like curl.</summary>
        public HashSet<string> BlankHeaders { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Header lines HttpClient wrote in its own form, replaced by the line as the user wrote it (several
        /// CRLF-separated lines for a header given with more than one <c>-H</c>).
        /// </summary>
        public Dictionary<string, string> ReplaceLines { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Header lines HttpClient could not send at all (invalid names), added at the end of the head.</summary>
        public List<string> ExtraLines { get; } = new List<string>();

        /// <summary>A chunked upload: curl writes chunk sizes in lower-case hex (<c>1e</c>), HttpClient in upper case.</summary>
        public bool LowercaseChunkSizes { get; private set; }

        /// <summary>True when <paramref name="name"/> is a valid HTTP header name (RFC 9110 token).</summary>
        public static bool IsToken(string name)
        {
            if (name.Length == 0)
            {
                return false;
            }
            foreach (var c in name)
            {
                var ok = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || "!#$%&'*+-.^_`|~".IndexOf(c) >= 0;
                if (!ok)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Refuses text that would end a request line or header line early: a CR or LF inside a header or
        /// the request target lets the rest pose as extra headers or a second request (request smuggling).
        /// </summary>
        /// <exception cref="CurlDotNet.Exceptions.CurlBadFunctionArgumentException">The text contains CR or LF (exit code 43).</exception>
        public static void RejectLineBreaks(string text, string option, string? command)
        {
            if (text.IndexOf('\r') < 0 && text.IndexOf('\n') < 0)
            {
                return;
            }
            throw new CurlDotNet.Exceptions.CurlBadFunctionArgumentException(option,
                CurlDotNet.Exceptions.CurlException.Compose(
                    $"The value given with {option} contains a line break (CR or LF): \"{CurlDotNet.Exceptions.ErrorText.Preview(HideSecretHeaderValue(text), 120)}\". " +
                    "A line break would end the header early and let the rest be read as extra headers or another request.",
                    option == "--request-target"
                        ? "Pass the request target on one line, for example --request-target '/api/items?id=7'; percent-encode a line break as %0D%0A if the server really needs it."
                        : "Pass one header per -H (for example -H 'Accept: application/json' -H 'X-Id: 7'), or one header per line in a -H @headers.txt file, without CR/LF inside a value.",
                    CurlDotNet.Exceptions.DocLinks.HowToHeaders),
                command);
        }

        /// <summary>Hides the value of Authorization, Proxy-Authorization and Cookie headers in messages.</summary>
        private static string HideSecretHeaderValue(string text)
        {
            var colon = text.IndexOf(':');
            if (colon <= 0)
            {
                return text;
            }
            var name = text.Substring(0, colon).Trim();
            return name.Equals("Authorization", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Proxy-Authorization", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Cookie", StringComparison.OrdinalIgnoreCase)
                ? name + ": (value hidden)"
                : text;
        }

        /// <summary>The rules a request built from <paramref name="options"/> needs, or null when HttpClient's head is already curl's.</summary>
        public static RequestRewrite? For(CurlOptions options)
        {
            var rewrite = new RequestRewrite();
            var needed = false;

            if (!string.IsNullOrEmpty(options.RequestTarget))
            {
                RejectLineBreaks(options.RequestTarget!, "--request-target", options.OriginalCommand);
                if (options.RequestTarget!.IndexOf(' ') >= 0)
                {
                    throw new CurlDotNet.Exceptions.CurlBadFunctionArgumentException("--request-target",
                        CurlDotNet.Exceptions.CurlException.Compose(
                            $"The --request-target \"{options.RequestTarget}\" contains a space, which would break the HTTP request line (METHOD TARGET HTTP/1.1).",
                            "Percent-encode spaces as %20, for example --request-target '/search?q=two%20words'.",
                            CurlDotNet.Exceptions.DocLinks.Url), options.OriginalCommand);
                }
                rewrite.Target = options.RequestTarget;
                needed = true;
            }

            foreach (var header in options.Headers)
            {
                if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(header.Value)
                    && !options.EmptyHeaders.Contains("Host"))
                {
                    rewrite.DropHost = true;
                    needed = true;
                }
            }

            foreach (var name in options.EmptyHeaders)
            {
                RejectLineBreaks(name, "-H/--header", options.OriginalCommand);
                if (options.Headers.TryGetValue(name, out var value) && !string.IsNullOrEmpty(value))
                {
                    continue; // a later -H gave it a value
                }
                rewrite.BlankHeaders.Add(name);
                needed = true;
            }

            if (options.Headers.TryGetValue("Transfer-Encoding", out var te) && te != null
                && te.IndexOf("chunked", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                rewrite.LowercaseChunkSizes = true;
                needed = true;
            }

            if (options.TimeCondition.HasValue && options.TimeConditionType == CurlTimeCondition.LastModified
                && !options.Headers.ContainsKey("Last-Modified"))
            {
                // curl's -z =DATE request header (HttpClient refuses it outside content)
                rewrite.ExtraLines.Add("Last-Modified: " + RequestShaping.FormatHttpDate(options.TimeCondition.Value));
                needed = true;
            }

            foreach (var raw in options.RawHeaderLines)
            {
                RejectLineBreaks(raw.Value, "-H/--header", options.OriginalCommand);
                if (!options.Headers.TryGetValue(raw.Key, out var value) || string.IsNullOrEmpty(value))
                {
                    continue; // removed (for example Authorization on a redirect to another host)
                }
                if (IsToken(raw.Key))
                {
                    rewrite.ReplaceLines[raw.Key] = raw.Value;
                }
                else
                {
                    rewrite.ExtraLines.Add(raw.Value);
                }
                needed = true;
            }

            if (!options.Headers.ContainsKey("Host") && Uri.TryCreate(options.Url, UriKind.Absolute, out var url) && HostAlias.IsAlias(url.Host))
            {
                // the URL's host was aliased because System.Uri refuses it; curl sends the name as written
                var original = HostAlias.Restore(url.Host);
                rewrite.ReplaceLines["Host"] = "Host: " + (url.IsDefaultPort ? original : original + ":" + url.Port.ToString(System.Globalization.CultureInfo.InvariantCulture));
                needed = true;
            }

            if (options.TransferEncoding && options.RepeatedHeaderValues.TryGetValue("Connection", out var connections)
                && options.Headers.TryGetValue("Connection", out var joined) && joined == string.Join(", ", connections)
                && !options.RawHeaderLines.ContainsKey("Connection"))
            {
                // --tr-encoding with several -H "Connection: ..." (curl test 1617): curl appends its TE token to
                // the first user Connection header and sends each one on its own line ("Connection: this, TE",
                // then "Connection: that"); HttpClient would join them into one line.
                var lines = connections.Select(v => "Connection: " + v).ToList();
                lines[0] += ", TE";
                rewrite.ReplaceLines["Connection"] = string.Join("\r\n", lines);
                needed = true;
            }

            return needed ? rewrite : null;
        }

        /// <summary>Rewrites the request head (request line and header lines, ending with an empty line).</summary>
        public byte[] Apply(byte[] head, int length)
        {
            var text = WireText.Latin1.GetString(head, 0, length);
            var lines = new List<string>(text.Split(new[] { "\r\n" }, StringSplitOptions.None));
            if (lines.Count == 0)
            {
                return WireText.Latin1.GetBytes(text);
            }

            if (Target != null)
            {
                var parts = lines[0].Split(' ');
                if (parts.Length == 3)
                {
                    lines[0] = parts[0] + " " + Target + " " + parts[2];
                }
            }

            var output = new StringBuilder(text.Length);
            output.Append(lines[0]).Append("\r\n");
            for (var i = 1; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Length == 0)
                {
                    continue; // the terminating empty line is added below
                }

                var colon = line.IndexOf(':');
                var name = colon > 0 ? line.Substring(0, colon) : line;
                if (name.Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    if (DropHost)
                    {
                        continue;
                    }
                    if (BlankHeaders.Contains("Host"))
                    {
                        line = name + ":";
                    }
                    else if (ReplaceLines.TryGetValue(name, out var host))
                    {
                        line = host;
                    }
                }
                else if (BlankHeaders.Contains(name) && line.Substring(colon + 1).Trim().Length == 0)
                {
                    line = name + ":";
                }
                else if (ReplaceLines.TryGetValue(name, out var written))
                {
                    line = written;
                }

                output.Append(line).Append("\r\n");
            }
            foreach (var extra in ExtraLines)
            {
                output.Append(extra).Append("\r\n");
            }
            output.Append("\r\n");
            return WireText.Latin1.GetBytes(output.ToString());
        }
    }
}
