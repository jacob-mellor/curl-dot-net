/***************************************************************************
 * NtlmAuth - NTLM type-1 / type-2 / type-3 messages like curl
 *
 * Transpiled from curl's lib/vauth/ntlm.c: the type-1 message carries curl's fixed flags, the
 * type-2 message gives the server challenge (and target info), and the type-3 message answers
 * with LM + NT responses (NTLMv1) or LMv2 + NTLMv2 when the server offers extended security.
 * The workstation name is always "WORKSTATION", like curl.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Security.Cryptography;
using System.Text;

namespace CurlDotNet.Core.Auth
{
    /// <summary>Where an NTLM handshake stands (curl's curlntlm states).</summary>
    internal enum NtlmStage
    {
        /// <summary>Nothing sent yet.</summary>
        None,

        /// <summary>A type-1 message should be (or was) sent.</summary>
        Type1,

        /// <summary>The server's type-2 challenge arrived; a type-3 message goes next.</summary>
        Type2,

        /// <summary>The type-3 answer was sent.</summary>
        Type3,

        /// <summary>The connection is authenticated; no more headers are needed.</summary>
        Last,
    }

    /// <summary>The NTLM state of one transfer, for the server or for the proxy.</summary>
    internal sealed class NtlmState
    {
        internal const uint FlagNegotiateUnicode = 1 << 0;
        internal const uint FlagNegotiateOem = 1 << 1;
        internal const uint FlagRequestTarget = 1 << 2;
        internal const uint FlagNegotiateNtlmKey = 1 << 9;
        internal const uint FlagNegotiateAlwaysSign = 1 << 15;
        internal const uint FlagNegotiateNtlm2Key = 1 << 19;
        internal const uint FlagNegotiateTargetInfo = 1 << 23;

        /// <summary>curl never reveals the local host name.</summary>
        private const string Workstation = "WORKSTATION";

        /// <summary>curl's NTLM_BUFSIZE: larger type-3 messages fail with CURLE_TOO_LARGE.</summary>
        internal const int BufferSize = 1024;

        private static readonly byte[] Signature = { (byte)'N', (byte)'T', (byte)'L', (byte)'M', (byte)'S', (byte)'S', (byte)'P', 0 };

        private uint _flags;
        private byte[] _nonce = new byte[8];
        private byte[] _targetInfo = Array.Empty<byte>();

        public NtlmStage Stage { get; set; } = NtlmStage.None;

        /// <summary>curl's type-1 message: OEM, request target, NTLM key, NTLM2 key and always-sign flags, no domain or host.</summary>
        public static string Type1()
        {
            var flags = FlagNegotiateOem | FlagRequestTarget | FlagNegotiateNtlmKey | FlagNegotiateNtlm2Key | FlagNegotiateAlwaysSign;
            var message = new byte[32];
            Buffer.BlockCopy(Signature, 0, message, 0, 8);
            message[8] = 1;
            WriteUInt32(message, 12, flags);
            return Convert.ToBase64String(message);
        }

        /// <summary>
        /// Reads the server's type-2 message (base64). Returns false when it is not a valid type-2
        /// message (curl: CURLE_BAD_CONTENT_ENCODING, treated as an authentication problem).
        /// </summary>
        public bool DecodeType2(string base64)
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64.Trim());
            }
            catch (FormatException)
            {
                return false;
            }

            if (data.Length < 32)
            {
                return false;
            }
            for (var i = 0; i < 8; i++)
            {
                if (data[i] != Signature[i])
                {
                    return false;
                }
            }
            if (ReadUInt32(data, 8) != 2)
            {
                return false;
            }

            _flags = ReadUInt32(data, 20);
            _nonce = new byte[8];
            Buffer.BlockCopy(data, 24, _nonce, 0, 8);
            _targetInfo = Array.Empty<byte>();
            if ((_flags & FlagNegotiateTargetInfo) != 0 && data.Length >= 48)
            {
                var length = data[40] | (data[41] << 8);
                var offset = (int)ReadUInt32(data, 44);
                if (length > 0)
                {
                    if (offset < 48 || offset > data.Length || offset + length > data.Length)
                    {
                        return false;
                    }
                    _targetInfo = new byte[length];
                    Buffer.BlockCopy(data, offset, _targetInfo, 0, length);
                }
            }
            return true;
        }

        /// <summary>
        /// The type-3 message (base64) for <paramref name="userWithDomain"/> (<c>DOMAIN\user</c> or
        /// <c>DOMAIN/user</c> splits off the domain). Returns null when the message would exceed curl's
        /// 1024-byte limit (an overlong user name).
        /// </summary>
        public string? Type3(string userWithDomain, string password)
        {
            var domain = string.Empty;
            var user = userWithDomain;
            var separator = userWithDomain.IndexOf('\\');
            if (separator < 0)
            {
                separator = userWithDomain.IndexOf('/');
            }
            if (separator >= 0)
            {
                domain = userWithDomain.Substring(0, separator);
                user = userWithDomain.Substring(separator + 1);
            }

            byte[] lmResponse;
            byte[] ntResponse;
            var flags = _flags;
            if ((_flags & FlagNegotiateNtlm2Key) != 0)
            {
                // NTLMv2 (curl uses it whenever the server offers extended security)
                var entropy = new byte[8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(entropy);
                }
                var ntHash = NtlmCrypto.NtHash(password);
                var v2Hash = NtlmCrypto.NtlmV2Hash(user, domain, ntHash);

                var lmData = new byte[16];
                Buffer.BlockCopy(_nonce, 0, lmData, 0, 8);
                Buffer.BlockCopy(entropy, 0, lmData, 8, 8);
                lmResponse = new byte[24];
                Buffer.BlockCopy(NtlmCrypto.HmacMd5(v2Hash, lmData), 0, lmResponse, 0, 16);
                Buffer.BlockCopy(entropy, 0, lmResponse, 16, 8);

                ntResponse = NtlmV2Response(v2Hash, entropy);
            }
            else
            {
                ntResponse = NtlmCrypto.LmResponse(NtlmCrypto.NtHash(password), _nonce);
                lmResponse = NtlmCrypto.LmResponse(NtlmCrypto.LmHash(password), _nonce);
                flags &= ~FlagNegotiateNtlm2Key;
            }

            var unicode = (_flags & FlagNegotiateUnicode) != 0;
            var domainBytes = Encode(domain, unicode);
            var userBytes = Encode(user, unicode);
            var hostBytes = Encode(Workstation, unicode);

            const int lmOffset = 64;
            var ntOffset = lmOffset + lmResponse.Length;
            var domainOffset = ntOffset + ntResponse.Length;
            var userOffset = domainOffset + domainBytes.Length;
            var hostOffset = userOffset + userBytes.Length;
            var size = hostOffset + hostBytes.Length;
            if (size >= BufferSize)
            {
                return null;
            }

            var message = new byte[size];
            Buffer.BlockCopy(Signature, 0, message, 0, 8);
            message[8] = 3;
            WriteSecurityBuffer(message, 12, lmResponse.Length, lmOffset);
            WriteSecurityBuffer(message, 20, ntResponse.Length, ntOffset);
            WriteSecurityBuffer(message, 28, domainBytes.Length, domainOffset);
            WriteSecurityBuffer(message, 36, userBytes.Length, userOffset);
            WriteSecurityBuffer(message, 44, hostBytes.Length, hostOffset);
            // 52..59: session key (empty, offset 0)
            WriteUInt32(message, 60, flags);
            Buffer.BlockCopy(lmResponse, 0, message, lmOffset, lmResponse.Length);
            Buffer.BlockCopy(ntResponse, 0, message, ntOffset, ntResponse.Length);
            Buffer.BlockCopy(domainBytes, 0, message, domainOffset, domainBytes.Length);
            Buffer.BlockCopy(userBytes, 0, message, userOffset, userBytes.Length);
            Buffer.BlockCopy(hostBytes, 0, message, hostOffset, hostBytes.Length);
            return Convert.ToBase64String(message);
        }

        /// <summary>curl's Curl_ntlm_core_mk_ntlmv2_resp(): HMAC-MD5 over challenge + blob, followed by the blob.</summary>
        private byte[] NtlmV2Response(byte[] v2Hash, byte[] clientChallenge)
        {
            var blobLength = 28 + _targetInfo.Length + 4;
            var buffer = new byte[16 + blobLength];
            buffer[16] = 1;
            buffer[17] = 1;
            var filetime = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 11644473600L) * 10000000L;
            for (var i = 0; i < 8; i++)
            {
                buffer[24 + i] = (byte)(filetime >> (8 * i));
            }
            Buffer.BlockCopy(clientChallenge, 0, buffer, 32, 8);
            Buffer.BlockCopy(_targetInfo, 0, buffer, 44, _targetInfo.Length);

            var hmacInput = new byte[8 + blobLength];
            Buffer.BlockCopy(_nonce, 0, hmacInput, 0, 8);
            Buffer.BlockCopy(buffer, 16, hmacInput, 8, blobLength);
            Buffer.BlockCopy(NtlmCrypto.HmacMd5(v2Hash, hmacInput), 0, buffer, 0, 16);
            return buffer;
        }

        private static byte[] Encode(string text, bool unicode)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            if (!unicode)
            {
                return bytes;
            }
            var wide = new byte[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                wide[i * 2] = bytes[i]; // curl's unicodecpy(): each byte widened
            }
            return wide;
        }

        private static void WriteSecurityBuffer(byte[] message, int at, int length, int offset)
        {
            message[at] = (byte)length;
            message[at + 1] = (byte)(length >> 8);
            message[at + 2] = (byte)length;
            message[at + 3] = (byte)(length >> 8);
            WriteUInt32(message, at + 4, (uint)offset);
        }

        private static void WriteUInt32(byte[] data, int at, uint value)
        {
            data[at] = (byte)value;
            data[at + 1] = (byte)(value >> 8);
            data[at + 2] = (byte)(value >> 16);
            data[at + 3] = (byte)(value >> 24);
        }

        private static uint ReadUInt32(byte[] data, int at) =>
            (uint)(data[at] | (data[at + 1] << 8) | (data[at + 2] << 16) | (data[at + 3] << 24));
    }
}
