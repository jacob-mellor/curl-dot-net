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
    /// The four parts of a <c>--aws-sigv4 provider1[:provider2[:region[:service]]]</c> value, which tell
    /// CurlDotNet how to sign a request with AWS Signature Version 4 (or a compatible scheme such as GCP's).
    /// </summary>
    /// <remarks>
    /// <para>You normally never build this yourself: <c>curl --aws-sigv4 "aws:amz:us-east-1:s3" -u ACCESS_KEY:SECRET_KEY URL</c>
    /// fills <see cref="CurlOptions.AwsSigV4"/> with it, and the request is signed just before it is sent.
    /// Set it directly only when you build <see cref="CurlOptions"/> in code:
    /// <c>options.AwsSigV4 = AwsSigV4Config.Parse("aws:amz:eu-west-1:execute-api");</c>.</para>
    /// <para>The parts: <see cref="Provider1"/> (required, <c>aws</c> or <c>gcp</c>) names the algorithm
    /// (<c>AWS4-HMAC-SHA256</c>) and the key-derivation prefix; <see cref="Provider2"/> (default: provider1,
    /// <c>amz</c> for AWS) names the headers (<c>x-amz-date</c>, <c>x-amz-content-sha256</c>);
    /// <see cref="Region"/> (default: taken from the host, else <c>us-east-1</c>) and <see cref="Service"/>
    /// (default: the first label of the host, such as <c>s3</c>) go into the credential scope.</para>
    /// <para>The access key and secret key are not part of this value: pass them with
    /// <c>-u ACCESS_KEY_ID:SECRET_ACCESS_KEY</c>. For temporary credentials add
    /// <c>-H "x-amz-security-token: TOKEN"</c>. When <c>--oauth2-bearer</c> is also given, no SigV4 signature is added.</para>
    /// <para>Guide with a full S3 example: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md.
    /// curl's own reference: https://curl.se/libcurl/c/CURLOPT_AWS_SIGV4.html.</para>
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// var result = await Curl.ExecuteAsync(
    ///     "curl --aws-sigv4 'aws:amz:us-east-1:s3' -u AKIA...:wJalr... https://my-bucket.s3.amazonaws.com/object.txt");
    /// </code>
    /// </example>
    /// <seealso cref="Parse(string)"/>
    /// <seealso cref="CurlOptions.AwsSigV4"/>
    public class AwsSigV4Config
    {
        /// <summary>
        /// The provider that names the signing algorithm and key prefix: <c>aws</c> gives <c>AWS4-HMAC-SHA256</c>,
        /// <c>gcp</c> gives <c>GCP4-HMAC-SHA256</c>. The first part of the <c>--aws-sigv4</c> value; required.
        /// </summary>
        /// <value>A lowercase identifier such as <c>aws</c> or <c>gcp</c>; empty only for an instance you built
        /// yourself without setting it (signing then produces an invalid signature).</value>
        /// <remarks>
        /// <para>It is upper-cased for the algorithm name (<c>AWS4-HMAC-SHA256</c>), prefixed to the secret when
        /// deriving the signing key (<c>AWS4</c> + secret) and lower-cased in the credential scope suffix
        /// (<c>aws4_request</c>).</para>
        /// <para>Example: <c>AwsSigV4Config.Parse("aws:amz").Provider1</c> is <c>"aws"</c>.</para>
        /// </remarks>
        /// <seealso cref="Provider2"/>
        /// <seealso cref="Parse(string)"/>
        public string Provider1 { get; set; } = string.Empty;

        /// <summary>
        /// The provider that names the signing headers: <c>amz</c> gives <c>x-amz-date</c> and
        /// <c>x-amz-content-sha256</c>. The second part of the <c>--aws-sigv4</c> value; defaults to <see cref="Provider1"/>.
        /// </summary>
        /// <value>A lowercase identifier such as <c>amz</c> (AWS) or <c>goog</c> (GCP).</value>
        /// <remarks>
        /// <para>AWS needs <c>aws:amz</c>: with only <c>aws</c> the headers would be <c>x-aws-date</c>, which AWS rejects.</para>
        /// <para>Example: <c>AwsSigV4Config.Parse("aws").Provider2</c> is <c>"aws"</c>; <c>Parse("aws:amz").Provider2</c> is <c>"amz"</c>.</para>
        /// </remarks>
        /// <seealso cref="Provider1"/>
        public string Provider2 { get; set; } = string.Empty;

        /// <summary>
        /// The region in the credential scope, such as <c>us-east-1</c> or <c>eu-west-1</c>. The third part of the
        /// <c>--aws-sigv4</c> value; when empty it is taken from the host name.
        /// </summary>
        /// <value>An AWS region code, or empty to detect it: the first host label that looks like a region
        /// (<c>s3.eu-west-2.amazonaws.com</c> gives <c>eu-west-2</c>), else <c>us-east-1</c>.</value>
        /// <remarks>
        /// <para>A wrong region makes AWS answer 403 with "The request signature we calculated does not match"
        /// or "credential should be scoped to a valid region"; set it explicitly for custom domains and
        /// S3-compatible stores.</para>
        /// <para>Example: <c>--aws-sigv4 "aws:amz:ap-southeast-2:s3"</c>.</para>
        /// </remarks>
        /// <seealso cref="Service"/>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// The service in the credential scope, such as <c>s3</c>, <c>execute-api</c> or <c>dynamodb</c>. The fourth
        /// part of the <c>--aws-sigv4</c> value; when empty it is taken from the host name.
        /// </summary>
        /// <value>An AWS service code, or empty to use the first label of the host
        /// (<c>dynamodb.us-east-1.amazonaws.com</c> gives <c>dynamodb</c>).</value>
        /// <remarks>
        /// <para>Set it explicitly whenever the first host label is not the service: API Gateway URLs such as
        /// <c>abc123.execute-api.us-east-1.amazonaws.com</c> need <c>execute-api</c>, and bucket-style S3 URLs such
        /// as <c>my-bucket.s3.amazonaws.com</c> need <c>s3</c>.</para>
        /// <para>Example: <c>--aws-sigv4 "aws:amz:us-east-1:execute-api"</c>.</para>
        /// </remarks>
        /// <seealso cref="Region"/>
        public string Service { get; set; } = string.Empty;

        /// <summary>
        /// Reads a <c>--aws-sigv4</c> value such as <c>aws:amz:us-east-1:s3</c> into its provider, header provider,
        /// region and service parts; missing parts fall back to their defaults.
        /// </summary>
        /// <remarks>
        /// <para>Accepted format: <c>provider1[:provider2[:region[:service]]]</c>, colon-separated, with empty parts
        /// allowed (<c>aws::us-east-1</c> keeps <see cref="Provider2"/> equal to <see cref="Provider1"/>). Nothing is
        /// validated against AWS: a typo in the region only shows up as a 403 from the service.</para>
        /// <para>Example: <c>var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");</c> gives
        /// <c>Provider1 = "aws"</c>, <c>Provider2 = "amz"</c>, <c>Region = "us-east-1"</c>, <c>Service = "s3"</c>;
        /// <c>AwsSigV4Config.Parse("aws")</c> gives <c>Provider2 = "aws"</c> and empty region and service (detected from the URL).</para>
        /// <para>The command line parser calls this for <c>--aws-sigv4</c>. Guide:
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md</para>
        /// </remarks>
        /// <param name="value">The <c>--aws-sigv4</c> argument, for example <c>"aws:amz:us-east-1:s3"</c> or <c>"gcp:goog"</c>.
        /// Must contain at least the first provider.</param>
        /// <returns>A new configuration whose <see cref="Provider1"/> is always set, <see cref="Provider2"/> defaults to
        /// it, and <see cref="Region"/>/<see cref="Service"/> are empty when not given (detected from the URL when signing).</returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> is null, empty or whitespace. Fix: pass at least
        /// the provider, for example <c>--aws-sigv4 "aws:amz:us-east-1:s3"</c>. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md</exception>
        /// <example>
        /// <code language="csharp">
        /// var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");
        /// // config.Provider1 == "aws", config.Provider2 == "amz", config.Region == "us-east-1", config.Service == "s3"
        /// </code>
        /// </example>
        /// <seealso cref="CurlOptions.AwsSigV4"/>
        public static AwsSigV4Config Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(CurlDotNet.Exceptions.CurlException.Compose(
                    $"--aws-sigv4 was given {(value == null ? "no value" : "an empty value")}, but it needs at least a provider such as 'aws'.",
                    "Pass provider1[:provider2[:region[:service]]], for example --aws-sigv4 \"aws:amz:us-east-1:s3\", together with -u ACCESS_KEY_ID:SECRET_ACCESS_KEY.",
                    CurlDotNet.Exceptions.DocLinks.CookbookAuthSchemes), nameof(value));
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
        private static string Missing(string what, string fix) => CurlDotNet.Exceptions.CurlException.Compose(
            $"AWS SigV4 signing (--aws-sigv4) cannot run: {what} is missing.", fix, CurlDotNet.Exceptions.DocLinks.CookbookAuthSchemes);

        /// <summary>
        /// Signs an <see cref="HttpRequestMessage"/> using AWS Signature Version 4.
        /// Adds the required Authorization, date, and content hash headers to the request.
        /// </summary>
        /// <param name="request">The HTTP request to sign. Will be modified in-place with signing headers.</param>
        /// <param name="config">The SigV4 configuration specifying provider, region, and service.</param>
        /// <param name="accessKey">The access key ID (from --user parameter, before the colon).</param>
        /// <param name="secretKey">The secret access key (from --user parameter, after the colon).</param>
        /// <param name="timestamp">Optional timestamp override for deterministic testing. Uses UTC now if null.</param>
        /// <exception cref="ArgumentNullException">A parameter is null, or a key is empty. Region and service never
        /// fail: they fall back to the host name and <c>us-east-1</c>.</exception>
        public static void SignRequest(HttpRequestMessage request, AwsSigV4Config config,
            string accessKey, string secretKey, DateTime? timestamp = null)
        {
            if (request == null) throw new ArgumentNullException(nameof(request), Missing("the HttpRequestMessage to sign", "Build the request (with its URL and content) before signing it."));
            if (config == null) throw new ArgumentNullException(nameof(config), Missing("the --aws-sigv4 configuration", "Pass AwsSigV4Config.Parse(\"aws:amz:REGION:SERVICE\")."));
            if (string.IsNullOrEmpty(accessKey)) throw new ArgumentNullException(nameof(accessKey), Missing("the access key ID", "Pass the keys as -u ACCESS_KEY_ID:SECRET_ACCESS_KEY."));
            if (string.IsNullOrEmpty(secretKey)) throw new ArgumentNullException(nameof(secretKey), Missing("the secret access key", "Pass the keys as -u ACCESS_KEY_ID:SECRET_ACCESS_KEY."));

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
