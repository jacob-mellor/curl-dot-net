#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.DescribeProxy\(CurlOptions, Uri\) Method

Produces a human\-readable description of the proxy that will be used for the
request \(explicit, SOCKS5, system, or none\) for inclusion in the trace\.

```csharp
private static string DescribeProxy(CurlDotNet.Core.CurlOptions options, System.Uri uri);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.DescribeProxy(CurlDotNet.Core.CurlOptions,System.Uri).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

<a name='CurlDotNet.Core.HttpHandler.DescribeProxy(CurlDotNet.Core.CurlOptions,System.Uri).uri'></a>

`uri` [System\.Uri](https://learn.microsoft.com/en-us/dotnet/api/system.uri 'System\.Uri')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')