/***************************************************************************
 * Content decoding for --compressed, following curl's lib/content_encoding.c
 *
 * curl decodes the body as it arrives, one zlib stream per encoding:
 *  - a corrupt stream fails the transfer at once with CURLE_BAD_CONTENT_ENCODING (61),
 *    without waiting for the rest of the body (curl test 223);
 *  - a gzip body holds exactly one member: a second member after it is
 *    "Multi-member gzip response not supported" and any other bytes after the
 *    member's trailer are CURLE_WRITE_ERROR (23) (curl test 2305).
 * .NET's GZipStream silently decodes every concatenated member and DeflateStream
 * only works on a whole buffer, so this file adds what is missing: a background
 * probe that decodes the body while it downloads, and a one-member gunzip.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// curl-compatible gzip/deflate decoding helpers for <c>--compressed</c>.
    /// </summary>
    internal static class ContentDecoding
    {
        /// <summary>
        /// Decodes a gzip body the way curl's gzip writer does: one member only. A zlib-wrapped body labelled gzip is
        /// accepted too (curl initialises zlib with automatic gzip/zlib header detection).
        /// </summary>
        /// <param name="body">The whole received body.</param>
        /// <param name="command">The curl command, for the exception.</param>
        /// <returns>The decoded bytes.</returns>
        /// <exception cref="InvalidDataException">The data is corrupt (the caller maps it to exit 61).</exception>
        /// <exception cref="CurlDecodedTrailingDataException">Bytes follow the first gzip member (exit 23).</exception>
        internal static byte[] Gunzip(byte[] body, string? command)
        {
            if (body.Length >= 2 && body[0] == 0x1f && body[1] == 0x8b)
            {
                var start = GzipHeaderLength(body);
                if (start < 0)
                {
                    return Array.Empty<byte>(); // the header itself is cut short: zlib would still be waiting for it
                }
                var decoded = Inflate(body, start, body.Length - start, s => new DeflateStream(s, CompressionMode.Decompress));
                var end = FindGzipTrailer(body, start, decoded);
                if (end >= 0)
                {
                    var trailing = body.Length - end;
                    if (trailing > 0)
                    {
                        var anotherMember = trailing >= 2 && body[end] == 0x1f && body[end + 1] == 0x8b;
                        throw new CurlDecodedTrailingDataException(anotherMember, trailing, command);
                    }
                    return decoded;
                }
                // No matching CRC-32/ISIZE trailer: a truncated or damaged member. Let the full gzip decoder decide
                // (it reports a bad checksum as InvalidDataException, exit 61, like zlib's "incorrect data check").
                return Inflate(body, 0, body.Length, s => new GZipStream(s, CompressionMode.Decompress));
            }
            if (IsZlibHeader(body, 0))
            {
                return Inflate(body, 2, body.Length - 2, s => new DeflateStream(s, CompressionMode.Decompress));
            }
            // Neither gzip nor zlib: zlib's "incorrect header check".
            return Inflate(body, 0, body.Length, s => new GZipStream(s, CompressionMode.Decompress));
        }

        /// <summary>True when <paramref name="data"/> starts (at <paramref name="offset"/>) with a valid zlib header.</summary>
        internal static bool IsZlibHeader(byte[] data, int offset)
        {
            if (data.Length - offset < 2)
            {
                return false;
            }
            var cmf = data[offset];
            var flg = data[offset + 1];
            return (cmf & 0x0f) == 8 && (cmf >> 4) <= 7 && ((cmf << 8) | flg) % 31 == 0;
        }

        /// <summary>
        /// The length of the gzip member header at the start of <paramref name="body"/> (RFC 1952: 10 fixed bytes plus
        /// the optional FEXTRA, FNAME, FCOMMENT and FHCRC fields), or -1 when the body ends inside the header.
        /// </summary>
        /// <exception cref="InvalidDataException">The compression method is not deflate (zlib: "unknown compression method").</exception>
        private static int GzipHeaderLength(byte[] body)
        {
            if (body.Length < 10)
            {
                return -1;
            }
            if (body[2] != 8)
            {
                throw new InvalidDataException("unknown compression method");
            }
            var flags = body[3];
            var pos = 10;
            if ((flags & 0x04) != 0) // FEXTRA
            {
                if (body.Length < pos + 2) return -1;
                pos += 2 + (body[pos] | (body[pos + 1] << 8));
            }
            if ((flags & 0x08) != 0) pos = SkipZeroTerminated(body, pos); // FNAME
            if (pos < 0) return -1;
            if ((flags & 0x10) != 0) pos = SkipZeroTerminated(body, pos); // FCOMMENT
            if (pos < 0) return -1;
            if ((flags & 0x02) != 0) pos += 2; // FHCRC
            return pos <= body.Length ? pos : -1;
        }

        private static int SkipZeroTerminated(byte[] body, int pos)
        {
            if (pos < 0 || pos > body.Length) return -1;
            var zero = Array.IndexOf(body, (byte)0, pos);
            return zero < 0 ? -1 : zero + 1;
        }

        /// <summary>
        /// Finds where the first gzip member ends: the position just past its 8-byte trailer (CRC-32 and size of the
        /// decoded data, little-endian), searched from <paramref name="start"/>; -1 when the trailer is not there.
        /// </summary>
        private static int FindGzipTrailer(byte[] body, int start, byte[] decoded)
        {
            var crc = Crc32(decoded);
            var size = (uint)decoded.LongLength;
            var trailer = new byte[]
            {
                (byte)crc, (byte)(crc >> 8), (byte)(crc >> 16), (byte)(crc >> 24),
                (byte)size, (byte)(size >> 8), (byte)(size >> 16), (byte)(size >> 24),
            };
            // A deflate stream is at least 2 bytes long, so the trailer cannot start before start + 2.
            for (var i = start + 2; i + 8 <= body.Length; i++)
            {
                if (body[i] != trailer[0])
                {
                    continue;
                }
                var match = true;
                for (var j = 1; j < 8 && match; j++)
                {
                    match = body[i + j] == trailer[j];
                }
                if (match)
                {
                    return i + 8;
                }
            }
            return -1;
        }

        private static readonly uint[] CrcTable = BuildCrcTable();

        private static uint[] BuildCrcTable()
        {
            var table = new uint[256];
            for (uint n = 0; n < 256; n++)
            {
                var c = n;
                for (var k = 0; k < 8; k++)
                {
                    c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
                }
                table[n] = c;
            }
            return table;
        }

        /// <summary>The gzip (ISO-HDLC) CRC-32 of <paramref name="data"/>.</summary>
        internal static uint Crc32(byte[] data)
        {
            var crc = 0xFFFFFFFFu;
            foreach (var b in data)
            {
                crc = CrcTable[(crc ^ b) & 0xff] ^ (crc >> 8);
            }
            return crc ^ 0xFFFFFFFFu;
        }

        private static byte[] Inflate(byte[] body, int offset, int count, Func<Stream, Stream> open)
        {
            using var input = new MemoryStream(body, offset, Math.Max(0, count), writable: false);
            using var decoder = open(input);
            using var output = new MemoryStream();
            decoder.CopyTo(output);
            return output.ToArray();
        }
    }

    /// <summary>
    /// Decodes a gzip or deflate body in the background while it downloads, so that a corrupt stream stops the
    /// transfer as soon as the bad bytes arrive, the way curl's streaming decoder fails with exit 61 on the first bad
    /// block instead of waiting for (or timing out on) the rest of the body. The decoded bytes are thrown away; the
    /// body is decoded for real once it is complete.
    /// </summary>
    internal sealed class ContentDecodeProbe : IDisposable
    {
        private readonly bool _gzip;
        private readonly ChunkQueueStream _input = new ChunkQueueStream();
        private readonly CancellationTokenSource _failed = new CancellationTokenSource();
        private byte[] _head = Array.Empty<byte>();
        private bool _started;

        private ContentDecodeProbe(bool gzip)
        {
            _gzip = gzip;
        }

        /// <summary>
        /// A probe for the outermost (last applied) of <paramref name="encodings"/>, or null when it is not gzip or
        /// deflate.
        /// </summary>
        internal static ContentDecodeProbe? For(System.Collections.Generic.ICollection<string>? encodings)
        {
            if (encodings == null || encodings.Count == 0)
            {
                return null;
            }
            string? last = null;
            foreach (var e in encodings)
            {
                last = e;
            }
            switch (last?.Trim().ToLowerInvariant())
            {
                case "gzip":
                case "x-gzip":
                    return new ContentDecodeProbe(gzip: true);
                case "deflate":
                    return new ContentDecodeProbe(gzip: false);
                default:
                    return null;
            }
        }

        /// <summary>Cancelled as soon as the decoder meets corrupt data.</summary>
        internal CancellationToken Failed => _failed.Token;

        /// <summary>The decoder's error once <see cref="Failed"/> is cancelled.</summary>
        internal Exception? Error { get; private set; }

        /// <summary>Hands the decoder the next received bytes (copied).</summary>
        internal void Feed(byte[] buffer, int count)
        {
            if (count <= 0 || _failed.IsCancellationRequested)
            {
                return;
            }
            var copy = new byte[count];
            Buffer.BlockCopy(buffer, 0, copy, 0, count);
            if (_started)
            {
                _input.Add(copy);
                return;
            }

            // the first two bytes pick the decoder, as zlib's header detection does
            var head = new byte[_head.Length + count];
            Buffer.BlockCopy(_head, 0, head, 0, _head.Length);
            Buffer.BlockCopy(copy, 0, head, _head.Length, count);
            if (head.Length < 2)
            {
                _head = head;
                return;
            }
            _started = true;
            _head = Array.Empty<byte>();

            bool gzipMember = head[0] == 0x1f && head[1] == 0x8b;
            bool zlib = !gzipMember && ContentDecoding.IsZlibHeader(head, 0);
            if (_gzip && !gzipMember && !zlib)
            {
                return; // neither gzip nor zlib: the decode of the whole body reports it
            }
            // Only a failure that the decode of the complete body would also report stops the download:
            // a gzip member, or raw deflate. A zlib-looking start may still be raw deflate (the full decode
            // then retries raw, like curl's first-write fallback), so a zlib failure only ends the probe.
            var stopsTransfer = gzipMember || !zlib;
            if (zlib)
            {
                var rest = new byte[head.Length - 2];
                Buffer.BlockCopy(head, 2, rest, 0, rest.Length);
                _input.Add(rest);
            }
            else
            {
                _input.Add(head);
            }
            // fire and forget: RunAsync catches everything it expects and Dispose stops it
            _ = Task.Run(() => RunAsync(gzipMember, stopsTransfer));
        }

        private async Task RunAsync(bool gzipMember, bool stopsTransfer)
        {
            try
            {
                Stream decoder = gzipMember
                    ? new GZipStream(_input, CompressionMode.Decompress)
                    : (Stream)new DeflateStream(_input, CompressionMode.Decompress);
                using (decoder)
                {
                    var sink = new byte[16384];
                    int decoded;
                    do
                    {
                        decoded = await decoder.ReadAsync(sink, 0, sink.Length).ConfigureAwait(false);
                    }
                    while (decoded > 0);
                }
            }
            catch (ObjectDisposedException)
            {
                // the transfer finished first
            }
            catch (Exception ex) when ((ex is InvalidDataException || ex is IOException) && stopsTransfer)
            {
                Error = ex;
                try
                {
                    _failed.Cancel();
                }
                catch (ObjectDisposedException)
                {
                }
            }
            catch (Exception)
            {
                // a zlib-looking start that is not zlib (the full decode retries it as raw deflate), or any
                // other decoder trouble: the decode of the complete body has the last word
            }
        }

        /// <summary>Stops the decoder (the body is complete or the transfer failed).</summary>
        public void Dispose()
        {
            _input.Complete();
            _failed.Dispose();
        }

        /// <summary>A read-only stream fed with chunks from another task; reads wait for the next chunk.</summary>
        private sealed class ChunkQueueStream : Stream
        {
            private readonly ConcurrentQueue<byte[]> _chunks = new ConcurrentQueue<byte[]>();
            private readonly SemaphoreSlim _available = new SemaphoreSlim(0);
            private byte[]? _current;
            private int _offset;
            private volatile bool _completed;

            public void Add(byte[] chunk)
            {
                if (_completed) return;
                _chunks.Enqueue(chunk);
                _available.Release();
            }

            public void Complete()
            {
                _completed = true;
                _available.Release();
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                while (_current == null || _offset >= _current.Length)
                {
                    if (_completed) return 0;
                    await _available.WaitAsync(cancellationToken).ConfigureAwait(false);
                    if (_completed) return 0;
                    if (!_chunks.TryDequeue(out _current)) return 0;
                    _offset = 0;
                }
                var n = Math.Min(count, _current.Length - _offset);
                Buffer.BlockCopy(_current, _offset, buffer, offset, n);
                _offset += n;
                return n;
            }

            public override int Read(byte[] buffer, int offset, int count) =>
                ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }
    }
}
