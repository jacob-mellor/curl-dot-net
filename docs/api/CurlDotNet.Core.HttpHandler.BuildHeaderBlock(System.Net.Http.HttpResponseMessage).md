#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.BuildHeaderBlock\(HttpResponseMessage\) Method

Rebuilds the response header block the way curl \-i prints it: status line, one header per
line, CRLF line endings, no trailing blank line\.

```csharp
private string BuildHeaderBlock(System.Net.Http.HttpResponseMessage response);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.BuildHeaderBlock(System.Net.Http.HttpResponseMessage).response'></a>

`response` [System\.Net\.Http\.HttpResponseMessage](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage 'System\.Net\.Http\.HttpResponseMessage')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')