#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.NeedsCustomTransport\(CurlOptions\) Method

Determines whether the request needs a dedicated HttpClient because it
specifies transport\-level options \(explicit proxy or relaxed TLS\) that the
shared pooled client does not carry\.

```csharp
private static bool NeedsCustomTransport(CurlDotNet.Core.CurlOptions options);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.NeedsCustomTransport(CurlDotNet.Core.CurlOptions).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')