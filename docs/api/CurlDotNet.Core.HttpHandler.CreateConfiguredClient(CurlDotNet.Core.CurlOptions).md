#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.CreateConfiguredClient\(CurlOptions\) Method

Builds an HttpClient configured with the command's proxy and TLS options so
that `--proxy`, `--socks5`, `--proxy-user` and `-k` are honored\.

```csharp
private static System.Net.Http.HttpClient CreateConfiguredClient(CurlDotNet.Core.CurlOptions options);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.CreateConfiguredClient(CurlDotNet.Core.CurlOptions).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

#### Returns
[System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient')