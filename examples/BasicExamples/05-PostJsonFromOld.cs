using System;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;

namespace PostJson
{
    /// <summary>
    /// Demonstrates sending JSON data in POST requests
    /// Based on cookbook/beginner/send-json.md
    /// </summary>
    class Program
    {
        // Sample data classes
        public class User
        {
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public int Age { get; set; }
        }

        public class TodoItem
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Title { get; set; } = "";
            public bool Completed { get; set; }
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CurlDotNet: POST JSON Example ===\n");

            // Example 1: POST a JSON string you already have
            Console.WriteLine("1. POSTing JSON string to httpbin.org...");
            var jsonString = "{\"name\":\"John Doe\",\"email\":\"john@example.com\",\"age\":30}";

            var result1 = await CurlRequestBuilder.Post("https://httpbin.org/post")
                .WithHeader("Content-Type", "application/json")
                .WithData(jsonString)
                .ExecuteAsync();

            if (result1.IsSuccess)
            {
                // httpbin.org echoes back what we sent in its 'data' field
                using var response = JsonDocument.Parse(result1.Body);
                Console.WriteLine($"Server received: {response.RootElement.GetProperty("data").GetString()}\n");
            }

            // Example 2: POST a C# object - PostJsonAsync serializes it for you
            Console.WriteLine("2. POSTing C# object as JSON...");
            var user = new User { Name = "Jane Smith", Email = "jane@example.com", Age = 25 };

            var result2 = await Curl.PostJsonAsync("https://httpbin.org/post", user);

            if (result2.IsSuccess)
            {
                using var response = JsonDocument.Parse(result2.Body);
                Console.WriteLine($"Sent: {response.RootElement.GetProperty("data").GetString()}\n");
            }

            // Example 3: POST to a fake REST API and read the created resource back as a type
            Console.WriteLine("3. Creating a todo item on JSONPlaceholder...");
            var todo = new TodoItem { UserId = 1, Title = "Learn CurlDotNet", Completed = false };

            var result3 = await Curl.PostJsonAsync("https://jsonplaceholder.typicode.com/todos", todo);

            if (result3.IsSuccess)
            {
                var createdTodo = result3.ParseJson<TodoItem>();
                Console.WriteLine($"Todo created with id {createdTodo.Id}: {createdTodo.Title}\n");
            }

            // Example 4: Using curl command syntax - newlines between options are fine
            Console.WriteLine("4. Using curl command syntax for JSON POST...");
            var result4 = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post
                -H 'Content-Type: application/json'
                -d '{""message"":""Hello from CurlDotNet!""}'");

            if (result4.IsSuccess)
            {
                using var response = JsonDocument.Parse(result4.Body);
                Console.WriteLine($"Data sent via curl command: {response.RootElement.GetProperty("data").GetString()}\n");
            }

            // Example 5: POST with custom headers
            Console.WriteLine("5. POSTing JSON with custom headers...");
            var result5 = await Curl.ExecuteAsync(@"
                curl -X POST https://httpbin.org/post
                -H 'Content-Type: application/json'
                -H 'X-API-Key: demo-key-12345'
                -H 'X-Correlation-ID: " + Guid.NewGuid() + @"'
                -d '{""action"":""create"",""resource"":""user""}'
            ");

            if (result5.IsSuccess)
            {
                using var response = JsonDocument.Parse(result5.Body);
                var headers = response.RootElement.GetProperty("headers");
                Console.WriteLine("Headers received by server:");
                Console.WriteLine($"  X-Api-Key: {headers.GetProperty("X-Api-Key").GetString()}");
                Console.WriteLine($"  X-Correlation-Id: {headers.GetProperty("X-Correlation-Id").GetString()}\n");
            }

            Console.WriteLine("=== Key Takeaways ===");
            Console.WriteLine("- PostJsonAsync / WithJson serialize objects and set Content-Type for you");
            Console.WriteLine("- WithData + a Content-Type header sends a JSON string as-is");
            Console.WriteLine("- ParseJson<T>() turns the response body back into a C# type");
            Console.WriteLine("- httpbin.org and JSONPlaceholder are handy for testing");
        }
    }
}
