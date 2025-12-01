#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.DownloadAsync\(string, string\) Method

Download a file asynchronously\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> DownloadAsync(string url, string outputPath);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.DownloadAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

URL to download from

<a name='CurlDotNet.DotNetCurl.DownloadAsync(string,string).outputPath'></a>

`outputPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path to save the file

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Task with result object