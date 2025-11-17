using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurlDotNet.Tests.TestServers
{
    /// <summary>
    /// Configuration for test servers with automatic fallback to alternatives.
    /// Provides multiple reliable options for HTTP testing.
    /// </summary>
    public class TestServerConfiguration
    {
        private static readonly HttpClient HealthCheckClient = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };

        /// <summary>
        /// List of available HTTP echo/test services in order of preference.
        /// These are all publicly available and free to use.
        /// </summary>
        public static readonly List<TestServerEndpoint> AvailableServers = new List<TestServerEndpoint>
        {
            // LOCAL OPTIONS (most reliable when available)
            new TestServerEndpoint
            {
                Name = "Local Docker httpbin",
                BaseUrl = "http://localhost:8080",
                HealthCheckPath = "/get",
                IsLocal = true,
                Priority = 1,
                Features = TestServerFeatures.All
            },
            new TestServerEndpoint
            {
                Name = "Local Test Server",
                BaseUrl = "http://localhost:5000",
                HealthCheckPath = "/health",
                IsLocal = true,
                Priority = 2,
                Features = TestServerFeatures.All
            },

            // PUBLIC HTTPBIN INSTANCES (different providers for redundancy)
            new TestServerEndpoint
            {
                Name = "httpbingo.org (Go implementation)",
                BaseUrl = "https://httpbingo.org",
                HealthCheckPath = "/get",
                IsLocal = false,
                Priority = 10,
                Features = TestServerFeatures.All,
                Notes = "Go implementation of httpbin, often more reliable"
            },
            new TestServerEndpoint
            {
                Name = "httpbin.dev",
                BaseUrl = "https://httpbin.dev",
                HealthCheckPath = "/get",
                IsLocal = false,
                Priority = 11,
                Features = TestServerFeatures.All,
                Notes = "Alternative httpbin instance"
            },
            new TestServerEndpoint
            {
                Name = "httpbun.com",
                BaseUrl = "https://httpbun.com",
                HealthCheckPath = "/get",
                IsLocal = false,
                Priority = 12,
                Features = TestServerFeatures.All,
                Notes = "httpbun.com - Another httpbin clone"
            },
            new TestServerEndpoint
            {
                Name = "httpbin.org (Official)",
                BaseUrl = "https://httpbin.org",
                HealthCheckPath = "/get",
                IsLocal = false,
                Priority = 15,  // Lower priority due to flakiness
                Features = TestServerFeatures.All,
                Notes = "Original httpbin by Kenneth Reitz - can be flaky"
            },
            new TestServerEndpoint
            {
                Name = "eu.httpbin.org (European mirror)",
                BaseUrl = "https://eu.httpbin.org",
                HealthCheckPath = "/get",
                IsLocal = false,
                Priority = 16,  // Lower priority due to being a mirror
                Features = TestServerFeatures.All,
                Notes = "European hosted httpbin instance"
            },
            new TestServerEndpoint
            {
                Name = "mockhttp.org",
                BaseUrl = "https://mockhttp.org",
                HealthCheckPath = "/get",
                IsLocal = false,
                Priority = 18,
                Features = TestServerFeatures.All,
                Notes = "Mock HTTP service"
            },
            new TestServerEndpoint
            {
                Name = "httpcan.org",
                BaseUrl = "https://httpcan.org",
                HealthCheckPath = "/get",
                IsLocal = false,
                Priority = 19,
                Features = TestServerFeatures.All,
                Notes = "HTTP testing service"
            },

            // POSTMAN SERVICES
            new TestServerEndpoint
            {
                Name = "Postman Echo",
                BaseUrl = "https://postman-echo.com",
                HealthCheckPath = "/get",
                IsLocal = false,
                Priority = 20,
                Features = TestServerFeatures.Basic | TestServerFeatures.Headers | TestServerFeatures.Auth,
                Notes = "Postman's echo service - reliable but limited features"
            },

            // OTHER ECHO SERVICES
            new TestServerEndpoint
            {
                Name = "Echo API",
                BaseUrl = "https://echo.hoppscotch.io",
                HealthCheckPath = "/",
                IsLocal = false,
                Priority = 30,
                Features = TestServerFeatures.Basic | TestServerFeatures.Headers,
                Notes = "Hoppscotch echo service"
            },
            new TestServerEndpoint
            {
                Name = "JSONPlaceholder",
                BaseUrl = "https://jsonplaceholder.typicode.com",
                HealthCheckPath = "/posts/1",
                IsLocal = false,
                Priority = 40,
                Features = TestServerFeatures.Basic,
                Notes = "Fake REST API for testing - very reliable but limited"
            },
            new TestServerEndpoint
            {
                Name = "ReqRes API",
                BaseUrl = "https://reqres.in",
                HealthCheckPath = "/api/users/1",
                IsLocal = false,
                Priority = 41,
                Features = TestServerFeatures.Basic | TestServerFeatures.Auth,
                Notes = "REST API test service with sample data"
            },
            new TestServerEndpoint
            {
                Name = "MockAPI.io",
                BaseUrl = "https://mockapi.io",
                HealthCheckPath = "/api/v1/health",
                IsLocal = false,
                Priority = 42,
                Features = TestServerFeatures.Basic,
                Notes = "Mock API service"
            },

            // CDN/EDGE HOSTED OPTIONS
            new TestServerEndpoint
            {
                Name = "httpstat.us",
                BaseUrl = "https://httpstat.us",
                HealthCheckPath = "/200",
                IsLocal = false,
                Priority = 50,
                Features = TestServerFeatures.StatusCodes,
                Notes = "Simple status code testing service"
            },
            new TestServerEndpoint
            {
                Name = "Mockbin",
                BaseUrl = "https://mockbin.com",
                HealthCheckPath = "/request",
                IsLocal = false,
                Priority = 51,
                Features = TestServerFeatures.Basic | TestServerFeatures.Headers,
                Notes = "Mock HTTP request service"
            },

            // WEBHOOKS/TESTING SERVICES
            new TestServerEndpoint
            {
                Name = "Webhook.site",
                BaseUrl = "https://webhook.site",
                HealthCheckPath = "/",
                IsLocal = false,
                Priority = 60,
                Features = TestServerFeatures.Basic,
                Notes = "Webhook testing service"
            },
            new TestServerEndpoint
            {
                Name = "Beeceptor",
                BaseUrl = "https://beeceptor.com",
                HealthCheckPath = "/api/health",
                IsLocal = false,
                Priority = 61,
                Features = TestServerFeatures.Basic | TestServerFeatures.Headers,
                Notes = "Mock server and API proxy"
            }
        };

        /// <summary>
        /// Find the best available server for testing.
        /// Checks health of servers in priority order and returns the first healthy one.
        /// </summary>
        public static async Task<TestServerEndpoint> GetBestAvailableServerAsync(TestServerFeatures requiredFeatures = TestServerFeatures.Basic)
        {
            var candidateServers = AvailableServers
                .Where(s => s.Features.HasFlag(requiredFeatures))
                .OrderBy(s => s.Priority)
                .ToList();

            foreach (var server in candidateServers)
            {
                if (await IsServerHealthyAsync(server))
                {
                    Console.WriteLine($"✅ Using test server: {server.Name} at {server.BaseUrl}");
                    return server;
                }
                else
                {
                    Console.WriteLine($"❌ Server unavailable: {server.Name}");
                }
            }

            // Fallback to the most reliable public option
            var fallback = AvailableServers.First(s => s.Name.Contains("httpbingo"));
            Console.WriteLine($"⚠️  Using fallback server: {fallback.Name}");
            return fallback;
        }

        /// <summary>
        /// Check if a server is healthy and responding.
        /// </summary>
        private static async Task<bool> IsServerHealthyAsync(TestServerEndpoint server)
        {
            try
            {
                var response = await HealthCheckClient.GetAsync($"{server.BaseUrl}{server.HealthCheckPath}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Represents a test server endpoint configuration.
    /// </summary>
    public class TestServerEndpoint
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string HealthCheckPath { get; set; }
        public bool IsLocal { get; set; }
        public int Priority { get; set; }
        public TestServerFeatures Features { get; set; }
        public string Notes { get; set; }
    }

    /// <summary>
    /// Features supported by test servers.
    /// </summary>
    [Flags]
    public enum TestServerFeatures
    {
        Basic = 1,
        Headers = 2,
        Auth = 4,
        Redirects = 8,
        StatusCodes = 16,
        Delay = 32,
        Streaming = 64,
        Compression = 128,
        All = Basic | Headers | Auth | Redirects | StatusCodes | Delay | Streaming | Compression
    }
}