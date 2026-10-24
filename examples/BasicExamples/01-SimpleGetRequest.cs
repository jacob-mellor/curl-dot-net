using System;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;

namespace CurlDotNet.Examples.BasicExamples
{
    /// <summary>
    /// Example: Simple GET request to fetch data from an API, three ways.
    /// </summary>
    public class SimpleGetRequestExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Simple GET Request Example ===\n");

            // Method 1: Direct curl command (copy/paste from documentation)
            Console.WriteLine("Method 1: Using curl command string");
            var result1 = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");
            Console.WriteLine($"Status: {result1.StatusCode}");
            Console.WriteLine($"Response length: {result1.Body.Length} characters\n");

            // Method 2: Using the fluent builder when you need headers or options
            Console.WriteLine("Method 2: Using the fluent CurlRequestBuilder");
            var result2 = await CurlRequestBuilder
                .Get("https://api.github.com/users/octocat")
                .WithHeader("Accept", "application/vnd.github+json")
                .ExecuteAsync();
            Console.WriteLine($"Status: {result2.StatusCode}");
            Console.WriteLine($"Response contains: {(result2.Body.Contains("octocat") ? "octocat user" : "unknown")}\n");

            // Method 3: Using the static convenience method - just a URL, no command syntax
            Console.WriteLine("Method 3: Using the Curl.GetAsync convenience method");
            var result3 = await Curl.GetAsync("https://api.github.com/users/octocat");
            Console.WriteLine($"Retrieved JSON with {result3.Body.Length} characters");

            // Every method returns the same CurlResult, so success checks look the same
            if (result1.IsSuccess && result2.IsSuccess && result3.IsSuccess)
            {
                Console.WriteLine("\nAll requests completed successfully.");
            }
        }
    }
}
