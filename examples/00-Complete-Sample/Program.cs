using System;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;

namespace CurlDotNet.Sample;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=================================");
        Console.WriteLine("   CurlDotNet Sample Application");
        Console.WriteLine("=================================");
        Console.WriteLine();

        // Run different examples
        await Example1_SimpleCurlCommand();
        await Example2_GetRequest();
        await Example3_PostJson();
        await Example4_FluentApi();
        await Example5_ErrorHandling();
        await Example6_Headers();

        Console.WriteLine("\nAll examples completed successfully!");
    }

    /// <summary>
    /// Example 1: Direct curl command execution
    /// </summary>
    static async Task Example1_SimpleCurlCommand()
    {
        Console.WriteLine("Example 1: Direct curl command");
        Console.WriteLine("-------------------------------");

        // You can paste curl commands directly!
        var result = await Curl.ExecuteAsync("curl https://api.github.com/zen");

        Console.WriteLine($"GitHub Zen: {result.Body}");
        Console.WriteLine($"Status: {result.StatusCode}");
        Console.WriteLine();
    }

    /// <summary>
    /// Example 2: Simple GET request
    /// </summary>
    static async Task Example2_GetRequest()
    {
        Console.WriteLine("Example 2: GET Request");
        Console.WriteLine("----------------------");

        var result = await Curl.GetAsync("https://jsonplaceholder.typicode.com/posts/1");

        if (result.IsSuccess)
        {
            var json = JsonDocument.Parse(result.Body);
            Console.WriteLine($"Post Title: {json.RootElement.GetProperty("title").GetString()}");
            Console.WriteLine($"Response Time: {result.ElapsedTime.TotalMilliseconds}ms");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Example 3: POST request with JSON
    /// </summary>
    static async Task Example3_PostJson()
    {
        Console.WriteLine("Example 3: POST JSON");
        Console.WriteLine("--------------------");

        var payload = new
        {
            title = "Sample Post",
            body = "This is a test post from CurlDotNet",
            userId = 1
        };

        var json = JsonSerializer.Serialize(payload);
        var result = await Curl.PostAsync("https://jsonplaceholder.typicode.com/posts", json, "application/json");

        Console.WriteLine($"Created Post ID: {JsonDocument.Parse(result.Body).RootElement.GetProperty("id")}");
        Console.WriteLine($"Status: {result.StatusCode}");
        Console.WriteLine();
    }

    /// <summary>
    /// Example 4: Fluent API builder pattern
    /// </summary>
    static async Task Example4_FluentApi()
    {
        Console.WriteLine("Example 4: Fluent API");
        Console.WriteLine("---------------------");

        var result = await new CurlRequestBuilder()
            .SetUrl("https://httpbin.org/get")
            .AddHeader("User-Agent", "CurlDotNet-Sample/1.0")
            .AddHeader("Accept", "application/json")
            .SetTimeout(TimeSpan.FromSeconds(10))
            .Build()
            .ExecuteAsync();

        var response = JsonDocument.Parse(result.Body);
        var userAgent = response.RootElement
            .GetProperty("headers")
            .GetProperty("User-Agent")
            .GetString();

        Console.WriteLine($"Sent User-Agent: {userAgent}");
        Console.WriteLine($"Response Size: {result.Body.Length} bytes");
        Console.WriteLine();
    }

    /// <summary>
    /// Example 5: Error handling
    /// </summary>
    static async Task Example5_ErrorHandling()
    {
        Console.WriteLine("Example 5: Error Handling");
        Console.WriteLine("-------------------------");

        try
        {
            // This will return 404
            var result = await Curl.GetAsync("https://httpbin.org/status/404");

            // Check status and throw if error
            result.EnsureSuccessStatusCode();
        }
        catch (CurlHttpException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.StatusCode} - {ex.Message}");
            Console.WriteLine($"Is Client Error: {ex.IsClientError}");
            Console.WriteLine($"Is Not Found: {ex.IsNotFound}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Example 6: Custom headers and authentication
    /// </summary>
    static async Task Example6_Headers()
    {
        Console.WriteLine("Example 6: Headers & Auth");
        Console.WriteLine("-------------------------");

        // Example with Bearer token (using httpbin echo)
        var command = @"curl -X GET https://httpbin.org/bearer \
            -H 'Authorization: Bearer sample-token-12345'";

        var result = await Curl.ExecuteAsync(command);

        if (result.IsSuccess)
        {
            var response = JsonDocument.Parse(result.Body);
            if (response.RootElement.TryGetProperty("token", out var token))
            {
                Console.WriteLine($"Token accepted: {token.GetString()}");
            }
            else
            {
                Console.WriteLine("Bearer auth endpoint reached");
            }
        }
        else
        {
            Console.WriteLine($"Auth required: {result.StatusCode}");
        }
        Console.WriteLine();
    }
}