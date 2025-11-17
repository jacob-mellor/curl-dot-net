#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DownloadAsync\(string, string\) Method


<b>Download a file from a URL and save it to disk.</b>

Downloads any file and saves it to the specified path. Shows progress if the file is large.

<b>Example:</b>

```csharp
// Download a PDF
await Curl.Download(
    "https://example.com/manual.pdf",
    @"C:\Downloads\manual.pdf"
);

// Download with original filename
await Curl.Download(
    "https://example.com/installer.exe",
    @"C:\Downloads\installer.exe"
);

Console.WriteLine("Download complete!");
```

<b>With Error Handling:</b>

```csharp
try
{
    await Curl.Download(url, "output.zip");
    Console.WriteLine("✅ Download successful");
}
catch (CurlHttpException ex) when (ex.StatusCode == 404)
{
    Console.WriteLine("❌ File not found");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Download failed: {ex.Message}");
}
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> DownloadAsync(string url, string outputPath);
```
#### Parameters

<a name='CurlDotNet.Curl.DownloadAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL of the file to download\.

<a name='CurlDotNet.Curl.DownloadAsync(string,string).outputPath'></a>

`outputPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Where to save the file\. Can be:
- Full path: `@"C:\Downloads\file.pdf"`
- Relative path: `"downloads/file.pdf"`
- Just filename: `"file.pdf"` (saves to current directory)

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with download information\.

### Remarks

This is equivalent to: `Curl.Execute($"curl -o {outputPath} {url}")`

For large files with progress, use [Execute\(string, CurlSettings\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.Core\.CurlSettings\)') with OnProgress callback.