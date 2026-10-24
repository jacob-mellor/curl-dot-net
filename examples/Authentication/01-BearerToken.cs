using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Lib;

namespace CurlDotNet.Examples.Authentication
{
    /// <summary>
    /// Example: Bearer token authentication.
    /// httpbin.org/bearer returns 200 when it sees a Bearer token and 401 when it does not.
    /// </summary>
    public class BearerTokenExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Bearer Token Authentication Example ===\n");

            // Read real tokens from the environment, never from source code
            string token = Environment.GetEnvironmentVariable("API_TOKEN") ?? "demo-token-12345";

            // Method 1: Using curl command with Authorization header
            Console.WriteLine("Method 1: Curl command with Authorization header");
            var result1 = await Curl.ExecuteAsync($@"
                curl https://httpbin.org/bearer \
                  -H 'Authorization: Bearer {token}'
            ");
            Console.WriteLine($"Status: {result1.StatusCode}\n");

            // Method 2: Using the fluent builder's WithBearerToken
            Console.WriteLine("Method 2: Fluent API with WithBearerToken");
            var result2 = await CurlRequestBuilder.Get("https://httpbin.org/bearer")
                .WithBearerToken(token)
                .ExecuteAsync();
            Console.WriteLine($"Status: {result2.StatusCode}\n");

            // Method 3: Multiple auth headers (e.g., API key + Bearer)
            Console.WriteLine("Method 3: Multiple authentication headers");
            var result3 = await CurlRequestBuilder.Get("https://httpbin.org/headers")
                .WithBearerToken(token)
                .WithHeader("X-API-Key", "my-api-key-12345")
                .WithHeader("X-Client-Id", "mobile-app-v2")
                .ExecuteAsync();
            Console.WriteLine($"Status: {result3.StatusCode}");
            Console.WriteLine("Headers sent: Authorization, X-API-Key, X-Client-Id");

            // Method 4: Refresh token pattern - retry once with a new token on 401
            Console.WriteLine("\nMethod 4: Token refresh pattern");
            var result4 = await CurlRequestBuilder.Get("https://httpbin.org/bearer").ExecuteAsync(); // no token: 401

            if (result4.StatusCode == 401)
            {
                Console.WriteLine("Got 401, refreshing token...");
                string accessToken = await RefreshAccessToken("refresh-token-here");
                result4 = await CurlRequestBuilder.Get("https://httpbin.org/bearer")
                    .WithBearerToken(accessToken)
                    .ExecuteAsync();
            }
            Console.WriteLine($"Final status: {result4.StatusCode}");

            // Method 5: A reusable client that sends the token on every request
            Console.WriteLine("\nMethod 5: LibCurl session with a default token");
            using (var client = new LibCurl().WithBearerToken(token))
            {
                var first = await client.GetAsync("https://httpbin.org/bearer");
                var second = await client.GetAsync("https://httpbin.org/headers");
                Console.WriteLine($"Statuses: {first.StatusCode}, {second.StatusCode}");
            }
        }

        private static async Task<string> RefreshAccessToken(string refreshToken)
        {
            // A real OAuth server returns {"access_token": "..."}; httpbin just echoes the form,
            // so it stands in for the token endpoint here.
            var result = await CurlRequestBuilder.Post("https://httpbin.org/post")
                .WithFormData(new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["refresh_token"] = refreshToken,
                    ["client_id"] = "your-client-id"
                })
                .ExecuteAsync();

            using var json = JsonDocument.Parse(result.Body);
            Console.WriteLine($"Token endpoint received grant_type={json.RootElement.GetProperty("form").GetProperty("grant_type")}");

            // With a real server: return result.ParseJson<TokenResponse>().AccessToken;
            return "new-access-token-" + DateTime.UtcNow.Ticks;
        }
    }
}
