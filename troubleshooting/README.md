---
title: "Troubleshooting CurlDotNet - fix curl errors in C#"
description: "Troubleshooting CurlDotNet: fix common curl errors in C# - timeouts, DNS and TLS failures, HTTP 4xx/5xx, JSON parsing and build errors, with diagnostics."
---

# Troubleshooting CurlDotNet: fix common curl errors in C#

Troubleshooting CurlDotNet: fix common curl errors in C# by matching the symptom you see (timeout, DNS or connection failure, TLS certificate error, HTTP 401/404/429/500, invalid JSON, compile error) to the page that solves it. This hub explains which exceptions CurlDotNet throws, how to inspect a `CurlResult`, and which diagnostics to run before asking for help.

## Quick Links

- [Common Issues](common-issues.html) - the most frequent problems, each with a fix
- [Error Reference](error-reference.html) - curl exit codes and the exception each one maps to
- [FAQ](faq.html) - short answers to common questions
- [Diagnostic Tools](#diagnostic-tools) - verbose mode, result inspection, network checks
- [Getting Help](#getting-help) - where to ask

## Find Your Problem

| Symptom | Category | Fix |
|---------|----------|-----|
| Cannot install or set up | Installation | [Installation Issues](common-issues.html#installation-issues) |
| Could not resolve host / connection refused | Network | [DNS and Connection Errors](common-issues.html#dns-and-connection-errors) |
| Request times out | Network | [Timeout Errors](common-issues.html#timeout-errors) |
| SSL certificate problem | Security | [SSL Certificate Errors](common-issues.html#ssl-certificate-errors) |
| 401 or 403 responses | Security | [Authentication Errors](common-issues.html#authentication-errors) |
| 404, 429, 500 responses | HTTP | [HTTP Errors](common-issues.html#http-errors) |
| JSON will not parse | Data | [JSON Parsing Errors](common-issues.html#json-parsing-errors) |
| Code will not compile | Development | [Compilation Errors](common-issues.html#compilation-errors) |
| NullReferenceException at runtime | Development | [Runtime Errors](common-issues.html#runtime-errors) |
| Cannot write `-o` file / read `-d @file` | Files | [Transfer and file errors](../exceptions/transfer-errors.html) |
| Any other curl exit code | Reference | [Error Reference](error-reference.html#exit-code-table) |

Each section states the problem, the cause, the fix (code or commands) and how to prevent it.

## Two Kinds of Failure

CurlDotNet separates transport failures from HTTP status codes, exactly as the curl command-line tool does:

- **Transport failures throw, with curl's exit code.** A host that does not resolve throws `CurlCouldntResolveHostException` (6), a refused connection `CurlCouldntConnectException` (7), a TLS failure `CurlSslConnectErrorException` (35) or `CurlPeerFailedVerificationException` (60), an expired `--max-time` or `--connect-timeout` `CurlOperationTimeoutException` (28; catch its base `CurlTimeoutException`), an unwritable `-o` file `CurlWriteErrorException` (23). All derive from `CurlException` in `CurlDotNet.Exceptions`, and `CurlErrorCode` holds the number.
- **HTTP 4xx and 5xx do not throw** unless the command has `-f`/`--fail`. `ExecuteAsync` returns a `CurlResult` with `IsSuccess == false` and the `StatusCode`. With `-f` it throws `CurlHttpReturnedErrorException` (22), like curl; or call `result.EnsureSuccess()` (`CurlDotNet.Core.CurlHttpException`).
- **Cancellation is standard .NET.** A cancelled `CancellationToken` throws `OperationCanceledException`.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/status/503");
    if (!result.IsSuccess)
        Console.WriteLine($"HTTP {result.StatusCode}: {result.Body}");
}
catch (CurlTimeoutException ex) { Console.WriteLine($"Timed out: {ex.Timeout}"); }
catch (CurlCouldntConnectException ex) { Console.WriteLine($"No connection to {ex.Host}:{ex.Port}"); }
catch (CurlException ex) { Console.WriteLine($"curl error {ex.CurlErrorCode}: {ex.Message}"); }
```

Every `CurlException` carries `Command`, `CurlErrorCode` (the curl exit code, when known), `StandardOutput` (what curl would have printed before failing), `Suggestions` (a list of fix hints) and `DiagnosticInfo`. Its message says how to fix the problem. See the [Error Reference](error-reference.html) for every exception type and code, and [curl exit codes](../reference/curl-exit-codes.html) for the curl tool's numbered list.

## Diagnostic Tools

### Enable verbose mode

The `-v` flag adds the request and response headers to the start of `result.Body`, the same information curl prints on the command line; `--trace-ascii FILE` writes a full trace:

```csharp
var result = await Curl.ExecuteAsync("curl -v https://httpbin.org/get");
```

### Inspect the result

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

Console.WriteLine($"Status: {result.StatusCode}");
Console.WriteLine($"Success: {result.IsSuccess}");
Console.WriteLine($"Content-Type: {result.GetHeader("Content-Type")}");
Console.WriteLine($"Body length: {result.Body.Length}");
Console.WriteLine($"Total time (ms): {result.Timings?.Total}");

foreach (var header in result.Headers)
    Console.WriteLine($"{header.Key}: {header.Value}");
```

`result.Headers` is case-insensitive; `GetHeader` returns `null` when the header is missing, so check `HasHeader` or test for `null` before using the value.

### Validate the command before running it

```csharp
var validation = Curl.Validate("curl -X POST -H 'Content-Type: application/json' https://httpbin.org/post");
if (!validation.IsValid)
    Console.WriteLine($"Invalid command: {validation.Error}");
```

### Compare with the curl command line

If the same command works in a terminal but not in CurlDotNet, that is a bug worth [reporting](https://github.com/jacob-mellor/curl-dot-net/issues).

```bash
# Same request with native curl
curl -v https://httpbin.org/get

# Save the response to a file
curl -v https://httpbin.org/get -o response.txt

# Headers only
curl -I https://httpbin.org/get
```

### Network diagnostics

```bash
# DNS resolution
nslookup api.github.com

# Reachability
ping api.github.com

# Is the port open?
nc -vz api.github.com 443

# TLS certificate details
openssl s_client -connect api.github.com:443 -servername api.github.com
```

### .NET diagnostics

```bash
dotnet --version
dotnet --list-sdks
dotnet --list-runtimes
dotnet restore
dotnet clean && dotnet build
```

## Debugging Checklist

### Basic checks

- Does the URL open in a browser?
- Is the URL spelled correctly, including the scheme (`https://`)?
- Are you using the right HTTP method (`-X POST` and so on)?
- Is the server running and reachable from this machine?

### Authentication

- Does the endpoint need credentials?
- Is the token current, and sent in the header the API expects?
- Does the response include a `WWW-Authenticate` header that names the expected scheme?

### Request configuration

- Are headers in `-H 'Name: value'` form?
- Is the body valid JSON or form data for the `Content-Type` you send?
- Are quotes inside the command string escaped (`\"` or `""` in verbatim strings)?
- Is `--max-time` long enough for the slowest expected response?

### Response handling

- Are you checking `result.IsSuccess` before reading `result.Body`?
- Are you catching `CurlException` (or specific subclasses)?
- Does `result.GetHeader("Content-Type")` match what you parse?

### Environment

- Is the .NET SDK installed and on `PATH`?
- Is the CurlDotNet package the latest version?
- Does a proxy or firewall block outbound HTTPS from this environment?

## Troubleshooting Approach

### Read the exception

`CurlException.Message` describes what went wrong; `Suggestions` lists likely fixes.

```csharp
catch (CurlException ex)
{
    Console.WriteLine(ex.Message);
    foreach (var tip in ex.Suggestions)
        Console.WriteLine($"  - {tip}");
}
```

### Catch specific exceptions first

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.github.com");
    result.EnsureSuccess();
}
catch (CurlCouldntConnectException ex) { Console.WriteLine($"Connection: {ex.Message}"); }
catch (CurlTimeoutException ex) { Console.WriteLine($"Timeout: {ex.Message}"); }
catch (CurlDotNet.Core.CurlHttpException ex) { Console.WriteLine($"HTTP {ex.StatusCode}"); }
catch (CurlException ex) { Console.WriteLine($"curl: {ex.Message}"); }
```

### Simplify, then add back

Start with a request that should always work, then add headers, data and authentication one step at a time:

```csharp
var r1 = await Curl.ExecuteAsync("curl https://httpbin.org/status/200");
var r2 = await Curl.ExecuteAsync("curl https://httpbin.org/get -H 'Accept: application/json'");
var r3 = await Curl.ExecuteAsync(@"curl https://httpbin.org/bearer \
    -H 'Accept: application/json' \
    -H 'Authorization: Bearer token'");
```

If `https://httpbin.org/get` and `https://api.github.com` work but your endpoint does not, the problem is specific to that endpoint or your configuration.

## Getting Help

### Before asking

1. Search [existing issues](https://github.com/jacob-mellor/curl-dot-net/issues) - the problem may already be solved.
2. Read the relevant section of [Common Issues](common-issues.html).
3. Run the same command with native curl in a terminal.
4. Reduce your code to the smallest example that still fails.

### Where to ask

- [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions) - questions and how-to
- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues) - bugs and feature requests

### What to include in a bug report

1. A clear title, for example "CurlCouldntConnectException for host that native curl reaches".
2. CurlDotNet version (`dotnet list package`).
3. .NET version (`dotnet --version`) and operating system.
4. The smallest code sample that reproduces the problem.
5. The full exception message and stack trace.
6. What you expected, what happened, and whether native curl behaves differently.

## Quick Reference

```bash
# Clear the NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Add or update CurlDotNet
dotnet add package CurlDotNet

# Build with detailed output
dotnet build -v detailed
```

```csharp
// Quick connectivity test
try
{
    var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/status/200");
    Console.WriteLine(result.IsSuccess ? "Connection OK" : $"HTTP {result.StatusCode}");
}
catch (CurlException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Related Pages

- [Common Issues](common-issues.html) - detailed solutions
- [Error Reference](error-reference.html) - exit codes and exceptions
- [FAQ](faq.html) - frequently asked questions
- [Installation](../getting-started/installation.html) - setup help
- [Handling Errors tutorial](../tutorials/06-handling-errors.html) - step-by-step error handling
- [Exception documentation](../exceptions/index.html) - one page per exception family
- [Cookbook](../cookbook/index.html) - ready-to-use recipes
