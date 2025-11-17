using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurlDotNet.Tests.TestServers
{
    /// <summary>
    /// Adapts different test server implementations to provide a uniform interface.
    /// Handles differences in endpoints, response formats, and capabilities.
    /// </summary>
    public class TestServerAdapter
    {
        private readonly string _baseUrl;
        private readonly ServerType _serverType;

        public enum ServerType
        {
            Httpbin,      // httpbin.org, httpbingo.org, httpbun.com, httpbin.dev
            PostmanEcho,  // postman-echo.com
            JsonPlaceholder, // jsonplaceholder.typicode.com
            MockHttp,     // mockhttp.org, httpcan.org
            Generic       // Unknown/generic servers
        }

        public TestServerAdapter(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _serverType = DetectServerType(baseUrl);
        }

        private ServerType DetectServerType(string url)
        {
            if (url.Contains("httpbin") || url.Contains("httpbingo") ||
                url.Contains("httpbun") || url.Contains("mockhttp") ||
                url.Contains("httpcan"))
                return ServerType.Httpbin;

            if (url.Contains("postman-echo"))
                return ServerType.PostmanEcho;

            if (url.Contains("jsonplaceholder"))
                return ServerType.JsonPlaceholder;

            return ServerType.Generic;
        }

        /// <summary>
        /// Get the appropriate endpoint for a GET request.
        /// </summary>
        public string GetEndpoint()
        {
            return _serverType switch
            {
                ServerType.Httpbin => $"{_baseUrl}/get",
                ServerType.PostmanEcho => $"{_baseUrl}/get",
                ServerType.JsonPlaceholder => $"{_baseUrl}/posts/1",
                _ => $"{_baseUrl}/get"
            };
        }

        /// <summary>
        /// Get the appropriate endpoint for a POST request.
        /// </summary>
        public string PostEndpoint()
        {
            return _serverType switch
            {
                ServerType.Httpbin => $"{_baseUrl}/post",
                ServerType.PostmanEcho => $"{_baseUrl}/post",
                ServerType.JsonPlaceholder => $"{_baseUrl}/posts",
                _ => $"{_baseUrl}/post"
            };
        }

        /// <summary>
        /// Get the appropriate endpoint for status code testing.
        /// </summary>
        public string StatusEndpoint(int statusCode)
        {
            return _serverType switch
            {
                ServerType.Httpbin => $"{_baseUrl}/status/{statusCode}",
                ServerType.PostmanEcho => statusCode == 200 ? $"{_baseUrl}/get" : $"{_baseUrl}/status/{statusCode}",
                _ => $"{_baseUrl}/status/{statusCode}"
            };
        }

        /// <summary>
        /// Get the appropriate endpoint for delay testing.
        /// </summary>
        public string DelayEndpoint(int seconds)
        {
            return _serverType switch
            {
                ServerType.Httpbin => $"{_baseUrl}/delay/{seconds}",
                ServerType.PostmanEcho => $"{_baseUrl}/delay/{seconds}",
                _ => $"{_baseUrl}/delay/{seconds}"
            };
        }

        /// <summary>
        /// Get the appropriate endpoint for redirect testing.
        /// </summary>
        public string RedirectEndpoint(int count = 1)
        {
            return _serverType switch
            {
                ServerType.Httpbin => count == 1 ? $"{_baseUrl}/redirect/1" : $"{_baseUrl}/redirect/{count}",
                ServerType.PostmanEcho => $"{_baseUrl}/get", // PostmanEcho doesn't have redirect endpoint
                _ => $"{_baseUrl}/redirect/{count}"
            };
        }

        /// <summary>
        /// Get the appropriate endpoint for basic auth testing.
        /// </summary>
        public string BasicAuthEndpoint(string user, string password)
        {
            return _serverType switch
            {
                ServerType.Httpbin => $"{_baseUrl}/basic-auth/{user}/{password}",
                ServerType.PostmanEcho => $"{_baseUrl}/basic-auth",
                _ => $"{_baseUrl}/basic-auth/{user}/{password}"
            };
        }

        /// <summary>
        /// Get the appropriate endpoint for bearer token testing.
        /// </summary>
        public string BearerEndpoint()
        {
            return _serverType switch
            {
                ServerType.Httpbin => $"{_baseUrl}/bearer",
                ServerType.PostmanEcho => $"{_baseUrl}/get",
                _ => $"{_baseUrl}/bearer"
            };
        }

        /// <summary>
        /// Get the appropriate endpoint for headers inspection.
        /// </summary>
        public string HeadersEndpoint()
        {
            return _serverType switch
            {
                ServerType.Httpbin => $"{_baseUrl}/headers",
                ServerType.PostmanEcho => $"{_baseUrl}/headers",
                _ => $"{_baseUrl}/headers"
            };
        }

        /// <summary>
        /// Check if the server supports a specific feature.
        /// </summary>
        public bool SupportsFeature(TestServerFeatures feature)
        {
            return _serverType switch
            {
                ServerType.Httpbin => true, // Httpbin supports all features
                ServerType.PostmanEcho => feature != TestServerFeatures.Redirects &&
                                          feature != TestServerFeatures.Delay,
                ServerType.JsonPlaceholder => feature == TestServerFeatures.Basic,
                _ => feature == TestServerFeatures.Basic
            };
        }

        /// <summary>
        /// Parse response to extract headers from different server formats.
        /// </summary>
        public Dictionary<string, string> ParseHeadersFromResponse(string responseBody)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                var json = JsonDocument.Parse(responseBody);

                // Try to find headers in common locations
                if (json.RootElement.TryGetProperty("headers", out var headersElement))
                {
                    foreach (var prop in headersElement.EnumerateObject())
                    {
                        headers[prop.Name] = prop.Value.GetString() ?? "";
                    }
                }

                return headers;
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Parse response to extract data/body from different server formats.
        /// </summary>
        public string ParseDataFromResponse(string responseBody)
        {
            try
            {
                var json = JsonDocument.Parse(responseBody);

                // Try common data property names
                if (json.RootElement.TryGetProperty("data", out var dataElement))
                {
                    return dataElement.GetRawText();
                }
                if (json.RootElement.TryGetProperty("json", out var jsonElement))
                {
                    return jsonElement.GetRawText();
                }

                return responseBody;
            }
            catch
            {
                return responseBody;
            }
        }

        /// <summary>
        /// Check if a response indicates authentication success.
        /// </summary>
        public bool IsAuthenticationSuccess(string responseBody, int statusCode)
        {
            if (statusCode != 200)
                return false;

            try
            {
                var json = JsonDocument.Parse(responseBody);

                // Check for common auth success indicators
                if (json.RootElement.TryGetProperty("authenticated", out var auth))
                {
                    return auth.GetBoolean();
                }

                if (json.RootElement.TryGetProperty("authorized", out var authorized))
                {
                    return authorized.GetBoolean();
                }

                // For httpbin style
                if (json.RootElement.TryGetProperty("user", out _))
                {
                    return true;
                }

                // If we got 200 and there's no error property, assume success
                return !json.RootElement.TryGetProperty("error", out _);
            }
            catch
            {
                // If it's 200 and we can't parse, assume success
                return statusCode == 200;
            }
        }
    }
}