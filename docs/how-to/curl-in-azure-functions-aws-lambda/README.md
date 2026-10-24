---
title: "How to Use curl in Azure Functions and AWS Lambda (C#)"
description: "Use curl in Azure Functions and AWS Lambda with C#: run curl commands through CurlDotNet in serverless code, no curl binary in the image."
---

# How to use curl in Azure Functions and AWS Lambda

To use curl in Azure Functions or AWS Lambda with C#, add the `CurlDotNet` package to the function project and call `Curl.ExecuteAsync` (or `CurlRequestBuilder`) from your handler. It is a managed library, so there is no curl binary to bundle or install and nothing to start with `Process.Start`.

```bash
dotnet add package CurlDotNet
```

## Azure Functions (isolated worker)

```csharp
using CurlDotNet;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public class ProxyStatus
{
    [Function("ProxyStatus")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/json");
        var response = req.CreateResponse((System.Net.HttpStatusCode)result.StatusCode);
        await response.WriteStringAsync(result.Body ?? ""); return response;
    }
}
```

A timer-triggered health check is a common use: paste the curl command your on-call runbook already uses.

## AWS Lambda

```csharp
using Amazon.Lambda.Core;
using CurlDotNet.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

public class Function
{
    public async Task<int> Handler(string url, ILambdaContext context)
    {
        var result = await CurlRequestBuilder.Get(url)
            .WithTimeout(TimeSpan.FromSeconds(10)).ExecuteAsync();
        context.Logger.LogLine($"{url} -> {result.StatusCode}");
        return result.StatusCode;
    }
}
```

## Calling AWS APIs with SigV4

`--aws-sigv4` signs the request like the curl CLI does, which is handy for API Gateway endpoints with IAM auth:

```csharp
using CurlDotNet;

var key = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
var secret = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

var result = await Curl.ExecuteAsync(
    $"curl --aws-sigv4 'aws:amz:us-east-1:execute-api' -u '{key}:{secret}' " +
    "https://abc123.execute-api.us-east-1.amazonaws.com/prod/items");
```

Inside Lambda, the role's credentials also include a session token (`AWS_SESSION_TOKEN`) that AWS expects in the `x-amz-security-token` header. CurlDotNet signs every request header, so add it with `-H "x-amz-security-token: $AWS_SESSION_TOKEN"`. The SigV4 examples on this page have not been run against a live AWS endpoint, so test against yours.

## Serverless checklist

- **Reuse across invocations.** The static `Curl` class keeps one `HttpClient` for the life of the process, so warm invocations reuse connections. Do not create and dispose a `LibCurl` on every invocation; keep it in a static field.
- **Timeouts.** `-m` (default 30 seconds) limits the wait for the response headers, not the body download. Keep `-m` plus the expected download time below the function's own timeout so you can return a clean error instead of being killed.
- **Writable paths.** Lambda only allows writes under `/tmp`. On Azure Functions use `Path.GetTempPath()`. Build `-o` paths from those.
- **Secrets.** Read tokens from configuration or Key Vault / Secrets Manager, not from the command literal. The parser expands `$NAME` environment variables inside commands.
- **Cold start.** CurlDotNet adds one managed assembly; it has no native dependencies to load.

## Related

- [Use CurlDotNet in ASP.NET Core](../curl-in-aspnet-core/README.md)
- [Timeouts](../curl-timeout-csharp/README.md)
- [Bearer token authentication](../curl-bearer-token-csharp/README.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [All how-to guides](../README.md)
