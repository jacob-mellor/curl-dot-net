---
title: "Run curl Commands in CI/CD with .NET - CurlDotNet"
description: "How to run curl commands in CI/CD with .NET using CurlDotNet: GitHub Actions, Azure DevOps and Docker smoke tests with timeouts, retries and secrets."
---

# Run curl Commands in CI/CD with .NET (GitHub Actions, Azure DevOps, Docker)

This article shows how to run curl commands in CI/CD with .NET: the same `curl ...` strings you use locally run inside GitHub Actions, Azure DevOps or a container through `Curl.ExecuteAsync`, with typed results you can assert on.

## Why Use CurlDotNet in Pipelines?

- **Consistency** - the same curl string runs locally and in CI with no shell quoting differences
- **Typed results** - check `StatusCode`, parse JSON with `ParseJson<T>()`, fail the build with `EnsureSuccess()`
- **Secrets from the runner** - `$API_TOKEN`, `%API_TOKEN%` and `$env:API_TOKEN` inside the command string are expanded from environment variables
- **Cross-platform** - behaves the same on Linux, macOS and Windows runners

## GitHub Actions Example

{% raw %}

```yaml
name: Api Smoke Tests
on:
  push:
    branches: [ master ]
jobs:
  smoke:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - name: Run smoke tests
        env:
          API_TOKEN: ${{ secrets.API_TOKEN }}
        run: dotnet run --project tools/SmokeTests/SmokeTests.csproj
```
{% endraw %}

Inside the smoke-test console app:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl https://httpbin.org/bearer -H 'Authorization: Bearer $API_TOKEN'");

result.EnsureSuccess(); // throws CurlDotNet.Core.CurlHttpException on 4xx/5xx
Console.WriteLine($"Health check passed: {result.StatusCode}");
```

`$API_TOKEN` is read from the process environment by the command parser, so the secret never appears in your source code.

## Azure DevOps Example

```yaml
steps:
- task: UseDotNet@2
  inputs:
    packageType: 'sdk'
    version: '8.0.x'

- script: dotnet test tests/SmokeTests/SmokeTests.csproj --filter Category=Smoke
  env:
    API_TOKEN: $(apiToken)
```

Use pipeline variables or Azure Key Vault secrets for sensitive data and reference them as `$API_TOKEN` in the curl string. CurlDotNet expands `$NAME`, `${NAME}` and `%NAME%` from the environment outside single quotes, as a shell would, so write `-H "Authorization: Bearer $API_TOKEN"` with double quotes.

## Containerized Smoke Tests

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish tests/SmokeTests/SmokeTests.csproj -c Release -o /app/tests

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/tests .
ENTRYPOINT ["dotnet", "SmokeTests.dll"]
```

Run the image from any orchestrator to confirm an API behaves before or after a deployment.

## Tips for Stable Pipelines

### Timeouts and retries

There is no timeout unless you set one, so always put `--max-time` in pipeline commands; it bounds the whole transfer, body included. Retry transient failures with curl's own `--retry`, which retries timeouts and HTTP 408/429/500/502/503/504 with doubling back-off and honours `Retry-After`:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl --max-time 10 --retry 3 --retry-max-time 60 https://httpbin.org/status/200");
Console.WriteLine(result.StatusCode);
```

Add `--retry-connrefused` while a freshly deployed service is still starting, or use `new CurlSettings().WithRetries(3, 500)` to retry any failure. See [curl --retry in C#](../cookbook/retry.html).

A request that exceeds `--max-time` throws `CurlDotNet.Exceptions.CurlOperationTimeoutException` (exit code 28); a DNS failure throws `CurlCouldntResolveHostException` (6) and a refused connection `CurlCouldntConnectException` (7). `ex.CurlErrorCode` is the exit code curl would have returned, which makes a convenient process exit code; see [curl exit codes](../reference/curl-exit-codes.html).

### Fail fast on HTTP errors

HTTP 4xx/5xx responses do not throw by default. Add `-f` (or `--fail-with-body` to keep the response body) to make them throw `CurlHttpReturnedErrorException` with exit code 22, exactly like `curl -f` in a shell script, or check the result and print the body when it fails:

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -f --max-time 10 https://httpbin.org/status/503");
}
catch (CurlException ex)
{
    Console.Error.WriteLine(ex.Message);
    Environment.Exit(ex.CurlErrorCode ?? 1);   // 22, as curl -f would exit
}
```

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/503");
if (!result.IsSuccess)
{
    Console.Error.WriteLine($"HTTP {result.StatusCode}: {result.Body}");
    Environment.Exit(1);
}
```

### Other tips

- Mask secrets in console output; log the command with the token replaced before printing it.
- Store the curl strings in source control (for example under `tests/Smoke/`) so the same file is used locally and in CI.
- Validate commands before running them with `Curl.Validate(command).IsValid` to catch typos early.

## Next Steps

- [Handle Errors](../cookbook/beginner/handle-errors.html) - exception types and status-code handling
- [Logging and Observability](../guides/logging-observability.html) - capture verbose output in CI logs
- [Framework Compatibility](../framework-compatibility.html) - which runtimes the package targets
