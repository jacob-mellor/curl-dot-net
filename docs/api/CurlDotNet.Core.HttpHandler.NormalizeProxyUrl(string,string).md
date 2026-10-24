#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.NormalizeProxyUrl\(string, string\) Method

Prepends a default scheme to a proxy address when the user supplied a bare
host:port \(curl allows `proxy.example.com:8080`\), since WebProxy requires a scheme\.

```csharp
private static string NormalizeProxyUrl(string proxy, string defaultScheme);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.NormalizeProxyUrl(string,string).proxy'></a>

`proxy` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.HttpHandler.NormalizeProxyUrl(string,string).defaultScheme'></a>

`defaultScheme` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')