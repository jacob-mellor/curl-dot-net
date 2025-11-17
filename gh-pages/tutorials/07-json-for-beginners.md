# Tutorial 07: JSON for Beginners

## 🎯 What You'll Learn

- What JSON is and why APIs use it
- How to read and understand JSON
- How to parse JSON in C#
- How to create JSON for API requests
- Working with nested JSON structures
- Common JSON patterns in APIs

## 📚 Prerequisites

- [Tutorial 04: Your First Request](04-your-first-request.md)
- [Tutorial 05: Understanding Results](05-understanding-results.md)
- Basic C# knowledge (classes and properties)

## 🤔 What is JSON?

**JSON** stands for **J**ava**S**cript **O**bject **N**otation. It's a way to represent data as text that both humans and computers can read.

Think of JSON like a recipe card:
- It has a standard format everyone understands
- It's easy to read
- It can be shared between different programs
- Most web APIs use it

### Why Do APIs Use JSON?

- **Human-readable** - You can read it without special tools
- **Language-independent** - Works with any programming language
- **Lightweight** - Smaller than XML
- **Standard** - Everyone uses the same format

## 📖 Reading JSON

### Basic JSON Syntax

JSON has only a few simple rules:

#### 1. Objects (Like a Dictionary)

Objects use curly braces `{}` and contain key-value pairs:

```json
{
  "name": "Alice",
  "age": 30,
  "city": "New York"
}
```

Think of this like a form:
- **Name:** Alice
- **Age:** 30
- **City:** New York

#### 2. Arrays (Like a List)

Arrays use square brackets `[]` and contain multiple items:

```json
[
  "apple",
  "banana",
  "orange"
]
```

This is just a list of fruits.

#### 3. Values

JSON values can be:
- **Strings**: `"Hello"` (in quotes)
- **Numbers**: `42` or `3.14` (no quotes)
- **Booleans**: `true` or `false`
- **Null**: `null` (means "no value")
- **Objects**: `{ "key": "value" }`
- **Arrays**: `[1, 2, 3]`

### Real API Example

Here's what GitHub's API returns for a user:

```json
{
  "login": "octocat",
  "id": 583231,
  "name": "The Octocat",
  "company": "GitHub",
  "location": "San Francisco",
  "email": null,
  "bio": "There once was...",
  "public_repos": 8,
  "followers": 9001,
  "following": 9,
  "created_at": "2011-01-25T18:44:36Z"
}
```

Let's break it down:
- `login` is a **string** (text)
- `id` is a **number**
- `name` is a **string**
- `email` is **null** (no email provided)
- `public_repos` is a **number**

## 💻 Parsing JSON in C#

### Method 1: Dynamic Parsing (Quick and Easy)

Best for: Exploring APIs, prototypes, small projects

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

        // Parse as dynamic - no class needed!
        dynamic user = result.ParseJson();

        Console.WriteLine($"Name: {user.name}");
        Console.WriteLine($"Company: {user.company}");
        Console.WriteLine($"Followers: {user.followers}");
        Console.WriteLine($"Repos: {user.public_repos}");
    }
}
```

**Output:**
```
Name: The Octocat
Company: GitHub
Followers: 9001
Repos: 8
```

**Pros:**
- Quick to write
- No class definitions needed
- Great for exploration

**Cons:**
- No IntelliSense
- Typos only found at runtime
- Harder to maintain

### Method 2: Strongly Typed (Professional)

Best for: Production code, large projects, team development

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class GitHubUser
{
    public string Login { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Company { get; set; }
    public string Location { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }
    public int Public_Repos { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public DateTime Created_At { get; set; }
}

class Program
{
    static async Task Main()
    {
        var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

        // Parse to your class
        GitHubUser user = result.ParseJson<GitHubUser>();

        Console.WriteLine($"Name: {user.Name}");
        Console.WriteLine($"Company: {user.Company}");
        Console.WriteLine($"Followers: {user.Followers}");
        Console.WriteLine($"Repos: {user.Public_Repos}");
        Console.WriteLine($"Joined: {user.Created_At:yyyy-MM-dd}");
    }
}
```

**Pros:**
- IntelliSense support
- Compile-time type checking
- Easier to maintain
- Self-documenting

**Cons:**
- Must define classes first
- More initial setup

## 🏗️ Creating Classes from JSON

### The Easy Way: Use QuickType.io

1. Copy the JSON response
2. Go to [quicktype.io](https://quicktype.io/)
3. Paste your JSON
4. Select "C#"
5. Copy the generated classes!

### Example: Weather API

**JSON Response:**
```json
{
  "location": "London",
  "temperature": 15,
  "condition": "Cloudy",
  "humidity": 65,
  "wind_speed": 10
}
```

**Generated C# Class:**
```csharp
public class Weather
{
    public string Location { get; set; }
    public int Temperature { get; set; }
    public string Condition { get; set; }
    public int Humidity { get; set; }
    public int Wind_Speed { get; set; }
}
```

**Usage:**
```csharp
var result = await Curl.ExecuteAsync("curl https://api.weather.com/current");
Weather weather = result.ParseJson<Weather>();

Console.WriteLine($"It's {weather.Temperature}°C and {weather.Condition} in {weather.Location}");
```

## 🎨 Working with Nested JSON

### Simple Nesting

Many APIs return nested objects:

```json
{
  "user": {
    "name": "Alice",
    "email": "alice@example.com",
    "address": {
      "street": "123 Main St",
      "city": "Boston",
      "country": "USA"
    }
  }
}
```

**C# Classes:**
```csharp
public class UserResponse
{
    public User User { get; set; }
}

public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}
```

**Usage:**
```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/user");
UserResponse response = result.ParseJson<UserResponse>();

Console.WriteLine($"Name: {response.User.Name}");
Console.WriteLine($"Email: {response.User.Email}");
Console.WriteLine($"City: {response.User.Address.City}");
```

### Arrays of Objects

APIs often return lists:

```json
{
  "users": [
    {
      "name": "Alice",
      "age": 30
    },
    {
      "name": "Bob",
      "age": 25
    },
    {
      "name": "Charlie",
      "age": 35
    }
  ]
}
```

**C# Classes:**
```csharp
public class UsersResponse
{
    public List<User> Users { get; set; }
}

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
}
```

**Usage:**
```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/users");
UsersResponse response = result.ParseJson<UsersResponse>();

foreach (var user in response.Users)
{
    Console.WriteLine($"{user.Name} is {user.Age} years old");
}
```

### Root-Level Arrays

Sometimes the entire response is an array:

```json
[
  {
    "id": 1,
    "title": "Buy milk"
  },
  {
    "id": 2,
    "title": "Walk dog"
  }
]
```

**C# Class:**
```csharp
public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; }
}
```

**Usage:**
```csharp
var result = await Curl.ExecuteAsync("curl https://jsonplaceholder.typicode.com/todos");
List<Todo> todos = result.ParseJson<List<Todo>>();

foreach (var todo in todos.Take(5))  // Show first 5
{
    Console.WriteLine($"[{todo.Id}] {todo.Title}");
}
```

## 📤 Creating JSON for Requests

### Sending JSON in POST Requests

Many APIs require you to send JSON data:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class CreateUserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

class Program
{
    static async Task Main()
    {
        // Create the object
        var newUser = new CreateUserRequest
        {
            Name = "Alice",
            Email = "alice@example.com",
            Age = 30
        };

        // Convert to JSON and send
        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://api.example.com/users \
              -H 'Content-Type: application/json' \
              -d '{""name"":""Alice"",""email"":""alice@example.com"",""age"":30}'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("User created!");
        }
    }
}
```

### Using System.Text.Json

For complex objects, use the JSON serializer:

```csharp
using System;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        var newUser = new
        {
            name = "Alice",
            email = "alice@example.com",
            age = 30,
            address = new
            {
                street = "123 Main St",
                city = "Boston"
            }
        };

        // Serialize to JSON
        string json = JsonSerializer.Serialize(newUser);
        Console.WriteLine($"Sending: {json}");

        // Send it
        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://api.example.com/users \
              -H 'Content-Type: application/json' \
              -d '{json}'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("User created!");
            var response = result.ParseJson<dynamic>();
            Console.WriteLine($"New user ID: {response.id}");
        }
    }
}
```

## 🎯 Common JSON Patterns

### Pattern 1: Wrapped Response

```json
{
  "success": true,
  "data": {
    "id": 123,
    "name": "Alice"
  },
  "message": "User retrieved successfully"
}
```

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Usage
var result = await Curl.ExecuteAsync("curl https://api.example.com/user/123");
var response = result.ParseJson<ApiResponse<User>>();

if (response.Success)
{
    Console.WriteLine($"User: {response.Data.Name}");
}
```

### Pattern 2: Paginated Response

```json
{
  "items": [
    {"id": 1, "name": "Item 1"},
    {"id": 2, "name": "Item 2"}
  ],
  "page": 1,
  "total_pages": 10,
  "total_items": 100
}
```

```csharp
public class PaginatedResponse<T>
{
    public List<T> Items { get; set; }
    public int Page { get; set; }
    public int Total_Pages { get; set; }
    public int Total_Items { get; set; }
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Usage
var result = await Curl.ExecuteAsync("curl https://api.example.com/items?page=1");
var response = result.ParseJson<PaginatedResponse<Item>>();

Console.WriteLine($"Page {response.Page} of {response.Total_Pages}");
Console.WriteLine($"Total items: {response.Total_Items}");

foreach (var item in response.Items)
{
    Console.WriteLine($"- {item.Name}");
}
```

### Pattern 3: Error Response

```json
{
  "error": {
    "code": "INVALID_INPUT",
    "message": "Email is required",
    "field": "email"
  }
}
```

```csharp
public class ErrorResponse
{
    public Error Error { get; set; }
}

public class Error
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string Field { get; set; }
}

// Usage
var result = await Curl.ExecuteAsync("curl https://api.example.com/user");

if (!result.IsSuccess)
{
    try
    {
        var error = result.ParseJson<ErrorResponse>();
        Console.WriteLine($"Error: {error.Error.Message}");
        Console.WriteLine($"Field: {error.Error.Field}");
    }
    catch
    {
        Console.WriteLine("Unknown error format");
    }
}
```

## 🔧 JSON Property Naming

### Handling Different Naming Conventions

APIs often use different naming styles:

**API Returns:**
```json
{
  "user_name": "alice",
  "first_name": "Alice",
  "last_name": "Smith"
}
```

**Option 1: Match the API (Ugly in C#)**
```csharp
public class User
{
    public string user_name { get; set; }  // Not C# style
    public string first_name { get; set; }
    public string last_name { get; set; }
}
```

**Option 2: Use JsonPropertyName Attribute (Best)**
```csharp
using System.Text.Json.Serialization;

public class User
{
    [JsonPropertyName("user_name")]
    public string UserName { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
}
```

Now you can use C# naming conventions in your code!

## 🎮 Complete Example: GitHub Repository Browser

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CurlDotNet;

public class Repository
{
    public string Name { get; set; }
    public string Description { get; set; }

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }

    [JsonPropertyName("stargazers_count")]
    public int Stars { get; set; }

    [JsonPropertyName("forks_count")]
    public int Forks { get; set; }

    public string Language { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}

class Program
{
    static async Task Main()
    {
        Console.WriteLine("GitHub Repository Browser");
        Console.WriteLine("=========================\n");

        Console.Write("Enter GitHub username: ");
        string username = Console.ReadLine();

        Console.WriteLine($"\nFetching repositories for {username}...\n");

        try
        {
            var result = await Curl.ExecuteAsync(
                $"curl https://api.github.com/users/{username}/repos?sort=updated&per_page=5"
            );

            if (!result.IsSuccess)
            {
                Console.WriteLine($"Error: Could not find user '{username}'");
                return;
            }

            var repos = result.ParseJson<List<Repository>>();

            if (repos.Count == 0)
            {
                Console.WriteLine("No repositories found.");
                return;
            }

            Console.WriteLine($"Top {repos.Count} repositories:\n");

            foreach (var repo in repos)
            {
                Console.WriteLine($"📦 {repo.Name}");

                if (!string.IsNullOrEmpty(repo.Description))
                    Console.WriteLine($"   {repo.Description}");

                Console.WriteLine($"   ⭐ {repo.Stars} stars | 🔱 {repo.Forks} forks | 💻 {repo.Language ?? "Unknown"}");
                Console.WriteLine($"   🔗 {repo.HtmlUrl}");
                Console.WriteLine($"   📅 Created: {repo.CreatedAt:yyyy-MM-dd}");
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

**Example Output:**
```
GitHub Repository Browser
=========================

Enter GitHub username: octocat

Fetching repositories for octocat...

Top 5 repositories:

📦 Hello-World
   My first repository on GitHub!
   ⭐ 2341 stars | 🔱 1234 forks | 💻 Unknown
   🔗 https://github.com/octocat/Hello-World
   📅 Created: 2011-01-26

📦 Spoon-Knife
   This repo is for demonstration purposes only.
   ⭐ 12345 stars | 🔱 140000 forks | 💻 HTML
   🔗 https://github.com/octocat/Spoon-Knife
   📅 Created: 2011-10-21
```

## 🎓 Key Takeaways

1. **JSON is simple** - Just objects `{}`, arrays `[]`, and values
2. **Use ParseJson()** to convert JSON strings to C# objects
3. **Create classes** that match your JSON structure
4. **Use QuickType.io** to generate classes automatically
5. **Handle nested objects** with nested classes
6. **Arrays become Lists** in C#
7. **JsonPropertyName** lets you use C# naming conventions
8. **Always handle errors** when parsing JSON

## 🚀 Next Steps

Now that you understand JSON:

1. **Next Tutorial** → [Headers Explained](08-headers-explained.md)
2. **Practice** - Parse responses from different APIs
3. **Experiment** - Try creating complex nested structures
4. **Explore** - Visit [JSONPlaceholder](https://jsonplaceholder.typicode.com/) for practice APIs

## 📚 Summary

JSON is the language of web APIs. You've learned how to:
- Read and understand JSON structure
- Parse JSON responses into C# objects
- Create classes that match JSON
- Work with nested objects and arrays
- Send JSON in POST requests
- Handle common JSON patterns

With these skills, you can work with any JSON-based API!

---

**Ready to learn about HTTP headers?** → [Headers Explained](08-headers-explained.md)

**Need help?** Check the [Troubleshooting Guide](../troubleshooting/README.md) or ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
