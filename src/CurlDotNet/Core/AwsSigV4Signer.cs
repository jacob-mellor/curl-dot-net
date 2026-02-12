/***************************************************************************
 * AwsSigV4Signer - AWS Signature Version 4 request signing
 *
 * Implements the Signature Version 4 signing process for authenticating
 * HTTP requests to AWS services (and compatible providers like GCP).
 * This is a pure C# implementation with no external dependencies.
 *
 * Matches curl's --aws-sigv4 behavior from lib/vauth/aws_sigv4.c
 *
 * Specification: https://docs.aws.amazon.com/general/latest/gr/signature-version-4.html
 * Curl source: https://github.com/curl/curl/blob/master/lib/vauth/aws_sigv4.c
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Configuration parsed from the --aws-sigv4 parameter value.
    /// </summary>
    /// <remarks>
    /// <para>The parameter format is <c>provider1[:provider2[:region[:service]]]</c>.</para>
    /// <para>Only <c>provider1</c> is required; all other components are optional and will
    /// be inferred from defaults or the request URL when omitted.</para>
    ///
    /// <para><b>Provider Roles:</b></para>
    /// <list type="bullet">
    /// <item><b>provider1</b> — Used in the algorithm name (uppercased + "4-HMAC-SHA256"),
    /// the signing key derivation prefix, and the credential scope suffix.</item>
    /// <item><b>provider2</b> — Used in the date header name ("x-{provider2}-date"),
    /// the content hash header, and the security token header. Defaults to provider1 if omitted.</item>
    /// </list>
    ///
    /// <para><b>Examples:</b></para>
    /// <list type="bullet">
    /// <item><c>"aws:amz"</c> — AWS with standard Amazon header prefix</item>
    /// <item><c>"aws:amz:us-east-1:s3"</c> — AWS S3 in us-east-1</item>
    /// <item><c>"gcp:goog"</c> — Google Cloud Platform</item>
    /// </list>
    ///
    /// <para>
    /// See curl documentation: <see href="https://curl.se/libcurl/c/CURLOPT_AWS_SIGV4.html"/>
    /// </para>
    /// </remarks>
    public class AwsSigV4Config
    {
        /// <summary>
        /// Primary provider identifier used in the signing algorithm name,
        /// signing key prefix, and credential scope suffix.
        /// For AWS this is "aws", for GCP this is "gcp".
        /// </summary>
        /// <example>
        /// <code language="csharp">
        /// // For AWS: algorithm becomes "AWS4-HMAC-SHA256"
        /// // For GCP: algorithm becomes "GCP4-HMAC-SHA256"
        /// config.Provider1 = "aws";
        /// </code>
        /// </example>
        public string Provider1 { get; set; } = string.Empty;

        /// <summary>
        /// Secondary provider identifier used in header names.
        /// For AWS this is "amz" (producing "x-amz-date", "x-amz-content-sha256").
        /// Defaults to <see cref="Provider1"/> if not specified.
        /// </summary>
        /// <example>
        /// <code language="csharp">
        /// // Produces headers: x-amz-date, x-amz-content-sha256
        /// config.Provider2 = "amz";
        /// </code>
        /// </example>
        public string Provider2 { get; set; } = string.Empty;

        /// <summary>
        /// AWS region for signing (e.g., "us-east-1", "eu-west-1").
        /// When empty, the signer attempts to extract the region from the request URL hostname.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// AWS service name for signing (e.g., "s3", "execute-api", "dynamodb").
        /// When empty, the signer attempts to extract the service from the request URL hostname.
        /// </summary>
        public string Service { get; set; } = string.Empty;

        /// <summary>
        /// Parses a --aws-sigv4 parameter value into an <see cref="AwsSigV4Config"/>.
        /// </summary>
        /// <param name="value">The parameter value in format <c>provider1[:provider2[:region[:service]]]</c>.</param>
        /// <returns>A configured <see cref="AwsSigV4Config"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the value is null or empty.</exception>
        /// <example>
        /// <code language="csharp">
        /// var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");
        /// // config.Provider1 == "aws"
        /// // config.Provider2 == "amz"
        /// // config.Region == "us-east-1"
        /// // config.Service == "s3"
        /// </code>
        /// </example>
        public static AwsSigV4Config Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("--aws-sigv4 requires a provider value (e.g., 'aws:amz:us-east-1:s3'). " +
                    "See https://curl.se/libcurl/c/CURLOPT_AWS_SIGV4.html", nameof(value));
            }

            var parts = value.Split(':');
            var config = new AwsSigV4Config
            {
                Provider1 = parts[0]
            };

            if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[1]))
            {
                config.Provider2 = parts[1];
            }
            else
            {
                config.Provider2 = config.Provider1;
            }

            if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]))
            {
                config.Region = parts[2];
            }

            if (parts.Length >= 4 && !string.IsNullOrEmpty(parts[3]))
            {
                config.Service = parts[3];
            }

            return config;
        }
    }

    /// <summary>
    /// Implements AWS Signature Version 4 request signing in pure C#.
    /// </summary>
    /// <remarks>
    /// <para>This signer produces the same Authorization header that curl generates
    /// with <c>--aws-sigv4</c>. It supports any SigV4-compatible provider by parameterizing
    /// the algorithm prefix, header names, and credential scope based on the provider values.</para>
    ///
    /// <para><b>Signing Process (4 steps):</b></para>
    /// <list type="number">
    /// <item>Create a canonical request from the HTTP method, URL, headers, and payload hash</item>
    /// <item>Create a string-to-sign from the algorithm, timestamp, credential scope, and canonical request hash</item>
    /// <item>Derive a signing key using HMAC-SHA256 chain: secret → date → region → service → request type</item>
    /// <item>Calculate the signature and build the Authorization header</item>
    /// </list>
    ///
    /// <para>
    /// Specification: <see href="https://docs.aws.amazon.com/general/latest/gr/signature-version-4.html"/>
    /// </para>
    /// </remarks>
    internal static class AwsSigV4Signer
    {
        /// <summary>
        /// Signs an <see cref="HttpRequestMessage"/> using AWS Signature Version 4.
        /// Adds the required Authorization, date, and content hash headers to the request.
        /// </summary>
        /// <param name="request">The HTTP request to sign. Will be modified in-place with signing headers.</param>
        /// <param name="config">The SigV4 configuration specifying provider, region, and service.</param>
        /// <param name="accessKey">The access key ID (from --user parameter, before the colon).</param>
        /// <param name="secretKey">The secret access key (from --user parameter, after the colon).</param>
        /// <param name="timestamp">Optional timestamp override for deterministic testing. Uses UTC now if null.</param>
        /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when region or service cannot be determined.</exception>
        public static void SignRequest(HttpRequestMessage request, AwsSigV4Config config,
            string accessKey, string secretKey, DateTime? timestamp = null)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrEmpty(accessKey)) throw new ArgumentNullException(nameof(accessKey));
            if (string.IsNullOrEmpty(secretKey)) throw new ArgumentNullException(nameof(secretKey));

            var now = timestamp ?? DateTime.UtcNow;
            var dateStamp = now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var amzDate = now.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);

            var provider1Upper = config.Provider1.ToUpperInvariant();
            var provider2Lower = config.Provider2.ToLowerInvariant();

            // Resolve region and service from URL if not specified
            var uri = request.RequestUri;
            var region = !string.IsNullOrEmpty(config.Region) ? config.Region : ExtractRegion(uri);
            var service = !string.IsNullOrEmpty(config.Service) ? config.Service : ExtractService(uri);

            // Algorithm: e.g., "AWS4-HMAC-SHA256"
            var algorithm = $"{provider1Upper}4-HMAC-SHA256";

            // Date header name: e.g., "x-amz-date"
            var dateHeaderName = $"x-{provider2Lower}-date";

            // Content hash header: e.g., "x-amz-content-sha256"
            var contentHashHeaderName = $"x-{provider2Lower}-content-sha256";

            // Hash the payload
            var payloadHash = HashPayload(request);

            // Add required headers before signing
            request.Headers.TryAddWithoutValidation(dateHeaderName, amzDate);
            request.Headers.TryAddWithoutValidation(contentHashHeaderName, payloadHash);

            // Ensure Host header is present
            if (!request.Headers.Contains("Host"))
            {
                request.Headers.Host = uri.Host;
            }

            // Step 1: Create canonical request
            var (canonicalRequest, signedHeaders) = CreateCanonicalRequest(request, payloadHash);

            // Step 2: Create string to sign
            var credentialScope = $"{dateStamp}/{region}/{service}/{config.Provider1.ToLowerInvariant()}4_request";
            var stringToSign = CreateStringToSign(algorithm, amzDate, credentialScope, canonicalRequest);

            // Step 3: Calculate signing key
            var signingKey = DeriveSigningKey(secretKey, dateStamp, region, service, config.Provider1);

            // Step 4: Calculate signature
            var signature = HexEncode(HmacSha256(signingKey, stringToSign));

            // Build Authorization header using typed API for proper header parsing
            var authParameter = $"Credential={accessKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(algorithm, authParameter);
        }

        /// <summary>
        /// Creates the canonical request string per the SigV4 specification.
        /// </summary>
        /// <returns>A tuple of (canonical request string, signed headers string).</returns>
        private static (string CanonicalRequest, string SignedHeaders) CreateCanonicalRequest(
            HttpRequestMessage request, string payloadHash)
        {
            var method = request.Method.Method;
            var uri = request.RequestUri;

            // Canonical URI - URL-encode path segments
            var canonicalUri = uri.AbsolutePath;
            if (string.IsNullOrEmpty(canonicalUri))
            {
                canonicalUri = "/";
            }

            // Canonical query string - sort by parameter name
            var canonicalQueryString = BuildCanonicalQueryString(uri.Query);

            // Collect all headers (request headers + content headers)
            var allHeaders = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in request.Headers)
            {
                allHeaders[header.Key.ToLowerInvariant()] = string.Join(",", header.Value).Trim();
            }
            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    allHeaders[header.Key.ToLowerInvariant()] = string.Join(",", header.Value).Trim();
                }
            }

            // Build canonical headers and signed headers
            var canonicalHeadersBuilder = new StringBuilder();
            var signedHeadersList = new List<string>();

            foreach (var header in allHeaders)
            {
                canonicalHeadersBuilder.Append($"{header.Key}:{header.Value}\n");
                signedHeadersList.Add(header.Key);
            }

            var canonicalHeaders = canonicalHeadersBuilder.ToString();
            var signedHeaders = string.Join(";", signedHeadersList);

            // Build canonical request
            var canonicalRequest = $"{method}\n{canonicalUri}\n{canonicalQueryString}\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";

            return (canonicalRequest, signedHeaders);
        }

        /// <summary>
        /// Builds the canonical query string by sorting parameters alphabetically.
        /// </summary>
        private static string BuildCanonicalQueryString(string query)
        {
            if (string.IsNullOrEmpty(query) || query == "?")
            {
                return string.Empty;
            }

            // Remove leading ?
            if (query.StartsWith("?"))
            {
                query = query.Substring(1);
            }

            var parameters = query.Split('&')
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p =>
                {
                    var parts = p.Split(new[] { '=' }, 2);
                    var key = Uri.EscapeDataString(Uri.UnescapeDataString(parts[0]));
                    var value = parts.Length > 1 ? Uri.EscapeDataString(Uri.UnescapeDataString(parts[1])) : string.Empty;
                    return $"{key}={value}";
                })
                .OrderBy(p => p, StringComparer.Ordinal);

            return string.Join("&", parameters);
        }

        /// <summary>
        /// Creates the string to sign from the algorithm, timestamp, credential scope, and canonical request hash.
        /// </summary>
        private static string CreateStringToSign(string algorithm, string amzDate,
            string credentialScope, string canonicalRequest)
        {
            var hashedCanonicalRequest = HexEncode(Sha256Hash(canonicalRequest));
            return $"{algorithm}\n{amzDate}\n{credentialScope}\n{hashedCanonicalRequest}";
        }

        /// <summary>
        /// Derives the signing key using the HMAC-SHA256 chain.
        /// </summary>
        /// <remarks>
        /// The derivation follows this chain:
        /// <code>
        /// kDate    = HMAC("PROVIDER4" + secretKey, dateStamp)
        /// kRegion  = HMAC(kDate, region)
        /// kService = HMAC(kRegion, service)
        /// kSigning = HMAC(kService, "provider4_request")
        /// </code>
        /// </remarks>
        private static byte[] DeriveSigningKey(string secretKey, string dateStamp,
            string region, string service, string provider1)
        {
            var provider1Upper = provider1.ToUpperInvariant();
            var provider1Lower = provider1.ToLowerInvariant();

            var kSecret = Encoding.UTF8.GetBytes($"{provider1Upper}4{secretKey}");
            var kDate = HmacSha256(kSecret, dateStamp);
            var kRegion = HmacSha256(kDate, region);
            var kService = HmacSha256(kRegion, service);
            var kSigning = HmacSha256(kService, $"{provider1Lower}4_request");

            return kSigning;
        }

        /// <summary>
        /// Hashes the request payload (body) using SHA-256.
        /// Returns the hex-encoded hash, or the hash of an empty string if no body.
        /// </summary>
        private static string HashPayload(HttpRequestMessage request)
        {
            if (request.Content == null)
            {
                return HexEncode(Sha256Hash(string.Empty));
            }

            // Read content bytes synchronously for signing
            var contentBytes = request.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            return HexEncode(Sha256HashBytes(contentBytes));
        }

        /// <summary>
        /// Attempts to extract the AWS region from the hostname.
        /// Handles common patterns like:
        /// <list type="bullet">
        /// <item>s3.us-east-1.amazonaws.com → us-east-1</item>
        /// <item>dynamodb.eu-west-1.amazonaws.com → eu-west-1</item>
        /// <item>my-api.execute-api.ap-southeast-2.amazonaws.com → ap-southeast-2</item>
        /// </list>
        /// </summary>
        internal static string ExtractRegion(Uri uri)
        {
            var host = uri.Host;
            var parts = host.Split('.');

            // Look for region pattern (e.g., us-east-1, eu-west-2, ap-southeast-1)
            for (int i = 0; i < parts.Length; i++)
            {
                if (IsRegionLike(parts[i]))
                {
                    return parts[i];
                }
            }

            // Default to us-east-1 if we can't determine the region
            return "us-east-1";
        }

        /// <summary>
        /// Attempts to extract the AWS service name from the hostname.
        /// Handles common patterns like:
        /// <list type="bullet">
        /// <item>s3.us-east-1.amazonaws.com → s3</item>
        /// <item>dynamodb.eu-west-1.amazonaws.com → dynamodb</item>
        /// <item>my-api.execute-api.ap-southeast-2.amazonaws.com → execute-api</item>
        /// </list>
        /// </summary>
        internal static string ExtractService(Uri uri)
        {
            var host = uri.Host;
            var parts = host.Split('.');

            // For amazonaws.com or similar, the service is typically the first subdomain
            // or the subdomain before the region
            if (parts.Length >= 3)
            {
                // Check common AWS service patterns
                // s3.amazonaws.com, s3.us-east-1.amazonaws.com
                if (!IsRegionLike(parts[0]) && parts[0] != "amazonaws" && parts[0] != "com")
                {
                    return parts[0];
                }
            }

            // Default to the first part of the hostname
            return parts.Length > 0 ? parts[0] : "service";
        }

        /// <summary>
        /// Checks whether a hostname part looks like an AWS region identifier.
        /// </summary>
        private static bool IsRegionLike(string part)
        {
            // AWS regions follow patterns like: us-east-1, eu-west-2, ap-southeast-1, etc.
            if (string.IsNullOrEmpty(part)) return false;

            var regionPrefixes = new[] { "us-", "eu-", "ap-", "sa-", "ca-", "me-", "af-", "il-" };
            foreach (var prefix in regionPrefixes)
            {
                if (part.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Computes SHA-256 hash of a string.
        /// </summary>
        private static byte[] Sha256Hash(string data)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        /// <summary>
        /// Computes SHA-256 hash of a byte array.
        /// </summary>
        private static byte[] Sha256HashBytes(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(data);
            }
        }

        /// <summary>
        /// Computes HMAC-SHA256 of a string using a byte array key.
        /// </summary>
        private static byte[] HmacSha256(byte[] key, string data)
        {
            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        /// <summary>
        /// Hex-encodes a byte array to a lowercase string.
        /// </summary>
        private static string HexEncode(byte[] data)
        {
            var builder = new StringBuilder(data.Length * 2);
            foreach (var b in data)
            {
                builder.AppendFormat("{0:x2}", b);
            }
            return builder.ToString();
        }
    }
}
