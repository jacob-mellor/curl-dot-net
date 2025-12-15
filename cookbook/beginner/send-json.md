# Recipe: Send JSON Data

## 🎯 What You'll Build

A program that sends JSON data to an API - one of the most common tasks in modern web development.

## 🥘 Ingredients

- CurlDotNet package
- An API endpoint that accepts JSON
- 5 minutes

## 📖 The Recipe

### Understanding JSON

JSON (JavaScript Object Notation) is how APIs share data. It looks like this:

```json
{
  "name": "Alice",
  "age": 30,
  "email": "alice@example.com"
}
```

### Step 1: Simple JSON POST

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        // Send JSON to an API
        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://httpbin.org/post \
            -H 'Content-Type: application/json' \
            -d '{""name"": ""Alice"", ""age"": 30}'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("JSON sent successfully!");
            Console.WriteLine(result.Body);  // httpbin echoes back what you sent
        }
    }
}
```

### Step 2: Using C# Objects

Instead of writing JSON strings, use C# objects:

```csharp
using System;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

// Define your data structure
public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
}

class Program
{
    static async Task Main()
    {
        // Create a C# object
        var user = new User
        {
            Name = "Alice",
            Age = 30,
            Email = "alice@example.com"
        };

        // Convert to JSON
        var json = JsonSerializer.Serialize(user);

        // Send it
        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://httpbin.org/post \
            -H 'Content-Type: application/json' \
            -d '{json}'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("User data sent!");
        }
    }
}
```

## 🍳 Complete Examples

### Example 1: Create a User

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class CreateUserRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
}

public class CreateUserResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Message { get; set; }
}

class UserRegistration
{
    static async Task Main()
    {
        Console.WriteLine("=== User Registration ===");

        Console.Write("Username: ");
        var username = Console.ReadLine();

        Console.Write("Email: ");
        var email = Console.ReadLine();

        Console.Write("Password: ");
        var password = Console.ReadLine();

        Console.Write("Full Name: ");
        var fullName = Console.ReadLine();

        // Create registration request
        var registration = new CreateUserRequest
        {
            Username = username,
            Password = password,
            Email = email,
            FullName = fullName
        };

        // Convert to JSON
        var json = System.Text.Json.JsonSerializer.Serialize(registration);

        // Send to API
        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://reqres.in/api/users \
            -H 'Content-Type: application/json' \
            -d '{json}'
        ");

        if (result.IsSuccess)
        {
            var response = result.ParseJson<CreateUserResponse>();
            Console.WriteLine($"✅ User created! ID: {response.Id}");
        }
        else
        {
            Console.WriteLine($"❌ Failed: {result.StatusCode}");
        }
    }
}
```

### Example 2: Update Settings

```csharp
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

public class Settings
{
    public bool Notifications { get; set; }
    public string Theme { get; set; }
    public string Language { get; set; }
    public Dictionary<string, bool> Features { get; set; }
}

class UpdateSettings
{
    static async Task Main()
    {
        // Create settings object
        var settings = new Settings
        {
            Notifications = true,
            Theme = "dark",
            Language = "en-US",
            Features = new Dictionary<string, bool>
            {
                ["beta_features"] = true,
                ["analytics"] = false,
                ["recommendations"] = true
            }
        };

        // Convert to JSON
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true  // Pretty print
        });

        Console.WriteLine("Sending settings:");
        Console.WriteLine(json);

        // Send to API
        var result = await Curl.ExecuteAsync($@"
            curl -X PUT https://httpbin.org/put \
            -H 'Content-Type: application/json' \
            -H 'Authorization: Bearer your-token-here' \
            -d '{json}'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✅ Settings updated!");
        }
    }
}
```

### Example 3: Complex Nested JSON

```csharp
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

public class Order
{
    public string OrderId { get; set; }
    public Customer Customer { get; set; }
    public List<Item> Items { get; set; }
    public Address ShippingAddress { get; set; }
    public decimal Total { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class Item
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
}

class ComplexJsonExample
{
    static async Task Main()
    {
        // Create complex order object
        var order = new Order
        {
            OrderId = "ORD-12345",
            Customer = new Customer
            {
                Id = 1001,
                Name = "John Smith",
                Email = "john@example.com"
            },
            Items = new List<Item>
            {
                new Item
                {
                    ProductId = "PROD-001",
                    Name = "Laptop",
                    Quantity = 1,
                    Price = 999.99m
                },
                new Item
                {
                    ProductId = "PROD-002",
                    Name = "Mouse",
                    Quantity = 2,
                    Price = 25.99m
                }
            },
            ShippingAddress = new Address
            {
                Street = "123 Main St",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701"
            },
            Total = 1051.97m
        };

        // Convert to JSON with formatting
        var json = JsonSerializer.Serialize(order, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Console.WriteLine("Sending order:");
        Console.WriteLine(json);

        // Send to API
        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://httpbin.org/post \
            -H 'Content-Type: application/json' \
            -H 'X-API-Key: your-api-key' \
            -d '{json.Replace("\"", "\\\"")}'  // Escape quotes for shell
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✅ Order submitted!");

            // Parse response
            dynamic response = result.AsJsonDynamic();
            Console.WriteLine($"Server received: {response.json.orderId}");
        }
    }
}
```

## 🎨 Variations

### Using Different HTTP Methods

```csharp
// POST - Create new resource
var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/users \
    -H 'Content-Type: application/json' \
    -d '{json}'
");

// PUT - Update entire resource
var result = await Curl.ExecuteAsync($@"
    curl -X PUT https://api.example.com/users/123 \
    -H 'Content-Type: application/json' \
    -d '{json}'
");

// PATCH - Partial update
var result = await Curl.ExecuteAsync($@"
    curl -X PATCH https://api.example.com/users/123 \
    -H 'Content-Type: application/json' \
    -d '{json}'
");
```

### With Authentication

```csharp
// Bearer token
var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/data \
    -H 'Content-Type: application/json' \
    -H 'Authorization: Bearer {token}' \
    -d '{json}'
");

// API key
var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/data \
    -H 'Content-Type: application/json' \
    -H 'X-API-Key: {apiKey}' \
    -d '{json}'
");

// Basic auth
var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/data \
    -H 'Content-Type: application/json' \
    -u username:password \
    -d '{json}'
");
```

### JSON from File

```csharp
// Read JSON from file
var json = await File.ReadAllTextAsync("data.json");

var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/import \
    -H 'Content-Type: application/json' \
    -d '{json}'
");
```

## 🐛 Troubleshooting

### Problem: "400 Bad Request"
**Solution**: Your JSON might be invalid

```csharp
// Validate JSON first
try
{
    var doc = JsonDocument.Parse(json);
    // JSON is valid
}
catch (JsonException ex)
{
    Console.WriteLine($"Invalid JSON: {ex.Message}");
}
```

### Problem: Quotes Breaking the Command
**Solution**: Escape quotes properly

```csharp
// Method 1: Escape quotes
var escapedJson = json.Replace("\"", "\\\"");

// Method 2: Use single quotes outside
var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com \
    -H 'Content-Type: application/json' \
    -d '{escapedJson}'
");

// Method 3: Use CurlRequestBuilder instead
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithMethod(HttpMethod.Post)
    .WithJson(yourObject)
    .ExecuteAsync();
```

### Problem: "415 Unsupported Media Type"
**Solution**: Add Content-Type header

```csharp
var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com \
    -H 'Content-Type: application/json' \  {/* Always include this! */}
    -d '{json}'
");
```

### Problem: Special Characters in JSON
**Solution**: Properly escape special characters

```csharp
// Use JsonSerializer to handle escaping
var safeJson = JsonSerializer.Serialize(new
{
    message = "Hello \"World\"",  // Quotes
    path = @"C:\Users\file.txt",  // Backslashes
    unicode = "Hello 世界 🌍"      // Unicode
});
```

## 📊 Common JSON Patterns

### Single Object
```json
{
  "name": "Alice",
  "age": 30
}
```

### Array of Objects
```json
[
  {"id": 1, "name": "Alice"},
  {"id": 2, "name": "Bob"}
]
```

### Nested Objects
```json
{
  "user": {
    "name": "Alice",
    "address": {
      "city": "New York",
      "country": "USA"
    }
  }
}
```

### Mixed Types
```json
{
  "string": "text",
  "number": 123,
  "boolean": true,
  "null": null,
  "array": [1, 2, 3],
  "object": {"key": "value"}
}
```

## 🚀 Next Steps

Now that you can send JSON:

1. Learn to [Handle API Responses](handle-errors.html)
2. Try [Uploading Files](upload-file.html)
3. Check the [API Guide](../../api-guide/README.html) for bearer tokens and authentication

## 📚 Related Recipes

- [Call an API](call-api.html)
- [Simple GET Request](simple-get.html)
- [POST Form Data](post-form.html)
- [Handle Errors](handle-errors.html)

## 🎓 Key Takeaways

- Always set `Content-Type: application/json`
- Use C# objects and serialize to JSON
- Escape quotes in JSON strings
- Check response status before using data
- Use proper HTTP method (POST, PUT, PATCH)

---

**Need help?** Check [Troubleshooting](../../troubleshooting/README.html) | **Have questions?** Ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)