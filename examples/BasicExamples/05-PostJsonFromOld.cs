using System;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

namespace PostJson
{
    /// <summary>
    /// Demonstrates sending JSON data in POST requests
    /// Based on cookbook/beginner/send-json.md
    /// </summary>
    class Program
    {
        // Sample data class
        public class User
        {
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public int Age { get; set; }
        }

        public class TodoItem
        {
            public int UserId { get; set; }
            public string Title { get; set; } = "";
            public bool Completed { get; set; }
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CurlDotNet: POST JSON Example ===\n");

            // Example 1: POST JSON as a string
            Console.WriteLine("1. POSTing JSON string to httpbin.org...");

            var jsonString = "{\"name\":\"John Doe\",\"email\":\"john@example.com\",\"age\":30}";

            var result1 = await Curl.PostAsync(
                "https://httpbin.org/post",
                jsonString,
                "application/json"
            );

            if (result1.IsSuccess)
            {
                Console.WriteLine("✅ JSON posted successfully!");
                // httpbin.org echoes back what we sent
                var response = JsonDocument.Parse(result1.Body);
                var echoedData = response.RootElement.GetProperty("data").GetString();
                Console.WriteLine($"Server received: {echoedData}\n");
            }

            // Example 2: POST JSON from C# object
            Console.WriteLine("2. POSTing C# object as JSON...");

            var user = new User
            {
                Name = "Jane Smith",
                Email = "jane@example.com",
                Age = 25
            };

            var userJson = JsonSerializer.Serialize(user);

            var result2 = await Curl.PostAsync(
                "https://httpbin.org/post",
                userJson,
                "application/json"
            );

            if (result2.IsSuccess)
            {
                Console.WriteLine("✅ Object serialized and posted!");
                Console.WriteLine($"Sent: {userJson}\n");
            }

            // Example 3: POST to a real API (JSONPlaceholder)
            Console.WriteLine("3. Creating a todo item on JSONPlaceholder...");

            var todo = new TodoItem
            {
                UserId = 1,
                Title = "Learn CurlDotNet",
                Completed = false
            };

            var todoJson = JsonSerializer.Serialize(todo);

            var result3 = await Curl.PostAsync(
                "https://jsonplaceholder.typicode.com/todos",
                todoJson,
                "application/json"
            );

            if (result3.IsSuccess)
            {
                Console.WriteLine("✅ Todo created!");

                // Parse the response
                var createdTodo = JsonSerializer.Deserialize<dynamic>(result3.Body);
                Console.WriteLine($"Server response: {result3.Body}\n");
            }

            // Example 4: Using curl command syntax
            Console.WriteLine("4. Using curl command syntax for JSON POST...");

            var curlCommand = @"curl -X POST https://httpbin.org/post
                -H 'Content-Type: application/json'
                -d '{""message"":""Hello from CurlDotNet!""}'";

            var result4 = await Curl.ExecuteAsync(curlCommand);

            if (result4.IsSuccess)
            {
                Console.WriteLine("✅ Curl command executed successfully!");
                var response = JsonDocument.Parse(result4.Body);
                var sentData = response.RootElement.GetProperty("data").GetString();
                Console.WriteLine($"Data sent via curl command: {sentData}\n");
            }

            // Example 5: POST with custom headers
            Console.WriteLine("5. POSTing JSON with custom headers...");

            var result5 = await Curl.ExecuteAsync(@"
                curl -X POST https://httpbin.org/post
                -H 'Content-Type: application/json'
                -H 'X-API-Key: demo-key-12345'
                -H 'X-Request-ID: " + Guid.NewGuid() + @"'
                -d '{""action"":""create"",""resource"":""user""}'
            ");

            if (result5.IsSuccess)
            {
                Console.WriteLine("✅ JSON with headers posted!");
                var response = JsonDocument.Parse(result5.Body);
                var headers = response.RootElement.GetProperty("headers");
                Console.WriteLine("Headers received by server:");
                Console.WriteLine($"  X-Api-Key: {headers.GetProperty("X-Api-Key").GetString()}");
                Console.WriteLine($"  X-Request-Id: {headers.GetProperty("X-Request-Id").GetString()}\n");
            }

            Console.WriteLine("=== Example Complete ===");
            Console.WriteLine("\nKey Takeaways:");
            Console.WriteLine("• Use Content-Type: application/json for JSON requests");
            Console.WriteLine("• Serialize C# objects with System.Text.Json");
            Console.WriteLine("• httpbin.org is great for testing");
            Console.WriteLine("• JSONPlaceholder provides a fake REST API for testing");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}