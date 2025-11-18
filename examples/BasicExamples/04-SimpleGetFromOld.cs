// NuGet Package: https://www.nuget.org/packages/CurlDotNet/
// Install: dotnet add package CurlDotNet
using System;
using System.Threading.Tasks;
using CurlDotNet;

namespace SimpleGet
{
    /// <summary>
    /// Demonstrates the simplest possible GET request using CurlDotNet
    /// Focus: Understanding the CurlResult object
    /// Based on cookbook/beginner/simple-get.md
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CurlDotNet: Simple GET Request Example ===\n");

            // Example 1: Simplest possible GET request
            Console.WriteLine("1. Fetching GitHub API root endpoint...");
            var result = await Curl.ExecuteAsync("https://api.github.com");

            if (result.IsSuccess)
            {
                Console.WriteLine($"✅ Success! Status: {result.StatusCode}");
                Console.WriteLine($"Response preview (first 200 chars):\n{result.Body.Substring(0, Math.Min(200, result.Body.Length))}...\n");
            }
            else
            {
                Console.WriteLine($"❌ Request failed with status: {result.StatusCode}");
            }

            // Example 2: GET with explicit curl command
            Console.WriteLine("2. Using explicit curl command syntax...");
            var curlResult = await Curl.ExecuteAsync("curl -X GET https://api.github.com/zen");

            if (curlResult.IsSuccess)
            {
                Console.WriteLine($"✅ GitHub Zen: {curlResult.Body}\n");
            }

            // Example 3: Using convenience method
            Console.WriteLine("3. Using Curl.GetAsync convenience method...");
            var getResult = await Curl.GetAsync("https://httpbin.org/get");

            if (getResult.IsSuccess)
            {
                Console.WriteLine($"✅ httpbin.org response received");
                Console.WriteLine($"Response size: {getResult.Body.Length} bytes\n");
            }

            // Example 4: GET with query parameters
            Console.WriteLine("4. GET request with query parameters...");
            var queryResult = await Curl.GetAsync("https://httpbin.org/get?name=CurlDotNet&version=1.0");

            if (queryResult.IsSuccess)
            {
                Console.WriteLine($"✅ Query parameters sent successfully");
                // The response will echo back our query parameters
                Console.WriteLine("Response includes our query params in 'args' field\n");
            }

            Console.WriteLine("=== Example Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}