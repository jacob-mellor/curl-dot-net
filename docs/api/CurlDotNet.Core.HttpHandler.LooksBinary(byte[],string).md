#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.LooksBinary\(byte\[\], string\) Method

Sniffs a payload that headers classified as text for binary content, so a
mislabeled binary file is stored as bytes instead of being corrupted by
string decoding\.

```csharp
internal static bool LooksBinary(byte[] data, string charset=null);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.LooksBinary(byte[],string).data'></a>

`data` [System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

The response body bytes\.

<a name='CurlDotNet.Core.HttpHandler.LooksBinary(byte[],string).charset'></a>

`charset` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The declared charset, if any \(UTF\-16 text legitimately contains NUL bytes\)\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')