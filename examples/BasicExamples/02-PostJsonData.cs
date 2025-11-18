using System;
using System.Threading.Tasks;
using CurlDotNet;

namespace CurlDotNet.Examples.BasicExamples
{
    /// <summary>
    /// Example: POST JSON data to an API endpoint
    /// </summary>
    public class PostJsonDataExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== POST JSON Data Example ===\n");

            // Example 1: Post raw JSON using curl command
            Console.WriteLine("Example 1: Raw curl command with JSON");
            var result1 = await Curl.ExecuteAsync(@"
                curl -X POST https://jsonplaceholder.typicode.com/posts \
                  -H 'Content-Type: application/json' \
                  -d '{""title"":""Hello World"",""body"":""This is a test post"",""userId"":1}'
            ");
            Console.WriteLine($"Status: {result1.StatusCode}");
            Console.WriteLine($"Created post ID: {(result1.Body.Contains("\"id\"") ? "Success" : "Failed")}\n");

            // Example 2: Post using fluent API with anonymous object
            Console.WriteLine("Example 2: Fluent API with anonymous object");
            var postData = new
            {
                title = "My Second Post",
                body = "Created using CurlDotNet fluent API",
                userId = 1
            };

            var result2 = await Curl.PostAsync("https://jsonplaceholder.typicode.com/posts")
                .WithJson(postData)
                .ExecuteAsync();

            Console.WriteLine($"Status: {result2.StatusCode}");
            Console.WriteLine($"Response: {result2.Body.Substring(0, Math.Min(100, result2.Body.Length))}...\n");

            // Example 3: Using convenience method PostJsonAsync
            Console.WriteLine("Example 3: PostJsonAsync convenience method");
            var result3 = await Curl.PostJsonAsync(
                "https://jsonplaceholder.typicode.com/posts",
                new { title = "Quick Post", body = "Using PostJsonAsync", userId = 2 }
            );

            Console.WriteLine($"Response received: {result3.Length} characters");

            // Example 4: POST with custom headers
            Console.WriteLine("\nExample 4: POST with authentication header");
            var result4 = await Curl.PostAsync("https://jsonplaceholder.typicode.com/posts")
                .WithHeader("Authorization", "Bearer fake-jwt-token")
                .WithHeader("X-Custom-Header", "CustomValue")
                .WithJson(new { title = "Authenticated Post", userId = 3 })
                .ExecuteAsync();

            Console.WriteLine($"Status: {result4.StatusCode}");
            Console.WriteLine($"Headers sent: Authorization, X-Custom-Header, Content-Type");
        }
    }
}