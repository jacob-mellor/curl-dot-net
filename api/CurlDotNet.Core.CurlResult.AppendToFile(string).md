#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.AppendToFile\(string\) Method


<b>Append response to an existing file.</b>

Add to a file without overwriting:

```csharp
// Log all responses
result.AppendToFile("api-log.txt");

// Build up a file over time
foreach (var url in urls)
{
    var r = await Curl.Execute($"curl {url}");
    r.AppendToFile("combined.txt");
}
```

```csharp
public CurlDotNet.Core.CurlResult AppendToFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.AppendToFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')