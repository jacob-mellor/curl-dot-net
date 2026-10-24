---
title: "Call a REST API with curl in C# - CurlDotNet Recipe"
description: "Call a REST API with curl in C# using CurlDotNet: GET/POST/PUT/PATCH/DELETE via -X, bearer tokens, the builder, a reusable LibCurl client and ParseJson."
---

# Call a REST API with curl in C#

To call a REST API with curl in C#, hand CurlDotNet the curl command you already know (`curl -X POST ... -H 'Authorization: Bearer ...'`) and read the typed result. This recipe walks through GET, POST, PUT, PATCH and DELETE with `-X`, sending a bearer token, the fluent builder, a reusable `LibCurl` client that carries the token for you, and turning the JSON reply into a C# object with `EnsureSuccess()` and `ParseJson<T>()`.

## The problem

You have an API's documentation written in curl examples and want to call the same endpoints from .NET without rewriting each one for `HttpClient`.

## Solution 1: run the curl command string

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl https://api.github.com/repos/curl/curl -A CurlDotNet-Recipe");

Console.WriteLine(result.StatusCode);   // 200
Console.WriteLine(result.Body);         // JSON describing the repository
```

HTTP error statuses do not throw; a 404 comes back as a result with `IsSuccess == false` and `StatusCode == 404`.

### Every method via -X

```csharp
var created = await Curl.ExecuteAsync(@"
    curl -X POST https://httpbin.org/post --json '{""name"": ""widget""}'");

var replaced = await Curl.ExecuteAsync(@"
    curl -X PUT https://httpbin.org/put --json '{""name"": ""widget"", ""qty"": 2}'");

var patched = await Curl.ExecuteAsync(@"
    curl -X PATCH https://httpbin.org/patch --json '{""qty"": 3}'");

var deleted = await Curl.ExecuteAsync("curl -X DELETE https://httpbin.org/delete");

Console.WriteLine($"{created.StatusCode} {replaced.StatusCode} {patched.StatusCode} {deleted.StatusCode}");
```

`--json` sets `Content-Type` and `Accept` to `application/json`; `-H 'Content-Type: application/json' -d '{...}'` is the long form. See [HTTP methods](../http-methods.html).

### Send a bearer token

Keep secrets out of source: read the token from an environment variable and interpolate it.

```csharp
string token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

var result = await Curl.ExecuteAsync($@"
    curl https://api.github.com/user \
      -H 'Authorization: Bearer {token}' \
      -H 'Accept: application/vnd.github+json' \
      -A 'CurlDotNet-Recipe'");

Console.WriteLine(result.StatusCode);   // 200, or 401 if the token is wrong
```

## Solution 2: the fluent builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithBearerToken(token)
    .WithJson(new { name = "widget", qty = 2 })
    .WithTimeout(TimeSpan.FromSeconds(15))
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

`CurlRequestBuilder.Get/Post/Put/Patch/Delete/Head(url)` pick the method; `Request("OPTIONS", url)` covers anything else. `.ToCurlCommand()` returns the curl string the builder will execute.

## Solution 3: a reusable client with LibCurl

When one API is called many times, create a `LibCurl` instance once with the headers, token and timeout, and reuse it.

```csharp
using CurlDotNet.Lib;

using var api = new LibCurl()
    .WithBearerToken(token)
    .WithHeader("Accept", "application/vnd.github+json")
    .WithUserAgent("CurlDotNet-Recipe")
    .WithTimeout(TimeSpan.FromSeconds(15));

var me = await api.GetAsync("https://api.github.com/user");
var starred = await api.GetAsync("https://api.github.com/user/starred");

Console.WriteLine($"{me.StatusCode} {starred.StatusCode}");
```

`LibCurl` also has `PostAsync(url, data)`, `PutAsync`, `PatchAsync`, `DeleteAsync` and `HeadAsync`; an object passed as `data` is serialized to JSON.

## Read the reply as a C# object

```csharp
using System.Text.Json.Serialization;

public class Repo
{
    [JsonPropertyName("full_name")] public string FullName { get; set; }
    [JsonPropertyName("stargazers_count")] public int Stars { get; set; }
}

var result = await Curl.ExecuteAsync("curl https://api.github.com/repos/curl/curl -A CurlDotNet");
var repo = result.EnsureSuccess().ParseJson<Repo>();

Console.WriteLine($"{repo.FullName} has {repo.Stars} stars");
```

`EnsureSuccess()` returns the same result on 2xx and throws `CurlDotNet.Core.CurlHttpException` (with `StatusCode`, `ResponseBody`, `ResponseHeaders`) otherwise, so you can chain straight into `ParseJson<T>()`. Property names must match the JSON unless you add `[JsonPropertyName]`; a `List<Repo>` works for array replies.

### Handle failures

```csharp
using CurlDotNet.Exceptions;

try
{
    var repo = (await Curl.ExecuteAsync("curl -m 10 https://api.github.com/repos/nope/nope -A X"))
        .EnsureSuccess().ParseJson<Repo>();
}
catch (CurlDotNet.Core.CurlHttpException ex)   // 4xx / 5xx
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.ResponseBody}");
}
catch (CurlOperationTimeoutException) { Console.WriteLine("timed out"); }
catch (CurlCouldntConnectException ex) { Console.WriteLine($"cannot reach {ex.Host}"); }
```

`CurlHttpException` exists in both `CurlDotNet.Core` and `CurlDotNet.Exceptions`; qualify it as above when both namespaces are imported.

## Variations

### API key in a header or query string

```csharp
var byHeader = await Curl.ExecuteAsync(
    "curl https://httpbin.org/get -H 'X-Api-Key: YOUR_KEY'");

string key = Uri.EscapeDataString("YOUR_KEY");
var byQuery = await Curl.ExecuteAsync($"curl 'https://httpbin.org/get?api_key={key}'");
```

### Basic auth (-u)

```csharp
var result = await Curl.ExecuteAsync("curl -u 'user:passwd' https://httpbin.org/basic-auth/user/passwd");
Console.WriteLine(result.StatusCode);   // 200
```

Digest, NTLM and other schemes are covered in [Authentication schemes](../auth-schemes.html).

### Several calls at once

```csharp
var results = await Curl.ExecuteManyAsync(
    "curl https://api.github.com/repos/curl/curl -A X",
    "curl https://api.github.com/repos/dotnet/runtime -A X");

foreach (var r in results)
    Console.WriteLine(r.StatusCode);
```

## Troubleshooting

**401 Unauthorized.** The token is missing, expired, or in the wrong header. Confirm it is `Authorization: Bearer <token>` (some APIs use `Token <token>` or `X-Api-Key`) and that the environment variable is actually set in the process that runs your app.

**403 Forbidden from api.github.com.** GitHub requires a `User-Agent`; CurlDotNet does not add one, so pass `-A` or `.WithUserAgent(...)`. A 403 with a `Retry-After` header means you are rate-limited; read `result.GetHeader("Retry-After")` and wait.

**404 Not Found.** Check the path and any ids in the URL. `result.Body` normally carries the API's own message.

**429 Too Many Requests.** Slow down and honour `Retry-After`: read `result.GetHeader("Retry-After")`, wait that many seconds, then send the request again (see [Handle Errors](handle-errors.html) for a retry loop).

**`ParseJson<T>()` throws.** The body is empty (204, HEAD) or not JSON (an HTML error page). Call `EnsureSuccess()` first and inspect `result.Body` or `result.AsJsonDynamic()`.

**The reply is a redirect (301/302).** Add `-L` to follow it; see [Follow Redirects](../follow-redirects.html).

## Related

- [Simple GET](simple-get.html) - headers, query parameters, saving the body
- [Send JSON](send-json.html) - `curl -X POST` with a JSON body
- [POST Form Data](post-form.html) - URL-encoded forms
- [Handle Errors](handle-errors.html) - status codes versus exceptions
- [Authentication schemes](../auth-schemes.html) - basic, bearer, digest, NTLM
- [Timeouts](../timeouts.html) - `-m` and `--connect-timeout`
- [Tutorial: Authentication Basics](../../tutorials/09-authentication-basics.html)
- [Cookbook index](../index.html)
