/***************************************************************************
 * DocLinks - every documentation URL that an exception message or XML doc comment points at.
 *
 * Each constant is a page that exists in the repository's docs/ folder; the unit test
 * ExceptionMessageTests.EveryDocLinkPointsAtAFileInTheRepository checks that on every build, so a
 * renamed page fails the tests instead of shipping a dead link.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Linq;

namespace CurlDotNet.Exceptions
{
    /// <summary>
    /// Documentation URLs used in exception messages ("See https://...") and suggestions.
    /// </summary>
    internal static class DocLinks
    {
        /// <summary>Prefix of every documentation page on GitHub.</summary>
        public const string Base = "https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/";

        // ---- exception pages (docs/exceptions/) ----
        public const string ExceptionsIndex = Base + "exceptions/index.md";
        public const string Auth = Base + "exceptions/auth-errors.md";
        public const string Cookie = Base + "exceptions/cookie-errors.md";
        public const string Dns = Base + "exceptions/dns-errors.md";
        public const string Http = Base + "exceptions/http-errors.md";
        public const string Init = Base + "exceptions/init-errors.md";
        public const string Redirect = Base + "exceptions/redirect-errors.md";
        public const string Ssl = Base + "exceptions/ssl-errors.md";
        public const string Timeout = Base + "exceptions/timeout-errors.md";
        public const string Url = Base + "exceptions/url-errors.md";

        // ---- troubleshooting ----
        public const string ErrorReference = Base + "troubleshooting/error-reference.md";
        public const string CommonIssues = Base + "troubleshooting/common-issues.md";
        public const string SupportedProtocols = Base + "supported-protocols.md";

        // ---- how-to guides (docs/how-to/<page>/README.md) ----
        public const string HowToConvert = Base + "how-to/convert-curl-to-csharp/README.md";
        public const string HowToBasicAuth = Base + "how-to/curl-basic-auth-csharp/README.md";
        public const string HowToBearerToken = Base + "how-to/curl-bearer-token-csharp/README.md";
        public const string HowToHeaders = Base + "how-to/curl-custom-headers-user-agent-csharp/README.md";
        public const string HowToDownload = Base + "how-to/curl-download-file-csharp/README.md";
        public const string HowToErrorHandling = Base + "how-to/curl-error-handling-csharp/README.md";
        public const string HowToUpload = Base + "how-to/curl-file-upload-csharp/README.md";
        public const string HowToFlavors = Base + "how-to/curl-flavors-bash-cmd-powershell/README.md";
        public const string HowToRedirects = Base + "how-to/curl-follow-redirects-csharp/README.md";
        public const string HowToHead = Base + "how-to/curl-head-request-csharp/README.md";
        public const string HowToIgnoreSsl = Base + "how-to/curl-ignore-ssl-certificate-csharp/README.md";
        public const string HowToAspNetCore = Base + "how-to/curl-in-aspnet-core/README.md";
        public const string HowToPostJson = Base + "how-to/curl-post-json-csharp/README.md";
        public const string HowToProxy = Base + "how-to/curl-proxy-csharp/README.md";
        public const string HowToPutDeletePatch = Base + "how-to/curl-put-delete-patch-csharp/README.md";
        public const string HowToSynchronous = Base + "how-to/curl-synchronously-in-csharp/README.md";
        public const string HowToTimeout = Base + "how-to/curl-timeout-csharp/README.md";
        public const string HowToVsHttpClient = Base + "how-to/curl-vs-httpclient-csharp/README.md";
        public const string HowItWorks = Base + "how-to/how-curldotnet-works/README.md";
        public const string HowToParseJson = Base + "how-to/parse-json-from-curl-in-csharp/README.md";
        public const string HowToRunCommand = Base + "how-to/run-curl-command-in-csharp/README.md";

        // ---- cookbook and tutorials ----
        public const string CookbookCookies = Base + "cookbook/cookies.md";
        public const string CookbookAuthSchemes = Base + "cookbook/auth-schemes.md";
        public const string CookbookProxy = Base + "cookbook/proxy.md";
        public const string CookbookTimeouts = Base + "cookbook/timeouts.md";
        public const string TutorialFilesAndDownloads = Base + "tutorials/10-files-and-downloads.md";
        public const string TutorialFormsAndData = Base + "tutorials/11-forms-and-data.md";
        public const string TutorialCancellation = Base + "tutorials/12-cancellation-tokens.md";
        public const string TutorialDebugging = Base + "tutorials/14-debugging-requests.md";
        public const string LoggingGuide = Base + "guides/logging-observability.md";
        public const string LibCurlMapping = Base + "reference/libcurl-mapping.md";
        public const string CliCompatibility = Base + "reference/curl-cli-compatibility.md";

        /// <summary>Every constant above, for the link test.</summary>
        internal static string[] All() =>
            typeof(DocLinks).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.IsLiteral && f.Name != nameof(Base))
                .Select(f => (string)f.GetRawConstantValue())
                .ToArray();

        /// <summary>
        /// The page that best explains a curl exit code, used when a message does not carry its own link.
        /// </summary>
        internal static string ForExitCode(int? code)
        {
            switch (code)
            {
                case 1: return SupportedProtocols;
                case 2: return Init;
                case 3: return Url;
                case 5:
                case 6:
                case 7:
                case 45:
                case 97:
                    return Dns;
                case 9:
                case 67:
                case 94:
                    return Auth;
                case 22:
                    return Http;
                case 28:
                    return Timeout;
                case 35:
                case 51:
                case 53:
                case 54:
                case 58:
                case 59:
                case 60:
                case 64:
                case 66:
                case 77:
                case 80:
                case 82:
                case 83:
                case 90:
                case 91:
                    return Ssl;
                case 47:
                    return Redirect;
                case 8:
                case 16:
                case 18:
                case 33:
                case 52:
                case 56:
                case 61:
                case 92:
                case 100:
                    return Http;
                default:
                    return ErrorReference;
            }
        }
    }
}
