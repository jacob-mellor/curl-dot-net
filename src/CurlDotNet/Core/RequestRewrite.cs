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

        /// <summary>Header lines HttpClient wrote in its own form, replaced by the line as the user wrote it.</summary>
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

        /// <summary>The rules a request built from <paramref name="options"/> needs, or null when HttpClient's head is already curl's.</summary>
        public static RequestRewrite? For(CurlOptions options)
        {
            var rewrite = new RequestRewrite();
            var needed = false;

            if (!string.IsNullOrEmpty(options.RequestTarget))
            {
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

            foreach (var raw in options.RawHeaderLines)
            {
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
