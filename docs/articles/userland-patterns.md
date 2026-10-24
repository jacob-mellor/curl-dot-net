---
title: "CurlDotNet Implementation Patterns for curl in C#"
description: "Reusable C# patterns for running curl commands with CurlDotNet: observability, retries, environment matrices, replay harnesses, policy checks and more."
---

# CurlDotNet Implementation Patterns for Running curl in C#

These CurlDotNet implementation patterns show how teams run curl commands in C# beyond a single call: adding logging and timings, retrying on transient failures, fanning out across environments, replaying captured commands, and guarding what a command is allowed to do. Every snippet uses the shipped API (`Curl.ExecuteAsync`, `CurlRequestBuilder`, `CurlSettings`, `CurlResult`) and compiles against the current package.

Install the package first:

```bash
dotnet add package CurlDotNet
```

Snippets assume `using CurlDotNet;` plus `using CurlDotNet.Core;` where `CurlResult`, `CurlSettings` or `CurlRequestBuilder` are named.

## Pattern 1: Observability Bootstrap

**Why it is useful:** Log status, size and timings for every request without wrapping the body of each call.

```csharp
using CurlDotNet;
using CurlDotNet.Core;
using Microsoft.Extensions.Logging;

async Task<CurlResult> ExecuteLoggedAsync(string command, ILogger logger)
{
    var result = await Curl.ExecuteAsync(command);
    logger.LogInformation("curl {Command} -> {Status} in {TotalMs} ms ({Bytes} bytes)",
        command, result.StatusCode, result.Timings?.Total, result.Body?.Length ?? 0);
    return result;
}

var r = await ExecuteLoggedAsync("curl https://httpbin.org/get", logger);
```

`CurlResult.Timings` exposes `NameLookup`, `Connect`, `AppConnect`, `PreTransfer`, `Redirect`, `StartTransfer` and `Total` in milliseconds for HTTP requests.

## Pattern 2: Environment Matrix Runner

**Why it is useful:** Run the same health check against dev, staging and production and see drift immediately.

```csharp
using CurlDotNet;

record TargetEnvironment(string Name, string Url, string Token);

async Task<Dictionary<string, bool>> RunMatrixAsync(IEnumerable<TargetEnvironment> envs)
{
    var results = new Dictionary<string, bool>();
    foreach (var env in envs)
    {
        var r = await Curl.ExecuteAsync(
            $"curl --max-time 10 {env.Url}/health -H 'Authorization: Bearer {env.Token}'");
        results[env.Name] = r.IsSuccess;
    }
    return results;
}
```

For independent environments, build the command strings and pass them to `Curl.ExecuteManyAsync` to run them in parallel.

## Pattern 3: Secrets from the Environment

**Why it is useful:** Keep tokens out of source. The parser expands `$VAR`, `${VAR}`, `%VAR%` and `$env:VAR` from the process environment, exactly like a shell would.

```csharp
using CurlDotNet;

// API_TOKEN is read from the environment at execution time; nothing is interpolated in C#.
var result = await Curl.ExecuteAsync(
    "curl https://httpbin.org/bearer -H 'Authorization: Bearer $API_TOKEN'");
Console.WriteLine(result.StatusCode);
```

If the token comes from a vault client instead, fetch it first and interpolate it into the string; never log the resulting command.

## Pattern 4: Profiles with CurlSettings

**Why it is useful:** Give different callers (support tooling, batch importers) different timeouts and retry budgets without changing the curl strings.

```csharp
using CurlDotNet;
using CurlDotNet.Core;

static class Profiles
{
    public static CurlSettings SupportAgent => new CurlSettings()
        .WithTimeout(10).WithHeader("X-Actor", "support");

    public static CurlSettings BatchImporter => new CurlSettings()
        .WithTimeout(60).WithHeader("X-Actor", "batch");
}

var result = await Curl.ExecuteAsync("curl https://httpbin.org/get", Profiles.BatchImporter);
Console.WriteLine(result.StatusCode);
```

`CurlSettings` also offers `WithConnectTimeout`, `WithFollowRedirects`, `WithInsecure`, `WithRetries`, `WithHeaders`, `WithCancellation`, `WithProgress`, `WithProxy`, `WithUserAgent`, `WithCookies` (a `CookieContainer` sent and updated on every hop, so a session carries over between calls), `WithRedirectHandler`, `WithAutoDecompression` and `WithBufferSize`. The engine applies every one of them, to every URL of a multi-URL command, and a value set in the settings overrides the same option in the command.

## Pattern 5: Contract-Locked Responses

**Why it is useful:** Fail loudly when an API changes shape, before bad data reaches production.

```csharp
using System.Text.Json;
using CurlDotNet;
using CurlDotNet.Core;

async Task<T> ExecuteWithContractAsync<T>(CurlRequestBuilder builder, Func<JsonElement, bool> check)
{
    var result = (await builder.ExecuteAsync()).EnsureSuccess(); // throws on 4xx/5xx
    using var doc = JsonDocument.Parse(result.Body);
    if (!check(doc.RootElement))
        throw new InvalidOperationException("Response violates contract");
    return result.ParseJson<T>();
}

var builder = CurlRequestBuilder.Get("https://api.github.com/repos/curl/curl");
var repo = await ExecuteWithContractAsync<Dictionary<string, object>>(builder, e => e.TryGetProperty("name", out _));
```

`EnsureSuccess()` throws `CurlDotNet.Core.CurlHttpException` carrying `StatusCode`, `ResponseBody` and `ResponseHeaders`.

## Pattern 6: Replay Harness

**Why it is useful:** Replay captured curl commands from a JSON file to reproduce an incident deterministically.

```csharp
using System.Text.Json;
using CurlDotNet;

record ReplayEnvelope(string Command, string? ExpectedBodySubstring);

var envelopes = JsonSerializer.Deserialize<List<ReplayEnvelope>>(await File.ReadAllTextAsync("replay.json"))!;
foreach (var env in envelopes)
{
    var r = await Curl.ExecuteAsync(env.Command);
    if (!r.IsSuccess)
        Console.WriteLine($"Replay failed: {r.StatusCode} for {env.Command}");
    else if (env.ExpectedBodySubstring is { Length: > 0 } needle && !r.Body.Contains(needle))
        Console.WriteLine($"Replay mismatch for {env.Command}");
}
```

Run `Curl.Validate(env.Command)` first to reject malformed entries without sending anything.

## Pattern 7: Feature-Flagged Commands

**Why it is useful:** Switch between a control and a candidate command (different headers, flags or endpoints) from a flag provider.

```csharp
using CurlDotNet;
using CurlDotNet.Core;

async Task<CurlResult> ExecuteFlaggedAsync(Func<bool> flag)
{
    var control = "curl https://httpbin.org/get -H 'X-Tier: control'";
    var candidate = "curl https://httpbin.org/get -H 'X-Tier: candidate' --compressed";
    return await Curl.ExecuteAsync(flag() ? candidate : control);
}
```

## Pattern 8: Tenant-Aware Routing

**Why it is useful:** Pick base URL and credentials per tenant while keeping one request shape.

```csharp
using CurlDotNet.Core;

record TenantSettings(string TenantId, string BaseUrl, string ApiKey);

Task<CurlResult> ExecuteForTenantAsync(TenantSettings tenant, string relativePath) =>
    CurlRequestBuilder.Get($"{tenant.BaseUrl}{relativePath}")
        .WithHeader("X-Tenant", tenant.TenantId)
        .WithBearerToken(tenant.ApiKey)
        .WithTimeout(TimeSpan.FromSeconds(15))
        .ExecuteAsync();

var result = await ExecuteForTenantAsync(
    new TenantSettings("acme", "https://httpbin.org", "key"), "/bearer");
```

Call `.ToCurlCommand()` on the builder to log the equivalent curl string.

## Pattern 9: Evidence Bundles

**Why it is useful:** Serialize command, status, headers and body so auditors can see exactly what ran.

```csharp
using System.Text.Json;
using CurlDotNet;

async Task CaptureAsync(string workflow, string operatorId, string command, string outputPath)
{
    var r = await Curl.ExecuteAsync(command);
    var bundle = new { workflow, operatorId, command, r.StatusCode, r.Headers, r.Body,
                       timestamp = DateTimeOffset.UtcNow };
    await File.WriteAllTextAsync(outputPath,
        JsonSerializer.Serialize(bundle, new JsonSerializerOptions { WriteIndented = true }));
}
```

Strip `Authorization` from `command` before writing if the bundle leaves your system.

## Pattern 10: Reading Rate-Limit Headers

**Why it is useful:** Track remaining quota from response headers and pause before the server starts returning 429.

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com/rate_limit");
var remaining = int.Parse(result.GetHeader("X-RateLimit-Remaining") ?? "0");
var resetAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(result.GetHeader("X-RateLimit-Reset") ?? "0"));

if (remaining == 0)
    Console.WriteLine($"Quota exhausted; resets at {resetAt:u}");
```

`CurlResult.Headers` is case-insensitive; `GetHeader` returns `null` when the header is absent.

## Pattern 11: Retries for Transient Failures

**Why it is useful:** Transient failures (timeouts, 429, 503) should not fail a whole job. curl's `--retry` handles the common case: it retries timeouts and HTTP 408/429/500/502/503/504 with doubling back-off, honours `Retry-After`, and `--retry-max-time` caps the total. `CurlSettings.WithRetries(count, delayMs)` retries any failure. See [curl --retry in C#](../cookbook/retry.html).

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var result = await Curl.ExecuteAsync("curl -m 10 --retry 3 https://httpbin.org/status/503");

var viaSettings = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/get",
    new CurlSettings().WithRetries(count: 3, delayMs: 500));
```

For a custom policy, such as retrying only some status codes, a small loop still works:

```csharp
using CurlDotNet;
using CurlDotNet.Core;

async Task<CurlResult> RetryOnStatusAsync(string command, int attempts = 3)
{
    CurlResult result = null!;
    for (var attempt = 1; attempt <= attempts; attempt++)
    {
        result = await Curl.ExecuteAsync(command, new CurlSettings().WithTimeout(10));
        if (result.StatusCode is not (429 or 502 or 503 or 504)) return result;
        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    }
    return result;
}
```

A result also offers `await result.Retry()` to re-run the same command once more.

## Pattern 12: Shadow (Dark-Launch) Calls

**Why it is useful:** Call the current and candidate API versions side by side and compare before cutting over.

```csharp
using CurlDotNet;

var results = await Curl.ExecuteManyAsync(
    "curl https://httpbin.org/anything/v1/orders",
    "curl https://httpbin.org/anything/v2/orders");

var (v1, v2) = (results[0], results[1]);
Console.WriteLine($"v1:{v1.StatusCode} v2:{v2.StatusCode} same body: {v1.Body == v2.Body}");
```

## Pattern 13: Cache Warmers and Health Probes

**Why it is useful:** Touch a list of URLs on a schedule with HEAD requests and report status codes, replacing a bash loop.

```csharp
using CurlDotNet;

async Task WarmAsync(IEnumerable<string> urls, CancellationToken token)
{
    using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
    do
    {
        foreach (var url in urls)
        {
            var r = await Curl.ExecuteAsync($"curl -I --max-time 5 {url}", token);
            Console.WriteLine($"{url}: {r.StatusCode}");
        }
    } while (await timer.WaitForNextTickAsync(token));
}
```

Cancelling the token while a request is in flight throws the standard `OperationCanceledException` (as does `PeriodicTimer.WaitForNextTickAsync`), so one `catch (OperationCanceledException)` around `WarmAsync` handles shutdown.

## Pattern 14: Policy Checks Before Execution

**Why it is useful:** Reject commands that violate policy (for example `--insecure` outside development) before anything is sent.

```csharp
using CurlDotNet;
using CurlDotNet.Core;

Task<CurlResult> ExecuteWithPolicyAsync(string command)
{
    var validation = Curl.Validate(command);
    if (!validation.IsValid)
        throw new ArgumentException(validation.Error, nameof(command));
    if (command.Contains("--insecure") || command.Contains(" -k "))
        throw new InvalidOperationException("Insecure TLS is not allowed");
    return Curl.ExecuteAsync(command);
}
```

## Pattern 15: Onboarding Katas

**Why it is useful:** Self-verifying xUnit tests give new engineers hands-on curl practice with immediate feedback.

```csharp
using CurlDotNet;
using Xunit;

public class Kata01_GetRequest
{
    [Fact]
    public async Task Should_Fetch_Public_Data()
    {
        var result = await Curl.ExecuteAsync("curl https://api.github.com");
        Assert.True(result.IsSuccess);
        Assert.Contains("current_user_url", result.Body);
    }
}
```

## Pattern 16: Generating Client Code from a Command

**Why it is useful:** Hand a working curl command to another team in their language, or show the equivalent `HttpClient` code during a migration.

```csharp
using CurlDotNet;

var command = "curl -X POST https://httpbin.org/post -H 'Accept: application/json' -d 'a=1'";
Console.WriteLine(Curl.ToHttpClient(command));     // C# HttpClient
Console.WriteLine(Curl.ToPythonRequests(command)); // Python requests
Console.WriteLine(Curl.ToFetch(command));          // JavaScript fetch
Console.WriteLine(Curl.ToPowershellCode(command)); // PowerShell
```

## Related Pages

- [Tutorial Projects](userland-tutorials.html)
- [CI/CD Integration](ci-cd-integration.html)
- [Logging and Observability](../guides/logging-observability.html)
- [Handle Errors](../cookbook/beginner/handle-errors.html)
