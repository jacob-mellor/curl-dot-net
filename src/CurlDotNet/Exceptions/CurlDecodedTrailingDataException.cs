/***************************************************************************
 * The --compressed decoder found data after the end of the compressed stream
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;

namespace CurlDotNet.Exceptions
{
    /// <summary>
    /// A <c>--compressed</c> body went on after its gzip stream had ended (exit code 23, CURLE_WRITE_ERROR, as in curl):
    /// either a second gzip member followed the first (<c>Multi-member gzip response not supported</c>) or other bytes did.
    /// </summary>
    /// <remarks>
    /// <para>curl's gzip decoder (lib/content_encoding.c) decodes exactly one gzip member. When the decoded member is followed
    /// by the gzip magic bytes <c>1f 8b</c> it fails with <c>Multi-member gzip response not supported</c>; any other bytes
    /// after the member's 8-byte trailer fail with curl's plain write error text
    /// <c>Failed writing received data to disk/application</c>. CurlDotNet reports both with the same exit code and the same
    /// first message line, so scripts that parse curl's <c>curl: (23) ...</c> line keep working.</para>
    /// <para>Fixes: drop <c>--compressed</c> to save the raw bytes and decompress them with a tool that reads concatenated
    /// gzip members (<c>gzip -dc</c>), or have the server send a single gzip member.</para>
    /// <para>Example: <c>catch (CurlDecodedTrailingDataException ex) when (ex.AnotherGzipMember) { /* fetch again without --compressed */ }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlBadContentEncodingException"/>
    /// <seealso cref="CurlWriteErrorException"/>
    [Serializable]
    public class CurlDecodedTrailingDataException : CurlException
    {
        /// <summary>curl's message for a second gzip member after the first.</summary>
        internal const string MultiMemberText = "Multi-member gzip response not supported";

        /// <summary>curl's text for CURLE_WRITE_ERROR (lib/strerror.c), printed when other bytes follow the stream.</summary>
        internal const string WriteErrorText = "Failed writing received data to disk/application";

        /// <summary>
        /// Creates the error (exit code 23) for <paramref name="trailingBytes"/> bytes found after the end of a gzip
        /// member in a body decoded for <c>--compressed</c>.
        /// </summary>
        /// <remarks>
        /// <para>The message is curl's own text: <c>Multi-member gzip response not supported</c> when
        /// <paramref name="anotherGzipMember"/> is true, otherwise <c>Failed writing received data to disk/application</c>.
        /// Example: <c>throw new CurlDecodedTrailingDataException(true, 27, command);</c></para>
        /// </remarks>
        /// <param name="anotherGzipMember">True when the bytes after the first member start a second gzip member (<c>1f 8b</c>).</param>
        /// <param name="trailingBytes">How many bytes followed the end of the first member.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="AnotherGzipMember"/>
        public CurlDecodedTrailingDataException(bool anotherGzipMember, long trailingBytes, string? command = null)
            : base(anotherGzipMember ? MultiMemberText : WriteErrorText, 23, command)
        {
            AnotherGzipMember = anotherGzipMember;
            TrailingBytes = trailingBytes;
        }

        /// <summary>
        /// True when the bytes after the first gzip member are a second gzip member (the server concatenated several
        /// compressed pieces), false when they are something else.
        /// </summary>
        /// <value>True for curl's <c>Multi-member gzip response not supported</c> case.</value>
        /// <remarks><para>A multi-member body is valid gzip that curl chooses not to decode; other trailing bytes are a
        /// server bug.</para></remarks>
        /// <seealso cref="TrailingBytes"/>
        public bool AnotherGzipMember { get; }

        /// <summary>
        /// How many body bytes followed the end of the first gzip member.
        /// </summary>
        /// <value>A byte count, 1 or more.</value>
        /// <remarks><para>Example: <c>Console.WriteLine($"{ex.TrailingBytes} bytes after the gzip stream");</c></para></remarks>
        /// <seealso cref="AnotherGzipMember"/>
        public long TrailingBytes { get; }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Http;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => AnotherGzipMember
            ? $"The server sent {TrailingBytes} more bytes after the first gzip member, starting a second member (concatenated gzip), which curl does not decode. " +
              "Drop --compressed to save the raw bytes and decompress them with a tool that reads every member (gzip -dc), or have the server send one gzip member."
            : $"The server sent {TrailingBytes} bytes after the end of the gzip stream. Drop --compressed to save the raw bytes, or have the server stop sending data after the compressed body.";
    }
}
