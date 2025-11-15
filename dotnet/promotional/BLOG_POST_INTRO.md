# CurlDotNet: The .NET Library That Understands curl

**By Jacob Mellor | Sponsored by IronSoftware**

---

## Copy, Paste, Execute

That curl command from Stripe's API docs? Paste it into C#. That Stack Overflow answer with a curl example? Paste it into C#. That bash script your team uses? The curl commands work in .NET now.

CurlDotNet is a pure .NET library that understands curl syntax. No translation. No `HttpClient` setup. Just paste and go.

```csharp
// This curl command from Stripe's docs...
var result = await Curl.ExecuteAsync(@"
  curl https://api.stripe.com/v1/charges \
    -u sk_test_4eC39HqLyjWDarjtT1zdp7dc: \
    -d amount=2000 \
    -d currency=usd \
    -d source=tok_mastercard
");

// ...works exactly as written in C#.
if (result.IsSuccess)
{
    var charge = result.ParseJson<StripeCharge>();
    Console.WriteLine($"Payment successful! ID: {charge.Id}");
}
```

## The Problem We All Face

Every API documentation shows curl commands. GitHub, Stripe, Twilio, AWS, Azure—they all use curl examples. But when you want to use those same requests in your .NET application, you're forced to manually translate them into `HttpClient` code.

This translation step is where bugs are born. You might forget a header, mis-handle authentication, or miss a parameter. The translation itself is tedious and slows down development.

**What if you didn't have to translate?**

## Enter CurlDotNet

CurlDotNet eliminates the translation step entirely. It's a pure .NET library that parses and executes curl commands natively. No shell execution. No external dependencies. Just paste curl commands and they work.

### Two Ways to Use It

**1. Paste curl Commands** (The Killer Feature)

When you have curl commands from documentation:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -X POST https://api.example.com/data -H 'Content-Type: application/json' -d '{\"key\":\"value\"}'"
);
```

**2. Fluent Builder API**

When you're building requests programmatically:

```csharp
var result = await CurlRequestBuilder
    .Post("https://api.example.com/data")
    .WithHeader("Content-Type", "application/json")
    .WithJson(new { key = "value" })
    .WithTimeout(TimeSpan.FromSeconds(30))
    .FollowRedirects()
    .ExecuteAsync();
```

Both approaches work great. Use whichever fits your workflow.

## Why This Matters

### 1. Eliminates Translation Friction

The manual translation from curl to `HttpClient` is error-prone. CurlDotNet eliminates this entire class of errors by making translation unnecessary.

### 2. Works with Any API Documentation

Every API provider shows curl examples. With CurlDotNet, you can use those exact examples in your .NET code without modification.

### 3. Perfect for Rapid Prototyping

When exploring an API, being able to paste curl commands directly into your code dramatically speeds up the process.

### 4. Enables Easy Migration

Many teams have bash scripts that use curl. With CurlDotNet, you can migrate those scripts to .NET while keeping the same curl commands.

## Real-World Example: GitHub API

Here's a complete example using GitHub's API:

```csharp
using CurlDotNet;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        
        // Get user info (paste from GitHub docs)
        var userResult = await Curl.ExecuteAsync($@"
          curl https://api.github.com/user \
            -H 'Accept: application/vnd.github.v3+json' \
            -H 'Authorization: token {token}'
        ");
        
        var user = userResult.ParseJson<GitHubUser>();
        Console.WriteLine($"Logged in as: {user.Login}");
        
        // List repositories
        var reposResult = await Curl.ExecuteAsync($@"
          curl https://api.github.com/user/repos \
            -H 'Accept: application/vnd.github.v3+json' \
            -H 'Authorization: token {token}'
        ");
        
        var repos = reposResult.ParseJson<Repository[]>();
        Console.WriteLine($"You have {repos.Length} repositories");
    }
}
```

Notice how the curl commands from GitHub's documentation work directly in C#. No translation needed.

## Key Features

- **Complete curl Compatibility**: Supports all 300+ curl options
- **Excellent Developer Experience**: Rich response object with helper methods
- **Comprehensive Error Handling**: Every curl error code has its own exception type
- **Memory Efficient**: Stream-based processing for large files
- **Cross-Shell Compatible**: Handles curl commands from Windows CMD, PowerShell, Bash, and ZSH

## Getting Started

Install from NuGet:

```bash
dotnet add package CurlDotNet
```

Then start using curl commands:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl https://api.github.com/users/octocat"
);

Console.WriteLine(result.Body);
```

That's it. You're done.

## The Vision

CurlDotNet represents a shift in how we think about HTTP client libraries. Instead of forcing developers to learn a new API, why not let them use the language they already know—curl?

This isn't just about convenience. It's about reducing cognitive load, eliminating translation errors, and making API integration accessible to developers who already understand curl.

## Conclusion

If you've ever struggled with translating curl commands to `HttpClient` code, CurlDotNet is for you. It eliminates that friction entirely, letting you paste curl commands directly into your .NET code and have them work.

Give it a try. I think you'll find it as liberating as I did.

---

**CurlDotNet** is open source and available on [GitHub](https://github.com/jacob/curl-dot-net).  
**NuGet**: [CurlDotNet](https://www.nuget.org/packages/CurlDotNet)  
**Sponsored by** [IronSoftware](https://ironsoftware.com) - Creators of IronPDF, IronOCR, IronXL, and IronBarcode

