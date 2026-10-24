/***************************************************************************
 * Transfer-level curl errors that the original hierarchy did not cover
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;

namespace CurlDotNet.Exceptions
{
    /// <summary>
    /// CURLE_PARTIAL_FILE (18): the server closed the connection before sending the whole body.
    /// </summary>
    /// <remarks>
    /// <para>Raised when a response announced <c>Content-Length: N</c> but fewer bytes arrived, or a
    /// chunked body ended without its final zero-length chunk. Whatever did arrive is written to the
    /// <c>-o</c> file unless <c>--remove-on-error</c> is used, exactly like curl.</para>
    /// <para>Example: <c>catch (CurlPartialFileException ex) { Console.WriteLine(ex.BytesReceived); }</c>
    /// and retry with <c>-C -</c> to resume.</para>
    /// </remarks>
    [Serializable]
    public class CurlPartialFileException : CurlException
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="url">The URL whose body was cut short.</param>
        /// <param name="bytesReceived">Body bytes that arrived.</param>
        /// <param name="bytesExpected">Body bytes the server announced, or null for a chunked body.</param>
        /// <param name="command">The curl command that was executing.</param>
        public CurlPartialFileException(string url, long bytesReceived, long? bytesExpected, string? command = null)
            : base(BuildMessage(url, bytesReceived, bytesExpected), 18, command)
        {
            Url = url;
            BytesReceived = bytesReceived;
            BytesExpected = bytesExpected;
        }

        /// <summary>The URL whose body was cut short.</summary>
        public string Url { get; }

        /// <summary>Body bytes that arrived before the connection closed.</summary>
        public long BytesReceived { get; }

        /// <summary>Body bytes the server announced with Content-Length, or null for chunked bodies.</summary>
        public long? BytesExpected { get; }

        private static string BuildMessage(string url, long received, long? expected)
        {
            var what = expected.HasValue
                ? $"transfer closed with {expected.Value - received} bytes remaining to read ({received} of {expected.Value} received)"
                : $"transfer closed with outstanding read data remaining ({received} bytes of a chunked body received, the final chunk never arrived)";
            return $"Partial file: {what} from {url}. The server or a proxy dropped the connection mid-transfer; retry, " +
                   "or resume the download with -C - (--continue-at -)." +
                   "\nFor HTTP error handling, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md";
        }
    }

    /// <summary>
    /// CURLE_TOO_LARGE (100): the server sent more response header data than curl accepts.
    /// </summary>
    /// <remarks>
    /// <para>curl refuses a response whose header block is too large (over 300 KB, or a single header
    /// line over 100 KB, or too many header fields). This protects against hostile servers.</para>
    /// </remarks>
    [Serializable]
    public class CurlTooLargeException : CurlException
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="detail">What exceeded the limit.</param>
        /// <param name="command">The curl command that was executing.</param>
        public CurlTooLargeException(string detail, string? command = null)
            : base($"Response too large: {detail}. The server's header block is larger than curl accepts; this is a server bug or " +
                   "an attack, not something a client option can fix." +
                   "\nFor HTTP error handling, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md",
                   100, command)
        {
        }
    }
}
