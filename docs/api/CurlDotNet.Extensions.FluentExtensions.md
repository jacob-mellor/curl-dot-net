#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions')

## FluentExtensions Class

Start a [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') from a URL string: `"https://api.example.com".ToCurl()`,
`.WithHeader(...)` or `.WithMethod("DELETE")`, then chain and `ExecuteAsync()`\.

```csharp
public static class FluentExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; FluentExtensions

### Remarks

Add `using CurlDotNet.Extensions;`. Each method returns a new builder; everything else (data, auth, timeouts)
            is set on that builder.

Example: `var r = await "https://api.example.com/items".WithHeader("Authorization", "Bearer " + token).WithTimeout(TimeSpan.FromSeconds(10)).ExecuteAsync();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md

### See Also
- [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')
- [StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

| Methods | |
| :--- | :--- |
| [ToCurl\(this string\)](CurlDotNet.Extensions.FluentExtensions.ToCurl(thisstring).md 'CurlDotNet\.Extensions\.FluentExtensions\.ToCurl\(this string\)') | Creates a GET [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') for this URL so you can chain headers, data and options before `ExecuteAsync()`\. |
| [WithHeader\(this string, string, string\)](CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).md 'CurlDotNet\.Extensions\.FluentExtensions\.WithHeader\(this string, string, string\)') | Creates a GET [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') for this URL with one header already set \(like `-H "key: value"`\), ready for more chaining\. |
| [WithMethod\(this string, string\)](CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).md 'CurlDotNet\.Extensions\.FluentExtensions\.WithMethod\(this string, string\)') | Creates a [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') for this URL with the HTTP method you name \(like `-X METHOD`\), such as DELETE, PUT, PATCH or OPTIONS\. |
