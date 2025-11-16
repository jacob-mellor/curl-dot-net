using System;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;

namespace CurlDotNet.Extensions
{
    /// <summary>
    /// Extension methods for string to provide syntactic sugar for curl operations.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Executes a curl command directly from a string.
        /// </summary>
        /// <param name="command">The curl command string</param>
        /// <returns>The curl result</returns>
        /// <example>
        /// var result = await "curl https://api.github.com".CurlAsync();
        /// var json = await "https://api.example.com/data".CurlGetAsync();
        /// </example>
        public static async Task<CurlResult> CurlAsync(this string command)
        {
            return await CurlDotNet.Curl.ExecuteAsync(command);
        }

        /// <summary>
        /// Executes a curl command with cancellation support.
        /// </summary>
        public static async Task<CurlResult> CurlAsync(this string command, CancellationToken cancellationToken)
        {
            return await CurlDotNet.Curl.ExecuteAsync(command, cancellationToken);
        }

        /// <summary>
        /// Performs a GET request on the URL.
        /// </summary>
        /// <param name="url">The URL to GET</param>
        /// <returns>The curl result</returns>
        /// <example>
        /// var result = await "https://api.github.com/users/octocat".CurlGetAsync();
        /// </example>
        public static async Task<CurlResult> CurlGetAsync(this string url)
        {
            var command = url.StartsWith("curl ") ? url : $"curl {url}";
            return await CurlDotNet.Curl.ExecuteAsync(command);
        }

        /// <summary>
        /// Performs a POST request with JSON data.
        /// </summary>
        /// <param name="url">The URL to POST to</param>
        /// <param name="json">The JSON data to send</param>
        /// <returns>The curl result</returns>
        /// <example>
        /// var result = await "https://api.example.com/users".CurlPostJsonAsync(@"{""name"":""John""}");
        /// </example>
        public static async Task<CurlResult> CurlPostJsonAsync(this string url, string json)
        {
            var command = $@"curl -X POST -H 'Content-Type: application/json' -d '{json}' {url}";
            return await CurlDotNet.Curl.ExecuteAsync(command);
        }

        /// <summary>
        /// Downloads a file from the URL.
        /// </summary>
        /// <param name="url">The URL to download from</param>
        /// <param name="outputFile">The file path to save to</param>
        /// <returns>The curl result</returns>
        /// <example>
        /// await "https://example.com/file.pdf".CurlDownloadAsync("local.pdf");
        /// </example>
        public static async Task<CurlResult> CurlDownloadAsync(this string url, string outputFile)
        {
            var command = $"curl -o {outputFile} {url}";
            return await CurlDotNet.Curl.ExecuteAsync(command);
        }

        /// <summary>
        /// Executes curl synchronously (blocking).
        /// </summary>
        /// <param name="command">The curl command string</param>
        /// <returns>The curl result</returns>
        /// <example>
        /// var result = "curl https://api.github.com".Curl();
        /// </example>
        public static CurlResult Curl(this string command)
        {
            return CurlAsync(command).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Quick one-liner to get response body as string.
        /// </summary>
        /// <param name="url">The URL to fetch</param>
        /// <returns>The response body</returns>
        /// <example>
        /// string json = await "https://api.github.com/users/octocat".CurlBodyAsync();
        /// </example>
        public static async Task<string> CurlBodyAsync(this string url)
        {
            var result = await url.CurlGetAsync();
            return result.Body;
        }
    }

    /// <summary>
    /// Extension methods for fluent curl building.
    /// </summary>
    public static class FluentExtensions
    {
        /// <summary>
        /// Starts building a curl command from a URL.
        /// </summary>
        /// <example>
        /// var result = await "https://api.example.com"
        ///     .WithHeader("Authorization", "Bearer token")
        ///     .WithData(@"{""key"":""value""}")
        ///     .ExecuteAsync();
        /// </example>
        public static CurlRequestBuilder ToCurl(this string url)
        {
            return CurlRequestBuilder.Get(url);
        }

        /// <summary>
        /// Adds a header to the curl builder.
        /// </summary>
        public static CurlRequestBuilder WithHeader(this string url, string key, string value)
        {
            return CurlRequestBuilder.Get(url).WithHeader(key, value);
        }

        /// <summary>
        /// Sets the HTTP method.
        /// </summary>
        public static CurlRequestBuilder WithMethod(this string url, string method)
        {
            return CurlRequestBuilder.Request(method, url);
        }
    }
}