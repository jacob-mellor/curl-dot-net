#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

## StringExtensions\.CurlDownloadAsync\(this string, string\) Method

Downloads a file from the URL\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlDownloadAsync(this string url, string outputFile);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to download from

<a name='CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).outputFile'></a>

`outputFile` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path to save to

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The curl result

### Example
await "https://example\.com/file\.pdf"\.CurlDownloadAsync\("local\.pdf"\);