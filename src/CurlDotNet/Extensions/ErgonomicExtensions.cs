using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet.Core;

namespace CurlDotNet
{
    /// <summary>
    /// Ergonomic extension methods for CurlResult to improve developer experience.
    /// These extensions are in the CurlDotNet namespace for easy discovery.
    /// </summary>
    public static class ErgonomicExtensions
    {
        /// <summary>
        /// Parse the response body as JSON and deserialize to a strongly-typed object.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to</typeparam>
        /// <param name="result">The CurlResult to parse</param>
        /// <param name="options">Optional JSON serializer options</param>
        /// <returns>The deserialized object</returns>
        /// <exception cref="JsonException">If the body is not valid JSON</exception>
        /// <exception cref="InvalidOperationException">If the result is not successful</exception>
        public static T ParseJson<T>(this CurlResult result, JsonSerializerOptions? options = null)
        {
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"Cannot parse JSON from failed request. Status: {result.StatusCode}");
            }

            if (string.IsNullOrWhiteSpace(result.Body))
            {
                throw new InvalidOperationException("Response body is empty");
            }

            return JsonSerializer.Deserialize<T>(result.Body, options)
                ?? throw new JsonException("Failed to deserialize JSON to target type");
        }

        /// <summary>
        /// Try to parse the response body as JSON. Returns false if parsing fails.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to</typeparam>
        /// <param name="result">The CurlResult to parse</param>
        /// <param name="value">The deserialized object if successful</param>
        /// <param name="options">Optional JSON serializer options</param>
        /// <returns>True if parsing succeeded, false otherwise</returns>
        public static bool TryParseJson<T>(this CurlResult result, out T value, JsonSerializerOptions? options = null) where T : class
        {
            value = default(T);

            if (!result.IsSuccess || string.IsNullOrWhiteSpace(result.Body))
            {
                return false;
            }

            try
            {
                value = JsonSerializer.Deserialize<T>(result.Body, options);
                return value != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Save the response body to a file.
        /// </summary>
        /// <param name="result">The CurlResult containing the data</param>
        /// <param name="filePath">The file path to save to</param>
        /// <returns>The number of bytes written</returns>
        public static long SaveToFile(this CurlResult result, string filePath)
        {
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"Cannot save failed response. Status: {result.StatusCode}");
            }

            if (result.BinaryData != null && result.BinaryData.Length > 0)
            {
                File.WriteAllBytes(filePath, result.BinaryData);
                return result.BinaryData.Length;
            }
            else if (!string.IsNullOrEmpty(result.Body))
            {
                File.WriteAllText(filePath, result.Body);
                return result.Body.Length;
            }
            else
            {
                throw new InvalidOperationException("No content to save");
            }
        }

        /// <summary>
        /// Get a specific header value from the response.
        /// </summary>
        /// <param name="result">The CurlResult</param>
        /// <param name="headerName">The header name (case-insensitive)</param>
        /// <returns>The header value if found, null otherwise</returns>
        public static string? GetHeader(this CurlResult result, string headerName)
        {
            if (result.Headers == null || !result.Headers.ContainsKey(headerName))
            {
                // Try case-insensitive lookup
                foreach (var header in result.Headers ?? new System.Collections.Generic.Dictionary<string, string>())
                {
                    if (string.Equals(header.Key, headerName, StringComparison.OrdinalIgnoreCase))
                    {
                        return header.Value;
                    }
                }
                return null;
            }

            return result.Headers[headerName];
        }

        /// <summary>
        /// Check if the response has a specific content type.
        /// </summary>
        /// <param name="result">The CurlResult</param>
        /// <param name="contentType">The content type to check for</param>
        /// <returns>True if the content type matches</returns>
        public static bool HasContentType(this CurlResult result, string contentType)
        {
            var actualContentType = result.GetHeader("Content-Type");
            if (actualContentType == null)
            {
                return false;
            }

            return actualContentType.IndexOf(contentType, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Throw an exception if the request was not successful.
        /// Similar to HttpResponseMessage.EnsureSuccessStatusCode().
        /// </summary>
        /// <param name="result">The CurlResult to check</param>
        /// <returns>The same CurlResult for chaining</returns>
        /// <exception cref="CurlHttpException">If the status code indicates failure</exception>
        public static CurlResult EnsureSuccessStatusCode(this CurlResult result)
        {
            if (!result.IsSuccess)
            {
                var message = $"Response status code does not indicate success: {result.StatusCode}";
                throw new Exceptions.CurlHttpException(
                    message,
                    result.StatusCode,
                    statusText: null, // StatusText doesn't exist in CurlResult
                    responseBody: result.Body,
                    command: result.Command
                );
            }

            return result;
        }

        /// <summary>
        /// Convert the CurlResult to a simplified success/error tuple.
        /// </summary>
        /// <param name="result">The CurlResult</param>
        /// <returns>A tuple of (success, body, error)</returns>
        public static (bool Success, string? Body, string? Error) ToSimple(this CurlResult result)
        {
            if (result.IsSuccess)
            {
                return (true, result.Body, null);
            }

            // Check if there's an exception stored
            var error = result.Exception?.Message ?? $"Request failed with status {result.StatusCode}";
            return (false, result.Body, error);
        }
    }

    /// <summary>
    /// Simplified API client builder for common HTTP operations.
    /// Provides a more ergonomic alternative to raw curl commands.
    /// </summary>
    public class CurlApiClient
    {
        private readonly string _baseUrl;
        private readonly TimeSpan _defaultTimeout;

        /// <summary>
        /// Create a new API client with a base URL.
        /// </summary>
        /// <param name="baseUrl">The base URL for all requests</param>
        /// <param name="defaultTimeout">Default timeout for requests</param>
        public CurlApiClient(string baseUrl, TimeSpan? defaultTimeout = null)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _defaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Perform a GET request.
        /// </summary>
        public Task<CurlResult> GetAsync(string path)
        {
            var url = string.IsNullOrEmpty(path) ? _baseUrl : $"{_baseUrl}/{path.TrimStart('/')}";
            return CurlRequestBuilder.Get(url)
                .WithTimeout(_defaultTimeout)
                .ExecuteAsync();
        }

        /// <summary>
        /// Perform a POST request with JSON body.
        /// </summary>
        public Task<CurlResult> PostJsonAsync(string path, object data)
        {
            var url = string.IsNullOrEmpty(path) ? _baseUrl : $"{_baseUrl}/{path.TrimStart('/')}";
            return CurlRequestBuilder.Post(url)
                .WithJson(data)
                .WithTimeout(_defaultTimeout)
                .ExecuteAsync();
        }

        /// <summary>
        /// Perform a PUT request with JSON body.
        /// </summary>
        public Task<CurlResult> PutJsonAsync(string path, object data)
        {
            var url = string.IsNullOrEmpty(path) ? _baseUrl : $"{_baseUrl}/{path.TrimStart('/')}";
            return CurlRequestBuilder.Put(url)
                .WithJson(data)
                .WithTimeout(_defaultTimeout)
                .ExecuteAsync();
        }

        /// <summary>
        /// Perform a DELETE request.
        /// </summary>
        public Task<CurlResult> DeleteAsync(string path)
        {
            var url = string.IsNullOrEmpty(path) ? _baseUrl : $"{_baseUrl}/{path.TrimStart('/')}";
            return CurlRequestBuilder.Delete(url)
                .WithTimeout(_defaultTimeout)
                .ExecuteAsync();
        }

        /// <summary>
        /// Perform a PATCH request with JSON body.
        /// </summary>
        public Task<CurlResult> PatchJsonAsync(string path, object data)
        {
            var url = string.IsNullOrEmpty(path) ? _baseUrl : $"{_baseUrl}/{path.TrimStart('/')}";
            return CurlRequestBuilder.Patch(url)
                .WithJson(data)
                .WithTimeout(_defaultTimeout)
                .ExecuteAsync();
        }
    }
}