#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.BuildIncludedHeaderText\(HttpResponseMessage, IReadOnlyList\<string\>\) Method

The complete header text curl \-i emits before the body: every redirect hop's block
\(with \-L\), then the final block, each followed by an empty line\.

```csharp
private string BuildIncludedHeaderText(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyList<string>? redirectHeaderBlocks);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.BuildIncludedHeaderText(System.Net.Http.HttpResponseMessage,System.Collections.Generic.IReadOnlyList_string_).response'></a>

`response` [System\.Net\.Http\.HttpResponseMessage](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage 'System\.Net\.Http\.HttpResponseMessage')

<a name='CurlDotNet.Core.HttpHandler.BuildIncludedHeaderText(System.Net.Http.HttpResponseMessage,System.Collections.Generic.IReadOnlyList_string_).redirectHeaderBlocks'></a>

`redirectHeaderBlocks` [System\.Collections\.Generic\.IReadOnlyList&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')