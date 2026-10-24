---
title: CurlDotNet Documentation Overview - curl in C# and .NET
description: Map of the CurlDotNet documentation: getting started, tutorials, API guide, cookbook, migration guides and troubleshooting for running curl in C#.
---

# CurlDotNet Documentation Overview: Every Section for curl in C#

This page is the map of the CurlDotNet documentation for running curl commands in C#: where to start, where the API is described, and where to look when something fails. The full site with search is at https://jacob-mellor.github.io/curl-dot-net/ and every page is also readable as Markdown on GitHub.

## Getting Started

- [Installation](../getting-started/installation.html) - Add the NuGet package and verify it
- [Quick Start](../getting-started/quickstart.html) - First request in a few minutes
- [First Request](../getting-started/first-request.html) - A complete request and response, explained
- [Configuration](../getting-started/configuration.html) - Default timeouts, redirects and TLS settings

## Core Concepts

- [What is .NET?](../tutorials/01-what-is-dotnet.html) - For developers new to .NET
- [What is curl?](../tutorials/02-what-is-curl.html) - curl basics
- [Async Programming](../tutorials/03-what-is-async.html) - async/await as used by CurlDotNet
- [Understanding Results](../tutorials/05-understanding-results.html) - `CurlResult`: status code, headers, body

## API Documentation

- [API Guide](../api-guide/README.html) - String API (`Curl.ExecuteAsync`), builder API (`CurlRequestBuilder`), instance API (`LibCurl`)
- [Generated API Reference](../api/index.html) - Types and members generated from the XML comments
- [libcurl Mapping](libcurl-mapping.html) - libcurl options and return codes mapped to C#
- [curl CLI Compatibility](curl-cli-compatibility.html) - Which curl flags are supported

## Practical Examples

- [Cookbook](../cookbook/) - Ready-to-use recipes
- [Simple GET Request](../cookbook/beginner/simple-get.html)
- [POST JSON Data](../cookbook/beginner/send-json.html)
- [Upload a File](../cookbook/beginner/upload-file.html)
- [Download a File](../cookbook/beginner/download-file.html)
- [Handle Errors](../cookbook/beginner/handle-errors.html)

## Authentication

- [Authentication Basics](../tutorials/09-authentication-basics.html) - Basic, bearer and digest auth
- [Call an API with Auth](../cookbook/beginner/call-api.html) - Bearer tokens and API keys

## Migration Guides

- [From HttpClient](../migration/httpclient.html)
- [From RestSharp](../migration/restsharp.html)

## Troubleshooting

- [Common Issues](../troubleshooting/common-issues.html)
- [Error Reference](../troubleshooting/error-reference.html) - Exception types and what causes them
- [FAQ](../troubleshooting/faq.html)
- [Exception Pages](../exceptions/) - One page per exception family

## Quick Code Examples

### Run a curl command string

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(result.StatusCode);
Console.WriteLine(result.Body);
```

### POST JSON

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post \
    -H 'Content-Type: application/json' \
    -d '{""name"":""Ada"",""email"":""ada@example.com""}'");
Console.WriteLine(result.IsSuccess);
```

### Builder API

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://api.github.com/user")
    .WithBearerToken("your-token")
    .ExecuteAsync();
Console.WriteLine(result.StatusCode);
```

## Quick Navigation

| I want to... | Go to... |
|-------------|----------|
| Install CurlDotNet | [Installation](../getting-started/installation.html) |
| Learn the basics | [Tutorials](../tutorials/) |
| See code examples | [Cookbook](../cookbook/) |
| Read API docs | [API Guide](../api-guide/README.html) |
| Solve a problem | [Common Issues](../troubleshooting/common-issues.html) |
| Migrate from HttpClient | [Migration Guide](../migration/httpclient.html) |
| Check runtime support | [Framework Compatibility](../framework-compatibility.html) |

## Contributing

Found an issue in the documentation or want to add an example?

- [Edit on GitHub](https://github.com/jacob-mellor/curl-dot-net/tree/master/docs)
- [Open an issue](https://github.com/jacob-mellor/curl-dot-net/issues)

## Links

- Documentation site: https://jacob-mellor.github.io/curl-dot-net/
- NuGet package: https://www.nuget.org/packages/CurlDotNet/
- GitHub repository: https://github.com/jacob-mellor/curl-dot-net
