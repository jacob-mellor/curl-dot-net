# CurlDotNet Tutorial Implementations

Every tutorial below is a direct lift from the dev.to skyscraper article and now includes runnable C# code. Install CurlDotNet before trying them:

```bash
dotnet add package CurlDotNet
```

All projects target .NET 8/10.

## Tutorial 1: Self-Service API Explorer

**Why it matters:** Build an ASP.NET Core explorer so C# developers can store and execute CurlDotNet commands without leaving .NET tooling.


Create an ASP.NET Core Minimal API that lets engineers run curated CurlDotNet templates.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ApiExplorerStore>();
var app = builder.Build();

app.MapPost("/requests", async (ApiExplorerStore store, RequestTemplate template) =>
{
    store.Add(template);
    return Results.Created($"/requests/{template.Id}", template);
});

app.MapGet("/requests/{id}", (ApiExplorerStore store, Guid id)
    => store.TryGet(id, out var template) ? Results.Ok(template) : Results.NotFound());

app.MapPost("/requests/{id}/execute", async (ApiExplorerStore store, Guid id) =>
{
    if (!store.TryGet(id, out var template))
    {
        return Results.NotFound();
    }

    var result = await Curl.ExecuteAsync(template.Command);
    store.LogExecution(id, result);
    return Results.Json(new { result.StatusCode, result.Body });
});

app.Run();

public sealed record RequestTemplate(Guid Id, string Name, string Command, string Description);

public sealed class ApiExplorerStore
{
    private readonly Dictionary<Guid, RequestTemplate> _templates = new();
    private readonly List<string> _logs = new();

    public void Add(RequestTemplate template) => _templates[template.Id] = template;
    public bool TryGet(Guid id, out RequestTemplate template) => _templates.TryGetValue(id, out template!);
    public void LogExecution(Guid id, CurlResult result)
        => _logs.Add($"{DateTimeOffset.UtcNow:o}|{id}|{result.StatusCode}");
}
```

## Tutorial 2: Disaster Recovery Runbook CLI

**Why it matters:** Wrap disaster-recovery curl workflows in a System.CommandLine CLI so operations teams run them from managed C# code.


Package critical workflows inside a `System.CommandLine` app so operators can execute recovery steps safely.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.CommandLine;
using CurlDotNet;

var environmentOption = new Option<string>("--environment", () => "production");
var dryRunOption = new Option<bool>("--dry-run", () => false);
var runbook = new RootCommand("DR runbook powered by CurlDotNet");

runbook.SetHandler(async (string environment, bool dryRun) =>
{
    var command = $"curl https://{environment}.example.com/api/restore -X POST";
    if (dryRun)
    {
        Console.WriteLine($"Would execute: {command}");
        return;
    }

    var result = await Curl.ExecuteAsync(command);
    Console.WriteLine($"Restore status {result.StatusCode}");
}, environmentOption, dryRunOption);

await runbook.InvokeAsync(args);
```

## Tutorial 3: Compliance Evidence Generator

**Why it matters:** Generate evidence bundles inside .NET by executing CurlDotNet commands and persisting their responses for auditors.


Generate immutable bundles proving regulated workflows were run through CurlDotNet.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Text.Json;
using CurlDotNet;

var evidence = new EvidenceGenerator("evidence");
await evidence.RunAsync("delete-user", "curl -X DELETE https://api.example.com/users/42");

public sealed class EvidenceGenerator
{
    private readonly string _directory;

    public EvidenceGenerator(string directory)
    {
        Directory.CreateDirectory(directory);
        _directory = directory;
    }

    public async Task RunAsync(string workflow, string command)
    {
        var result = await Curl.ExecuteAsync(command);
        var bundle = new
        {
            Workflow = workflow,
            Timestamp = DateTimeOffset.UtcNow,
            result.StatusCode,
            result.Headers,
            result.Body
        };

        var path = Path.Combine(_directory, $"{workflow}-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.json");
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(bundle, new JsonSerializerOptions { WriteIndented = true }));
    }
}
```

## Tutorial 4: Migrating Bash Scripts to CurlDotNet

**Why it matters:** Port legacy bash scripts to a modern C# console app that calls CurlDotNet instead of shelling out to curl.


Wrap legacy curl calls in a single C# console application.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

string[] commands =
[
    "curl https://api.legacy.com/login -d 'user=svc&password=secret'",
    "curl https://api.legacy.com/data -H 'Accept: application/json'",
    "curl https://api.legacy.com/logout"
];

foreach (var command in commands)
{
    var result = await Curl.ExecuteAsync(command);
    Console.WriteLine($"{command} => {result.StatusCode}");
}
```

## Tutorial 5: Full-Fidelity Mock Server

**Why it matters:** Host a realistic mock API in ASP.NET Core and exercise it with CurlDotNet to keep integration tests all within .NET.


Use ASP.NET Core to emulate partner APIs while verifying behavior with CurlDotNet.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/orders", () => Results.Json(new { items = new[] { new { id = 1, total = 99 } } }));
app.MapPost("/orders", (Order payload) => Results.Created($"/orders/{payload.Id}", payload));

await app.StartAsync();
var listResult = await Curl.ExecuteAsync("curl http://localhost:5000/orders");
Console.WriteLine(listResult.Body);
await app.StopAsync();

public sealed record Order(int Id, decimal Total);
```

## Tutorial 6: CurlDotNet + Message Buses

**Why it matters:** Publish CurlDotNet outcomes to Service Bus so distributed .NET systems can react to curl-like activity.


Publish events whenever CurlDotNet completes a request.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Text.Json;
using CurlDotNet;
using Azure.Messaging.ServiceBus;

var connectionString = Environment.GetEnvironmentVariable("SERVICEBUS")
    ?? throw new InvalidOperationException("SERVICEBUS connection string missing");
var client = new ServiceBusClient(connectionString);
var sender = client.CreateSender("curl-events");

var result = await Curl.ExecuteAsync("curl https://api.example.com/payments");
var message = new ServiceBusMessage(JsonSerializer.Serialize(new
{
    Command = "payments",
    result.StatusCode,
    Timestamp = DateTimeOffset.UtcNow
}));
await sender.SendMessageAsync(message);
```

## Tutorial 7: Observability-First Mobile Backend

**Why it matters:** Expose a mobile-friendly backend endpoint that enriches CurlDotNet calls with device metadata, keeping the entire flow in C#.


Backend service that enriches CurlDotNet calls with mobile metadata.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/mobile/proxy", async (MobileRequest request) =>
{
    var command = $"curl {request.UpstreamUrl} -H 'X-App-Version: {request.AppVersion}' -H 'X-Platform: {request.Platform}'";
    var result = await Curl.ExecuteAsync(command);
    return Results.Json(new { result.StatusCode, result.Body });
});

app.Run();

public sealed record MobileRequest(string UpstreamUrl, string AppVersion, string Platform);
```

## Tutorial 8: Multi-Cloud Disaster Recovery Bridge

**Why it matters:** Deploy CurlDotNet bridges across clouds using lightweight C# workers so failover scripts never rely on ad-hoc curl binaries.


Deploy the same bridge in multiple clouds; configuration shown here for a console worker.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

var targets = new[]
{
    "curl https://azure-region.example.com/bridge",
    "curl https://aws-region.example.com/bridge"
};

foreach (var target in targets)
{
    var response = await Curl.ExecuteAsync(target);
    Console.WriteLine($"{target} => {response.StatusCode}");
}
```

## Tutorial 9: Streaming Analytics Pipeline

**Why it matters:** Ingest streaming responses through CurlDotNet and channel them via modern C# concurrency primitives for analytics.


Read server-sent events and push them into `System.Threading.Channels`.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Threading.Channels;
using CurlDotNet;

var channel = Channel.CreateUnbounded<string>();
var streamingTask = Task.Run(async () =>
{
    var result = await Curl.ExecuteAsync("curl https://stream.example.com/logs");
    foreach (var line in result.Body.Split('\n', StringSplitOptions.RemoveEmptyEntries))
    {
        await channel.Writer.WriteAsync(line);
    }
    channel.Writer.Complete();
});

await foreach (var line in channel.Reader.ReadAllAsync())
{
    Console.WriteLine(line);
}

await streamingTask;
```

## Tutorial 10: Customer Support Troubleshooting Toolkit

**Why it matters:** Give support agents a safe C# console that runs approved CurlDotNet commands rather than raw curl on their laptops.


Build a trimmed-down desktop console that executes pre-approved CurlDotNet commands.

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using System.Text.Json;
using CurlDotNet;

var recipes = new Dictionary<int, string>
{
    [1] = "curl https://api.example.com/accounts/lookup?email=support@example.com",
    [2] = "curl https://api.example.com/orders/latest"
};

Console.WriteLine("Select action: 1) Lookup account 2) Latest order");
var choice = int.Parse(Console.ReadLine() ?? "1");
var command = recipes[choice];
var result = await Curl.ExecuteAsync(command);
Console.WriteLine(JsonSerializer.Serialize(new { result.StatusCode, result.Body }, new JsonSerializerOptions { WriteIndented = true }));
```
