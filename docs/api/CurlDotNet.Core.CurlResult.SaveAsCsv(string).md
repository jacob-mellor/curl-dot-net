#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.SaveAsCsv\(string\) Method


<b>Save JSON response as CSV file (for JSON arrays).</b>

Converts JSON arrays to CSV for Excel:

```csharp
// JSON: [{"name":"John","age":30}, {"name":"Jane","age":25}]
result.SaveAsCsv("users.csv");

// Creates CSV:
// name,age
// John,30
// Jane,25

// Open in Excel
Process.Start("users.csv");
```

<b>Note:</b> Only works with JSON arrays of objects.

```csharp
public CurlDotNet.Core.CurlResult SaveAsCsv(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveAsCsv(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')