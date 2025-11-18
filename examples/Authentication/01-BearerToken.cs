using System;
using System.Threading.Tasks;
using CurlDotNet;

namespace CurlDotNet.Examples.Authentication
{
    /// <summary>
    /// Example: Bearer token authentication
    /// </summary>
    public class BearerTokenExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Bearer Token Authentication Example ===\n");

            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

            // Method 1: Using curl command with Authorization header
            Console.WriteLine("Method 1: Curl command with Authorization header");
            var result1 = await Curl.ExecuteAsync($@"
                curl https://api.example.com/protected \
                  -H 'Authorization: Bearer {token}'
            ");
            Console.WriteLine($"Status: {result1.StatusCode}\n");

            // Method 2: Using fluent API WithBearerToken
            Console.WriteLine("Method 2: Fluent API with WithBearerToken");
            var result2 = await Curl.GetAsync("https://api.example.com/protected")
                .WithBearerToken(token)
                .ExecuteAsync();
            Console.WriteLine($"Status: {result2.StatusCode}\n");

            // Method 3: Multiple auth headers (e.g., API key + Bearer)
            Console.WriteLine("Method 3: Multiple authentication headers");
            var result3 = await Curl.GetAsync("https://api.example.com/secure")
                .WithBearerToken(token)
                .WithHeader("X-API-Key", "my-api-key-12345")
                .WithHeader("X-Client-Id", "mobile-app-v2")
                .ExecuteAsync();
            Console.WriteLine($"Status: {result3.StatusCode}");
            Console.WriteLine("Headers sent: Authorization, X-API-Key, X-Client-Id");

            // Method 4: Refresh token pattern
            Console.WriteLine("\nMethod 4: Token refresh pattern");
            string accessToken = await RefreshAccessToken("refresh-token-here");
            var result4 = await Curl.GetAsync("https://api.example.com/data")
                .WithBearerToken(accessToken)
                .ExecuteAsync();

            if (result4.StatusCode == 401) // Token expired
            {
                Console.WriteLine("Token expired, refreshing...");
                accessToken = await RefreshAccessToken("refresh-token-here");
                result4 = await Curl.GetAsync("https://api.example.com/data")
                    .WithBearerToken(accessToken)
                    .ExecuteAsync();
            }
            Console.WriteLine($"Final status: {result4.StatusCode}");
        }

        private static async Task<string> RefreshAccessToken(string refreshToken)
        {
            var result = await Curl.PostAsync("https://auth.example.com/token")
                .WithFormData(new {
                    grant_type = "refresh_token",
                    refresh_token = refreshToken,
                    client_id = "your-client-id",
                    client_secret = "your-client-secret"
                })
                .ExecuteAsync();

            // In real app, parse JSON response to get new access token
            return "new-access-token-" + DateTime.Now.Ticks;
        }
    }
}