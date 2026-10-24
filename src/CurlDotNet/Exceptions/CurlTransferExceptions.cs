/***************************************************************************
 * Transfer-level curl errors that the original hierarchy did not cover
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;

namespace CurlDotNet.Exceptions
{
    /// <summary>
    /// The server closed the connection before the whole body arrived (exit code 18, CURLE_PARTIAL_FILE):
    /// fewer bytes than its Content-Length, or a chunked body without its final chunk.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the transport while reading the body. Whatever did arrive is still written to the <c>-o</c> file
    /// (unless <c>--remove-on-error</c> is used), exactly like curl, so a large download can be resumed with
    /// <c>-C -</c>. <see cref="BytesReceived"/> and <see cref="BytesExpected"/> say how far it got. Retrying helps when a
    /// proxy or flaky network cut the transfer; <c>--ignore-content-length</c> accepts what arrived when the server's length
    /// is known to be wrong.</para>
    /// <para>Example: <c>catch (CurlPartialFileException ex) { Console.WriteLine($"{ex.BytesReceived} of {ex.BytesExpected} bytes"); await Curl.ExecuteAsync($"curl -C - -o big.iso {url}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlReceiveErrorException"/>
    /// <seealso cref="CurlRangeErrorException"/>
    [Serializable]
    public class CurlPartialFileException : CurlException
    {
        /// <summary>
        /// Creates the partial-transfer error (exit code 18) with curl's wording
        /// <c>transfer closed with N bytes remaining to read</c> and advice to retry or resume.
        /// </summary>
        /// <remarks>
        /// <para>For a chunked body (<paramref name="bytesExpected"/> null) the wording is
        /// <c>transfer closed with outstanding read data remaining</c>. The URL is printed without credentials or query
        /// string. Example: <c>throw new CurlPartialFileException(url, 512, 1024, command);</c></para>
        /// </remarks>
        /// <param name="url">The URL whose body was cut short.</param>
        /// <param name="bytesReceived">Body bytes that arrived before the connection closed.</param>
        /// <param name="bytesExpected">Body bytes the server announced with Content-Length, or null for a chunked body.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlRangeErrorException"/>
        public CurlPartialFileException(string url, long bytesReceived, long? bytesExpected, string? command = null)
            : base(BuildMessage(url, bytesReceived, bytesExpected), 18, command)
        {
            Url = url;
            BytesReceived = bytesReceived;
            BytesExpected = bytesExpected;
        }

        /// <summary>
        /// The URL whose body was cut short, as requested (it may contain credentials; the message shows a safe
        /// version).
        /// </summary>
        /// <value>An absolute URL.</value>
        /// <remarks><para>Use it to resume: <c>curl -C - -o FILE {Url}</c>.</para></remarks>
        /// <seealso cref="BytesReceived"/>
        public string Url { get; }

        /// <summary>
        /// Body bytes that arrived before the connection closed; also the resume offset for <c>-C -</c> when they were
        /// written to a file.
        /// </summary>
        /// <value>A byte count, 0 or more.</value>
        /// <remarks><para>Example: <c>Console.WriteLine($"{100.0 * ex.BytesReceived / ex.BytesExpected:F0}% received");</c></para></remarks>
        /// <seealso cref="BytesExpected"/>
        public long BytesReceived { get; }

        /// <summary>
        /// Body bytes the server announced with Content-Length, or null for chunked bodies, where the total is unknown.
        /// </summary>
        /// <value>A byte count, or null.</value>
        /// <remarks><para><c>BytesExpected - BytesReceived</c> is the number of missing bytes.</para></remarks>
        /// <seealso cref="BytesReceived"/>
        public long? BytesExpected { get; }

        private static string BuildMessage(string url, long received, long? expected)
        {
            var what = expected.HasValue
                ? $"transfer closed with {expected.Value - received} bytes remaining to read ({received} of {expected.Value} received)"
                : $"transfer closed with outstanding read data remaining ({received} bytes of a chunked body received, the final chunk never arrived)";
            return CurlException.Compose(
                $"Partial file: {what} from {ErrorText.SafeUrl(url)}.",
                "The server or a proxy dropped the connection mid-transfer; retry, or resume the download with -C - (--continue-at -).",
                DocLinks.HowToDownload);
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToDownload;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Retry the transfer, or resume it with -C - so the bytes already saved are kept; use --ignore-content-length only if the server's Content-Length is known to be wrong.";
    }

    /// <summary>
    /// <c>-C/--continue-at</c> asked to resume, but the server sent the whole document (or an error) instead of the
    /// requested byte range (exit code 33, CURLE_RANGE_ERROR).
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the HTTP handler; curl refuses to append a full response to a partial download, which would corrupt
    /// it. <see cref="Offset"/> is the resume position and <see cref="StatusCode"/> what the server answered (usually 200
    /// instead of 206). Download from the start (drop <c>-C</c>) or use a server that supports byte ranges
    /// (<c>Accept-Ranges: bytes</c>).</para>
    /// <para>Example: <c>catch (CurlRangeErrorException ex) { File.Delete(partialFile); /* start over without -C */ }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlBadDownloadResumeException"/>
    /// <seealso cref="CurlPartialFileException"/>
    [Serializable]
    public class CurlRangeErrorException : CurlException
    {
        /// <summary>
        /// Creates the range error (exit code 33) with curl's wording
        /// <c>HTTP server doesn't seem to support byte ranges. Cannot resume.</c> and the offset and status involved.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlRangeErrorException(1048576, 200, command);</c></para></remarks>
        /// <param name="offset">The byte offset the transfer tried to resume from.</param>
        /// <param name="statusCode">The HTTP status the server answered with (usually 200 instead of 206).</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlBadDownloadResumeException"/>
        public CurlRangeErrorException(long offset, int statusCode, string? command = null)
            : base(CurlException.Compose(
                       $"HTTP server doesn't seem to support byte ranges. Cannot resume: asked for bytes {offset}- with -C/--continue-at, but the server answered {statusCode} without a Content-Range.",
                       "Download the whole file again without -C, or use a server that supports byte ranges (Accept-Ranges: bytes).",
                       DocLinks.HowToDownload),
                   33, command)
        {
            Offset = offset;
            StatusCode = statusCode;
        }

        /// <summary>
        /// The resume offset that was requested, in bytes (the size of the partial file for <c>-C -</c>).
        /// </summary>
        /// <value>A byte offset greater than 0.</value>
        /// <remarks><para>The partial file is left untouched so you can decide whether to delete it.</para></remarks>
        /// <seealso cref="StatusCode"/>
        public long Offset { get; }

        /// <summary>
        /// The HTTP status of the response that did not honour the range, usually 200 (whole document) instead of 206.
        /// </summary>
        /// <value>An HTTP status code.</value>
        /// <remarks><para>A 4xx/5xx status here means the resource itself failed, not just the range.</para></remarks>
        /// <seealso cref="Offset"/>
        public int StatusCode { get; }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToDownload;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Download the whole file again without -C (the server answered {StatusCode} instead of 206 Partial Content for bytes {Offset}-).";
    }

    /// <summary>
    /// An authentication scheme failed while answering the server's (or proxy's) challenge (exit code 94,
    /// CURLE_AUTH_ERROR), for example a malformed Digest or NTLM challenge.
    /// </summary>
    /// <remarks>
    /// <para>Raised when Digest, NTLM or Negotiate negotiation (<c>--digest</c>, <c>--ntlm</c>,
    /// <c>--negotiate</c>, <c>--anyauth</c>, or the <c>--proxy-*</c> variants) cannot process the
    /// <c>WWW-Authenticate</c>/<c>Proxy-Authenticate</c> challenge, for example because the server sent
    /// a parameter twice. A plain wrong password is not this error: it ends with a normal 401 result.
    /// <see cref="Scheme"/> and <see cref="Proxy"/> say which negotiation failed.</para>
    /// <para>Example: <c>catch (CurlAuthErrorException ex) when (ex.Proxy) { Console.WriteLine($"Proxy {ex.Scheme} auth failed"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md schemes:
    /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md</para>
    /// </remarks>
    /// <seealso cref="CurlAuthenticationException"/>
    [Serializable]
    public class CurlAuthErrorException : CurlException
    {
        /// <summary>
        /// Creates the authentication-negotiation error (exit code 94), naming the scheme, whether it was the proxy, and
        /// what the authentication code reported.
        /// </summary>
        /// <remarks>
        /// <para>The fix suggests <c>--basic</c>/<c>--proxy-basic</c> when the server also offers Basic, and <c>-v</c> to see
        /// the challenge. Example: <c>throw new CurlAuthErrorException("digest", false, "the challenge has two realm= parameters", command);</c></para>
        /// </remarks>
        /// <param name="scheme">The scheme that failed: digest, ntlm, negotiate or anyauth.</param>
        /// <param name="proxy">True when the failing negotiation was with the proxy rather than the server.</param>
        /// <param name="detail">What the authentication code reported, in words.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlAuthenticationException"/>
        public CurlAuthErrorException(string scheme, bool proxy, string detail, string? command = null)
            : base(CurlException.Compose(
                       $"{(proxy ? "Proxy" : "Server")} authentication failed: the {scheme} negotiation could not process the {(proxy ? "Proxy-Authenticate" : "WWW-Authenticate")} challenge ({detail}).",
                       $"Try {(proxy ? "--proxy-basic" : "--basic")} if the {(proxy ? "proxy" : "server")} also offers Basic, or check the challenge with -v.",
                       DocLinks.Auth),
                   94, command)
        {
            Scheme = scheme;
            Proxy = proxy;
        }

        /// <summary>
        /// The scheme whose negotiation failed: <c>digest</c>, <c>ntlm</c>, <c>negotiate</c> or <c>anyauth</c>.
        /// </summary>
        /// <value>The scheme name in lower case.</value>
        /// <remarks><para>Switching to another scheme the server offers (see its WWW-Authenticate header) often works around a buggy challenge.</para></remarks>
        /// <seealso cref="Proxy"/>
        public string Scheme { get; }

        /// <summary>
        /// True when the failing negotiation was with the proxy (<c>--proxy-digest</c>, <c>--proxy-ntlm</c>...) rather than
        /// with the server.
        /// </summary>
        /// <value>True for proxy authentication, false for server authentication.</value>
        /// <remarks><para>Use it to decide whether the proxy settings or the server credentials need attention.</para></remarks>
        /// <seealso cref="Scheme"/>
        public bool Proxy { get; }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Auth;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"The {Scheme} challenge could not be answered: try {(Proxy ? "--proxy-basic" : "--basic")} if Basic is offered, or inspect the challenge with -v.";
    }

    /// <summary>
    /// The server sent more response header data than curl accepts (exit code 100, CURLE_TOO_LARGE): over 300 KB of
    /// headers, a single header line over 100 KB, or too many header fields.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the transport to protect against hostile or broken servers. There is no client option that raises the
    /// limit; the server (often a cookie that keeps growing, or a redirect loop adding headers) must be fixed.
    /// Example: <c>catch (CurlTooLargeException ex) { Console.WriteLine(ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlWeirdServerReplyException"/>
    [Serializable]
    public class CurlTooLargeException : CurlException
    {
        /// <summary>
        /// Creates the error (exit code 100) describing which header limit was exceeded, with advice that the server must
        /// be fixed.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlTooLargeException("the header block is 412000 bytes (limit 307200)", command);</c></para></remarks>
        /// <param name="detail">What exceeded the limit, with the sizes involved.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlWeirdServerReplyException"/>
        public CurlTooLargeException(string detail, string? command = null)
            : base(CurlException.Compose(
                       $"Response too large: {detail}.",
                       "The server's header block is larger than curl accepts; this is a server bug (often a cookie that keeps growing) or an attack, not something a client option can fix.",
                       DocLinks.Http),
                   100, command)
        {
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Http;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Fix the server so it sends fewer or smaller headers (check for a cookie that keeps growing); no client option raises curl's limit.";
    }
}
