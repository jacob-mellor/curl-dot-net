#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.IsUserForcedText\(HttpContent, CurlOptions\) Method

True when the user explicitly registered this response's media type as text
via [TextContentTypes](CurlDotNet.Core.CurlOptions.TextContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.TextContentTypes')\. Explicit user overrides beat
content sniffing\.

```csharp
private static bool IsUserForcedText(System.Net.Http.HttpContent content, CurlDotNet.Core.CurlOptions options);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.IsUserForcedText(System.Net.Http.HttpContent,CurlDotNet.Core.CurlOptions).content'></a>

`content` [System\.Net\.Http\.HttpContent](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcontent 'System\.Net\.Http\.HttpContent')

<a name='CurlDotNet.Core.HttpHandler.IsUserForcedText(System.Net.Http.HttpContent,CurlDotNet.Core.CurlOptions).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')