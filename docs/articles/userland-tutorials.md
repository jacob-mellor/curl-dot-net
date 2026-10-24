---
title: CurlDotNet Tutorial Projects - curl in C# Applications
description: Ten small C# projects built on CurlDotNet: minimal APIs, CLIs, mock servers, evidence generators and workers that run curl commands in .NET.
---

# CurlDotNet Tutorial Projects: Running curl Commands in Real C# Applications

These CurlDotNet tutorial projects show curl commands running inside real C# applications: an ASP.NET Core minimal API, a `System.CommandLine` tool, a mock server, a message-bus publisher and more. Each snippet is a starting point you can paste into a .NET 8 or later project.

Install the package first:

```bash
dotnet add package CurlDotNet
```

Every snippet assumes `using CurlDotNet;` (and `using CurlDotNet.Core;` where `CurlResult` is named). Results come back as `CurlResult` with `StatusCode`, `Headers`, `Body` and `IsSuccess`.

## Tutorial 1: Self-Service API Explorer

**Why it matters:** Store curated curl commands and let engineers run them from an ASP.NET Core endpoint without leaving .NET tooling.

```csharp
using CurlDotNet;

var app = WebApplication.CreateBuilder(args).Build();
var templates = new Dictionary<Guid, string>();
app.MapPost("/requests", (string command) =>
{
    var id = Guid.NewGuid(); templates[id] = command;
    return Results.Created($"/requests/{id}", new { id, command });
});
app.MapPost("/requests/{id}/execute", async (Guid id) =>
{
    var r = await Curl.ExecuteAsync(templates[id]);
    return Results.Json(new { r.StatusCode, r.Body });
});
app.Run();
```

Validate submitted commands with `Curl.Validate(command).IsValid` before storing them.

## Tutorial 2: Disaster Recovery Runbook CLI

**Why it matters:** Wrap recovery curl calls in a `System.CommandLine` app so operators run them from reviewed C# code, with a dry-run switch.

```csharp
using System.CommandLine;
using CurlDotNet;

var envOption = new Option<string>("--environment", () => "staging");
var dryRun = new Option<bool>("--dry-run", () => false);
var runbook = new RootCommand("DR runbook") { envOption, dryRun };
runbook.SetHandler(async (string env, bool dry) =>
{
    var command = $"curl -X POST https://{env}.example.com/api/restore";
    if (dry) { Console.WriteLine($"Would execute: {command}"); return; }
    var result = await Curl.ExecuteAsync(command);
    Console.WriteLine($"Restore status {result.StatusCode}");
}, envOption, dryRun);

await runbook.InvokeAsync(args);
```

## Tutorial 3: Compliance Evidence Generator

**Why it matters:** Persist each command and its response as JSON so auditors can see exactly which workflows ran.

```csharp
using System.Text.Json;
using CurlDotNet;

async Task CaptureAsync(string workflow, string command, string directory)
{
    Directory.CreateDirectory(directory);
    var result = await Curl.ExecuteAsync(command);
    var bundle = new { workflow, command, result.StatusCode, result.Headers, result.Body,
                       timestamp = DateTimeOffset.UtcNow };
    var path = Path.Combine(directory, $"{workflow}-{DateTime.UtcNow:yyyyMMddHHmmss}.json");
    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(bundle,
        new JsonSerializerOptions { WriteIndented = true }));
}

await CaptureAsync("delete-user", "curl -X DELETE https://httpbin.org/delete", "evidence");
```

## Tutorial 4: Migrating Bash Scripts to C#

**Why it matters:** Port a legacy bash script that shells out to curl into a console app; the curl strings stay exactly as they were.

```csharp
using CurlDotNet;

string[] commands =
[
    "curl https://httpbin.org/post -d 'user=svc&password=$SVC_PASSWORD'",
    "curl https://httpbin.org/get -H 'Accept: application/json'",
    "curl https://httpbin.org/status/204"
];

foreach (var command in commands)
{
    var result = await Curl.ExecuteAsync(command);
    Console.WriteLine($"{command} => {result.StatusCode}");
}
```

`$SVC_PASSWORD` is expanded from the environment by the parser, as it would be in bash.

## Tutorial 5: Full-Fidelity Mock Server

**Why it matters:** Host a mock partner API in ASP.NET Core and exercise it with the same curl commands your integration uses.

```csharp
using CurlDotNet;

var app = WebApplication.CreateBuilder(args).Build();
app.MapGet("/orders", () => Results.Json(new { items = new[] { new { id = 1, total = 99 } } }));
app.MapPost("/orders", (Order o) => Results.Created($"/orders/{o.Id}", o));

await app.StartAsync();
var list = await Curl.ExecuteAsync("curl http://localhost:5000/orders");
Console.WriteLine(list.Body);
await app.StopAsync();

public sealed record Order(int Id, decimal Total);
```

## Tutorial 6: Publishing Results to a Message Bus

**Why it matters:** Emit an event after each request so other services can react to API outcomes.

```csharp
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using CurlDotNet;

var client = new ServiceBusClient(Environment.GetEnvironmentVariable("SERVICEBUS")!);
var sender = client.CreateSender("curl-events");

var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
var payload = JsonSerializer.Serialize(new { Command = "payments", result.StatusCode,
                                             Timestamp = DateTimeOffset.UtcNow });
await sender.SendMessageAsync(new ServiceBusMessage(payload));
```

## Tutorial 7: Mobile Backend Proxy

**Why it matters:** Expose one endpoint that forwards a mobile client's request upstream with device metadata added as headers.

```csharp
using CurlDotNet;

var app = WebApplication.CreateBuilder(args).Build();

app.MapPost("/mobile/proxy", async (MobileRequest request) =>
{
    var command = $"curl {request.UpstreamUrl} -H 'X-App-Version: {request.AppVersion}'" +
                  $" -H 'X-Platform: {request.Platform}'";
    var result = await Curl.ExecuteAsync(command);
    return Results.Json(new { result.StatusCode, result.Body });
});

app.Run();

public sealed record MobileRequest(string UpstreamUrl, string AppVersion, string Platform);
```

Validate `UpstreamUrl` against an allow-list before interpolating it into a command.

## Tutorial 8: Multi-Cloud Health Bridge

**Why it matters:** Run the same health probes against several regions from one worker and fail over on the result.

```csharp
using CurlDotNet;

var results = await Curl.ExecuteManyAsync(
    "curl --max-time 5 https://azure-region.example.com/bridge",
    "curl --max-time 5 https://aws-region.example.com/bridge");

foreach (var r in results)
    Console.WriteLine($"{r.Command} => {r.StatusCode}");
```

## Tutorial 9: Line-Oriented Analytics Pipeline

**Why it matters:** Push each line of a response through `System.Threading.Channels` for downstream processing.

```csharp
using System.Threading.Channels;
using CurlDotNet;

var channel = Channel.CreateUnbounded<string>();
var producer = Task.Run(async () =>
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/stream/5");
    foreach (var line in result.Body.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        await channel.Writer.WriteAsync(line);
    channel.Writer.Complete();
});

await foreach (var line in channel.Reader.ReadAllAsync())
    Console.WriteLine(line);
await producer;
```

The whole response is read before lines are emitted; CurlDotNet does not stream partial bodies.

## Tutorial 10: Support Troubleshooting Console

**Why it matters:** Give support agents a console that runs pre-approved commands instead of raw curl on their laptops.

```csharp
using CurlDotNet;

var recipes = new Dictionary<int, string>
{
    [1] = "curl https://httpbin.org/get?lookup=account",
    [2] = "curl https://httpbin.org/uuid"
};

Console.WriteLine("Select action: 1) Lookup account 2) Latest order");
var choice = int.Parse(Console.ReadLine() ?? "1");
var result = await Curl.ExecuteAsync(recipes[choice]);
Console.WriteLine($"{result.StatusCode}\n{result.Body}");
```

## Related Pages

- [Implementation Patterns](userland-patterns.html)
- [CI/CD Integration](ci-cd-integration.html)
- [Cookbook](../cookbook/)
