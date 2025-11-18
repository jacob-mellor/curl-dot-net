using System;
using System.Threading.Tasks;
using CurlDotNet;

namespace CurlDotNet.Examples.BasicExamples
{
    /// <summary>
    /// Example: Simple GET request to fetch data from an API
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

            // Method 2: Using the fluent API
            Console.WriteLine("Method 2: Using fluent API");
            var result2 = await Curl.GetAsync("https://api.github.com/users/octocat")
                .WithHeader("Accept", "application/vnd.github.v3+json")
                .ExecuteAsync();
            Console.WriteLine($"Status: {result2.StatusCode}");
            Console.WriteLine($"Response contains: {(result2.Body.Contains("octocat") ? "octocat user" : "unknown")}\n");

            // Method 3: Using static convenience method
            Console.WriteLine("Method 3: Using static convenience method");
            var json = await Curl.GetStringAsync("https://api.github.com/users/octocat");
            Console.WriteLine($"Retrieved JSON with {json.Length} characters");

            // Check if request was successful
            if (result1.IsSuccess)
            {
                Console.WriteLine("\n✅ All requests completed successfully!");
            }
        }
    }
}