// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using System.Text.Json;

namespace CurlDotNet.Examples;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("============================================");
        Console.WriteLine("  CurlDotNet Examples - Comprehensive Demo");
        Console.WriteLine("============================================");
        Console.WriteLine();

        // Run all examples
        await Example1_HelloWorld();
        await Example2_DirectCurlCommand();
        await Example3_WorkingWithResult();
        await Example4_ParseJsonResponse();
        await Example5_DownloadToFile();
        await Example6_ErrorHandling();
        await Example7_HeadersAndStatus();
        await Example8_FluentBuilder();
        await Example9_ApiClient();
        await Example10_ExtensionMethods();

        Console.WriteLine("\n✅ All examples completed successfully!");
    }

    /// <summary>
    /// Example 1: Hello World in 5 minutes - Simplest possible example
    /// </summary>
    static async Task Example1_HelloWorld()
    {
        Console.WriteLine("📌 Example 1: Hello World - GET Request");
        Console.WriteLine("-----------------------------------------");

        // NuGet: https://www.nuget.org/packages/CurlDotNet/
        var result = await Curl.GetAsync("https://api.github.com/zen");

        Console.WriteLine($"GitHub Zen: {result.Body}");
        Console.WriteLine($"Success: {result.IsSuccess}");
        Console.WriteLine();
    }

    /// <summary>
    /// Example 2: Direct curl command - paste any curl command
    /// </summary>
    static async Task Example2_DirectCurlCommand()
    {
        Console.WriteLine("📌 Example 2: Direct curl Command");
        Console.WriteLine("----------------------------------");

        // You can paste curl commands directly - with or without "curl" prefix!
        var result = await Curl.ExecuteAsync(@"
            curl -X GET https://httpbin.org/get \
            -H 'User-Agent: CurlDotNet/1.0' \
            -H 'Accept: application/json'
        ");

        Console.WriteLine($"Status: {result.StatusCode}");
        Console.WriteLine($"Response length: {result.Body.Length} bytes");
        Console.WriteLine();
    }

    /// <summary>
    /// Example 3: Working with the Result object - the core of CurlDotNet
    /// </summary>
    static async Task Example3_WorkingWithResult()
    {
        Console.WriteLine("📌 Example 3: Understanding the Result Object");
        Console.WriteLine("----------------------------------------------");

        var result = await Curl.GetAsync("https://httpbin.org/json");

        // Result object properties
        Console.WriteLine($"✅ IsSuccess: {result.IsSuccess}");
        Console.WriteLine($"📊 StatusCode: {result.StatusCode}");
        Console.WriteLine($"📝 Body length: {result.Body?.Length ?? 0} characters");
        Console.WriteLine($"⏱️ Elapsed time: {result.ElapsedTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"🔗 Command executed: {result.Command}");

        // Accessing headers
        if (result.Headers != null && result.Headers.ContainsKey("Content-Type"))
        {
            Console.WriteLine($"📋 Content-Type: {result.Headers["Content-Type"]}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Example 4: Parse JSON responses - common use case
    /// </summary>
    static async Task Example4_ParseJsonResponse()
    {
        Console.WriteLine("📌 Example 4: Parse JSON Response");
        Console.WriteLine("----------------------------------");

        var result = await Curl.GetAsync("https://jsonplaceholder.typicode.com/users/1");

        if (result.IsSuccess)
        {
            // Parse JSON using the extension method
            var user = result.ParseJson<User>();
            Console.WriteLine($"User: {user.Name}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Company: {user.Company?.Name}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Example 5: Download to file - binary data handling
    /// </summary>
    static async Task Example5_DownloadToFile()
    {
        Console.WriteLine("📌 Example 5: Download to File");
        Console.WriteLine("-------------------------------");

        // Download a small image
        var result = await Curl.GetAsync("https://via.placeholder.com/150");

        if (result.IsSuccess && result.BinaryData != null)
        {
            var fileName = "downloaded_image.png";
            result.SaveToFile(fileName);
            Console.WriteLine($"✅ Downloaded {result.BinaryData.Length} bytes to {fileName}");

            // Clean up
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                Console.WriteLine("🧹 Cleaned up test file");
            }
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Example 6: Error handling - dealing with failures
    /// </summary>
    static async Task Example6_ErrorHandling()
    {
        Console.WriteLine("📌 Example 6: Error Handling");
        Console.WriteLine("-----------------------------");

        try
        {
            var result = await Curl.GetAsync("https://httpbin.org/status/404");

            // Method 1: Check IsSuccess
            if (!result.IsSuccess)
            {
                Console.WriteLine($"❌ Request failed with status: {result.StatusCode}");
            }

            // Method 2: Use EnsureSuccessStatusCode (will throw)
            result.EnsureSuccessStatusCode();
        }
        catch (Exceptions.CurlHttpException ex)
        {
            Console.WriteLine($"❌ HTTP Error: {ex.StatusCode}");
            Console.WriteLine($"   IsClientError: {ex.IsClientError}");
            Console.WriteLine($"   IsNotFound: {ex.IsNotFound}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Example 7: Headers and status codes - detailed response info
    /// </summary>
    static async Task Example7_HeadersAndStatus()
    {
        Console.WriteLine("📌 Example 7: Headers and Status");
        Console.WriteLine("---------------------------------");

        var result = await Curl.PostAsync(
            "https://httpbin.org/post",
            "{\"test\": \"data\"}",
            "application/json"
        );

        // Check status categories
        Console.WriteLine($"2xx Success: {result.IsSuccess}");
        Console.WriteLine($"Status Code: {result.StatusCode}");

        // Access headers using extension method
        var contentType = result.GetHeader("Content-Type");
        Console.WriteLine($"Content-Type: {contentType}");

        // Check if response is JSON
        bool isJson = result.HasContentType("application/json");
        Console.WriteLine($"Is JSON response: {isJson}");

        Console.WriteLine();
    }

    /// <summary>
    /// Example 8: Fluent builder API - programmatic request building
    /// </summary>
    static async Task Example8_FluentBuilder()
    {
        Console.WriteLine("📌 Example 8: Fluent Builder API");
        Console.WriteLine("---------------------------------");

        var result = await Core.CurlRequestBuilder
            .Get("https://httpbin.org/headers")
            .WithHeader("X-Custom-Header", "CurlDotNet")
            .WithUserAgent("CurlDotNet-Examples/1.0")
            .WithTimeout(TimeSpan.FromSeconds(10))
            .ExecuteAsync();

        Console.WriteLine($"Status: {result.StatusCode}");

        // Parse response to see our headers were sent
        if (result.IsSuccess)
        {
            var response = JsonDocument.Parse(result.Body);
            var headers = response.RootElement.GetProperty("headers");

            if (headers.TryGetProperty("X-Custom-Header", out var customHeader))
            {
                Console.WriteLine($"Custom header received: {customHeader.GetString()}");
            }
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Example 9: API Client - simplified REST operations
    /// </summary>
    static async Task Example9_ApiClient()
    {
        Console.WriteLine("📌 Example 9: API Client Pattern");
        Console.WriteLine("---------------------------------");

        // Create a reusable API client
        var api = new CurlApiClient("https://jsonplaceholder.typicode.com");

        // GET request
        var getResult = await api.GetAsync("posts/1");
        Console.WriteLine($"GET Status: {getResult.StatusCode}");

        // POST request
        var newPost = new
        {
            title = "Test Post",
            body = "This is a test from CurlDotNet",
            userId = 1
        };

        var postResult = await api.PostJsonAsync("posts", newPost);
        Console.WriteLine($"POST Status: {postResult.StatusCode}");

        if (postResult.IsSuccess)
        {
            var created = JsonDocument.Parse(postResult.Body);
            Console.WriteLine($"Created post ID: {created.RootElement.GetProperty("id")}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Example 10: Extension methods - ergonomic helpers
    /// </summary>
    static async Task Example10_ExtensionMethods()
    {
        Console.WriteLine("📌 Example 10: Extension Methods");
        Console.WriteLine("---------------------------------");

        var result = await Curl.GetAsync("https://jsonplaceholder.typicode.com/todos/1");

        // Convert to simple tuple
        var (success, body, error) = result.ToSimple();
        Console.WriteLine($"Simple result - Success: {success}");

        // Try parse JSON (safe, returns false if fails)
        if (result.TryParseJson<Todo>(out var todo))
        {
            Console.WriteLine($"Todo: {todo.Title}");
            Console.WriteLine($"Completed: {todo.Completed}");
        }

        // Save to file (commented out to avoid file creation)
        // result.SaveToFile("response.json");

        Console.WriteLine();
    }

    // Simple models for JSON parsing examples
    class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public Company? Company { get; set; }
    }

    class Company
    {
        public string Name { get; set; } = "";
    }

    class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public bool Completed { get; set; }
    }
}