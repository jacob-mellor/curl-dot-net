#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.ToStream\(\) Method

Returns the response as a new readable [System\.IO\.MemoryStream](https://learn.microsoft.com/en-us/dotnet/api/system.io.memorystream 'System\.IO\.MemoryStream'): the bytes of [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData'), or
[Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') encoded as UTF\-8, or an empty stream\.

```csharp
public System.IO.Stream ToStream();
```

#### Returns
[System\.IO\.Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream 'System\.IO\.Stream')  
A new [System\.IO\.MemoryStream](https://learn.microsoft.com/en-us/dotnet/api/system.io.memorystream 'System\.IO\.MemoryStream') over the response data \(never null\)\.

### Remarks

The data is already in memory; this does not stream from the network. Each call returns a new stream
            positioned at 0; dispose it when done. Useful for APIs that take a [System\.IO\.Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream 'System\.IO\.Stream'), such as
            `XDocument.Load`, `ZipArchive` or an image library.

Example: `using var zip = new System.IO.Compression.ZipArchive((await Curl.ExecuteAsync("curl -L https://example.com/files.zip")).ToStream());`

### See Also
- [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')
- [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')