using System;
using System.Threading.Tasks;
using System.Text.Json;
using CurlDotNet;

namespace BearerTokenAuth
{
    /// <summary>
    /// Demonstrates Bearer token, API key and Basic authentication with pasted curl commands
    /// Based on cookbook/beginner/call-api.md
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CurlDotNet: Bearer Token Authentication ===\n");

            // Example 1: Basic Bearer token (using demo token)
            Console.WriteLine("1. Bearer token authentication with httpbin.org...");
            var demoToken = "demo-bearer-token-12345";

            var result1 = await Curl.ExecuteAsync($@"
                curl -X GET https://httpbin.org/bearer
                -H 'Authorization: Bearer {demoToken}'
            ");

            if (result1.IsSuccess)
            {
                using var response = JsonDocument.Parse(result1.Body);
                Console.WriteLine($"Authenticated: {response.RootElement.GetProperty("authenticated").GetBoolean()}");
                Console.WriteLine($"Token received by server: {response.RootElement.GetProperty("token").GetString()}\n");
            }
            else
            {
                Console.WriteLine($"Authentication failed: {result1.StatusCode}\n");
            }

            // Example 2: API Key authentication (different pattern)
            Console.WriteLine("2. API Key authentication pattern...");
            var apiKey = "demo-api-key-67890";

            var result2 = await Curl.ExecuteAsync($"curl https://httpbin.org/headers -H 'X-API-Key: {apiKey}'");

            if (result2.IsSuccess)
            {
                using var response = JsonDocument.Parse(result2.Body);
                Console.WriteLine($"X-Api-Key header: {response.RootElement.GetProperty("headers").GetProperty("X-Api-Key").GetString()}\n");
            }

            // Example 3: GitHub API with token (using environment variable)
            Console.WriteLine("3. GitHub API with Bearer token...");
            Console.WriteLine("   (Set GITHUB_TOKEN for authenticated requests)");
            var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

            // The rate limit endpoint works with or without auth, so the header is optional
            var command = "curl https://api.github.com/rate_limit -H 'Accept: application/vnd.github+json'";
            if (!string.IsNullOrEmpty(githubToken))
            {
                command += $" -H 'Authorization: Bearer {githubToken}'";
            }

            var githubResult = await Curl.ExecuteAsync(command);

            if (githubResult.IsSuccess)
            {
                using var response = JsonDocument.Parse(githubResult.Body);
                var rate = response.RootElement.GetProperty("rate");
                Console.WriteLine($"Rate limit: {rate.GetProperty("remaining").GetInt32()}/{rate.GetProperty("limit").GetInt32()}");
                Console.WriteLine($"Authenticated: {!string.IsNullOrEmpty(githubToken)}\n");
            }

            // Example 4: Multiple authentication headers (OAuth-style)
            Console.WriteLine("4. Multiple authentication headers (OAuth-style)...");
            var result4 = await Curl.ExecuteAsync(@"
                curl -X POST https://httpbin.org/post
                -H 'Authorization: Bearer access-token-here'
                -H 'X-Refresh-Token: refresh-token-here'
                -H 'X-Client-ID: my-app-client-id'
                -H 'Content-Type: application/json'
                -d '{""grant_type"":""client_credentials""}'
            ");

            if (result4.IsSuccess)
            {
                using var response = JsonDocument.Parse(result4.Body);
                var headers = response.RootElement.GetProperty("headers");
                Console.WriteLine("Authentication headers received:");
                Console.WriteLine($"  Authorization: {headers.GetProperty("Authorization").GetString()}");
                Console.WriteLine($"  X-Refresh-Token: {headers.GetProperty("X-Refresh-Token").GetString()}");
                Console.WriteLine($"  X-Client-Id: {headers.GetProperty("X-Client-Id").GetString()}\n");
            }

            // Example 5: Basic Authentication (username:password) with -u
            Console.WriteLine("5. Basic Authentication...");
            var username = "testuser";
            var password = "testpass";

            var result5 = await Curl.ExecuteAsync(
                $"curl https://httpbin.org/basic-auth/{username}/{password} -u {username}:{password}");

            if (result5.IsSuccess)
            {
                using var response = JsonDocument.Parse(result5.Body);
                Console.WriteLine($"Authenticated: {response.RootElement.GetProperty("authenticated").GetBoolean()}");
                Console.WriteLine($"Username: {response.RootElement.GetProperty("user").GetString()}\n");
            }

            Console.WriteLine("=== Security Best Practices ===");
            Console.WriteLine("- Never hardcode tokens in source code");
            Console.WriteLine("- Use environment variables or secure vaults");
            Console.WriteLine("- Use HTTPS for all authenticated requests");
        }
    }
}
