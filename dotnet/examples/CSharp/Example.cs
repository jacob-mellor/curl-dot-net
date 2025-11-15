/***************************************************************************
 * CurlDotNet C# Examples
 *
 * Examples showing how to use CurlDotNet in C#
 ***************************************************************************/

using System;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;

namespace CurlDotNet.Examples.CSharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CurlDotNet C# Examples ===\n");

            // Example 1: Simple GET request
            await SimpleGet();

            // Example 2: POST with JSON
            await PostWithJson();

            // Example 3: Using fluent builder
            await FluentBuilder();

            // Example 4: File operations
            await FileOperations();

            // Example 5: Authentication
            await Authentication();

            Console.WriteLine("\n=== Examples Complete ===");
        }

        static async Task SimpleGet()
        {
            Console.WriteLine("Example 1: Simple GET Request");
            
            var result = await Curl.ExecuteAsync(
                "curl https://jsonplaceholder.typicode.com/posts/1"
            );

            if (result.IsSuccess)
            {
                Console.WriteLine($"Status: {result.StatusCode}");
                Console.WriteLine($"Body: {result.Body.Substring(0, Math.Min(100, result.Body.Length))}...");
            }

            Console.WriteLine();
        }

        static async Task PostWithJson()
        {
            Console.WriteLine("Example 2: POST with JSON");

            var result = await Curl.ExecuteAsync(@"
                curl -X POST https://jsonplaceholder.typicode.com/posts \
                    -H 'Content-Type: application/json' \
                    -d '{
                      ""title"": ""My Post"",
                      ""body"": ""This is the content"",
                      ""userId"": 1
                    }'
            ");

            if (result.IsSuccess)
            {
                Console.WriteLine($"Created post with status: {result.StatusCode}");
                Console.WriteLine($"Response: {result.Body.Substring(0, Math.Min(200, result.Body.Length))}...");
            }

            Console.WriteLine();
        }

        static async Task FluentBuilder()
        {
            Console.WriteLine("Example 3: Fluent Builder API");

            var result = await CurlRequestBuilder
                .Get("https://jsonplaceholder.typicode.com/users/1")
                .WithHeader("Accept", "application/json")
                .WithTimeout(TimeSpan.FromSeconds(30))
                .FollowRedirects()
                .ExecuteAsync();

            if (result.IsSuccess)
            {
                Console.WriteLine($"User data: {result.Body.Substring(0, Math.Min(150, result.Body.Length))}...");
            }

            Console.WriteLine();
        }

        static async Task FileOperations()
        {
            Console.WriteLine("Example 4: File Operations");

            // Download and save
            var downloadResult = await Curl.ExecuteAsync(
                "curl -o example.txt https://jsonplaceholder.typicode.com/posts/1"
            );

            if (downloadResult.IsSuccess)
            {
                Console.WriteLine($"File saved to: {downloadResult.OutputFiles[0]}");
                
                // Also available in memory
                Console.WriteLine($"Size in memory: {downloadResult.Body.Length} bytes");
            }

            Console.WriteLine();
        }

        static async Task Authentication()
        {
            Console.WriteLine("Example 5: Authentication");

            // Basic auth
            var basicAuthResult = await Curl.ExecuteAsync(
                "curl -u username:password https://httpbin.org/basic-auth/username/password"
            );

            Console.WriteLine($"Basic auth result: {basicAuthResult.StatusCode}");

            // Bearer token
            var bearerResult = await Curl.ExecuteAsync(
                "curl -H 'Authorization: Bearer token123' https://httpbin.org/headers"
            );

            if (bearerResult.IsSuccess)
            {
                Console.WriteLine($"Bearer token result: {bearerResult.StatusCode}");
            }

            Console.WriteLine();
        }
    }
}

