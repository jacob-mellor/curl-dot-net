#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.DownloadAsync\(string, string\) Method

Downloads [url](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md#CurlDotNet.DotNetCurl.DownloadAsync(string,string).url 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)\.url') to [outputPath](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md#CurlDotNet.DotNetCurl.DownloadAsync(string,string).outputPath 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)\.outputPath') asynchronously; same as
[DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)') \(like `curl -o PATH URL`\)\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> DownloadAsync(string url, string outputPath);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.DownloadAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL of the file\.

<a name='CurlDotNet.DotNetCurl.DownloadAsync(string,string).outputPath'></a>

`outputPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Where to save it \(absolute, relative or a file name\); its folder must exist\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'); [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles') contains [outputPath](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md#CurlDotNet.DotNetCurl.DownloadAsync(string,string).outputPath 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)\.outputPath')\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md#CurlDotNet.DotNetCurl.DownloadAsync(string,string).url 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)\.url') or [outputPath](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md#CurlDotNet.DotNetCurl.DownloadAsync(string,string).outputPath 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)\.outputPath') is null, empty or whitespace\.

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
The file could not be written \(exit code 23\)\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Remarks

Paths with spaces or quotes need no escaping. For redirecting download links use
            `await DotNetCurl.CurlAsync("curl -L -f -o file.zip URL")`.

Example: `var result = await DotNetCurl.DownloadAsync("https://example.com/manual.pdf", @"C:\Downloads\manual.pdf");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.DotNetCurl.Download(string,string).md 'CurlDotNet\.DotNetCurl\.Download\(string, string\)')
- [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)')