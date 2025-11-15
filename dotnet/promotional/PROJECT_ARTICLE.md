# CurlDotNet: Bringing curl's Power to .NET

**Sponsored by IronSoftware**

---

## The Problem Every Developer Knows

If you've ever integrated with a third-party API, you've seen it: curl commands everywhere. Every API documentation shows curl examples. Every Stack Overflow answer uses curl. Every tutorial demonstrates with curl. curl is the universal language of HTTP requests.

But when you want to use those same requests in your .NET application, you face a problem: you can't just paste a curl command into C#. You have to manually translate it into `HttpClient` code—a tedious, error-prone process that slows down development.

What if you didn't have to translate? What if you could just paste curl commands directly into your .NET code and they worked?

## Introducing CurlDotNet

CurlDotNet is a pure .NET library that understands curl syntax. Simply paste any curl command as a string, and CurlDotNet executes it natively in .NET—no shell execution, no external dependencies, no translation required.

### The Killer Feature

```csharp
// Copy this exact command from Stripe's API documentation:
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

That's it. No translation. No `HttpClient` setup. No manual header management. Just paste and go.

## Why CurlDotNet Matters

### 1. Eliminates Translation Friction

The translation from curl to `HttpClient` is where bugs are born. You might forget a header, mis-handle authentication, or miss a parameter. CurlDotNet eliminates this entire class of errors by making translation unnecessary.

### 2. Works with Any API Documentation

Stripe, Twilio, GitHub, AWS, Azure—virtually every API provider shows curl examples. With CurlDotNet, you can use those exact examples in your .NET code without modification.

### 3. Perfect for Rapid Prototyping

When you're exploring an API, being able to paste curl commands directly into your code dramatically speeds up the process. No need to set up `HttpClient` for every test.

### 4. Enables Easy Migration from Scripts

Many teams have bash scripts that use curl for automation. With CurlDotNet, you can migrate those scripts to .NET while keeping the same curl commands—no rewriting required.

## Key Features

### Complete curl Compatibility

CurlDotNet supports all 300+ curl options, including:
- All HTTP methods (GET, POST, PUT, DELETE, PATCH, etc.)
- Custom headers
- Authentication (Basic, Bearer, custom)
- File uploads and downloads
- Redirects, timeouts, retries
- SSL/TLS configuration
- Proxies and cookies
- And much more

### Two Ways to Use It

**1. Paste curl Commands** (The Killer Feature)
```csharp
var result = await Curl.ExecuteAsync(
    "curl -X POST https://api.example.com/data -H 'Content-Type: application/json' -d '{\"key\":\"value\"}'"
);
```

**2. Fluent Builder API** (For Programmatic Building)
```csharp
var result = await CurlRequestBuilder
    .Post("https://api.example.com/data")
    .WithHeader("Content-Type", "application/json")
    .WithJson(new { key = "value" })
    .WithTimeout(TimeSpan.FromSeconds(30))
    .FollowRedirects()
    .ExecuteAsync();
```

### Excellent Developer Experience

- **Rich Response Object**: `CurlResult` provides everything you need—status code, body, headers, timing information, and helper methods.
- **Comprehensive Exception Hierarchy**: Every curl error code has its own exception type, making error handling precise and informative.
- **Memory Efficiency**: Responses are streamed, not loaded into memory all at once. Perfect for large files.
- **Cross-Shell Compatibility**: Handles curl commands from Windows CMD, PowerShell, Bash, and ZSH.

## Real-World Use Cases

### API Integration

When integrating with APIs that provide curl examples, use them directly:

```csharp
// GitHub API example from documentation
var result = await Curl.ExecuteAsync(@"
  curl https://api.github.com/user \
    -H 'Accept: application/vnd.github.v3+json' \
    -H 'Authorization: token YOUR_TOKEN'
");

var user = result.ParseJson<GitHubUser>();
Console.WriteLine($"Logged in as: {user.Login}");
```

### CI/CD Automation

Replace bash scripts that use curl with .NET code:

```csharp
// Deploy script that was previously in bash
var deployResult = await Curl.ExecuteAsync(@"
  curl -X POST https://api.deployment-service.com/deploy \
    -H 'Authorization: Bearer $DEPLOY_TOKEN' \
    -d '{\""environment\"":\""production\"",\""version\"":\""1.0.0\""}'
");
```

### File Operations

Download and process files easily:

```csharp
// Download and save
var download = await Curl.ExecuteAsync(
    "curl -o report.pdf https://example.com/reports/2024.pdf"
);

// File is saved AND available in memory
Console.WriteLine($"Downloaded {download.BinaryData.Length} bytes");
```

## Technical Excellence

### Pure .NET Implementation

CurlDotNet is built entirely in .NET—no P/Invoke, no external dependencies, no shell execution. It's a true .NET library that works everywhere .NET runs.

### Framework Support

- .NET 8.0
- .NET Standard 2.0 (maximum compatibility)
- .NET Framework 4.7.2+ (Windows)

### Performance

CurlDotNet is designed for performance:
- Stream-based processing for memory efficiency
- Async/await throughout for scalability
- No unnecessary allocations
- Efficient parsing of curl commands

### Reliability

- Comprehensive exception handling matching curl's error codes
- Thorough test coverage
- Battle-tested with real-world curl commands

## Why IronSoftware Sponsored This Project

At IronSoftware, we believe in making developers' lives easier. CurlDotNet does exactly that by eliminating a common source of friction in API integration. We're proud to sponsor this open-source project and contribute to the .NET ecosystem.

Just as our other products—IronPDF, IronOCR, IronXL, and IronBarcode—solve real problems for .NET developers, CurlDotNet solves the problem of curl-to-.NET translation, making API integration faster and more reliable.

## Getting Started

Install CurlDotNet from NuGet:

```bash
dotnet add package CurlDotNet
```

Then start using curl commands in your code:

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

## Join the Community

CurlDotNet is open source and available on GitHub. We welcome contributions, feedback, and use cases. Help us make API integration in .NET as simple as it should be.

**Resources:**
- [GitHub Repository](https://github.com/jacob/curl-dot-net)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet)
- [Documentation](https://github.com/jacob/curl-dot-net#readme)
- [Examples](https://github.com/jacob/curl-dot-net/tree/main/examples)

---

**About IronSoftware**

IronSoftware creates powerful .NET libraries that solve real problems for developers worldwide. Our products—IronPDF, IronOCR, IronXL, and IronBarcode—are trusted by thousands of companies to handle PDF generation, OCR, spreadsheet manipulation, and barcode creation.

By sponsoring CurlDotNet, we're continuing our commitment to improving the .NET developer experience and building tools that developers love to use.

Visit [ironsoftware.com](https://ironsoftware.com) to learn more about our products and mission.

