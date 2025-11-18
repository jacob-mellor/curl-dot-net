using System;
using System.Threading.Tasks;
using System.Text.Json;
using CurlDotNet;

namespace BearerTokenAuth
{
    /// <summary>
    /// Demonstrates Bearer token authentication with APIs
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
                Console.WriteLine("✅ Bearer token sent successfully!");
                var response = JsonDocument.Parse(result1.Body);
                var authenticated = response.RootElement.GetProperty("authenticated").GetBoolean();
                var receivedToken = response.RootElement.GetProperty("token").GetString();
                Console.WriteLine($"Authenticated: {authenticated}");
                Console.WriteLine($"Token received by server: {receivedToken}\n");
            }
            else
            {
                Console.WriteLine($"❌ Authentication failed: {result1.StatusCode}\n");
            }

            // Example 2: API Key authentication (different pattern)
            Console.WriteLine("2. API Key authentication pattern...");

            var apiKey = "demo-api-key-67890";

            var result2 = await Curl.ExecuteAsync($@"
                curl -X GET https://httpbin.org/headers
                -H 'X-API-Key: {apiKey}'
            ");

            if (result2.IsSuccess)
            {
                Console.WriteLine("✅ API Key sent in header!");
                var response = JsonDocument.Parse(result2.Body);
                var headers = response.RootElement.GetProperty("headers");
                Console.WriteLine($"X-Api-Key header: {headers.GetProperty("X-Api-Key").GetString()}\n");
            }

            // Example 3: GitHub API with token (using environment variable)
            Console.WriteLine("3. GitHub API with Bearer token...");
            Console.WriteLine("   (Set GITHUB_TOKEN environment variable for authenticated requests)");

            var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? "ghp_demo_token";
            var useRealToken = githubToken != "ghp_demo_token";

            if (!useRealToken)
            {
                Console.WriteLine("   ⚠️  Using demo token - will make unauthenticated request");
            }

            // For demo, we'll check rate limit which works without auth
            var githubResult = await Curl.ExecuteAsync(@"
                curl -X GET https://api.github.com/rate_limit
                -H 'Accept: application/vnd.github.v3+json'
            " + (useRealToken ? $"-H 'Authorization: Bearer {githubToken}'" : ""));

            if (githubResult.IsSuccess)
            {
                Console.WriteLine("✅ GitHub API request successful!");
                var response = JsonDocument.Parse(githubResult.Body);
                var coreLimit = response.RootElement.GetProperty("rate").GetProperty("limit").GetInt32();
                var remaining = response.RootElement.GetProperty("rate").GetProperty("remaining").GetInt32();
                Console.WriteLine($"Rate limit: {remaining}/{coreLimit}");
                Console.WriteLine($"Authenticated requests get 5000/hour, anonymous get 60/hour\n");
            }

            // Example 4: Multiple authentication headers
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
                Console.WriteLine("✅ Multiple auth headers sent!");
                var response = JsonDocument.Parse(result4.Body);
                var headers = response.RootElement.GetProperty("headers");
                Console.WriteLine("Authentication headers received:");
                Console.WriteLine($"  Authorization: {headers.GetProperty("Authorization").GetString()}");
                Console.WriteLine($"  X-Refresh-Token: {headers.GetProperty("X-Refresh-Token").GetString()}");
                Console.WriteLine($"  X-Client-Id: {headers.GetProperty("X-Client-Id").GetString()}\n");
            }

            // Example 5: Basic Authentication (username:password)
            Console.WriteLine("5. Basic Authentication...");

            var username = "testuser";
            var password = "testpass";

            var result5 = await Curl.ExecuteAsync($@"
                curl -X GET https://httpbin.org/basic-auth/{username}/{password}
                -u {username}:{password}
            ");

            if (result5.IsSuccess)
            {
                Console.WriteLine("✅ Basic auth successful!");
                var response = JsonDocument.Parse(result5.Body);
                var authenticated = response.RootElement.GetProperty("authenticated").GetBoolean();
                var user = response.RootElement.GetProperty("user").GetString();
                Console.WriteLine($"Authenticated: {authenticated}");
                Console.WriteLine($"Username: {user}\n");
            }

            Console.WriteLine("=== Security Best Practices ===");
            Console.WriteLine("• Never hardcode tokens in source code");
            Console.WriteLine("• Use environment variables or secure vaults");
            Console.WriteLine("• Rotate tokens regularly");
            Console.WriteLine("• Use HTTPS for all authenticated requests");
            Console.WriteLine("• Implement token refresh for long-running apps");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}