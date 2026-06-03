/***************************************************************************
 * TraceWriter - Detailed transfer trace logging
 *
 * Implements curl's --trace / --trace-ascii / --trace-time diagnostics for
 * the .NET implementation. Produces a human-readable record of every
 * observable phase of an HTTP transfer so users can pinpoint exactly where a
 * request succeeds or is blocked - which is especially valuable when working
 * behind a corporate proxy.
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Writes a detailed, human-readable diagnostic trace of an HTTP transfer to a
    /// file (or standard output), mirroring curl's <c>--trace</c>, <c>--trace-ascii</c>
    /// and <c>--trace-time</c> options.
    /// </summary>
    /// <remarks>
    /// <para>The trace records every observable phase of a request: connection and
    /// proxy selection, TLS certificate verification, the exact request line, headers
    /// and body that were sent, the response status, headers and body that were
    /// received, transfer timings, and - most importantly for troubleshooting - the
    /// precise error if a request is blocked or fails.</para>
    /// <para>This is invaluable when working behind a corporate or transparent proxy,
    /// where it is often unclear at which stage a request is being rejected.</para>
    /// <para>The output is buffered in memory and written to the target when the
    /// writer is disposed, so a partially completed (failed) request still produces a
    /// complete trace up to the point of failure.</para>
    /// </remarks>
    internal sealed class TraceWriter : IDisposable
    {
        /// <summary>
        /// Maximum number of payload bytes dumped for a single send/receive block.
        /// Larger payloads are truncated with a note so trace files stay manageable.
        /// </summary>
        private const int MaxDumpBytes = 100 * 1024;

        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly string _filePath;
        private readonly bool _toStdout;
        private readonly bool _asciiOnly;
        private readonly bool _timestamps;
        private bool _disposed;

        private TraceWriter(string filePath, bool toStdout, bool asciiOnly, bool timestamps)
        {
            _filePath = filePath;
            _toStdout = toStdout;
            _asciiOnly = asciiOnly;
            _timestamps = timestamps;
        }

        /// <summary>
        /// Creates a <see cref="TraceWriter"/> for the supplied options, or returns
        /// <c>null</c> when neither <c>--trace</c> nor <c>--trace-ascii</c> was requested.
        /// </summary>
        /// <param name="options">The parsed curl options.</param>
        /// <returns>A configured writer, or <c>null</c> if tracing is not enabled.</returns>
        public static TraceWriter TryCreate(CurlOptions options)
        {
            if (options == null)
            {
                return null;
            }

            // --trace-ascii implies ASCII-only formatting and takes precedence over
            // the binary/hex --trace target when both are supplied.
            var asciiOnly = !string.IsNullOrEmpty(options.TraceAsciiFile);
            var target = asciiOnly ? options.TraceAsciiFile : options.TraceFile;
            if (string.IsNullOrEmpty(target))
            {
                return null;
            }

            var toStdout = target == "-";
            string filePath = null;
            if (!toStdout)
            {
                filePath = target;
                var dir = Path.GetDirectoryName(Path.GetFullPath(filePath));
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            var writer = new TraceWriter(filePath, toStdout, asciiOnly, options.TraceTime);
            writer.WriteHeader(options);
            return writer;
        }

        /// <summary>
        /// Records an informational milestone (connection, proxy, TLS, timing, etc.).
        /// </summary>
        /// <param name="message">The message to record.</param>
        public void Info(string message)
        {
            WriteLine("== Info: " + message);
        }

        /// <summary>
        /// Records an error - typically the precise reason a request was blocked or failed.
        /// </summary>
        /// <param name="message">The error description.</param>
        public void Error(string message)
        {
            WriteLine("== Error: " + message);
        }

        /// <summary>
        /// Dumps the outgoing request line and headers as they are sent to the server.
        /// </summary>
        /// <param name="request">The request being sent.</param>
        public void SendHeaders(HttpRequestMessage request)
        {
            if (request == null)
            {
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(BuildRequestHead(request));
            WriteBlock("=> Send header", bytes);
        }

        /// <summary>
        /// Dumps the outgoing request body, if any. Best-effort: bodies that cannot be
        /// re-read for inspection are skipped without affecting the request itself.
        /// </summary>
        /// <param name="request">The request being sent.</param>
        public async Task SendDataAsync(HttpRequestMessage request)
        {
            if (request?.Content == null)
            {
                return;
            }

            try
            {
                var bytes = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                if (bytes != null && bytes.Length > 0)
                {
                    WriteBlock("=> Send data", bytes);
                }
            }
            catch
            {
                // The body is not buffered/re-readable - skip the data dump.
            }
        }

        /// <summary>
        /// Dumps the response status line and headers received from the server.
        /// </summary>
        /// <param name="response">The received response.</param>
        public void RecvHeaders(HttpResponseMessage response)
        {
            if (response == null)
            {
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(BuildResponseHead(response));
            WriteBlock("<= Recv header", bytes);
        }

        /// <summary>
        /// Dumps the received response body bytes.
        /// </summary>
        /// <param name="data">The response body, or <c>null</c>/empty if none.</param>
        public void RecvData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }

            WriteBlock("<= Recv data", data);
        }

        /// <summary>
        /// Dumps the received response body text.
        /// </summary>
        /// <param name="text">The response body text, or <c>null</c>/empty if none.</param>
        public void RecvData(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            RecvData(Encoding.UTF8.GetBytes(text));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            WriteLine("== Info: Trace finished.");

            try
            {
                var text = _buffer.ToString();
                if (_toStdout)
                {
                    Console.Out.Write(text);
                }
                else if (_filePath != null)
                {
                    File.WriteAllText(_filePath, text);
                }
            }
            catch
            {
                // A trace is a diagnostic aid; never let writing it break the request.
            }
        }

        private void WriteHeader(CurlOptions options)
        {
            WriteLine("== Info: CurlDotNet trace started " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            WriteLine("== Info: Format: " + (_asciiOnly ? "ascii (--trace-ascii)" : "hex+ascii (--trace)"));
            if (!string.IsNullOrEmpty(options.OriginalCommand))
            {
                WriteLine("== Info: Command: " + options.OriginalCommand);
            }
        }

        private static string BuildRequestHead(HttpRequestMessage request)
        {
            var sb = new StringBuilder();
            var uri = request.RequestUri;
            var version = request.Version != null ? request.Version.Major + "." + request.Version.Minor : "1.1";

            sb.Append(request.Method).Append(' ')
              .Append(uri != null ? uri.PathAndQuery : "/")
              .Append(" HTTP/").Append(version).Append("\r\n");

            if (uri != null)
            {
                sb.Append("Host: ").Append(uri.Authority).Append("\r\n");
            }

            foreach (var header in request.Headers)
            {
                sb.Append(header.Key).Append(": ").Append(string.Join(", ", header.Value)).Append("\r\n");
            }

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    sb.Append(header.Key).Append(": ").Append(string.Join(", ", header.Value)).Append("\r\n");
                }
            }

            sb.Append("\r\n");
            return sb.ToString();
        }

        private static string BuildResponseHead(HttpResponseMessage response)
        {
            var sb = new StringBuilder();
            var version = response.Version != null ? response.Version.Major + "." + response.Version.Minor : "1.1";

            sb.Append("HTTP/").Append(version).Append(' ')
              .Append((int)response.StatusCode).Append(' ')
              .Append(response.ReasonPhrase).Append("\r\n");

            foreach (var header in response.Headers)
            {
                sb.Append(header.Key).Append(": ").Append(string.Join(", ", header.Value)).Append("\r\n");
            }

            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    sb.Append(header.Key).Append(": ").Append(string.Join(", ", header.Value)).Append("\r\n");
                }
            }

            sb.Append("\r\n");
            return sb.ToString();
        }

        private void WriteBlock(string label, byte[] data)
        {
            var total = data.Length;
            var shown = Math.Min(total, MaxDumpBytes);

            WriteLine(label + ", " + total + " bytes (0x" + total.ToString("x") + ")");
            DumpBytes(data, shown);

            if (shown < total)
            {
                WriteLine("== Info: [trace truncated: " + (total - shown) + " more bytes not shown]");
            }
        }

        private void DumpBytes(byte[] data, int count)
        {
            for (var offset = 0; offset < count; offset += 16)
            {
                var sb = new StringBuilder();
                sb.Append(offset.ToString("x4")).Append(": ");

                var lineLength = Math.Min(16, count - offset);

                if (!_asciiOnly)
                {
                    for (var i = 0; i < 16; i++)
                    {
                        if (i < lineLength)
                        {
                            sb.Append(data[offset + i].ToString("x2")).Append(' ');
                        }
                        else
                        {
                            sb.Append("   ");
                        }
                    }

                    sb.Append(' ');
                }

                for (var i = 0; i < lineLength; i++)
                {
                    var b = data[offset + i];
                    sb.Append(b >= 0x20 && b < 0x7f ? (char)b : '.');
                }

                WriteLine(sb.ToString());
            }
        }

        private void WriteLine(string line)
        {
            if (_timestamps)
            {
                _buffer.Append(DateTime.Now.ToString("HH:mm:ss.ffffff")).Append(' ');
            }

            _buffer.Append(line).Append('\n');
        }
    }
}
