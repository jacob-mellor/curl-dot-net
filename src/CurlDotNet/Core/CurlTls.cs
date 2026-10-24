/***************************************************************************
 * CurlTls - curl's TLS options on .NET's SslStream
 *
 * --cacert / --capath (a private trust store instead of the system one), --cert / --key /
 * --cert-type / --key-type / --pass (client certificates), --pinnedpubkey (SHA-256 public key pins),
 * --tlsv1.x / --tls-max (protocol range) and --ciphers / --tls13-ciphers (CipherSuitesPolicy where the
 * platform has one). The trust store, pins and client certificates need .NET 5 or later; on
 * .NET Standard 2.0 / .NET Framework the parser records those options in CurlOptions.UnsupportedOptions.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>What the certificate check of one handler saw, so a failed handshake can be reported with curl's exit code.</summary>
    internal sealed class TlsVerificationState
    {
        /// <summary>Why the server certificate was rejected (untrusted by the --cacert bundle, wrong name...), or null.</summary>
        public string? Failure { get; set; }

        /// <summary>True when the certificate was fine but its public key matched no --pinnedpubkey pin (exit 90).</summary>
        public bool PinMismatch { get; set; }

        /// <summary>The SHA-256 pin of the key the server presented, for the pin-mismatch message.</summary>
        public string? PresentedPin { get; set; }
    }

    /// <summary>
    /// Turns curl's TLS options into <see cref="SslProtocols"/>, client certificates, a trust store, public key pins
    /// and a cipher policy for the HTTP handler.
    /// </summary>
    internal static class CurlTls
    {
        private const SslProtocols Tls10 = (SslProtocols)0x00C0;
        private const SslProtocols Tls11 = (SslProtocols)0x0300;
        private const SslProtocols Tls12 = (SslProtocols)0x0C00;
        private const SslProtocols Tls13 = (SslProtocols)0x3000;

        /// <summary>True when the options need a handler with their own TLS settings.</summary>
        public static bool IsConfigured(CurlOptions options) =>
            !string.IsNullOrEmpty(options.CaCertFile) || !string.IsNullOrEmpty(options.CaPath)
            || !string.IsNullOrEmpty(options.CertFile) || !string.IsNullOrEmpty(options.PinnedPublicKey)
            || !string.IsNullOrEmpty(options.TlsMinVersion) || !string.IsNullOrEmpty(options.TlsMaxVersion)
            || !string.IsNullOrEmpty(options.Ciphers);

        /// <summary>True when this build can apply <c>--cacert</c>, <c>--capath</c>, <c>--cert</c> and <c>--pinnedpubkey</c>.</summary>
        public static bool SupportsTrustAndClientCertificates
        {
            get
            {
#if NETSTANDARD2_0 || NET472 || NET48
                return false;
#else
                return true;
#endif
            }
        }

        /// <summary>
        /// The protocols allowed by <c>--tlsv1.x</c> (minimum) and <c>--tls-max</c> (maximum), or
        /// <see cref="SslProtocols.None"/> to let the operating system choose. Like curl, a maximum alone keeps
        /// curl's default minimum of TLS 1.2, and the minimum options below 1.2 only mean "the system default".
        /// </summary>
        /// <exception cref="CurlSslConnectErrorException">The range is empty (exit 35).</exception>
        public static SslProtocols Protocols(CurlOptions options)
        {
            var min = Rank(options.TlsMinVersion);
            var max = Rank(options.TlsMaxVersion);
            if (min == 0 && max == 0)
            {
                return SslProtocols.None;
            }

            if (min <= 2 && max == 0)
            {
                return SslProtocols.None; // --tlsv1, --tlsv1.0, --tlsv1.1: anything the system offers (1.2+ today)
            }

            var low = min == 0 ? 3 : min;
            var high = max == 0 ? 4 : max;
            if (low > high)
            {
                throw new CurlSslConnectErrorException(CurlException.Compose(
                    $"--tls-max {options.TlsMaxVersion} is lower than the minimum TLS {options.TlsMinVersion} asked for with --tlsv{options.TlsMinVersion}, so no TLS version is left to use for {ErrorText.SafeUrl(options.Url)}",
                    "Raise --tls-max or lower the --tlsv1.x minimum (for example --tlsv1.2 --tls-max 1.3).",
                    DocLinks.Ssl), options.OriginalCommand);
            }

            var protocols = SslProtocols.None;
            for (var rank = low; rank <= high; rank++)
            {
                protocols |= rank switch { 1 => Tls10, 2 => Tls11, 3 => Tls12, _ => Tls13 };
            }
            return protocols;
        }

        /// <summary>1 for TLS 1.0 ... 4 for TLS 1.3, 0 for "not given" ("default" too).</summary>
        private static int Rank(string? version)
        {
            switch (version)
            {
                case "1.0": return 1;
                case "1.1": return 2;
                case "1.2": return 3;
                case "1.3": return 4;
                default: return 0;
            }
        }

        /// <summary>
        /// Reads a <c>--tls-max</c> value (<c>1.0</c>, <c>1.1</c>, <c>1.2</c>, <c>1.3</c> or <c>default</c>); anything
        /// else fails with exit code 2, like curl.
        /// </summary>
        public static string? ParseMaxVersion(string value, string? command)
        {
            var text = (value ?? string.Empty).Trim();
            if (text == "default")
            {
                return null;
            }
            if (Rank(text) > 0)
            {
                return text;
            }
            throw new CurlException(CurlException.Compose(
                $"option --tls-max: \"{value}\" is not a TLS version.",
                "Give --tls-max one of 1.0, 1.1, 1.2, 1.3 or default (for example --tls-max 1.2).",
                DocLinks.Ssl), 2, command);
        }

#if !NETSTANDARD2_0 && !NET472 && !NET48
        /// <summary>
        /// Applies every TLS option to <paramref name="ssl"/> (the handler's <c>SslOptions</c>), including <c>-k</c>, and
        /// remembers what the certificate check sees in <see cref="CurlOptions.TlsVerification"/>.
        /// </summary>
        public static void Apply(SslClientAuthenticationOptions ssl, CurlOptions options)
        {
            var protocols = Protocols(options);
            if (protocols != SslProtocols.None)
            {
                ssl.EnabledSslProtocols = protocols;
            }

            var policy = CipherPolicy(options);
            if (policy != null)
            {
                ssl.CipherSuitesPolicy = policy;
            }

            var client = LoadClientCertificate(options);
            if (client != null)
            {
                ssl.ClientCertificates = new X509CertificateCollection { client };
            }

            var trust = LoadTrustStore(options);
            var pins = LoadPins(options);
            var state = new TlsVerificationState();
            options.TlsVerification = state;
            var insecure = options.Insecure;
            ssl.RemoteCertificateValidationCallback = (sender, certificate, chain, errors) =>
                Verify(certificate, chain, errors, insecure, trust, pins, state);
        }

        /// <summary>The certificate check: -k, the --cacert/--capath store or the system store, then the pins.</summary>
        private static bool Verify(X509Certificate? certificate, X509Chain? chain, SslPolicyErrors errors, bool insecure,
            X509Certificate2Collection? trust, List<byte[]>? pins, TlsVerificationState state)
        {
            if (certificate == null)
            {
                state.Failure = "the server sent no certificate";
                return false;
            }

            var leaf = certificate as X509Certificate2 ?? new X509Certificate2(certificate);
            if (!insecure)
            {
                if (trust == null)
                {
                    if (errors != SslPolicyErrors.None)
                    {
                        state.Failure = errors.ToString();
                        return false;
                    }
                }
                else
                {
                    // curl --cacert replaces the system store: only the bundle's CAs are trusted
                    if ((errors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
                    {
                        state.Failure = "RemoteCertificateNameMismatch: the certificate was not issued for this host name";
                        return false;
                    }

                    using var custom = new X509Chain();
                    custom.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                    custom.ChainPolicy.CustomTrustStore.AddRange(trust);
                    custom.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    if (chain != null)
                    {
                        foreach (var element in chain.ChainElements)
                        {
                            if (!ReferenceEquals(element.Certificate, leaf) && element.Certificate.Thumbprint != leaf.Thumbprint)
                            {
                                custom.ChainPolicy.ExtraStore.Add(element.Certificate);
                            }
                        }
                    }
                    if (!custom.Build(leaf))
                    {
                        var status = string.Join(", ", custom.ChainStatus.Select(s => s.Status.ToString()).Distinct());
                        state.Failure = $"not issued by a CA in the --cacert/--capath bundle ({(status.Length == 0 ? "untrusted" : status)})";
                        return false;
                    }
                }
            }

            if (pins != null)
            {
                using var sha = SHA256.Create();
                var hash = sha.ComputeHash(leaf.PublicKey.ExportSubjectPublicKeyInfo());
                state.PresentedPin = "sha256//" + Convert.ToBase64String(hash);
                if (!pins.Any(p => p.SequenceEqual(hash)))
                {
                    state.PinMismatch = true;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// <c>--ciphers</c>/<c>--tls13-ciphers</c>: IANA suite names (<c>TLS_AES_128_GCM_SHA256</c>,
        /// <c>TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256</c>) separated by <c>:</c>, <c>,</c> or spaces. The parser only keeps the
        /// value when every name is known and the platform supports <see cref="CipherSuitesPolicy"/>.
        /// </summary>
        private static CipherSuitesPolicy? CipherPolicy(CurlOptions options)
        {
            if (OperatingSystem.IsWindows() || string.IsNullOrEmpty(options.Ciphers) || !TryParseCipherSuites(options.Ciphers!, out var suites))
            {
                return null; // Windows (SChannel) has no per-connection cipher policy
            }
            try
            {
                return new CipherSuitesPolicy(suites);
            }
            catch (PlatformNotSupportedException)
            {
                return null; // the parser already listed the option in UnsupportedOptions
            }
        }

        /// <summary>True when every name in <paramref name="value"/> is a <see cref="TlsCipherSuite"/>.</summary>
        internal static bool TryParseCipherSuites(string value, out List<TlsCipherSuite> suites)
        {
            suites = new List<TlsCipherSuite>();
            foreach (var name in value.Split(new[] { ':', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!Enum.TryParse<TlsCipherSuite>(name.Trim(), ignoreCase: false, out var suite) || name.Trim().All(char.IsDigit))
                {
                    return false;
                }
                suites.Add(suite);
            }
            return suites.Count > 0;
        }

        /// <summary>True when this platform lets a client choose its cipher suites.</summary>
        internal static bool PlatformSupportsCipherPolicy()
        {
            if (OperatingSystem.IsWindows())
            {
                return false;
            }
            try
            {
                _ = new CipherSuitesPolicy(new[] { TlsCipherSuite.TLS_AES_128_GCM_SHA256 });
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
        }

        /// <summary>
        /// <c>--cacert FILE</c> and <c>--capath DIR</c>: every certificate of the PEM (or DER) bundle and of every file
        /// in the folder, or null when neither is given.
        /// </summary>
        /// <exception cref="CurlSslCaCertBadFileException">The file or folder cannot be read or holds no certificate (exit 77).</exception>
        private static X509Certificate2Collection? LoadTrustStore(CurlOptions options)
        {
            if (string.IsNullOrEmpty(options.CaCertFile) && string.IsNullOrEmpty(options.CaPath))
            {
                return null;
            }

            var store = new X509Certificate2Collection();
            if (!string.IsNullOrEmpty(options.CaCertFile))
            {
                var path = options.CaCertFile!;
                if (!File.Exists(path))
                {
                    throw CaBadFile(path, $"--cacert names \"{path}\", which does not exist", options);
                }
                if (!TryImport(store, path, out var error))
                {
                    throw CaBadFile(path, $"--cacert \"{path}\" could not be read as a PEM or DER certificate bundle ({error})", options);
                }
            }

            if (!string.IsNullOrEmpty(options.CaPath))
            {
                var dir = options.CaPath!;
                if (!Directory.Exists(dir))
                {
                    throw CaBadFile(dir, $"--capath names \"{dir}\", which is not a folder", options);
                }
                foreach (var file in Directory.GetFiles(dir))
                {
                    TryImport(store, file, out _); // like OpenSSL's hashed folder: files that are not certificates are skipped
                }
            }

            if (store.Count == 0)
            {
                throw CaBadFile(options.CaCertFile ?? options.CaPath!,
                    $"no CA certificate was found in {(string.IsNullOrEmpty(options.CaCertFile) ? "--capath \"" + options.CaPath + "\"" : "--cacert \"" + options.CaCertFile + "\"")}", options);
            }
            return store;
        }

        /// <summary>Adds the certificates of one PEM or DER file to <paramref name="store"/>.</summary>
        private static bool TryImport(X509Certificate2Collection store, string path, out string? error)
        {
            error = null;
            try
            {
                var bytes = File.ReadAllBytes(path);
                var before = store.Count;
                if (LooksLikePem(bytes))
                {
                    store.ImportFromPem(Encoding.ASCII.GetString(bytes));
                }
                else
                {
                    store.Add(LoadCertificate(bytes));
                }
                if (store.Count == before)
                {
                    error = "it contains no -----BEGIN CERTIFICATE----- block";
                    return false;
                }
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is CryptographicException || ex is ArgumentException)
            {
                error = ex.Message.TrimEnd('.');
                return false;
            }
        }

        private static bool LooksLikePem(byte[] bytes) =>
            Encoding.ASCII.GetString(bytes, 0, Math.Min(bytes.Length, 4096)).IndexOf("-----BEGIN ", StringComparison.Ordinal) >= 0;

        private static X509Certificate2 LoadCertificate(byte[] der)
        {
#if NET9_0_OR_GREATER
            return X509CertificateLoader.LoadCertificate(der);
#else
            return new X509Certificate2(der);
#endif
        }

        private static X509Certificate2 LoadPkcs12(byte[] pfx, string? password)
        {
#if NET9_0_OR_GREATER
            return X509CertificateLoader.LoadPkcs12(pfx, password);
#else
            return new X509Certificate2(pfx, password);
#endif
        }

        private static CurlSslCaCertBadFileException CaBadFile(string path, string what, CurlOptions options) =>
            new CurlSslCaCertBadFileException(path, CurlException.Compose(
                what,
                "Pass a PEM file with one or more -----BEGIN CERTIFICATE----- blocks (the CA that issued the server's certificate) to --cacert, " +
                "or a folder of such files to --capath; export a Windows .cer as Base-64 to get PEM.",
                DocLinks.Ssl), options.OriginalCommand);

        /// <summary>
        /// <c>--pinnedpubkey</c>: <c>sha256//BASE64</c> pins separated by <c>;</c>, or a PEM/DER public key file; null when
        /// not given.
        /// </summary>
        /// <exception cref="CurlSslPinnedPublicKeyMismatchException">The value is neither (exit 90).</exception>
        private static List<byte[]>? LoadPins(CurlOptions options)
        {
            var value = options.PinnedPublicKey;
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var pins = new List<byte[]>();
            if (value!.StartsWith("sha256//", StringComparison.Ordinal))
            {
                foreach (var part in value.Split(';'))
                {
                    var text = part.Trim();
                    if (!text.StartsWith("sha256//", StringComparison.Ordinal))
                    {
                        throw BadPin(options, $"the pin \"{text}\" does not start with sha256//");
                    }
                    try
                    {
                        var hash = Convert.FromBase64String(text.Substring(8));
                        if (hash.Length != 32)
                        {
                            throw BadPin(options, $"the pin \"{text}\" is not a base64 SHA-256 hash (32 bytes)");
                        }
                        pins.Add(hash);
                    }
                    catch (FormatException)
                    {
                        throw BadPin(options, $"the pin \"{text}\" is not valid base64");
                    }
                }
                return pins;
            }

            // a public key file: PEM "-----BEGIN PUBLIC KEY-----" or DER SubjectPublicKeyInfo
            byte[] spki;
            try
            {
                var bytes = File.ReadAllBytes(value);
                var text = Encoding.ASCII.GetString(bytes);
                const string begin = "-----BEGIN PUBLIC KEY-----";
                const string end = "-----END PUBLIC KEY-----";
                var start = text.IndexOf(begin, StringComparison.Ordinal);
                if (start >= 0)
                {
                    var stop = text.IndexOf(end, start, StringComparison.Ordinal);
                    var body = text.Substring(start + begin.Length, (stop < 0 ? text.Length : stop) - start - begin.Length);
                    spki = Convert.FromBase64String(new string(body.Where(c => !char.IsWhiteSpace(c)).ToArray()));
                }
                else
                {
                    spki = bytes;
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is FormatException || ex is ArgumentException)
            {
                throw BadPin(options, $"the public key file \"{value}\" could not be read ({ex.Message.TrimEnd('.')})");
            }

            using var sha = SHA256.Create();
            pins.Add(sha.ComputeHash(spki));
            return pins;
        }

        private static CurlSslPinnedPublicKeyMismatchException BadPin(CurlOptions options, string what) =>
            new CurlSslPinnedPublicKeyMismatchException(CurlException.Compose(
                $"--pinnedpubkey could not be used: {what}",
                "Give sha256//BASE64 (the base64 SHA-256 of the server's public key, several separated by ;) or the path of a PEM/DER public key file; " +
                "openssl x509 -in cert.pem -pubkey -noout | openssl pkey -pubin -outform der | openssl dgst -sha256 -binary | base64 prints the pin.",
                DocLinks.Ssl), options.OriginalCommand);

        /// <summary>
        /// <c>--cert FILE[:PASSWORD]</c> with <c>--cert-type PEM|DER|P12</c>, <c>--key FILE</c>, <c>--key-type</c> and
        /// <c>--pass</c>: the client certificate with its private key, or null when no <c>--cert</c> is given.
        /// </summary>
        /// <exception cref="CurlSslCertificateProblemException">The files cannot be read or do not form a certificate with a key (exit 58).</exception>
        private static X509Certificate2? LoadClientCertificate(CurlOptions options)
        {
            if (string.IsNullOrEmpty(options.CertFile))
            {
                return null;
            }

            var certPath = options.CertFile!;
            var keyPath = string.IsNullOrEmpty(options.KeyFile) ? null : options.KeyFile;
            var password = options.CertPassword ?? options.KeyPassword;
            var type = (options.CertType ?? (certPath.EndsWith(".p12", StringComparison.OrdinalIgnoreCase) || certPath.EndsWith(".pfx", StringComparison.OrdinalIgnoreCase) ? "P12" : "PEM")).ToUpperInvariant();
            try
            {
                if (!File.Exists(certPath))
                {
                    throw CertProblem(options, $"--cert names \"{certPath}\", which does not exist");
                }
                if (keyPath != null && !File.Exists(keyPath))
                {
                    throw CertProblem(options, $"--key names \"{keyPath}\", which does not exist");
                }

                X509Certificate2 certificate;
                switch (type)
                {
                    case "P12":
                    case "PKCS12":
                    case "PFX":
                        return LoadPkcs12(File.ReadAllBytes(certPath), password);
                    case "DER":
                        if (keyPath == null)
                        {
                            throw CertProblem(options, $"--cert-type DER \"{certPath}\" holds no private key, and no --key was given");
                        }
                        certificate = WithPemKey(LoadCertificate(File.ReadAllBytes(certPath)), keyPath, options.KeyPassword ?? options.CertPassword);
                        break;
                    case "PEM":
                        var keyFile = keyPath ?? certPath;
                        var keyPassword = options.KeyPassword ?? options.CertPassword;
                        certificate = string.IsNullOrEmpty(keyPassword)
                            ? X509Certificate2.CreateFromPemFile(certPath, keyFile)
                            : X509Certificate2.CreateFromEncryptedPemFile(certPath, keyPassword.AsSpan(), keyFile);
                        break;
                    default:
                        throw CertProblem(options, $"--cert-type {options.CertType} is not one CurlDotNet can read");
                }

                // SslStream needs a key that the platform can use: an ephemeral PEM key works after a PKCS#12 round trip
                using (certificate)
                {
                    return LoadPkcs12(certificate.Export(X509ContentType.Pkcs12), null);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is CryptographicException || ex is ArgumentException)
            {
                throw CertProblem(options, $"the client certificate \"{certPath}\"{(keyPath == null ? string.Empty : " with key \"" + keyPath + "\"")} " +
                                           $"could not be loaded as {type} ({ex.Message.TrimEnd('.')})");
            }
        }

        /// <summary>Attaches the RSA or ECDSA private key of a PEM file to <paramref name="certificate"/>.</summary>
        private static X509Certificate2 WithPemKey(X509Certificate2 certificate, string keyPath, string? password)
        {
            var pem = File.ReadAllText(keyPath);
            using (certificate)
            {
                if (certificate.GetRSAPublicKey() != null)
                {
                    using var rsa = RSA.Create();
                    if (string.IsNullOrEmpty(password)) rsa.ImportFromPem(pem); else rsa.ImportFromEncryptedPem(pem, password.AsSpan());
                    return RSACertificateExtensions.CopyWithPrivateKey(certificate, rsa);
                }
                using var ecdsa = ECDsa.Create();
                if (string.IsNullOrEmpty(password)) ecdsa.ImportFromPem(pem); else ecdsa.ImportFromEncryptedPem(pem, password.AsSpan());
                return ECDsaCertificateExtensions.CopyWithPrivateKey(certificate, ecdsa);
            }
        }

        private static CurlSslCertificateProblemException CertProblem(CurlOptions options, string what) =>
            new CurlSslCertificateProblemException(CurlException.Compose(
                what,
                "Check --cert (PEM with the key in the same file or in --key, DER with --key, or P12 as --cert file.p12:password --cert-type P12), " +
                "that the key matches the certificate, and the password (--cert FILE:PASSWORD or --pass).",
                DocLinks.Ssl), options.OriginalCommand);
#endif
    }
}
