# CI/CD Integration with CurlDotNet

Bring the copy-paste power of curl into your automation pipelines. This article walks through practical ways to run CurlDotNet inside GitHub Actions, Azure DevOps, and other CI/CD systems.

## Why Use CurlDotNet in Pipelines?

- **Consistency** – run the same curl snippets locally and in CI without shell differences
- **Strong Typing & Validation** – parse responses with .NET types before deploying
- **Secure Secrets Handling** – consume secrets from CI vaults instead of embedding tokens in scripts
- **Cross-Platform** – works identically on Linux, macOS, and Windows runners

## GitHub Actions Example

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
      - name: Restore tools
        run: dotnet restore

      - name: Run CurlDotNet smoke tests
        env:
          API_TOKEN: ${{ secrets.API_TOKEN }}
        run: dotnet run --project examples/00-Complete-Sample/00-Complete-Sample.csproj
```

Inside the sample app:

```csharp
var response = await Curl.ExecuteAsync($@"
    curl -X GET https://api.example.com/health
    -H 'Authorization: Bearer {Environment.GetEnvironmentVariable("API_TOKEN")}'
");

response.EnsureSuccess();
Console.WriteLine("Health endpoint passed CI check");
```

## Azure DevOps Example

```yaml
steps:
- task: UseDotNet@2
  inputs:
    packageType: 'sdk'
    version: '8.0.x'

- script: dotnet test tests/CurlDotNet.IntegrationTests/CurlDotNet.IntegrationTests.csproj
  env:
    API_TOKEN: $(apiToken)
```

Use pipeline variables or Azure Key Vault secrets for sensitive data. CurlDotNet automatically reads environment variables via `--header "Authorization: Bearer $API_TOKEN"` just like native curl.

## Containerized Smoke Tests

Use the DotNet CLI to run a portable test harness:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet publish tests/CurlDotNet.Tests/CurlDotNet.Tests.csproj -c Release -o /app/tests

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/tests .

ENTRYPOINT ["dotnet", "CurlDotNet.Tests.dll", "--filter", "Category=Smoke"]
```

Run it from any orchestrator to confirm APIs behave before or after deployments.

## Tips for Stable Pipelines

1. **Use `--connect-timeout` + retries** to handle transient network blips:
   ```csharp
   var settings = new CurlSettings()
       .WithConnectTimeout(5)
       .WithRetries(3, delayMs: 500);
   await Curl.ExecuteAsync("curl --max-time 10 https://api.example.com/status", settings);
   ```
2. **Mask secrets** in console output. Prefer `curl --header "Authorization: Bearer ****"` when logging commands.
3. **Store sample requests in source control** (e.g., under `tests/Smoke/`) to reuse locally and remotely.
4. **Fail Fast** by calling `result.EnsureSuccess()` and surfacing the body for debugging.

## Next Steps

- Add targeted smoke or contract tests under `tests/` so pipelines can run `dotnet test --filter Category=Smoke`.
- Combine CurlDotNet with approval gates to validate external dependencies before promoting builds.
- Explore [Logging & Observability](../guides/logging-observability.md) to capture verbose output automatically in CI logs.
