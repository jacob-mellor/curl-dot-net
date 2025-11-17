# Recipe: Call an API (Complete Beginner Guide)

## 🎯 What You'll Build

A simple program that calls an API and displays the results. We'll use real APIs that don't require authentication to get you started quickly.

## 🥘 Ingredients

- CurlDotNet package
- No API key needed (we'll use free public APIs)
- 5 minutes

## 📖 The Recipe

### Step 1: Understanding APIs

An API is like a restaurant menu:
- You look at the menu (API documentation)
- You order something (make a request)
- You get your food (receive response)

### Step 2: Your First API Call

Let's get information about Bitcoin price:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        // Call the API
        var result = await Curl.ExecuteAsync(
            "curl https://api.coindesk.com/v1/bpi/currentprice.json"
        );

        // Check if it worked
        if (result.IsSuccess)
        {
            Console.WriteLine("Bitcoin Prices:");
            Console.WriteLine(result.Body);
        }
        else
        {
            Console.WriteLine($"Error: {result.StatusCode}");
        }
    }
}
```

### Step 3: Parse the JSON Response

APIs usually return JSON. Let's parse it:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

// Classes that match the API response
public class BitcoinPrice
{
    public Time Time { get; set; }
    public Bpi Bpi { get; set; }
}

public class Time
{
    public string Updated { get; set; }
}

public class Bpi
{
    public Currency USD { get; set; }
    public Currency EUR { get; set; }
}

public class Currency
{
    public string Code { get; set; }
    public string Rate { get; set; }
    public string Description { get; set; }
}

class Program
{
    static async Task Main()
    {
        var result = await Curl.ExecuteAsync(
            "curl https://api.coindesk.com/v1/bpi/currentprice.json"
        );

        if (result.IsSuccess)
        {
            // Parse JSON to our classes
            var prices = result.ParseJson<BitcoinPrice>();

            Console.WriteLine($"Bitcoin Price Update: {prices.Time.Updated}");
            Console.WriteLine($"USD: {prices.Bpi.USD.Rate}");
            Console.WriteLine($"EUR: {prices.Bpi.EUR.Rate}");
        }
    }
}
```

## 🍳 Complete Examples

### Example 1: Weather API

Get current weather for any city:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class WeatherApp
{
    static async Task Main()
    {
        Console.Write("Enter city name: ");
        var city = Console.ReadLine();

        // Call weather API
        var result = await Curl.ExecuteAsync($@"
            curl 'https://wttr.in/{city}?format=3'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine($"Weather: {result.Body}");
        }
        else
        {
            Console.WriteLine("Couldn't get weather data");
        }
    }
}
```

### Example 2: Random User Generator

Get random user data for testing:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class RandomUser
{
    public Result[] Results { get; set; }
}

public class Result
{
    public Name Name { get; set; }
    public string Email { get; set; }
    public Location Location { get; set; }
}

public class Name
{
    public string First { get; set; }
    public string Last { get; set; }
}

public class Location
{
    public string City { get; set; }
    public string Country { get; set; }
}

class Program
{
    static async Task Main()
    {
        // Get 5 random users
        var result = await Curl.ExecuteAsync(
            "curl https://randomuser.me/api/?results=5"
        );

        if (result.IsSuccess)
        {
            var data = result.ParseJson<RandomUser>();

            Console.WriteLine("Random Users:");
            foreach (var user in data.Results)
            {
                Console.WriteLine($"- {user.Name.First} {user.Name.Last}");
                Console.WriteLine($"  Email: {user.Email}");
                Console.WriteLine($"  Location: {user.Location.City}, {user.Location.Country}");
                Console.WriteLine();
            }
        }
    }
}
```

### Example 3: REST API with Multiple Endpoints

Working with a complete REST API:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
}

class BlogApiClient
{
    private const string BaseUrl = "https://jsonplaceholder.typicode.com";

    // GET all posts
    public static async Task<Post[]> GetAllPosts()
    {
        var result = await Curl.ExecuteAsync($"curl {BaseUrl}/posts");
        return result.IsSuccess
            ? result.ParseJson<Post[]>()
            : new Post[0];
    }

    // GET single post
    public static async Task<Post> GetPost(int id)
    {
        var result = await Curl.ExecuteAsync($"curl {BaseUrl}/posts/{id}");
        return result.IsSuccess
            ? result.ParseJson<Post>()
            : null;
    }

    // POST new post
    public static async Task<Post> CreatePost(string title, string body)
    {
        var result = await Curl.ExecuteAsync($@"
            curl -X POST {BaseUrl}/posts \
            -H 'Content-Type: application/json' \
            -d '{{
                ""title"": ""{title}"",
                ""body"": ""{body}"",
                ""userId"": 1
            }}'
        ");

        return result.IsSuccess
            ? result.ParseJson<Post>()
            : null;
    }

    // DELETE post
    public static async Task<bool> DeletePost(int id)
    {
        var result = await Curl.ExecuteAsync($"curl -X DELETE {BaseUrl}/posts/{id}");
        return result.IsSuccess;
    }

    static async Task Main()
    {
        // Get all posts
        Console.WriteLine("Getting all posts...");
        var posts = await GetAllPosts();
        Console.WriteLine($"Found {posts.Length} posts");

        // Get specific post
        Console.WriteLine("\nGetting post #1...");
        var post = await GetPost(1);
        if (post != null)
        {
            Console.WriteLine($"Title: {post.Title}");
        }

        // Create new post
        Console.WriteLine("\nCreating new post...");
        var newPost = await CreatePost("My Title", "My content here");
        if (newPost != null)
        {
            Console.WriteLine($"Created post with ID: {newPost.Id}");
        }

        // Delete post
        Console.WriteLine("\nDeleting post #1...");
        var deleted = await DeletePost(1);
        Console.WriteLine(deleted ? "Deleted!" : "Failed to delete");
    }
}
```

## 🎨 Variations

### With API Key

Many APIs require authentication:

```csharp
// API key in header
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/data \
    -H 'X-API-Key: your-api-key-here'
");

// API key in query string
var result = await Curl.ExecuteAsync(
    "curl 'https://api.example.com/data?api_key=your-api-key-here'"
);

// Bearer token
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/data \
    -H 'Authorization: Bearer your-token-here'
");
```

### With Request Body

Sending data to the API:

```csharp
// JSON body
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/users \
    -H 'Content-Type: application/json' \
    -d '{""name"": ""John"", ""age"": 30}'
");

// Form data
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/form \
    -d 'name=John&age=30'
");
```

### With Custom Headers

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/data \
    -H 'Accept: application/json' \
    -H 'Accept-Language: en-US' \
    -H 'User-Agent: MyApp/1.0'
");
```

## 🐛 Troubleshooting

### Problem: "401 Unauthorized"
**Solution**: You need to provide authentication (API key, token, etc.)

```csharp
// Add authentication
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com \
    -H 'Authorization: Bearer YOUR_TOKEN'
");
```

### Problem: "404 Not Found"
**Solution**: Check the URL - you might have a typo

```csharp
// Double-check the URL
var result = await Curl.ExecuteAsync("curl https://api.example.com/correct-endpoint");
```

### Problem: JSON Parsing Fails
**Solution**: Your classes don't match the response structure

```csharp
// First, check what the API actually returns
var result = await Curl.ExecuteAsync("curl https://api.example.com");
Console.WriteLine(result.Body);  // Look at the actual JSON

// Then create matching classes
public class CorrectStructure
{
    // Properties that match the JSON
}
```

### Problem: "429 Too Many Requests"
**Solution**: You're calling the API too fast

```csharp
// Add delay between requests
await Task.Delay(1000);  // Wait 1 second
```

## 📊 Common API Status Codes

| Code | Meaning | What to Do |
|------|---------|------------|
| 200 | Success | Use the response |
| 201 | Created | Item was created successfully |
| 400 | Bad Request | Check your request format |
| 401 | Unauthorized | Add authentication |
| 403 | Forbidden | Check permissions |
| 404 | Not Found | Check the URL |
| 429 | Too Many Requests | Slow down |
| 500 | Server Error | Try again later |

## 🎯 Practice APIs

Try these free APIs to practice:

```csharp
// Joke API
var joke = await Curl.ExecuteAsync(
    "curl https://official-joke-api.appspot.com/random_joke"
);

// Cat Facts
var catFact = await Curl.ExecuteAsync(
    "curl https://catfact.ninja/fact"
);

// Random Quote
var quote = await Curl.ExecuteAsync(
    "curl https://api.quotable.io/random"
);

// Country Information
var country = await Curl.ExecuteAsync(
    "curl https://restcountries.com/v3.1/name/canada"
);

// Space Station Location
var iss = await Curl.ExecuteAsync(
    "curl http://api.open-notify.org/iss-now.json"
);
```

## 🚀 Next Steps

Now that you can call APIs:

1. Try [Sending JSON Data](send-json.md)
2. Learn about [Error Handling](handle-errors.md)
3. Build reusable API clients with [CurlRequestBuilder](../../api-guide/README.md)

## 📚 Related Recipes

- [Simple GET Request](simple-get.md)
- [POST JSON Data](send-json.md)
- [Handle Errors](handle-errors.md)

## 🎓 Key Takeaways

- APIs return data in JSON format
- Always check `IsSuccess` before using the response
- Parse JSON to C# classes for easy access
- Different APIs need different authentication
- Status codes tell you what happened

---

**Need help?** Check [Troubleshooting](../../troubleshooting/common-issues.md) | **Have questions?** Ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)