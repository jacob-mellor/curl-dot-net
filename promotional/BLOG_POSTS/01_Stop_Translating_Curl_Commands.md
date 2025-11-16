---
title: "Stop Translating curl Commands - Just Paste Them in .NET"
published: true
tags: ['dotnet', 'csharp', 'http', 'api', 'curl', 'rest', 'tutorial']
date: 2025-01-20
canonical_url: https://dev.to/jacob/stop-translating-curl-commands
---

# Stop Translating curl Commands - Just Paste Them in .NET

Every API documentation shows curl commands. Stack Overflow answers use curl. Tutorials demonstrate with curl. Yet when we want to use those same requests in our .NET applications, we're forced to manually translate them into `HttpClient` code. This translation step is tedious, error-prone, and completely unnecessary.

What if you could just paste those curl commands directly into your C# code? What if they just... worked?

## The Problem We All Face

Picture this scenario: you're integrating with Stripe's API. Their documentation shows a curl command like this:

```bash
curl https://api.stripe.com/v1/charges \
  -u sk_test_4eC39HqLyjWDarjtT1zdp7dc: \
  -d amount=2000 \
  -d currency=usd \
  -d source=tok_mastercard \
  -d description="My First Test Charge"
```

To use this in .NET, you'd typically translate it to something like:

```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
    "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("sk_test_...:")));

var content = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("amount", "2000"),
    new KeyValuePair<string, string>("currency", "usd"),
    new KeyValuePair<string, string>("source", "tok_mastercard"),
    new KeyValuePair<string, string>("description", "My First Test Charge")
});

var response = await client.PostAsync("https://api.stripe.com/v1/charges", content);
var responseBody = await response.Content.ReadAsStringAsync();
```

That's a lot of code. And it's easy to make mistakes during translation. You might forget to set the Content-Type header. You might mis-handle authentication. You might miss a parameter. The list goes on.

## The Solution: CurlDotNet

What if, instead of all that translation, you could just do this?

```csharp
var result = await Curl.ExecuteAsync(@"
  curl https://api.stripe.com/v1/charges \
    -u sk_test_4eC39HqLyjWDarjtT1zdp7dc: \
    -d amount=2000 \
    -d currency=usd \
    -d source=tok_mastercard \
    -d description='My First Test Charge'
");

if (result.IsSuccess)
{
    var charge = result.ParseJson<StripeCharge>();
    Console.WriteLine($"Payment successful! ID: {charge.Id}");
}
```

That's it. No translation. No `HttpClient` setup. No manual header management. Just paste the curl command and it works.

## How It Works

CurlDotNet is a pure .NET library that understands curl syntax. It parses curl command strings exactly as curl itself does, supporting all 300+ curl options. Whether you're dealing with:

- Basic GET requests
- Complex POST requests with multipart forms
- File uploads
- Authentication (basic, bearer, custom)
- Custom headers
- Redirects, timeouts, retries
- SSL/TLS configuration

All of it works exactly as it would in curl.

## Real-World Examples

Let me show you some practical examples of how this changes your workflow.

### Example 1: GitHub API

GitHub's API documentation provides curl examples. With CurlDotNet, you can use them directly:

```csharp
// Get user info
var result = await Curl.ExecuteAsync(
    "curl -H 'Accept: application/vnd.github.v3+json' https://api.github.com/users/octocat"
);

var user = result.ParseJson<GitHubUser>();
Console.WriteLine($"{user.Name} has {user.PublicRepos} public repositories");

// Create a repository
var createResult = await Curl.ExecuteAsync(@"
  curl -X POST https://api.github.com/user/repos \
    -H 'Accept: application/vnd.github.v3+json' \
    -H 'Authorization: token YOUR_TOKEN' \
    -d '{\""name\"":\""my-new-repo\"",\""private\"":true}'
");
```

Notice how the JSON escaping works naturally with C# verbatim strings. The backslash line continuations work too.

### Example 2: File Uploads

Uploading files becomes trivial:

```csharp
// Upload to a service
var uploadResult = await Curl.ExecuteAsync(@"
  curl -X POST https://api.example.com/upload \
    -F 'file=@/path/to/document.pdf' \
    -F 'description=Quarterly Report' \
    -H 'Authorization: Bearer YOUR_TOKEN'
");

Console.WriteLine($"Uploaded: {uploadResult.Headers["Location"]}");
```

### Example 3: Downloading Files

Downloading files with curl is straightforward. CurlDotNet makes it just as easy:

```csharp
// Download and save to file (memory efficient)
var download = await Curl.ExecuteAsync(
    "curl -o report.pdf https://example.com/reports/2024.pdf"
);

// Or work with it in memory
var download2 = await Curl.ExecuteAsync(
    "curl https://example.com/data.json"
);
var data = download2.ParseJson<DataModel>();

// Then save if needed
download2.SaveToFile("backup.json");
```

## Two Ways to Use It

CurlDotNet provides two APIs depending on your preference:

### 1. Paste curl Commands (The Killer Feature)

When you have curl commands from documentation or examples:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -X POST https://api.example.com/data -H 'Content-Type: application/json' -d '{\"key\":\"value\"}'"
);
```

### 2. Fluent Builder API

When you're building requests programmatically and want IntelliSense:

```csharp
var result = await CurlRequestBuilder
    .Post("https://api.example.com/data")
    .WithHeader("Content-Type", "application/json")
    .WithJson(new { key = "value" })
    .WithTimeout(TimeSpan.FromSeconds(30))
    .FollowRedirects()
    .ExecuteAsync();
```

Both approaches work great. Use whichever fits your workflow better.

## Working with Responses

The response object (`CurlResult`) makes working with HTTP responses delightful:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/users/123");

// Check success
if (result.IsSuccess)  // True for 200-299 status codes
{
    // Parse JSON
    var user = result.ParseJson<User>();
    
    // Access headers
    var rateLimit = result.Headers["X-RateLimit-Remaining"];
    
    // Get status code
    Console.WriteLine($"Status: {result.StatusCode}");
    
    // Save to file if needed
    result.SaveToFile("user-data.json");
}

// Or use fluent chaining
var user = result
    .EnsureSuccess()        // Throws if not 200-299
    .SaveToFile("backup.json")
    .ParseJson<User>();
```

## Memory vs Disk: The Choice Is Yours

CurlDotNet gives you flexibility in how you handle responses:

1. **Implicit from curl command**: If your curl command includes `-o filename`, it saves to disk automatically
2. **Explicit via API**: Use `result.SaveToFile("path")` when you want to save
3. **Always in memory**: The response is always available in `result.Body` or `result.BinaryData`, even if you also save to disk

This means you can do things like:

```csharp
// Download and immediately process
var result = await Curl.ExecuteAsync(
    "curl -o image.png https://example.com/image.png"
);

// It's saved to disk AND available in memory
Console.WriteLine($"Downloaded {result.BinaryData.Length} bytes");
var bitmap = new Bitmap(new MemoryStream(result.BinaryData));
```

## Error Handling

CurlDotNet provides a comprehensive exception hierarchy matching curl's error codes:

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://example.com/api");
}
catch (CurlDnsException ex)
{
    Console.WriteLine($"DNS lookup failed: {ex.Message}");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Request timed out: {ex.Message}");
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP error: {ex.StatusCode} - {ex.Message}");
}
catch (CurlException ex)
{
    Console.WriteLine($"Curl error: {ex.Message} (Code: {ex.ErrorCode})");
}
```

Every curl error code has its own exception type, making error handling precise and informative.

## Getting Started

Install CurlDotNet from NuGet:

```bash
dotnet add package CurlDotNet
```

Then start using curl commands in your C# code:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl https://api.github.com/users/octocat"
);

Console.WriteLine(result.Body);
```

That's it. You're done. No configuration. No setup. Just paste and go.

## When to Use CurlDotNet

CurlDotNet shines in several scenarios:

1. **API Integration**: When working with APIs that provide curl examples (Stripe, Twilio, GitHub, etc.)
2. **Prototyping**: Quickly test API endpoints by pasting curl commands
3. **Scripting**: Convert bash scripts that use curl to .NET applications
4. **Documentation**: Use actual curl commands from docs without translation
5. **Learning**: Experiment with HTTP requests using familiar curl syntax

## Limitations and Considerations

CurlDotNet is great, but it's not a replacement for `HttpClient` in every scenario. Use `HttpClient` when:

- You need advanced connection pooling control
- You're building a high-performance HTTP client library
- You need very specific .NET HTTP features

Use CurlDotNet when:

- You have curl commands you want to use directly
- You want simplicity and ease of use
- You're prototyping or scripting
- You're working with API documentation that provides curl examples

## Conclusion

The days of manually translating curl commands to `HttpClient` code are over. With CurlDotNet, you can paste curl commands directly into your C# code and they just work. This eliminates a source of errors, speeds up development, and makes working with APIs that provide curl examples a joy.

Give it a try. Paste a curl command into your next .NET project. I think you'll find it as liberating as I did.

---

**Resources:**
- [CurlDotNet on GitHub](https://github.com/jacob/curl-dot-net)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet)
- [Curl String Deep Dive Manual](../../manual/06-Curl-String-Deep-Dive.md)
- [Fluent Builder Cookbook](../../manual/07-Fluent-Builder-Cookbook.md)

**About the Author:** Jacob Mellor is a .NET developer passionate about making API integration simpler and more enjoyable. CurlDotNet is sponsored by IronSoftware.

