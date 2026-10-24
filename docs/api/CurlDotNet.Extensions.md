#### [CurlDotNet](index.md 'index')

## CurlDotNet\.Extensions Namespace

| Classes | |
| :--- | :--- |
| [FluentExtensions](CurlDotNet.Extensions.FluentExtensions.md 'CurlDotNet\.Extensions\.FluentExtensions') | Start a [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') from a URL string: `"https://api.example.com".ToCurl()`, `.WithHeader(...)` or `.WithMethod("DELETE")`, then chain and `ExecuteAsync()`\. |
| [StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions') | Run curl straight from a string: `await "curl https://api.github.com".CurlAsync()`, `await "https://example.com".CurlBodyAsync()`, `"curl ...".Curl()` for synchronous code\. |
