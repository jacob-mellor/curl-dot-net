# Tutorial 7: JSON for Beginners

JSON (JavaScript Object Notation) is the most common data format for modern APIs. This tutorial shows you how to work with JSON data using CurlDotNet.

## What is JSON?

JSON is a human-readable format for structuring data:

```json
{
  "name": "John Doe",
  "age": 30,
  "email": "john@example.com",
  "isActive": true,
  "scores": [95, 87, 92],
  "address": {
    "street": "123 Main St",
    "city": "Springfield"
  }
}
```

## Receiving JSON Data

### Basic JSON GET Request
```csharp
var curl = new Curl();
var result = await curl.GetAsync("https://api.example.com/user/123");

if (result.IsSuccess)
{
    // The JSON is in result.Data as a string
    Console.WriteLine(result.Data);
    // Output: {"name":"John Doe","age":30,...}
}
```

### Parsing JSON to Objects
```csharp
// Define a class matching the JSON structure
public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}

// Parse the JSON
var result = await curl.GetAsync("https://api.example.com/user/123");
if (result.IsSuccess)
{
    var user = JsonSerializer.Deserialize<User>(result.Data);
    Console.WriteLine($"Name: {user.Name}, Age: {user.Age}");
}
```

## Sending JSON Data

### POST with JSON
```csharp
// Method 1: Using anonymous objects
var data = new
{
    name = "Jane Smith",
    age = 25,
    email = "jane@example.com"
};

var result = await curl.PostJsonAsync("https://api.example.com/users", data);

// Method 2: Using typed objects
var newUser = new User
{
    Name = "Jane Smith",
    Age = 25,
    Email = "jane@example.com"
};

var result = await curl.PostJsonAsync("https://api.example.com/users", newUser);
```

### PUT/PATCH with JSON
```csharp
// Update user data
var updates = new
{
    age = 26,
    email = "newemail@example.com"
};

// PUT replaces entire resource
var putResult = await curl.PutJsonAsync("https://api.example.com/users/123", updates);

// PATCH updates specific fields
var patchResult = await curl.PatchJsonAsync("https://api.example.com/users/123", updates);
```

## Working with Complex JSON

### Nested Objects
```csharp
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
}

public class Person
{
    public string Name { get; set; }
    public Address HomeAddress { get; set; }
    public Address WorkAddress { get; set; }
}

// Sending nested data
var person = new Person
{
    Name = "John Doe",
    HomeAddress = new Address
    {
        Street = "123 Main St",
        City = "Springfield",
        ZipCode = "12345"
    },
    WorkAddress = new Address
    {
        Street = "456 Office Blvd",
        City = "Business City",
        ZipCode = "67890"
    }
};

var result = await curl.PostJsonAsync("https://api.example.com/people", person);
```

### Arrays and Lists
```csharp
public class Order
{
    public int OrderId { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
}

public class OrderItem
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

// Working with arrays
var order = new Order
{
    OrderId = 12345,
    Items = new List<OrderItem>
    {
        new OrderItem { ProductName = "Widget", Quantity = 2, Price = 9.99m },
        new OrderItem { ProductName = "Gadget", Quantity = 1, Price = 19.99m }
    },
    TotalAmount = 39.97m
};

var result = await curl.PostJsonAsync("https://api.example.com/orders", order);
```

## JSON Serialization Options

### Case Sensitivity
```csharp
// JSON often uses camelCase, C# uses PascalCase
var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

// Deserialize with camelCase mapping
var json = @"{"firstName":"John","lastName":"Doe"}";
var user = JsonSerializer.Deserialize<User>(json, options);

// Serialize to camelCase
var userData = new User { FirstName = "John", LastName = "Doe" };
var jsonString = JsonSerializer.Serialize(userData, options);
// Output: {"firstName":"John","lastName":"Doe"}
```

### Handling Null Values
```csharp
var options = new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

var data = new
{
    name = "John",
    age = (int?)null,  // This will be omitted
    email = "john@example.com"
};

var json = JsonSerializer.Serialize(data, options);
// Output: {"name":"John","email":"john@example.com"}
```

### Pretty Printing
```csharp
var options = new JsonSerializerOptions
{
    WriteIndented = true  // Format JSON with indentation
};

var data = new { name = "John", age = 30 };
var json = JsonSerializer.Serialize(data, options);
Console.WriteLine(json);
/* Output:
{
  "name": "John",
  "age": 30
}
*/
```

## Error Handling with JSON

### Safe Parsing
```csharp
public async Task<User?> GetUserSafely(int id)
{
    var curl = new Curl();
    var result = await curl.GetAsync($"https://api.example.com/users/{id}");

    if (!result.IsSuccess)
    {
        _logger.LogError($"Failed to fetch user: {result.Error}");
        return null;
    }

    try
    {
        return JsonSerializer.Deserialize<User>(result.Data);
    }
    catch (JsonException ex)
    {
        _logger.LogError($"Invalid JSON response: {ex.Message}");
        _logger.LogDebug($"Raw response: {result.Data}");
        return null;
    }
}
```

### Validating JSON Structure
```csharp
public bool IsValidJson(string json)
{
    try
    {
        using var doc = JsonDocument.Parse(json);
        return true;
    }
    catch (JsonException)
    {
        return false;
    }
}

// Check before parsing
var result = await curl.GetAsync(url);
if (result.IsSuccess && IsValidJson(result.Data))
{
    var data = JsonSerializer.Deserialize<MyType>(result.Data);
}
```

## Working with Dynamic JSON

### Using JsonDocument
```csharp
var result = await curl.GetAsync("https://api.example.com/data");

if (result.IsSuccess)
{
    using var doc = JsonDocument.Parse(result.Data);
    var root = doc.RootElement;

    // Access properties dynamically
    if (root.TryGetProperty("name", out var nameElement))
    {
        string name = nameElement.GetString();
    }

    // Navigate nested structures
    if (root.TryGetProperty("address", out var address))
    {
        if (address.TryGetProperty("city", out var city))
        {
            Console.WriteLine($"City: {city.GetString()}");
        }
    }

    // Work with arrays
    if (root.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array)
    {
        foreach (var item in items.EnumerateArray())
        {
            Console.WriteLine(item.GetProperty("name").GetString());
        }
    }
}
```

### Using Dynamic Objects
```csharp
// For simple scenarios, use dynamic (requires Newtonsoft.Json)
dynamic data = JsonConvert.DeserializeObject(result.Data);
Console.WriteLine(data.name);
Console.WriteLine(data.address.city);
```

## Common JSON Patterns

### Pagination
```csharp
public class PagedResponse<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public bool HasNextPage { get; set; }
}

// Fetch paginated data
var result = await curl.GetAsync("https://api.example.com/users?page=1&size=10");
if (result.IsSuccess)
{
    var pagedUsers = JsonSerializer.Deserialize<PagedResponse<User>>(result.Data);

    Console.WriteLine($"Total users: {pagedUsers.TotalCount}");
    foreach (var user in pagedUsers.Items)
    {
        Console.WriteLine(user.Name);
    }

    if (pagedUsers.HasNextPage)
    {
        // Fetch next page...
    }
}
```

### API Responses with Metadata
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; }
}

// Handle wrapped responses
var result = await curl.GetAsync("https://api.example.com/users/123");
if (result.IsSuccess)
{
    var apiResponse = JsonSerializer.Deserialize<ApiResponse<User>>(result.Data);

    if (apiResponse.Success)
    {
        var user = apiResponse.Data;
        Console.WriteLine($"User: {user.Name}");
    }
    else
    {
        Console.WriteLine($"API Error: {apiResponse.Message}");
        foreach (var error in apiResponse.Errors ?? new List<string>())
        {
            Console.WriteLine($"  - {error}");
        }
    }
}
```

## Extension Methods for JSON

Create helpers to simplify JSON operations:

```csharp
public static class CurlJsonExtensions
{
    public static async Task<T?> GetJsonAsync<T>(this Curl curl, string url)
    {
        var result = await curl.GetAsync(url);

        if (!result.IsSuccess)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(result.Data);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    public static async Task<CurlResult> PostAsJsonAsync<T>(
        this Curl curl, string url, T data)
    {
        var json = JsonSerializer.Serialize(data);
        return await curl.PostAsync(url, json, "application/json");
    }
}

// Usage
var user = await curl.GetJsonAsync<User>("https://api.example.com/users/123");
```

## Best Practices

1. **Always validate JSON** before parsing
2. **Use strongly-typed classes** when structure is known
3. **Handle parsing exceptions** gracefully
4. **Use appropriate naming policies** (camelCase vs PascalCase)
5. **Consider memory usage** with large JSON documents
6. **Log raw JSON** when debugging parsing issues
7. **Use JsonDocument** for unknown structures
8. **Dispose JsonDocument** after use

## Summary

Working with JSON in CurlDotNet is straightforward:
- Receive JSON as strings in `result.Data`
- Parse with `JsonSerializer.Deserialize<T>()`
- Send JSON with `PostJsonAsync()` or serialize manually
- Handle errors and validate structure
- Use appropriate options for your API's conventions

## What's Next?

Learn about [HTTP headers](08-headers-explained.html) and how they control request and response behavior.

---

[← Previous: Handling Errors](06-handling-errors.html) | [Next: Headers Explained →](08-headers-explained.html)