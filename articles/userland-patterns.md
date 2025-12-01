# CurlDotNet Implementation Patterns (With C# Code)

> Install CurlDotNet from NuGet before running these snippets:
>
> ```bash
> dotnet add package CurlDotNet
> ```
>
> All samples target .NET 8/10 and assume `using CurlDotNet;` plus any namespaces mentioned inline.

## Pattern 1: Observability Bootstrap

**Why it’s useful:** This C#/.NET helper wraps a CurlDotNet request with `ILogger` and `ActivitySource` instrumentation so every curl-style call emits reliable telemetry for tracing and diagnostics.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using CurlDotNet;

public static class ObservabilityBootstrap
{
    public static async Task ExecuteAsync(ActivitySource activitySource, ILogger logger)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await Curl.ExecuteAsync("curl https://status.example.com/health -H 'Accept: application/json'");
        stopwatch.Stop();

        logger.LogInformation("curl.request {@Envelope}", new
        {
            Url = "https://status.example.com/health",
            result.StatusCode,
            DurationMs = stopwatch.ElapsedMilliseconds,
            result.IsSuccess
        });

        activitySource.StartActivity("curl.request")?
            .SetTag("http.url", "https://status.example.com/health")
            .SetTag("http.status_code", result.StatusCode)
            .SetTag("curl.flags", "-H 'Accept: application/json'");
    }
}
```

## Pattern 2: Environment Matrix Runner

**Why it’s useful:** When your .NET services span dev, staging, and production, this C# routine fans out CurlDotNet health checks across each environment to verify curl parity and expose drift immediately.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public record TargetEnvironment(string Name, string Url, string Token);

public static async Task<IDictionary<string, bool>> RunMatrixAsync(IEnumerable<TargetEnvironment> environments)
{
    var results = new Dictionary<string, bool>();

    foreach (var env in environments)
    {
        var response = await Curl.ExecuteAsync($@"curl {env.Url}/health -H 'Authorization: Bearer {env.Token}'");
        results[env.Name] = response.IsSuccess;
    }

    return results;
}
```

## Pattern 3: Secrets-as-Code Providers

**Why it’s useful:** Instead of hardcoding tokens, this .NET interface shows how C# services can fetch secrets from managed sources and feed them directly into CurlDotNet to execute curl-equivalent calls securely.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public interface ITokenProvider
{
    ValueTask<string> GetTokenAsync(CancellationToken cancellationToken = default);
}

public sealed class AzureIdentityTokenProvider : ITokenProvider
{
    public async ValueTask<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        // Example only: plug in real Managed Identity code here
        await Task.Delay(50, cancellationToken);
        return Environment.GetEnvironmentVariable("API_TOKEN") ?? throw new InvalidOperationException("Token missing");
    }
}

public static class SecretsAsCodeSample
{
    public static async Task ExecuteAsync(ITokenProvider provider)
    {
        var token = await provider.GetTokenAsync();
        var result = await Curl.ExecuteAsync($"curl https://api.example.com -H 'Authorization: Bearer {token}'");
        Console.WriteLine(result.StatusCode);
    }
}
```

## Pattern 4: Personas and Profiles

**Why it’s useful:** Apply persona-aware defaults inside your .NET apps so every CurlDotNet call inherits the right curl flags without copy/paste.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using CurlDotNet.Core;

public sealed record CurlProfile(string Name, Action<CurlRequestBuilder> Apply);

public static class ProfileCatalog
{
    public static readonly CurlProfile SupportAgent = new(
        "SupportAgent",
        builder => builder
            .WithHeader("X-Actor", "support")
            .WithRetry(2, TimeSpan.FromSeconds(1))
            .WithTimeout(TimeSpan.FromSeconds(10))
    );

    public static readonly CurlProfile BatchImporter = new(
        "BatchImporter",
        builder => builder
            .WithHeader("X-Actor", "batch-importer")
            .WithTimeout(TimeSpan.FromSeconds(60))
            .WithRetry(5, TimeSpan.FromSeconds(5))
    );
}

public static Task<CurlResult> ExecuteWithProfileAsync(CurlProfile profile, string url)
{
    var builder = CurlRequestBuilder.Get(url);
    profile.Apply(builder);
    return builder.ExecuteAsync();
}
```

## Pattern 5: Contract-Locked Builders

**Why it’s useful:** Wrap CurlDotNet builders in schema validation so your .NET code spots breaking API changes before they impact production.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using System.Text.Json;
using CurlDotNet.Core;

public static class ContractLockedBuilder
{
    public static async Task<T> ExecuteWithSchemaAsync<T>(CurlRequestBuilder builder, Func<JsonElement, bool> schemaValidator)
    {
        var result = await builder.ExecuteAsync();
        if (!result.IsSuccess)
        {
            throw new CurlException($"Unexpected status: {result.StatusCode}");
        }

        using var doc = JsonDocument.Parse(result.Body);
        if (!schemaValidator(doc.RootElement))
        {
            throw new InvalidOperationException("Schema validation failed");
        }

        return result.ParseJson<T>();
    }
}
```

## Pattern 6: Sandbox Replay Harness

**Why it’s useful:** Replay captured curl commands through CurlDotNet to reproduce issues inside C#, giving .NET teams deterministic debugging.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Text.Json;
using CurlDotNet;

public record ReplayEnvelope(string Command, string? ExpectedBodySubstring);

public static async Task ReplayAsync(string replayFile)
{
    var payload = await File.ReadAllTextAsync(replayFile);
    var envelopes = JsonSerializer.Deserialize<List<ReplayEnvelope>>(payload)!;

    foreach (var envelope in envelopes)
    {
        var response = await Curl.ExecuteAsync(envelope.Command);
        if (!response.IsSuccess)
        {
            Console.WriteLine($"Replay failed: {response.StatusCode}");
        }
        else if (envelope.ExpectedBodySubstring is { Length: >0 } needle && !response.Body.Contains(needle, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Replay mismatch for command {envelope.Command}");
        }
    }
}
```

## Pattern 7: Feature Flagged Experiments

**Why it’s useful:** Flip between control and candidate CurlDotNet commands so you can experiment in .NET without rewriting curl-heavy automation.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public static class FeatureFlaggedExperiments
{
    public static async Task<CurlResult> ExecuteAsync(bool useCandidate)
    {
        var control = "curl https://api.example.com -H 'X-Tier: control'";
        var candidate = "curl https://api.example.com -H 'X-Tier: candidate' --compressed";
        var selected = useCandidate ? candidate : control;
        return await Curl.ExecuteAsync(selected);
    }
}
```

## Pattern 8: Documentation-Driven SDKs

**Why it’s useful:** Turn Markdown curl snippets into executable C# delegates so your .NET SDKs stay synchronized with documentation.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using System.Text.RegularExpressions;
using CurlDotNet.Core;

public static class MarkdownCommandLoader
{
    private static readonly Regex CommandRegex = new(@"```curl(?<body>[\s\S]*?)```", RegexOptions.Compiled);

    public static IEnumerable<Func<Task<CurlResult>>> LoadFromMarkdown(string markdown)
    {
        foreach (Match match in CommandRegex.Matches(markdown))
        {
            var command = match.Groups["body"].Value.Trim();
            yield return () => Curl.ExecuteAsync(command);
        }
    }
}

// Usage
foreach (var operation in MarkdownCommandLoader.LoadFromMarkdown(File.ReadAllText("docs/payments.md")))
{
    var response = await operation();
    Console.WriteLine(response.StatusCode);
}
```

## Pattern 9: Tenant-Aware Routing

**Why it’s useful:** Route CurlDotNet calls based on tenant metadata so your C# platform honors tenant-specific URLs and tokens just like curl scripts.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using CurlDotNet.Core;

public sealed record TenantSettings(string TenantId, string BaseUrl, string ApiKey);

public sealed class TenantRouter
{
    private readonly IReadOnlyDictionary<string, TenantSettings> _map;

    public TenantRouter(IEnumerable<TenantSettings> settings)
    {
        _map = settings.ToDictionary(s => s.TenantId, s => s);
    }

    public Task<CurlResult> ExecuteForTenantAsync(string tenantId, string relativePath)
    {
        var tenant = _map[tenantId];
        return CurlRequestBuilder
            .Get($"{tenant.BaseUrl}{relativePath}")
            .WithHeader("X-Tenant", tenant.TenantId)
            .WithHeader("Authorization", $"Bearer {tenant.ApiKey}")
            .ExecuteAsync();
    }
}
```

## Pattern 10: Compliance Evidence Bundles

**Why it’s useful:** Serialize CurlDotNet responses so compliance auditors can see exactly which curl-style workflows ran inside your .NET systems.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Text.Json;
using CurlDotNet;

public sealed record EvidenceBundle(string Workflow, DateTimeOffset Timestamp, CurlResult Result, string OperatorId);

public static class EvidenceWriter
{
    public static async Task CaptureAsync(string workflowName, string operatorId, string command, string outputPath)
    {
        var result = await Curl.ExecuteAsync(command);
        var bundle = new EvidenceBundle(workflowName, DateTimeOffset.UtcNow, result, operatorId);
        var json = JsonSerializer.Serialize(bundle, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(outputPath, json);
    }
}
```

## Pattern 11: Progressive Rollout Gates

**Why it’s useful:** Gradually rollout new curl behavior by hashing identifiers in C# and deciding which CurlDotNet command to run per request.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Security.Cryptography;
using System.Text;
using CurlDotNet;

public static class RolloutGate
{
    public static async Task<CurlResult> ExecuteAsync(string rolloutKey, int enabledPercentage)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(rolloutKey));
        var value = hash[0];
        var threshold = (byte)(255 * (enabledPercentage / 100.0));
        var command = value <= threshold
            ? "curl https://api.example.com/new-feature"
            : "curl https://api.example.com/current";
        return await Curl.ExecuteAsync(command);
    }
}
```

## Pattern 12: Incident Reconstruction Timelines

**Why it’s useful:** Record each CurlDotNet action into a C# timeline so incidents can be reconstructed step-by-step without guessing which curl command ran.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Text.Json;
using CurlDotNet;

public sealed record TimelineEvent(DateTimeOffset Timestamp, string Activity, CurlResult Result, string Commit);

public static class TimelineRecorder
{
    private static readonly List<TimelineEvent> _events = new();

    public static async Task<CurlResult> RecordAsync(string activity, string command, string commit)
    {
        var result = await Curl.ExecuteAsync(command);
        _events.Add(new TimelineEvent(DateTimeOffset.UtcNow, activity, result, commit));
        return result;
    }

    public static Task ExportAsync(string path)
    {
        var json = JsonSerializer.Serialize(_events, new JsonSerializerOptions { WriteIndented = true });
        return File.WriteAllTextAsync(path, json);
    }
}
```

## Pattern 13: Rate-Limit Diplomacy Layer

**Why it’s useful:** Share rate-limit budgets across microservices by parsing response headers in CurlDotNet and caching them inside .NET state.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Collections.Concurrent;
using CurlDotNet;

public sealed class SharedRateBudget
{
    private readonly ConcurrentDictionary<string, int> _remaining = new();

    public void Update(string tenant, int remaining) => _remaining[tenant] = remaining;

    public bool CanExecute(string tenant) => _remaining.GetOrAdd(tenant, _ => 0) > 0;
}

public sealed class DiplomaticCurlClient
{
    private readonly SharedRateBudget _budget;

    public DiplomaticCurlClient(SharedRateBudget budget) => _budget = budget;

    public async Task<CurlResult> ExecuteAsync(string tenant, string command)
    {
        if (!_budget.CanExecute(tenant))
        {
            throw new InvalidOperationException($"Rate limit exhausted for {tenant}");
        }

        var result = await Curl.ExecuteAsync(command);
        if (result.Headers.TryGetValue("X-RateLimit-Remaining", out var remainingHeader))
        {
            _budget.Update(tenant, int.Parse(remainingHeader));
        }

        return result;
    }
}
```

## Pattern 14: Schema Evolution Alerts

**Why it’s useful:** Hash response shapes in C# after each CurlDotNet call so schema changes trigger alerts before they break downstream code.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CurlDotNet;

public static class SchemaMonitor
{
    public static async Task MonitorAsync(string command, string schemaHashPath)
    {
        var result = await Curl.ExecuteAsync(command);
        using var json = JsonDocument.Parse(result.Body);
        var shape = string.Join('\n', json.RootElement.EnumerateObject().Select(p => $"{p.Name}:{p.Value.ValueKind}"));
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(shape)));

        var previous = File.Exists(schemaHashPath) ? await File.ReadAllTextAsync(schemaHashPath) : string.Empty;
        if (!string.Equals(previous, hash, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Schema changed from {previous} to {hash}");
            await File.WriteAllTextAsync(schemaHashPath, hash);
        }
    }
}
```

## Pattern 15: Temporal Workflow Bridges

**Why it’s useful:** Wrap CurlDotNet commands as workflow activities so deterministic .NET orchestrators (Durable Functions, Temporal) can replay curl work safely.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public interface IWorkflowActivity
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}

public sealed class CurlWorkflowActivity : IWorkflowActivity
{
    private readonly string _command;

    public CurlWorkflowActivity(string command) => _command = command;

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Curl.ExecuteAsync(_command);
    }
}
```

## Pattern 16: Data Provenance Tags

**Why it’s useful:** Attach provenance metadata to CurlDotNet responses so your .NET applications always know where curl-fetched data originated.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed record ProvenanceEnvelope<T>(string Source, DateTimeOffset RetrievedAt, T Data);

public static async Task<ProvenanceEnvelope<T>> FetchWithProvenanceAsync<T>(string source, string command)
{
    var result = await Curl.ExecuteAsync(command);
    var payload = result.ParseJson<T>();
    return new ProvenanceEnvelope<T>(Source: source, RetrievedAt: DateTimeOffset.UtcNow, Data: payload);
}
```

## Pattern 17: Developer Onboarding Katas

**Why it’s useful:** Use xUnit katas that run CurlDotNet commands, giving new .NET engineers practical curl experience with self-verifying C# tests.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
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

## Pattern 18: Immutable Run Logs

**Why it’s useful:** Append CurlDotNet command hashes to an immutable log so regulated .NET environments can prove every curl-style action taken.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Security.Cryptography;
using System.Text;
using CurlDotNet;

public static class ImmutableLogger
{
    private const string LogPath = "runlog.txt";

    public static async Task<CurlResult> ExecuteLoggedAsync(string command)
    {
        var result = await Curl.ExecuteAsync(command);
        var line = $"{DateTimeOffset.UtcNow:o}|{command}|{result.StatusCode}|{Hash(result.Body)}";
        await File.AppendAllLinesAsync(LogPath, new[] { line });
        return result;
    }

    private static string Hash(string body) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(body)));
}
```

## Pattern 19: Synthetic Partner Simulation

**Why it’s useful:** Spin up ASP.NET Core mocks and hit them with CurlDotNet so .NET teams can simulate partner APIs without real dependencies.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/partner/ping", () => Results.Json(new { status = "ok", latencyMs = 123 }));
app.MapGet("/partner/feature", () => Results.Json(new { enabled = true }));

await app.StartAsync();
var result = await Curl.ExecuteAsync("curl http://localhost:5000/partner/ping");
Console.WriteLine(result.Body);
await app.StopAsync();
```

## Pattern 20: Documentation Screenshots as Code

**Why it’s useful:** Capture terminal sessions that execute CurlDotNet commands so your documentation shows real curl-on-.NET output.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using System.Diagnostics;

public static class TerminalCapture
{
    public static void Capture(string scriptPath, string outputPath)
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "asciinema",
            ArgumentList = { "rec", "--command", scriptPath, outputPath },
            RedirectStandardOutput = true,
            RedirectStandardError = true
        });
        process?.WaitForExit();
    }
}
```

## Pattern 21: Multi-Hop Proxy Playbooks

**Why it’s useful:** Encapsulate multiple proxies inside a CurlRequestBuilder extension so .NET services can express complex curl proxy chains in C#.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using CurlDotNet.Core;

public static class ProxyChainExtensions
{
    public static CurlRequestBuilder WithProxyChain(this CurlRequestBuilder builder, params string[] proxies)
    {
        foreach (var proxy in proxies)
        {
            builder = builder.WithProxy(proxy);
        }
        return builder;
    }
}
```

## Pattern 22: Content Negotiation Laboratories

**Why it’s useful:** Request different content-types via CurlDotNet and compare payloads in C#, making content negotiation experiments reproducible.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

string[] accepts = [
    "application/json",
    "application/xml",
    "text/csv"
];

foreach (var accept in accepts)
{
    var response = await Curl.ExecuteAsync($"curl https://api.example.com/items -H 'Accept: {accept}'");
    Console.WriteLine($"{accept}: {response.Body.Length} bytes");
}
```

## Pattern 23: Dark Launch Shadow Clients

**Why it’s useful:** Run production and candidate CurlDotNet calls side-by-side so .NET teams can dark launch new API versions without risk.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public static async Task ExecuteShadowAsync()
{
    var production = Curl.ExecuteAsync("curl https://api.example.com/v1/orders");
    var candidate = Curl.ExecuteAsync("curl https://api.example.com/v2/orders");

    await Task.WhenAll(production, candidate);
    Console.WriteLine($"v1:{production.Result.StatusCode} v2:{candidate.Result.StatusCode}");
}
```

## Pattern 24: Edge Cache Warmers

**Why it’s useful:** Use CurlDotNet to prefetch assets on a schedule from C#, keeping CDN caches warm without scripting bash curl loops.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public static async Task WarmAsync(IEnumerable<string> urls, CancellationToken token)
{
    using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
    do
    {
        foreach (var url in urls)
        {
            var response = await Curl.ExecuteAsync($"curl -s -o /dev/null -w '%{% raw %}{{http_code}}{% endraw %}' {url}");
            Console.WriteLine($"Warmed {url}: {response.StatusCode}");
        }
    } while (await timer.WaitForNextTickAsync(token));
}
```

## Pattern 25: Event-Sourced API History

**Why it’s useful:** Write every CurlDotNet result into a channel so .NET analytics pipelines can rebuild API history just like event sourcing.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Threading.Channels;
using CurlDotNet;

public static class ApiEventStream
{
    private static readonly Channel<CurlResult> _channel = Channel.CreateUnbounded<CurlResult>();

    public static async Task<CurlResult> ExecuteTrackedAsync(string command)
    {
        var result = await Curl.ExecuteAsync(command);
        await _channel.Writer.WriteAsync(result);
        return result;
    }

    public static IAsyncEnumerable<CurlResult> ReadAllAsync() => _channel.Reader.ReadAllAsync();
}
```

## Pattern 26: Collaborative Postman Replacement

**Why it’s useful:** Store curated curl recipes as C# records so teams can collaborate on CurlDotNet-powered replacements for Postman collections.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed record CurlRecipe(string Name, string Command, string Description);

public sealed class RecipeStore
{
    private readonly List<CurlRecipe> _recipes = new();

    public void Add(CurlRecipe recipe) => _recipes.Add(recipe);
    public IEnumerable<CurlRecipe> All => _recipes;
}
```

## Pattern 27: Infrastructure Drift Scanners

**Why it’s useful:** Validate TLS certificates and other infra signals in C# before executing CurlDotNet commands to catch drift early.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using CurlDotNet;

public static async Task ScanAsync(string url, string expectedThumbprint)
{
    using var client = new HttpClient(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, cert, _, _) =>
        {
            if (cert is null)
            {
                return false;
            }

            var thumbprint = cert.GetCertHashString();
            if (!string.Equals(thumbprint, expectedThumbprint, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Drift detected for {url}: {thumbprint}");
            }
            return true;
        }
    });

    var response = await client.GetAsync(url);
    Console.WriteLine(response.StatusCode);
}
```

## Pattern 28: Domain-Specific Languages

**Why it’s useful:** Parse a lightweight DSL in C# and translate it to CurlDotNet invocations, letting domain experts describe curl flows without coding.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public static class DslRunner
{
    public static Task<CurlResult> ExecuteAsync(string dsl)
    {
        // Example DSL: "GET https://api.example.com/items?limit=10"
        var (method, url) = ParseDsl(dsl);
        return Curl.ExecuteAsync($"curl -X {method} {url}");
    }

    private static (string Method, string Url) ParseDsl(string dsl)
    {
        var parts = dsl.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return (parts[0], parts[1]);
    }
}
```

## Pattern 29: Knowledge Graph Integrations

**Why it’s useful:** Emit knowledge-graph events for each CurlDotNet call so architects can see how .NET services depend on external APIs.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Text.Json;
using CurlDotNet;

public static class KnowledgeGraphEmitter
{
    public static async Task EmitAsync(string service, string endpoint, string command, string outputPath)
    {
        var result = await Curl.ExecuteAsync(command);
        var graphEvent = new
        {
            Service = service,
            Endpoint = endpoint,
            result.StatusCode,
            Timestamp = DateTimeOffset.UtcNow
        };
        await File.AppendAllTextAsync(outputPath, JsonSerializer.Serialize(graphEvent) + "\n");
    }
}
```

## Pattern 30: Security Chaos Drills

**Why it’s useful:** Execute predefined CurlDotNet chaos scenarios so security teams can prove how .NET code handles hostile curl situations.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed record ChaosScenario(string Name, string Command, Func<CurlResult, bool> Expectation);

public static async Task RunChaosAsync(IEnumerable<ChaosScenario> scenarios)
{
    foreach (var scenario in scenarios)
    {
        var result = await Curl.ExecuteAsync(scenario.Command);
        if (!scenario.Expectation(result))
        {
            Console.WriteLine($"Scenario {scenario.Name} failed");
        }
    }
}
```

## Pattern 31: Accessibility-Focused Tooling

**Why it’s useful:** Build accessible CLIs in C# that wrap CurlDotNet, ensuring screen-reader-friendly tooling for curl workflows on .NET.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using Spectre.Console;
using CurlDotNet;

public static async Task RunAccessibleCliAsync()
{
    var url = AnsiConsole.Ask<string>("Enter URL:");
    var result = await Curl.ExecuteAsync($"curl {url}");
    AnsiConsole.MarkupLine($"[bold]Status:[/] {result.StatusCode}");
    AnsiConsole.WriteLine(result.Body);
}
```

## Pattern 32: Offline-First Mocking Kits

**Why it’s useful:** Package mock data and CurlDotNet responses into offline kits so field engineers can run curl-style diagnostics without internet access.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.IO.Compression;
using CurlDotNet;

public static class OfflineKitBuilder
{
    public static async Task BuildAsync(string kitPath, params string[] commands)
    {
        using var archive = ZipFile.Open(kitPath, ZipArchiveMode.Create);
        foreach (var command in commands)
        {
            var result = await Curl.ExecuteAsync(command);
            var entry = archive.CreateEntry(Guid.NewGuid() + ".json");
            await using var stream = entry.Open();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(result.Body);
        }
    }
}
```

## Pattern 33: Governance Scorecards

**Why it’s useful:** Score each CurlDotNet integration in C# so platform teams know whether logging, tests, and docs meet .NET governance standards.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed record IntegrationScore(string Name, int Logging, int Tests, int Docs)
{
    public int Total => Logging + Tests + Docs;
}

public static IntegrationScore Score(string name, bool hasLogging, bool hasTests, bool hasDocs)
    => new(name, hasLogging ? 3 : 0, hasTests ? 3 : 0, hasDocs ? 4 : 0);
```

## Pattern 34: Auto-Generated Runbooks

**Why it’s useful:** Reflect over C# runbook steps and emit Markdown so every CurlDotNet-powered operational flow ships with living documentation.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Reflection;
using CurlDotNet;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RunbookStepAttribute(string Description) : Attribute
{
    public string Description { get; } = Description;
}

public static class RunbookGenerator
{
    public static IEnumerable<string> Generate(Type type)
        => type.GetMethods()
            .Select(m => m.GetCustomAttribute<RunbookStepAttribute>())
            .Where(attr => attr is not null)
            .Select(attr => $"- {attr!.Description}");
}
```

## Pattern 35: Threat Modeling Workshops

**Why it’s useful:** Capture threat scenarios by executing CurlDotNet scripts in C#, feeding security reviews with concrete curl abuse cases.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed record ThreatScenario(string Name, string Command, string AbuseCase);

public static async Task DocumentThreatsAsync(IEnumerable<ThreatScenario> scenarios, string outputPath)
{
    var lines = new List<string>();
    foreach (var scenario in scenarios)
    {
        await Curl.ExecuteAsync(scenario.Command);
        lines.Add($"{scenario.Name}: {scenario.AbuseCase}");
    }
    await File.WriteAllLinesAsync(outputPath, lines);
}
```

## Pattern 36: High-Fidelity Mock Clients for QA

**Why it’s useful:** Model high-fidelity QA journeys as CurlDotNet sequences so testers can mimic user flows without brittle UI scripts.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed class CheckoutScenario
{
    public async Task RunAsync()
    {
        await Curl.ExecuteAsync("curl https://shop.example.com/api/cart -H 'Authorization: Bearer token'");
        await Curl.ExecuteAsync("curl https://shop.example.com/api/checkout -X POST -d '{\"total\":1200}'");
    }
}
```

## Pattern 37: Intelligent Retries with Telemetry Feedback

**Why it’s useful:** Adapt retry counts dynamically in C# based on CurlDotNet responses so .NET services respect curl-style backoff semantics.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed class AdaptiveRetryClient
{
    private int _maxRetries = 3;

    public async Task<CurlResult> ExecuteAsync(string command)
    {
        for (var attempt = 1; attempt <= _maxRetries; attempt++)
        {
            var result = await Curl.ExecuteAsync(command);
            if (result.IsSuccess)
            {
                return result;
            }

            if (result.StatusCode is 429)
            {
                _maxRetries = Math.Min(5, _maxRetries + 1);
                await Task.Delay(TimeSpan.FromSeconds(attempt));
            }
        }

        throw new TimeoutException("Retries exhausted");
    }
}
```

## Pattern 38: Service Mesh Alignment

**Why it’s useful:** Align mesh policies with CurlDotNet commands so .NET developers honor mTLS and retry rules while still composing curl options.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed record MeshPolicy(bool EnforceMtls, bool AllowRetries);

public static class MeshAlignment
{
    public static async Task<CurlResult> ExecuteAsync(MeshPolicy policy, string command)
    {
        if (!policy.EnforceMtls)
        {
            throw new InvalidOperationException("Mesh requires mTLS");
        }

        return await Curl.ExecuteAsync(command + (policy.AllowRetries ? " --retry 3" : string.Empty));
    }
}
```

## Pattern 39: Business KPI Hooks

**Why it’s useful:** Tag CurlDotNet requests with business metadata in C# so product teams see which curl flows drive key KPIs.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed record BusinessRequest(string Feature, string Segment, string Command);

public static async Task<CurlResult> ExecuteWithKpisAsync(BusinessRequest request)
{
    var enrichedCommand = $"curl {request.Command} -H 'X-Feature: {request.Feature}' -H 'X-Segment: {request.Segment}'";
    return await Curl.ExecuteAsync(enrichedCommand);
}
```

## Pattern 40: Blue-Green CLI Deployments

**Why it’s useful:** Route CLI traffic between blue/green environments by choosing which CurlDotNet endpoint to ping, giving .NET tools safe rollouts.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public static async Task<CurlResult> ExecuteCliAsync(string version)
{
    var command = version switch
    {
        "blue" => "curl https://cli.example.com/v1/ping",
        "green" => "curl https://cli.example.com/v2/ping",
        _ => throw new ArgumentOutOfRangeException(nameof(version))
    };

    return await Curl.ExecuteAsync(command);
}
```

## Pattern 41: Developer Persona Dashboards

**Why it’s useful:** Fetch persona-specific dashboards via CurlDotNet and render summaries in C#, bringing curl-sourced telemetry to each team role.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Text.Json;
using CurlDotNet;

public static async Task RenderDashboardAsync(string persona)
{
    var response = await Curl.ExecuteAsync($"curl https://metrics.example.com/{persona}");
    var json = JsonDocument.Parse(response.Body);
    Console.WriteLine($"{persona} dashboard -> {json.RootElement.GetProperty("summary")}");
}
```

## Pattern 42: Chaos-Friendly Feature Flags

**Why it’s useful:** Provide safe fallbacks when feature flags fail by retrying CurlDotNet commands in C#, keeping curl-based experiments resilient.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public static async Task<CurlResult> ExecuteWithFallbackAsync(Func<bool> flagProvider, string controlCommand, string experimentCommand)
{
    try
    {
        var command = flagProvider() ? experimentCommand : controlCommand;
        return await Curl.ExecuteAsync(command);
    }
    catch
    {
        return await Curl.ExecuteAsync(controlCommand);
    }
}
```

## Pattern 43: Threat Detection Hooks

**Why it’s useful:** Hook security policies into CurlDotNet execution so suspicious curl commands are blocked before they leave your .NET app.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public static class DetectionHooks
{
    public static Func<string, bool>? CommandValidator { get; set; }
        = command => !command.Contains("--insecure", StringComparison.OrdinalIgnoreCase);

    public static Task<CurlResult> ExecuteAsync(string command)
    {
        if (CommandValidator is not null && !CommandValidator(command))
        {
            throw new InvalidOperationException("Command violates policy");
        }

        return Curl.ExecuteAsync(command);
    }
}
```

## Pattern 44: Education Through Storytelling

**Why it’s useful:** Turn CurlDotNet executions into teachable stories so C# teams can share real-world curl learnings during onboarding.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed record Story(string Title, string Command, string Lesson);

public sealed class StoryVault
{
    private readonly List<Story> _stories = new();

    public void Add(Story story) => _stories.Add(story);

    public async Task PlayAsync()
    {
        foreach (var story in _stories)
        {
            Console.WriteLine($"Story: {story.Title} - {story.Lesson}");
            await Curl.ExecuteAsync(story.Command);
        }
    }
}
```

## Pattern 45: Policy-as-Code Enforcement

**Why it’s useful:** Validate curl command strings in C# before passing them to CurlDotNet, enforcing policy-as-code across your .NET repos.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public static class CommandPolicy
{
    public static void Validate(string command)
    {
        if (command.Contains("--insecure", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Insecure TLS not allowed");
        }
    }
}
```

## Pattern 46: Knowledge Handoffs via Pull Requests

**Why it’s useful:** Generate pull-request templates that include curl commands so knowledge handoffs stay tied to executable CurlDotNet samples.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
public static class PullRequestTemplate
{
    public static string Create(string feature, string dataset, string curlCommand)
        => $"""
### Context
- Feature: {feature}
- Dataset: {dataset}

### Verification
```bash
{curlCommand}
```
""";
}
```

## Pattern 47: Domain-Specific Telemetry Taxonomy

**Why it’s useful:** Capture domain/feature metadata with each CurlDotNet result so telemetry across your .NET estate stays consistent.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public sealed record TelemetryEnvelope(string Domain, string Feature, CurlResult Result);

public static TelemetryEnvelope Capture(string domain, string feature, CurlResult result)
    => new(domain, feature, result);
```

## Pattern 48: Playwright/Browser Harness Bridges

**Why it’s useful:** Combine Playwright and CurlDotNet so UI tests and backend curl checks run in the same C# harness.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using Microsoft.Playwright;

public static async Task ValidateUiAndApiAsync()
{
    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();
    await page.GotoAsync("https://shop.example.com");

    var apiResponse = await Curl.ExecuteAsync("curl https://shop.example.com/api/products");
    Console.WriteLine(apiResponse.Body.Length);
}
```

## Pattern 49: Observability-Driven Alert Suppression

**Why it’s useful:** Check suppression conditions via CurlDotNet before paging humans so .NET alerting pipelines stay sane.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public static async Task<bool> ShouldAlertAsync(string suppressionCommand)
{
    var response = await Curl.ExecuteAsync(suppressionCommand);
    return !response.IsSuccess;
}
```

## Pattern 50: Retirement and Archiving Rituals

**Why it’s useful:** Log every retirement curl command from C# so service owners can prove when old workflows were archived.


```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

public static async Task RetireWorkflowAsync(string disableCommand, string archivePath)
{
    var response = await Curl.ExecuteAsync(disableCommand);
    await File.AppendAllTextAsync(archivePath, $"{DateTimeOffset.UtcNow:o}: {disableCommand} => {response.StatusCode}\n");
}
```
