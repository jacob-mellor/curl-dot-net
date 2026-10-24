---
title: "What is curl? How curl Commands Work in C# with CurlDotNet"
description: "What curl is, how curl commands work, and how to run curl commands in C# with CurlDotNet without translating them to HttpClient."
---

# Tutorial 2: What is curl?

What is curl and how do curl commands work in C#? curl is the command-line tool that almost every API's documentation uses for its examples, and CurlDotNet lets you run those same curl commands from C# with `Curl.ExecuteAsync`. This tutorial explains the tool, its most common options, and how it maps to CurlDotNet.

## What You'll Learn

- What curl is and why it is everywhere
- How curl relates to everyday web browsing
- Why developers rely on curl
- How CurlDotNet brings curl to C#

## Prerequisites

- [Tutorial 1: What is .NET and C#?](01-what-is-dotnet.html)

## The Big Picture

### What is curl?

**curl** is a command-line tool for transferring data to and from servers. Think of it as a web browser without the graphics: you type a URL (plus options) and it prints what the server sent back.

### Real-World Analogy

Imagine ordering a pizza:

- **Web browser** = walking into the pizza shop, looking at the menu, pointing at what you want
- **curl** = phoning the shop with exact instructions: "One large pepperoni, thin crust, deliver to 123 Main St"

curl is:

- Fast (no graphics to load)
- Precise (you say exactly what you want)
- Scriptable (you can automate it)
- Available on Linux, macOS, Windows 10 and later, and most containers

## Why curl is Everywhere

### 1. API Documentation Uses curl

When you read API documentation from GitHub, Stripe, or almost any web service, the examples are curl commands:

```bash
# GitHub API
curl https://api.github.com/users/octocat

# Stripe API
curl https://api.stripe.com/v1/charges \
  -u sk_test_KEY: \
  -d amount=2000
```

### 2. Developers Use It Every Day

- Testing APIs while building them
- Debugging network issues
- Quick checks of website status
- Automating tasks in scripts and CI pipelines

## Understanding curl Commands

A typical curl command:

```bash
curl -X POST https://httpbin.org/post \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer token123" \
  -d '{"name":"Alice","email":"alice@example.com"}'
```

### Breaking It Down

| Part | What It Does |
|------|--------------|
| `curl` | The command name |
| `-X POST` | Use the POST method (like submitting a form) |
| `https://httpbin.org/post` | Where to send the request |
| `-H "..."` | Add a header (extra information) |
| `-d '...'` | The data to send |

### Common curl Options

| Option | What It Does | Example |
|--------|--------------|---------|
| `-X` | HTTP method | `-X POST`, `-X PUT` |
| `-H` | Add header | `-H "Authorization: Bearer token"` |
| `-d` | Send data | `-d '{"key":"value"}'` |
| `-o` | Save output to file | `-o result.json` |
| `-u` | Username and password | `-u user:pass` |
| `-L` | Follow redirects | `-L` |
| `-v` | Verbose (show details) | `-v` |
| `-k` | Skip SSL verification | `-k` (testing only) |
| `-i` | Include response headers in output | `-i` |

All of these options work in CurlDotNet. See the [curl CLI compatibility reference](../reference/curl-cli-compatibility.html) for the full list.

## Common curl Examples

### Example 1: Get a Web Page

```bash
curl https://httpbin.org/html
```

Fetches the HTML, like viewing a page's source in your browser.

### Example 2: Get JSON Data

```bash
curl https://api.github.com/users/octocat
```

Gets user data in JSON format, which is what mobile apps do behind the scenes.

### Example 3: Send Data to an API

```bash
curl -X POST https://httpbin.org/post \
  -H "Content-Type: application/json" \
  -d '{"message":"Hello World"}'
```

### Example 4: Download a File

```bash
curl -o image.png https://httpbin.org/image/png
```

Like right-clicking and choosing "Save As" in a browser.

### Example 5: Authentication

```bash
curl -u username:password https://httpbin.org/basic-auth/username/password
```

Sends a username and password using HTTP Basic authentication.

## From curl to CurlDotNet

### The Problem

You find the right curl command in API documentation:

```bash
curl -X POST https://httpbin.org/post \
  -H "Authorization: Bearer token123" \
  -d '{"name":"Alice"}'
```

But you are writing a C# application. Traditionally you would have to understand what the command does, translate it to `HttpClient` calls, wire up headers and data separately, and then test that your translation behaves the same way.

### The Solution: CurlDotNet

With CurlDotNet you paste the curl command into C#:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"
    curl -X POST https://httpbin.org/post \
      -H 'Authorization: Bearer token123' \
      -d '{""name"":""Alice""}'
");

Console.WriteLine(result.StatusCode);
```

No translation step. The leading `curl` word is optional and backslash line continuations are fine.

## How CurlDotNet Works

### Not a Wrapper

CurlDotNet does not run `curl.exe` behind the scenes and does not need curl installed.

### Pure .NET Implementation

CurlDotNet is written in C#. It:

1. Parses the curl command syntax
2. Maps curl options to HTTP settings
3. Sends the request with .NET's own HTTP stack
4. Returns a `CurlResult` with the status code, headers, and body

### What You Get

- `async`/`await` support
- Strong typing and IntelliSense on the result
- Typed exceptions for network failures (see the [exception reference](../exceptions/index.html))
- No external dependencies
- Cross-platform: Windows, macOS, and Linux

## curl vs CurlDotNet Comparison

| Feature | curl (command line) | CurlDotNet |
|---------|---------------------|------------|
| Parse responses | Manual | `result.ParseJson<T>()` |
| Error handling | Exit codes | C# exceptions and `result.StatusCode` |
| Type safety | None | C# types |
| IntelliSense | None | Yes |
| Integration | Shell scripts | Any .NET application |
| Async support | No | `async`/`await` |

## Real-World Scenarios

### Scenario 1: Testing an API

With curl:

```bash
curl -X POST https://httpbin.org/post \
  -H "Content-Type: application/json" \
  -d '{"test":true}'
```

With CurlDotNet in a C# test:

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://httpbin.org/post \
      -H 'Content-Type: application/json' \
      -d '{""test"":true}'
");

Assert.Equal(200, result.StatusCode);
dynamic echo = result.AsJsonDynamic();
Assert.Equal(true, (bool)echo.json.test);
```

### Scenario 2: Building an Integration

Found in API docs (Stripe example):

```bash
curl https://api.stripe.com/v1/charges \
  -u sk_test_KEY: \
  -d amount=2000 \
  -d currency=usd
```

Used in your C# app with values filled in:

```csharp
var charge = await Curl.ExecuteAsync($@"
    curl https://api.stripe.com/v1/charges \
      -u {apiKey}: \
      -d amount={amount} \
      -d currency={currency}
");

if (charge.IsSuccess)
{
    dynamic data = charge.AsJsonDynamic();
    Console.WriteLine($"Charge id: {data.id}");
}
```

### Scenario 3: Migrating Bash Scripts

Old bash script:

```bash
#!/bin/bash
for user in $(cat users.txt); do
  curl -X POST https://httpbin.org/post -d "user=$user"
done
```

New C# version:

```csharp
var users = await File.ReadAllLinesAsync("users.txt");

foreach (var user in users)
{
    await Curl.ExecuteAsync($"curl -X POST https://httpbin.org/post -d 'user={user}'");
}
```

## Understanding curl Options in Depth

### HTTP Methods

```bash
curl -X GET https://httpbin.org/get           # Read data
curl -X POST https://httpbin.org/post         # Create new
curl -X PUT https://httpbin.org/put           # Update entire
curl -X PATCH https://httpbin.org/patch       # Update partial
curl -X DELETE https://httpbin.org/delete     # Delete
```

### Headers

Headers are like the envelope of a letter: metadata about the request.

```bash
# Tell the server what format you want back
curl -H "Accept: application/json" https://httpbin.org/get

# Tell the server what format you are sending
curl -H "Content-Type: application/json" -d '{"key":"value"}' https://httpbin.org/post

# Authentication
curl -H "Authorization: Bearer token123" https://httpbin.org/bearer

# Custom headers
curl -H "X-API-Key: key123" -H "X-Request-ID: unique-id" https://httpbin.org/headers
```

### Sending Data

```bash
# JSON data
curl -d '{"name":"Alice"}' -H "Content-Type: application/json" https://httpbin.org/post

# Form data
curl -d "name=Alice&age=30" https://httpbin.org/post

# File upload (multipart form)
curl -F "file=@document.pdf" https://httpbin.org/post

# Request body read from a file
curl -d @data.json https://httpbin.org/post
```

## Try It Yourself

### Exercise 1: Understand This curl Command

Identify each part:

```bash
curl -X POST https://httpbin.org/post \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer abc123" \
  -d '{"name":"John","email":"john@example.com"}'
```

<details>
<summary>Click for Answer</summary>

- `-X POST` = use the POST method (creating something new)
- URL = send to https://httpbin.org/post
- First `-H` = tell the server we are sending JSON
- Second `-H` = provide an authentication token
- `-d` = the user data (name and email)

</details>

### Exercise 2: Translate to CurlDotNet

Convert the command above to CurlDotNet C# code.

<details>
<summary>Click for Answer</summary>

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://httpbin.org/post \
      -H 'Content-Type: application/json' \
      -H 'Authorization: Bearer abc123' \
      -d '{""name"":""John"",""email"":""john@example.com""}'
");
```

Inside a C# verbatim string (`@"..."`), write each double quote as `""`.

</details>

### Exercise 3: Real API Test

httpbin.org is a free service that echoes back whatever you send it:

```csharp
using System;
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"
    curl -X POST https://httpbin.org/post \
      -H 'Content-Type: application/json' \
      -d '{""message"":""Hello from CurlDotNet!""}'
");

Console.WriteLine($"Status: {result.StatusCode}");
Console.WriteLine($"Response: {result.Body}");
```

## Common Mistakes

### Mistake 1: Forgetting to Escape Quotes

```csharp
// Wrong - the inner double quotes end the C# string early
var result = await Curl.ExecuteAsync("curl -d '{"name":"Alice"}' https://httpbin.org/post");

// Right - verbatim string with doubled quotes
var result = await Curl.ExecuteAsync(@"curl -d '{""name"":""Alice""}' https://httpbin.org/post");
```

### Mistake 2: Launching the curl Binary Instead of CurlDotNet

```csharp
// Wrong - starts an external curl process (if one is installed)
Process.Start("curl", "https://httpbin.org/get");

// Right - runs inside your .NET process
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
```

### Mistake 3: Forgetting async/await

```csharp
// Wrong - missing await
var result = Curl.ExecuteAsync("curl https://httpbin.org/get");

// Right - with await
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
```

## curl Resources

Want to learn more about curl itself?

- [Official curl website](https://curl.se/)
- [curl documentation](https://curl.se/docs/)
- [curl tutorial](https://curl.se/docs/tutorial.html)
- [curl man page](https://curl.se/docs/manpage.html)

For CurlDotNet-specific help see the [troubleshooting guide](../troubleshooting/common-issues.html).

## Key Takeaways

- curl is a command-line tool for transferring data
- Most API documentation shows curl examples
- curl is fast, precise, reproducible, and scriptable
- CurlDotNet lets you run curl commands directly in C#
- No translation needed: paste the command and run it

## Questions You Might Have

**Q: Do I need to install curl to use CurlDotNet?**
A: No. CurlDotNet is a pure .NET implementation.

**Q: Can I use curl commands I find online directly?**
A: Yes, that is the point of the library. See the [compatibility reference](../reference/curl-cli-compatibility.html) for the supported options.

**Q: What if I don't know curl syntax?**
A: CurlDotNet also has a fluent builder (`CurlRequestBuilder`) for building requests in code. Knowing curl is still useful because most API docs use it.

**Q: Why not just use HttpClient?**
A: You can. CurlDotNet is handy when you already have curl commands from API documentation. See [migrating from HttpClient](../migration/httpclient.html).

## Summary

curl is the common language developers use to talk to web services. CurlDotNet brings that language to C#: you paste a curl command into `Curl.ExecuteAsync` and get a typed `CurlResult` back, which removes the friction between reading API documentation and implementing it.

**Next tutorial:** [Understanding Async/Await](03-what-is-async.html)

**Need help?** Check the [troubleshooting guide](../troubleshooting/common-issues.html) or ask in [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions).
