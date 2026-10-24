#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.DecodeTextBody\(byte\[\], string\) Method

Decodes a text body from its raw bytes honoring the declared charset and any
byte\-order mark, falling back to UTF\-8\.

```csharp
private static string DecodeTextBody(byte[] data, string charset);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.DecodeTextBody(byte[],string).data'></a>

`data` [System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

<a name='CurlDotNet.Core.HttpHandler.DecodeTextBody(byte[],string).charset'></a>

`charset` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')