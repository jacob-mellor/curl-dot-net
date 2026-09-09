#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.BinaryData Property


<b>Binary data for files like images, PDFs, downloads.</b>

When you download non-text files, the bytes are here:

```csharp
// Download an image
var result = await Curl.Execute("curl https://example.com/logo.png");

if (result.IsBinary)
{
    File.WriteAllBytes("logo.png", result.BinaryData);
    Console.WriteLine($"Saved {result.BinaryData.Length} bytes");
}
```

```csharp
public byte[] BinaryData { get; set; }
```

#### Property Value
[System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')