#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.IsTextContent\(HttpContent, CurlOptions\) Method

Determines whether the HTTP content should be treated as text or binary\.
Uses precise MIME type matching, a comprehensive known\-binary registry,
and user\-configurable overrides to avoid corrupting binary data\.

```csharp
private bool IsTextContent(System.Net.Http.HttpContent content, CurlDotNet.Core.CurlOptions options=null);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.IsTextContent(System.Net.Http.HttpContent,CurlDotNet.Core.CurlOptions).content'></a>

`content` [System\.Net\.Http\.HttpContent](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcontent 'System\.Net\.Http\.HttpContent')

The HTTP response content to inspect\.

<a name='CurlDotNet.Core.HttpHandler.IsTextContent(System.Net.Http.HttpContent,CurlDotNet.Core.CurlOptions).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

The curl options that may contain user overrides for content type detection\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
`true` if the content should be read as text; `false` for binary\.