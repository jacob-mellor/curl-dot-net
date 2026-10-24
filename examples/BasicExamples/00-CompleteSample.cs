using System;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;

namespace CurlDotNet.Sample;

/// <summary>
/// A tour of the main CurlDotNet entry points: pasted commands, quick helpers,
/// the fluent builder, error handling and headers.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== CurlDotNet Sample Application ===\n");

        await Example1_SimpleCurlCommand();
        await Example2_GetRequest();
        await Example3_PostJson();
        await Example4_FluentApi();
        await Example5_ErrorHandling();
        await Example6_Headers();

        Console.WriteLine("All examples finished.");
    }

    // Example 1: Direct curl command execution - paste it exactly as you would in a terminal
    static async Task Example1_SimpleCurlCommand()
    {
        Console.WriteLine("Example 1: Direct curl command");
        var result = await Curl.ExecuteAsync("curl https://api.github.com/zen");

        Console.WriteLine($"GitHub Zen: {result.Body}");
        Console.WriteLine($"Status: {result.StatusCode}\n");
    }

    // Example 2: Simple GET request with the Curl.GetAsync helper
    static async Task Example2_GetRequest()
    {
        Console.WriteLine("Example 2: GET request");
        var result = await Curl.GetAsync("https://jsonplaceholder.typicode.com/posts/1");

        if (result.IsSuccess)
        {
            using var json = JsonDocument.Parse(result.Body);
            Console.WriteLine($"Post title: {json.RootElement.GetProperty("title").GetString()}");

            // Timings are in milliseconds, like curl's -w '%{time_total}'
            Console.WriteLine($"Response time: {result.Timings?.Total:F0} ms");
        }
        Console.WriteLine();
    }

    // Example 3: POST an object as JSON - serialized for you, Content-Type set for you
    static async Task Example3_PostJson()
    {
        Console.WriteLine("Example 3: POST JSON");
        var payload = new { title = "Sample Post", body = "Sent from CurlDotNet", userId = 1 };

        var result = await Curl.PostJsonAsync("https://jsonplaceholder.typicode.com/posts", payload);

        using var json = JsonDocument.Parse(result.Body);
        Console.WriteLine($"Created post id: {json.RootElement.GetProperty("id")}");
        Console.WriteLine($"Status: {result.StatusCode}\n");
    }

    // Example 4: Fluent builder - CurlRequestBuilder is the chainable API
    static async Task Example4_FluentApi()
    {
        Console.WriteLine("Example 4: Fluent API");
        var result = await CurlRequestBuilder
            .Get("https://httpbin.org/get")
            .WithUserAgent("CurlDotNet-Sample/1.0")
            .WithHeader("Accept", "application/json")
            .WithTimeout(TimeSpan.FromSeconds(10))
            .ExecuteAsync();

        // httpbin echoes back the headers it received
        using var response = JsonDocument.Parse(result.Body);
        var userAgent = response.RootElement.GetProperty("headers").GetProperty("User-Agent").GetString();

        Console.WriteLine($"Sent User-Agent: {userAgent}");
        Console.WriteLine($"Response size: {result.Body.Length} characters\n");
    }

    // Example 5: Error handling - a 404 is a normal result until you call EnsureSuccess()
    static async Task Example5_ErrorHandling()
    {
        Console.WriteLine("Example 5: Error handling");
        try
        {
            var result = await Curl.GetAsync("https://httpbin.org/status/404");
            result.EnsureSuccess(); // throws CurlDotNet.Core.CurlHttpException for non-2xx
        }
        catch (CurlHttpException ex)
        {
            Console.WriteLine($"HTTP error: {ex.StatusCode} - {ex.Message}");
            Console.WriteLine($"Is client error: {ex.StatusCode >= 400 && ex.StatusCode < 500}");
            Console.WriteLine($"Is not found: {ex.StatusCode == 404}");
        }
        Console.WriteLine();
    }

    // Example 6: Custom headers and authentication inside a pasted, multi-line command
    static async Task Example6_Headers()
    {
        Console.WriteLine("Example 6: Headers and auth");
        var command = @"curl -X GET https://httpbin.org/bearer \
            -H 'Authorization: Bearer sample-token-12345'";

        var result = await Curl.ExecuteAsync(command);

        if (result.IsSuccess)
        {
            using var response = JsonDocument.Parse(result.Body);
            Console.WriteLine($"Token accepted: {response.RootElement.GetProperty("token").GetString()}");
        }
        else
        {
            Console.WriteLine($"Auth required: {result.StatusCode}");
        }
        Console.WriteLine();
    }
}
