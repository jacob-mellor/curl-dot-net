#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.DecodeContentEncoding\(byte\[\], ICollection\<string\>\) Method

Decodes gzip, deflate \(zlib or raw\) and, where the framework supports it, Brotli bodies
for \-\-compressed\. Unknown or identity encodings are returned untouched\.

```csharp
private static byte[] DecodeContentEncoding(byte[] body, System.Collections.Generic.ICollection<string> encodings);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.DecodeContentEncoding(byte[],System.Collections.Generic.ICollection_string_).body'></a>

`body` [System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

<a name='CurlDotNet.Core.HttpHandler.DecodeContentEncoding(byte[],System.Collections.Generic.ICollection_string_).encodings'></a>

`encodings` [System\.Collections\.Generic\.ICollection&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1 'System\.Collections\.Generic\.ICollection\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1 'System\.Collections\.Generic\.ICollection\`1')

#### Returns
[System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')