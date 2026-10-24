---
title: "How to Parse JSON from curl in C#"
description: "Parse JSON from curl in C#: call ParseJson<T>() on the CurlDotNet result for typed objects, or AsJsonDynamic() for ad-hoc access. Casing rules explained."
---

# How to parse JSON from curl in C#

To parse JSON from a curl response in C#, run the command with `Curl.ExecuteAsync` and call `result.ParseJson<T>()` to deserialize `result.Body` into your type. For quick, untyped access use `result.AsJsonDynamic()` or `ParseJson<JsonElement>()`.

```bash
dotnet add package CurlDotNet
```

## Typed: ParseJson<T>()

```csharp
using System.Text.Json.Serialization;
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/json");
var doc = result.ParseJson<SlideDoc>();
Console.WriteLine(doc.Slideshow.Title);  // Sample Slide Show

public record SlideDoc([property: JsonPropertyName("slideshow")] Slideshow Slideshow);
public record Slideshow(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("author")] string Author);
```

`AsJson<T>()` is an alias for `ParseJson<T>()`.

## Watch the property-name casing

Which serializer `ParseJson<T>()` uses depends on the build of CurlDotNet your app loads:

| Your app targets | Serializer | Property names |
|---|---|---|
| .NET 8 / 9 / 10, .NET Framework 4.7.2 / 4.8 | System.Text.Json, default options | **case-sensitive** |
| Anything that resolves the netstandard2.0 build (for example .NET 6 or 7) | Newtonsoft.Json | case-insensitive |

So on .NET 8, a C# property `Title` does **not** bind to JSON `"title"` unless you add `[JsonPropertyName]` as above, or pass options. The extension overload in the `CurlDotNet` namespace accepts `JsonSerializerOptions`:

```csharp
using System.Text.Json;
using CurlDotNet;

var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var result = await Curl.ExecuteAsync("curl https://httpbin.org/json");

var doc = result.ParseJson<SlideDoc>(options);
Console.WriteLine(doc.Slideshow.Title);

public record SlideDoc(Slideshow Slideshow);
public record Slideshow(string Title, string Author);
```

That overload also refuses to parse a failed response: it throws `InvalidOperationException` when `IsSuccess` is false.

## Untyped: JsonElement or AsJsonDynamic()

```csharp
using System.Text.Json;
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/json");
JsonElement root = result.ParseJson<JsonElement>();

Console.WriteLine(root.GetProperty("slideshow").GetProperty("title").GetString());
```

`AsJsonDynamic()` returns a `System.Text.Json.JsonDocument` on .NET 8+ and .NET Framework builds, and a Newtonsoft `JObject` on the netstandard2.0 build. Member access such as `json.slideshow.title` only works with the `JObject`. On .NET 8+, treat it as a `JsonDocument`:

```csharp
using System.Text.Json;
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/json");
using JsonDocument json = result.AsJsonDynamic();
Console.WriteLine(json.RootElement.GetProperty("slideshow").GetProperty("title"));
```

## The fluent builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/uuid")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();

var id = result.ParseJson<System.Text.Json.JsonElement>().GetProperty("uuid").GetString();
```

## When parsing fails

- Empty body: `InvalidOperationException` ("Body is empty"). Check `result.IsSuccess` first, because error responses are often empty or HTML.
- Invalid JSON or a type mismatch: `InvalidOperationException` that wraps the serializer's exception.
- Binary responses have an empty `Body`; check `result.IsBinary`.

## Save the JSON

`result.SaveAsJson("response.json")` writes the body pretty-printed (pass `indented: false` for compact output).

## Related

- [POST JSON](../curl-post-json-csharp/README.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [Read JSON in PowerShell with ConvertFrom-Json](../../powershell/run-curl-commands-powershell/README.md)
- [All how-to guides](../README.md)
