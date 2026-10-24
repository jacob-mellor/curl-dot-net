/***************************************************************************
 * NtlmCrypto - the primitives NTLM needs, in pure C#
 *
 * Transpiled from curl's lib/curl_ntlm_core.c (Curl_ntlm_core_lm_resp, mk_lm_hash, mk_nt_hash,
 * mk_ntlmv2_hash) plus a DES block cipher (FIPS 46-3) and MD4 (RFC 1320). Both are written out
 * here because .NET has no MD4 at all, and its DES refuses the "weak" keys a password hash can
 * produce and is missing on some platforms.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Security.Cryptography;
using System.Text;

namespace CurlDotNet.Core.Auth
{
    /// <summary>LM/NT hashes and responses as curl computes them.</summary>
    internal static class NtlmCrypto
    {
        private static readonly byte[] LmMagic = Encoding.ASCII.GetBytes("KGS!@#$%");

        /// <summary>The LM hash: DES of "KGS!@#$%" under the upper-cased, 14-byte padded password, padded to 21 bytes.</summary>
        public static byte[] LmHash(string password)
        {
            var pw = new byte[14];
            var bytes = Encoding.UTF8.GetBytes(password);
            for (var i = 0; i < pw.Length && i < bytes.Length; i++)
            {
                var b = bytes[i];
                pw[i] = b >= (byte)'a' && b <= (byte)'z' ? (byte)(b - 32) : b; // curl's Curl_raw_toupper: ASCII only
            }

            var hash = new byte[21];
            Des.Encrypt(ExpandKey(pw, 0), LmMagic, hash, 0);
            Des.Encrypt(ExpandKey(pw, 7), LmMagic, hash, 8);
            return hash;
        }

        /// <summary>The NT hash: MD4 of the password as UTF-16LE (each UTF-8 byte widened, like curl), padded to 21 bytes.</summary>
        public static byte[] NtHash(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var wide = new byte[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                wide[i * 2] = bytes[i];
            }

            var hash = new byte[21];
            Buffer.BlockCopy(Md4.Hash(wide), 0, hash, 0, 16);
            return hash;
        }

        /// <summary>curl's Curl_ntlm_core_lm_resp(): the 8-byte challenge DES-encrypted under three 7-byte slices of the 21-byte hash.</summary>
        public static byte[] LmResponse(byte[] hash21, byte[] challenge)
        {
            var result = new byte[24];
            Des.Encrypt(ExpandKey(hash21, 0), challenge, result, 0);
            Des.Encrypt(ExpandKey(hash21, 7), challenge, result, 8);
            Des.Encrypt(ExpandKey(hash21, 14), challenge, result, 16);
            return result;
        }

        /// <summary>The NTLMv2 hash: HMAC-MD5 under the NT hash of UTF-16LE(UPPER(user) + domain).</summary>
        public static byte[] NtlmV2Hash(string user, string domain, byte[] ntHash21)
        {
            var key = new byte[16];
            Buffer.BlockCopy(ntHash21, 0, key, 0, 16);
            var identity = Encoding.Unicode.GetBytes(user.ToUpperInvariant() + domain);
            using (var hmac = new HMACMD5(key))
            {
                return hmac.ComputeHash(identity);
            }
        }

        /// <summary>HMAC-MD5 of <paramref name="data"/> under <paramref name="key"/>.</summary>
        public static byte[] HmacMd5(byte[] key, byte[] data)
        {
            using (var hmac = new HMACMD5(key))
            {
                return hmac.ComputeHash(data);
            }
        }

        /// <summary>curl's extend_key_56_to_64(): 7 key bytes spread over 8 DES key bytes (parity bits are ignored by DES).</summary>
        internal static byte[] ExpandKey(byte[] source, int offset)
        {
            var k = new byte[7];
            Buffer.BlockCopy(source, offset, k, 0, 7);
            return new[]
            {
                k[0],
                (byte)(((k[0] << 7) & 0xFF) | (k[1] >> 1)),
                (byte)(((k[1] << 6) & 0xFF) | (k[2] >> 2)),
                (byte)(((k[2] << 5) & 0xFF) | (k[3] >> 3)),
                (byte)(((k[3] << 4) & 0xFF) | (k[4] >> 4)),
                (byte)(((k[4] << 3) & 0xFF) | (k[5] >> 5)),
                (byte)(((k[5] << 2) & 0xFF) | (k[6] >> 6)),
                (byte)((k[6] << 1) & 0xFF),
            };
        }
    }

    /// <summary>The DES block cipher (one 8-byte block, ECB), as NTLM uses it.</summary>
    internal static class Des
    {
        private static readonly byte[] Ip =
        {
            58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7,
        };

        private static readonly byte[] Fp =
        {
            40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25,
        };

        private static readonly byte[] E =
        {
            32, 1, 2, 3, 4, 5, 4, 5, 6, 7, 8, 9, 8, 9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25, 24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32, 1,
        };

        private static readonly byte[] P =
        {
            16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10,
            2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25,
        };

        private static readonly byte[] Pc1 =
        {
            57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18,
            10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22,
            14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4,
        };

        private static readonly byte[] Pc2 =
        {
            14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2,
            41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32,
        };

        private static readonly byte[] Shifts = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        private static readonly byte[,] S =
        {
            {
                14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7, 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,
                4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0, 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13,
            },
            {
                15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10, 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,
                0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15, 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9,
            },
            {
                10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8, 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,
                13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7, 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12,
            },
            {
                7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15, 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,
                10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4, 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14,
            },
            {
                2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9, 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,
                4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14, 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3,
            },
            {
                12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11, 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,
                9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6, 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13,
            },
            {
                4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1, 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,
                1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2, 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12,
            },
            {
                13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7, 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,
                7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8, 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11,
            },
        };

        /// <summary>Encrypts one 8-byte block of <paramref name="input"/> into <paramref name="output"/> at <paramref name="offset"/>.</summary>
        public static void Encrypt(byte[] key, byte[] input, byte[] output, int offset)
        {
            var subkeys = KeySchedule(ToUlong(key, 0));
            var block = Permute(ToUlong(input, 0), Ip, 64);
            var left = (uint)(block >> 32);
            var right = (uint)block;
            for (var round = 0; round < 16; round++)
            {
                var next = left ^ Feistel(right, subkeys[round]);
                left = right;
                right = next;
            }
            var result = Permute(((ulong)right << 32) | left, Fp, 64);
            for (var i = 0; i < 8; i++)
            {
                output[offset + i] = (byte)(result >> (56 - 8 * i));
            }
        }

        private static ulong ToUlong(byte[] data, int offset)
        {
            ulong value = 0;
            for (var i = 0; i < 8; i++)
            {
                value = (value << 8) | data[offset + i];
            }
            return value;
        }

        /// <summary>Applies a DES permutation table (1-based bit positions counted from the most significant bit of an <paramref name="inputBits"/>-bit value).</summary>
        private static ulong Permute(ulong input, byte[] table, int inputBits)
        {
            ulong output = 0;
            foreach (var position in table)
            {
                output = (output << 1) | ((input >> (inputBits - position)) & 1UL);
            }
            return output;
        }

        private static ulong[] KeySchedule(ulong key)
        {
            var permuted = Permute(key, Pc1, 64); // 56 bits
            var c = (uint)(permuted >> 28) & 0x0FFFFFFF;
            var d = (uint)permuted & 0x0FFFFFFF;
            var subkeys = new ulong[16];
            for (var i = 0; i < 16; i++)
            {
                c = ((c << Shifts[i]) | (c >> (28 - Shifts[i]))) & 0x0FFFFFFF;
                d = ((d << Shifts[i]) | (d >> (28 - Shifts[i]))) & 0x0FFFFFFF;
                subkeys[i] = Permute(((ulong)c << 28) | d, Pc2, 56); // 48 bits
            }
            return subkeys;
        }

        private static uint Feistel(uint right, ulong subkey)
        {
            var expanded = Permute(right, E, 32) ^ subkey; // 48 bits
            uint substituted = 0;
            for (var box = 0; box < 8; box++)
            {
                var six = (int)((expanded >> (42 - 6 * box)) & 0x3F);
                var row = ((six & 0x20) >> 4) | (six & 1);
                var column = (six >> 1) & 0x0F;
                substituted = (substituted << 4) | S[box, row * 16 + column];
            }
            return (uint)Permute(substituted, P, 32);
        }
    }

    /// <summary>The MD4 message digest (RFC 1320), used for the NT password hash.</summary>
    internal static class Md4
    {
        /// <summary>The 16-byte MD4 digest of <paramref name="message"/>.</summary>
        public static byte[] Hash(byte[] message)
        {
            uint a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476;

            var bitLength = (ulong)message.Length * 8;
            var paddedLength = ((message.Length + 8) / 64 + 1) * 64;
            var padded = new byte[paddedLength];
            Buffer.BlockCopy(message, 0, padded, 0, message.Length);
            padded[message.Length] = 0x80;
            for (var i = 0; i < 8; i++)
            {
                padded[paddedLength - 8 + i] = (byte)(bitLength >> (8 * i));
            }

            var x = new uint[16];
            for (var chunk = 0; chunk < paddedLength; chunk += 64)
            {
                for (var i = 0; i < 16; i++)
                {
                    x[i] = BitConverterLe(padded, chunk + i * 4);
                }

                uint aa = a, bb = b, cc = c, dd = d;

                // round 1
                foreach (var k in new[] { 0, 4, 8, 12 })
                {
                    a = Rotl(a + F(b, c, d) + x[k], 3);
                    d = Rotl(d + F(a, b, c) + x[k + 1], 7);
                    c = Rotl(c + F(d, a, b) + x[k + 2], 11);
                    b = Rotl(b + F(c, d, a) + x[k + 3], 19);
                }

                // round 2
                foreach (var k in new[] { 0, 1, 2, 3 })
                {
                    a = Rotl(a + G(b, c, d) + x[k] + 0x5A827999, 3);
                    d = Rotl(d + G(a, b, c) + x[k + 4] + 0x5A827999, 5);
                    c = Rotl(c + G(d, a, b) + x[k + 8] + 0x5A827999, 9);
                    b = Rotl(b + G(c, d, a) + x[k + 12] + 0x5A827999, 13);
                }

                // round 3
                foreach (var k in new[] { 0, 2, 1, 3 })
                {
                    a = Rotl(a + H(b, c, d) + x[k] + 0x6ED9EBA1, 3);
                    d = Rotl(d + H(a, b, c) + x[k + 8] + 0x6ED9EBA1, 9);
                    c = Rotl(c + H(d, a, b) + x[k + 4] + 0x6ED9EBA1, 11);
                    b = Rotl(b + H(c, d, a) + x[k + 12] + 0x6ED9EBA1, 15);
                }

                a += aa;
                b += bb;
                c += cc;
                d += dd;
            }

            var digest = new byte[16];
            var words = new[] { a, b, c, d };
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    digest[i * 4 + j] = (byte)(words[i] >> (8 * j));
                }
            }
            return digest;
        }

        private static uint BitConverterLe(byte[] data, int offset) =>
            (uint)(data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16) | (data[offset + 3] << 24));

        private static uint F(uint x, uint y, uint z) => (x & y) | (~x & z);

        private static uint G(uint x, uint y, uint z) => (x & y) | (x & z) | (y & z);

        private static uint H(uint x, uint y, uint z) => x ^ y ^ z;

        private static uint Rotl(uint value, int bits) => (value << bits) | (value >> (32 - bits));
    }
}
