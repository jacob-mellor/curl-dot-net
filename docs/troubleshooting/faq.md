---
title: "CurlDotNet FAQ - curl in C# questions answered"
description: "CurlDotNet FAQ: curl in C# questions answered - supported .NET versions, authentication, JSON, downloads, timeouts, retries, errors and code generation."
---

# CurlDotNet FAQ: curl in C# questions answered

CurlDotNet FAQ: curl in C# questions answered - what the library is, which .NET versions it supports, and how to do the common things (authentication, JSON, downloads, timeouts, retries, error handling) with a curl command string in .NET.

## General Questions

### What is CurlDotNet?

CurlDotNet is a pure C# implementation of the curl command line. You pass a curl command as a string and get back a `CurlResult` with the status code, headers and body. There is no need to translate curl commands from API documentation into `HttpClient` code.

### Why use CurlDotNet instead of HttpClient?

| Feature | CurlDotNet | HttpClient |
|---------|------------|------------|
| Paste curl commands from API docs | Yes | No |
| Learning curve if you know curl | Minimal | Moderate |
| Moving bash scripts to .NET | Paste the command | Manual translation |
| Code generation to other languages | `Curl.ToHttpClient`, `ToFetch`, `ToPythonRequests` | External tools |

CurlDotNet fits when you have curl commands from documentation or scripts, or want to prototype quickly. `HttpClient` is the better choice when you need fine-grained control over connections or are building a low-level library. Internally CurlDotNet uses `HttpClient`, so you can mix both.

### Does CurlDotNet use the native curl library?

No. CurlDotNet is pure C# with no P/Invoke and no native binaries, so it is a single NuGet package with no platform-specific installation.

### What platforms are supported?

The package targets .NET 10, .NET 9, .NET 8, .NET Framework 4.7.2 and 4.8, and .NET Standard 2.0. Any runtime that consumes .NET Standard 2.0 (for example .NET 6, Mono, Unity, Xamarin) can use the .NET Standard build. See [Framework Compatibility](../framework-compatibility.html).

## Technical Questions

### Do I need to include "curl" in the command string?

No. The `curl` prefix is optional:

```csharp
using CurlDotNet;

await Curl.ExecuteAsync("curl https://httpbin.org/get");
await Curl.ExecuteAsync("https://httpbin.org/get");
```

### How do I handle authentication?

```csharp
// Basic auth
await Curl.ExecuteAsync("curl -u username:password https://httpbin.org/basic-auth/username/password");

// Bearer token
await Curl.ExecuteAsync("curl -H 'Authorization: Bearer token123' https://httpbin.org/bearer");

// API key header
await Curl.ExecuteAsync("curl -H 'X-API-Key: mykey' https://httpbin.org/headers");
```

`--digest`, `--ntlm`, `--negotiate` and `--anyauth` are also accepted. See [Authentication Basics](../tutorials/09-authentication-basics.html).

### How do I send JSON data?

```csharp
// Convenience method
var result = await Curl.PostJsonAsync("https://httpbin.org/post", new { name = "John" });

// Equivalent curl command
var result2 = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post \
    -H 'Content-Type: application/json' \
    -d '{""name"": ""John""}'");
```

### How do I download files?

```csharp
// Convenience method
await Curl.DownloadAsync("https://httpbin.org/image/png", "output.png");

// Equivalent curl command
await Curl.ExecuteAsync("curl -o output.png https://httpbin.org/image/png");
```

Binary responses are available as `result.BinaryData`, and `result.SaveToFile(path)` writes any response to disk.

### Is CurlDotNet thread-safe?

The static `Curl` methods can be called from multiple threads at once; each call builds its own request. The `Curl.Default*` settings (`DefaultMaxTimeSeconds`, `DefaultConnectTimeoutSeconds`, `DefaultFollowRedirects`, `DefaultInsecure`) are process-wide, so set them once at startup rather than per request.

### How do I handle errors?

HTTP 4xx and 5xx responses do not throw; check `IsSuccess` and `StatusCode`. Transport failures (DNS, connection, TLS, timeout) throw a `CurlException` subclass.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/status/404");
    if (!result.IsSuccess)
        Console.WriteLine($"HTTP {result.StatusCode}: {result.Body}");
}
catch (CurlTimeoutException ex) { Console.WriteLine($"Timed out after {ex.Timeout}"); }
catch (CurlCouldntConnectException ex) { Console.WriteLine($"Cannot reach {ex.Host}:{ex.Port}"); }
```

If you prefer exceptions for HTTP errors, call `result.EnsureSuccess()`, which throws `CurlDotNet.Core.CurlHttpException`. See the [Error Reference](error-reference.html).

### How do I set a timeout?

```csharp
// Process-wide default for commands without -m (HTTP falls back to 30 s when this is 0)
Curl.DefaultMaxTimeSeconds = 30;

// Per request
await Curl.ExecuteAsync("curl --max-time 10 https://httpbin.org/get");

// Caller-side cancellation (throws CurlAbortedByCallbackException)
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
await Curl.ExecuteAsync("curl https://httpbin.org/get", cts.Token);
```

### Does curl's --retry flag work?

Not yet. Retry in your own code instead; a short loop around `ExecuteAsync` is enough for most cases:

```csharp
using CurlDotNet.Core;

async Task<CurlResult> ExecuteWithRetries(string command, int attempts = 3)
{
    for (int i = 1; ; i++)
    {
        var result = await Curl.ExecuteAsync(command);
        if (result.StatusCode < 500 || i == attempts) return result;
        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
    }
}
```

See [Timeout Errors](common-issues.html#timeout-errors) for retrying on exceptions as well.

### Do cookies work?

Cookie strings work: `curl -b 'session=abc; theme=dark' https://httpbin.org/cookies`. Cookie jar files (`-c`) and reading cookies from a file are not supported yet.

## Performance Questions

### Is CurlDotNet fast?

CurlDotNet is built for developer productivity. Under the hood it uses `HttpClient`, so request throughput is similar; the extra cost is parsing the command string, which is small. For very high-throughput paths, use `HttpClient` directly or generate the equivalent code with `Curl.ToHttpClient(command)`.

### Can I make parallel requests?

Yes, `ExecuteManyAsync` runs the commands concurrently:

```csharp
var results = await Curl.ExecuteManyAsync(
    "curl https://api.github.com/users/octocat",
    "curl https://api.github.com/users/torvalds",
    "curl https://api.github.com/users/dotnet"
);
Console.WriteLine(results.Length); // 3
```

See [Parallel Requests](../tutorials/13-parallel-requests.html).

### How do I handle large downloads?

Write directly to a file with `-o` or `Curl.DownloadAsync(url, path)` rather than reading `result.Body`. For a stream over a response you already have, use `result.ToStream()`.

## Compatibility Questions

### Does CurlDotNet support all curl options?

The common options work: `-X -H -d --data-binary --data-urlencode --json -F -u -A -e -b -L --max-redirs -k -x -m -o -O -I -i -v --compressed -r -w` and the auth switches. Some options are not implemented yet (for example `--retry`, `-c` cookie jars, `--cert`/`--cacert`; `--connect-timeout` is parsed but only enforced for FTP). See [curl CLI Compatibility](../reference/curl-cli-compatibility.html), and [open an issue](https://github.com/jacob-mellor/curl-dot-net/issues) if you need one that is missing.

### Can I use CurlDotNet with ASP.NET Core?

Yes. Set defaults at startup and call `Curl` from controllers or services:

```csharp
// Program.cs
Curl.DefaultMaxTimeSeconds = 30;
Curl.DefaultFollowRedirects = true;

// In a controller
[HttpGet]
public async Task<IActionResult> GetData()
{
    var result = await Curl.ExecuteAsync("curl https://api.github.com/zen");
    return result.IsSuccess ? Ok(result.Body) : StatusCode(result.StatusCode);
}
```

### Does it work with .NET Framework?

Yes. The package has `net472` and `net48` builds as well as `netstandard2.0`.

### Can I use CurlDotNet in a Blazor app?

Blazor Server works like any other ASP.NET Core app. In Blazor WebAssembly the browser's networking rules (CORS, no raw sockets) apply to every HTTP library, including this one.

## Troubleshooting Questions

### Why am I getting SSL errors?

TLS failures (self-signed, expired or mismatched certificates) surface as `CurlCouldntConnectException`. For development only, skip verification:

```csharp
await Curl.ExecuteAsync("curl -k https://localhost:5001/");
```

For production, install the server's CA certificate in the operating system trust store. `--cacert` is not supported yet. See [SSL Certificate Errors](common-issues.html#ssl-certificate-errors).

### Why does my curl command work in the terminal but not in CurlDotNet?

Common reasons:

1. **Shell expansion** - backticks and `$(...)` are shell features; compute the value in C# and interpolate it.
2. **File paths** - `@file` paths are relative to the process working directory, not your source file.
3. **Quoting** - inside a C# string, `"` must be escaped (`\"`, or `""` in a verbatim `@"..."` string).
4. **Unsupported option** - check [curl CLI Compatibility](../reference/curl-cli-compatibility.html).

### Why am I getting a 401 Unauthorized response?

The credentials are missing, expired, or sent in the wrong form. Check `result.GetHeader("WWW-Authenticate")` to see which scheme the server expects, then send it:

```csharp
var result = await Curl.ExecuteAsync("curl -H 'Authorization: Bearer YOUR_TOKEN' https://httpbin.org/bearer");
Console.WriteLine(result.StatusCode);
Console.WriteLine(result.GetHeader("WWW-Authenticate"));
```

### How do I debug requests?

Use `-v` for request and response headers, and validate the command before running it:

```csharp
var result = await Curl.ExecuteAsync("curl -v https://httpbin.org/get");

var validation = Curl.Validate("curl -X POST https://httpbin.org/post");
if (!validation.IsValid)
    Console.WriteLine($"Invalid: {validation.Error}");
```

See [Debugging Requests](../tutorials/14-debugging-requests.html).

## Migration Questions

### How do I migrate from HttpClient to CurlDotNet?

Paste the curl command from the API documentation. To see the equivalent `HttpClient` code for any command, use the generator:

```csharp
string httpClientCode = Curl.ToHttpClient("curl -X POST -d 'data' https://httpbin.org/post");
Console.WriteLine(httpClientCode);
```

See the [HttpClient migration guide](../migration/httpclient.html).

### Can I convert curl commands to other languages?

```csharp
string js = Curl.ToFetch("curl https://httpbin.org/get");
string python = Curl.ToPythonRequests("curl https://httpbin.org/get");
string powershell = Curl.ToPowershellCode("curl https://httpbin.org/get");
```

## Still Have Questions?

- [Common Issues](common-issues.html) - solutions to frequent problems
- [Error Reference](error-reference.html) - exit codes and exception types
- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues) - report bugs
- [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions) - ask questions
