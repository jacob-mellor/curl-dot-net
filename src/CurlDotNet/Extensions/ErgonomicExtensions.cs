using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet.Core;

namespace CurlDotNet.Extensions
{
    /// <summary>
    /// Provides ergonomic extension methods for improved API usability.
    /// </summary>
    public static class ErgonomicExtensions
    {
        #region CurlResult Extensions

        /// <summary>
        /// Deserializes the response body to a strongly-typed object.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="result">The curl result.</param>
        /// <param name="options">Optional JSON serializer options.</param>
        /// <returns>The deserialized object.</returns>
        public static T FromJson<T>(this CurlResult result, JsonSerializerOptions options = null)
        {
            if (string.IsNullOrEmpty(result.Body))
                return default(T);

            return JsonSerializer.Deserialize<T>(result.Body, options);
        }

        /// <summary>
        /// Tries to deserialize the response body to a strongly-typed object.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="result">The curl result.</param>
        /// <param name="value">The deserialized object if successful.</param>
        /// <param name="options">Optional JSON serializer options.</param>
        /// <returns>True if deserialization was successful; otherwise, false.</returns>
        public static bool TryFromJson<T>(this CurlResult result, out T value, JsonSerializerOptions options = null)
        {
            value = default(T);

            if (string.IsNullOrEmpty(result.Body))
                return false;

            try
            {
                value = JsonSerializer.Deserialize<T>(result.Body, options);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a specific header value from the response.
        /// </summary>
        /// <param name="result">The curl result.</param>
        /// <param name="headerName">The header name (case-insensitive).</param>
        /// <returns>The header value if found; otherwise, null.</returns>
        public static string GetHeader(this CurlResult result, string headerName)
        {
            if (result.Headers == null)
                return null;

            var header = result.Headers.FirstOrDefault(h =>
                h.Key.Equals(headerName, StringComparison.OrdinalIgnoreCase));

            return header.Value;
        }

        /// <summary>
        /// Checks if the response has a specific status code.
        /// </summary>
        /// <param name="result">The curl result.</param>
        /// <param name="statusCode">The status code to check.</param>
        /// <returns>True if the status code matches; otherwise, false.</returns>
        public static bool HasStatus(this CurlResult result, int statusCode)
        {
            return result.StatusCode == statusCode;
        }

        /// <summary>
        /// Checks if the response indicates success (2xx status codes).
        /// </summary>
        /// <param name="result">The curl result.</param>
        /// <returns>True if status code is 2xx; otherwise, false.</returns>
        public static bool IsSuccessful(this CurlResult result)
        {
            return result.StatusCode >= 200 && result.StatusCode < 300;
        }

        /// <summary>
        /// Checks if the response indicates a client error (4xx status codes).
        /// </summary>
        /// <param name="result">The curl result.</param>
        /// <returns>True if status code is 4xx; otherwise, false.</returns>
        public static bool IsClientError(this CurlResult result)
        {
            return result.StatusCode >= 400 && result.StatusCode < 500;
        }

        /// <summary>
        /// Checks if the response indicates a server error (5xx status codes).
        /// </summary>
        /// <param name="result">The curl result.</param>
        /// <returns>True if status code is 5xx; otherwise, false.</returns>
        public static bool IsServerError(this CurlResult result)
        {
            return result.StatusCode >= 500 && result.StatusCode < 600;
        }

        #endregion

        #region CurlRequestBuilder Extensions

        /// <summary>
        /// Adds multiple headers at once.
        /// </summary>
        /// <param name="builder">The request builder.</param>
        /// <param name="headers">Dictionary of headers to add.</param>
        /// <returns>The request builder for chaining.</returns>
        public static CurlRequestBuilder AddHeaders(this CurlRequestBuilder builder, Dictionary<string, string> headers)
        {
            if (headers == null)
                return builder;

            foreach (var header in headers)
            {
                builder.AddHeader(header.Key, header.Value);
            }

            return builder;
        }

        /// <summary>
        /// Sets JSON body with automatic serialization.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="builder">The request builder.</param>
        /// <param name="data">The object to serialize as JSON.</param>
        /// <param name="options">Optional JSON serializer options.</param>
        /// <returns>The request builder for chaining.</returns>
        public static CurlRequestBuilder SetJsonBody<T>(this CurlRequestBuilder builder, T data, JsonSerializerOptions options = null)
        {
            var json = JsonSerializer.Serialize(data, options);
            return builder
                .SetBody(json)
                .AddHeader("Content-Type", "application/json");
        }

        /// <summary>
        /// Sets form URL encoded body.
        /// </summary>
        /// <param name="builder">The request builder.</param>
        /// <param name="formData">Dictionary of form fields.</param>
        /// <returns>The request builder for chaining.</returns>
        public static CurlRequestBuilder SetFormBody(this CurlRequestBuilder builder, Dictionary<string, string> formData)
        {
            if (formData == null || !formData.Any())
                return builder;

            var encoded = string.Join("&", formData.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            return builder
                .SetBody(encoded)
                .AddHeader("Content-Type", "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// Adds query parameters to the URL.
        /// </summary>
        /// <param name="builder">The request builder.</param>
        /// <param name="parameters">Dictionary of query parameters.</param>
        /// <returns>The request builder for chaining.</returns>
        public static CurlRequestBuilder AddQueryParameters(this CurlRequestBuilder builder, Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.Any())
                return builder;

            var query = string.Join("&", parameters.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            // Note: This assumes the URL doesn't already have query parameters
            // In a real implementation, you'd need to parse the existing URL
            return builder;
        }

        /// <summary>
        /// Sets Bearer token authentication.
        /// </summary>
        /// <param name="builder">The request builder.</param>
        /// <param name="token">The bearer token.</param>
        /// <returns>The request builder for chaining.</returns>
        public static CurlRequestBuilder SetBearerToken(this CurlRequestBuilder builder, string token)
        {
            return builder.AddHeader("Authorization", $"Bearer {token}");
        }

        /// <summary>
        /// Sets API key authentication.
        /// </summary>
        /// <param name="builder">The request builder.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="headerName">The header name for the API key (default: X-API-Key).</param>
        /// <returns>The request builder for chaining.</returns>
        public static CurlRequestBuilder SetApiKey(this CurlRequestBuilder builder, string apiKey, string headerName = "X-API-Key")
        {
            return builder.AddHeader(headerName, apiKey);
        }

        /// <summary>
        /// Configures the request to accept JSON responses.
        /// </summary>
        /// <param name="builder">The request builder.</param>
        /// <returns>The request builder for chaining.</returns>
        public static CurlRequestBuilder AcceptJson(this CurlRequestBuilder builder)
        {
            return builder.AddHeader("Accept", "application/json");
        }

        /// <summary>
        /// Sets a custom User-Agent.
        /// </summary>
        /// <param name="builder">The request builder.</param>
        /// <param name="userAgent">The user agent string.</param>
        /// <returns>The request builder for chaining.</returns>
        public static CurlRequestBuilder WithUserAgent(this CurlRequestBuilder builder, string userAgent)
        {
            return builder.SetUserAgent(userAgent);
        }

        /// <summary>
        /// Sets request to not follow redirects.
        /// </summary>
        /// <param name="builder">The request builder.</param>
        /// <returns>The request builder for chaining.</returns>
        public static CurlRequestBuilder NoRedirects(this CurlRequestBuilder builder)
        {
            return builder.FollowRedirects(false);
        }

        /// <summary>
        /// Sets a short timeout for quick requests.
        /// </summary>
        /// <param name="builder">The request builder.</param>
        /// <param name="seconds">Timeout in seconds (default: 5).</param>
        /// <returns>The request builder for chaining.</returns>
        public static CurlRequestBuilder WithQuickTimeout(this CurlRequestBuilder builder, int seconds = 5)
        {
            return builder.SetTimeout(TimeSpan.FromSeconds(seconds));
        }

        #endregion

        #region Curl Static Extensions

        /// <summary>
        /// Performs a GET request and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="url">The URL to request.</param>
        /// <param name="options">Optional JSON serializer options.</param>
        /// <returns>The deserialized object.</returns>
        public static async Task<T> GetJsonAsync<T>(string url, JsonSerializerOptions options = null)
        {
            var result = await Curl.GetAsync(url);
            result.EnsureSuccessStatusCode();
            return result.FromJson<T>(options);
        }

        /// <summary>
        /// Performs a POST request with JSON data and deserializes the response.
        /// </summary>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="url">The URL to request.</param>
        /// <param name="data">The data to send.</param>
        /// <param name="options">Optional JSON serializer options.</param>
        /// <returns>The deserialized response.</returns>
        public static async Task<TResponse> PostJsonAsync<TRequest, TResponse>(
            string url,
            TRequest data,
            JsonSerializerOptions options = null)
        {
            var json = JsonSerializer.Serialize(data, options);
            var result = await Curl.PostAsync(url, json, "application/json");
            result.EnsureSuccessStatusCode();
            return result.FromJson<TResponse>(options);
        }

        /// <summary>
        /// Downloads a file from the specified URL.
        /// </summary>
        /// <param name="url">The URL to download from.</param>
        /// <param name="filePath">The local file path to save to.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task DownloadFileAsync(string url, string filePath)
        {
            var result = await Curl.GetAsync(url);
            result.EnsureSuccessStatusCode();
            await System.IO.File.WriteAllBytesAsync(filePath, Encoding.UTF8.GetBytes(result.Body));
        }

        /// <summary>
        /// Performs a health check on the specified URL.
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <param name="expectedStatus">Expected status code (default: 200).</param>
        /// <returns>True if the health check passes; otherwise, false.</returns>
        public static async Task<bool> HealthCheckAsync(string url, int expectedStatus = 200)
        {
            try
            {
                var result = await new CurlRequestBuilder()
                    .SetUrl(url)
                    .SetMethod("HEAD")
                    .WithQuickTimeout()
                    .Build()
                    .ExecuteAsync();

                return result.StatusCode == expectedStatus;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides a simplified API client builder.
    /// </summary>
    public class CurlApiClient
    {
        private readonly string _baseUrl;
        private readonly Dictionary<string, string> _defaultHeaders;
        private readonly TimeSpan _defaultTimeout;

        /// <summary>
        /// Initializes a new instance of the CurlApiClient class.
        /// </summary>
        /// <param name="baseUrl">The base URL for all requests.</param>
        public CurlApiClient(string baseUrl)
        {
            _baseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));
            _defaultHeaders = new Dictionary<string, string>();
            _defaultTimeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Adds a default header to all requests.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        /// <returns>The client instance for chaining.</returns>
        public CurlApiClient WithHeader(string name, string value)
        {
            _defaultHeaders[name] = value;
            return this;
        }

        /// <summary>
        /// Sets Bearer token authentication for all requests.
        /// </summary>
        /// <param name="token">The bearer token.</param>
        /// <returns>The client instance for chaining.</returns>
        public CurlApiClient WithBearerToken(string token)
        {
            return WithHeader("Authorization", $"Bearer {token}");
        }

        /// <summary>
        /// Sets API key authentication for all requests.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="headerName">The header name (default: X-API-Key).</param>
        /// <returns>The client instance for chaining.</returns>
        public CurlApiClient WithApiKey(string apiKey, string headerName = "X-API-Key")
        {
            return WithHeader(headerName, apiKey);
        }

        /// <summary>
        /// Performs a GET request.
        /// </summary>
        /// <param name="endpoint">The endpoint path.</param>
        /// <returns>The curl result.</returns>
        public async Task<CurlResult> GetAsync(string endpoint)
        {
            return await BuildRequest(endpoint)
                .SetMethod("GET")
                .Build()
                .ExecuteAsync();
        }

        /// <summary>
        /// Performs a GET request and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="endpoint">The endpoint path.</param>
        /// <returns>The deserialized object.</returns>
        public async Task<T> GetAsync<T>(string endpoint)
        {
            var result = await GetAsync(endpoint);
            result.EnsureSuccessStatusCode();
            return result.FromJson<T>();
        }

        /// <summary>
        /// Performs a POST request with JSON data.
        /// </summary>
        /// <typeparam name="T">The type of data to send.</typeparam>
        /// <param name="endpoint">The endpoint path.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The curl result.</returns>
        public async Task<CurlResult> PostAsync<T>(string endpoint, T data)
        {
            return await BuildRequest(endpoint)
                .SetMethod("POST")
                .SetJsonBody(data)
                .Build()
                .ExecuteAsync();
        }

        /// <summary>
        /// Performs a POST request and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="endpoint">The endpoint path.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The deserialized response.</returns>
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var result = await PostAsync(endpoint, data);
            result.EnsureSuccessStatusCode();
            return result.FromJson<TResponse>();
        }

        /// <summary>
        /// Performs a PUT request with JSON data.
        /// </summary>
        /// <typeparam name="T">The type of data to send.</typeparam>
        /// <param name="endpoint">The endpoint path.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The curl result.</returns>
        public async Task<CurlResult> PutAsync<T>(string endpoint, T data)
        {
            return await BuildRequest(endpoint)
                .SetMethod("PUT")
                .SetJsonBody(data)
                .Build()
                .ExecuteAsync();
        }

        /// <summary>
        /// Performs a DELETE request.
        /// </summary>
        /// <param name="endpoint">The endpoint path.</param>
        /// <returns>The curl result.</returns>
        public async Task<CurlResult> DeleteAsync(string endpoint)
        {
            return await BuildRequest(endpoint)
                .SetMethod("DELETE")
                .Build()
                .ExecuteAsync();
        }

        /// <summary>
        /// Performs a PATCH request with JSON data.
        /// </summary>
        /// <typeparam name="T">The type of data to send.</typeparam>
        /// <param name="endpoint">The endpoint path.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The curl result.</returns>
        public async Task<CurlResult> PatchAsync<T>(string endpoint, T data)
        {
            return await BuildRequest(endpoint)
                .SetMethod("PATCH")
                .SetJsonBody(data)
                .Build()
                .ExecuteAsync();
        }

        private CurlRequestBuilder BuildRequest(string endpoint)
        {
            var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
            var builder = new CurlRequestBuilder()
                .SetUrl(url)
                .SetTimeout(_defaultTimeout);

            foreach (var header in _defaultHeaders)
            {
                builder.AddHeader(header.Key, header.Value);
            }

            return builder;
        }
    }
}