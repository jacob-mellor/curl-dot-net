#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.IsBinary Property


<b>Is this binary data? (images, PDFs, etc.)</b>

Quick check before accessing BinaryData:

```csharp
if (result.IsBinary)
    File.WriteAllBytes("file.bin", result.BinaryData);
else
    File.WriteAllText("file.txt", result.Body);
```

```csharp
public bool IsBinary { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')