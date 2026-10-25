#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.Download\(string, string\) Method

Downloads [url](CurlDotNet.DotNetCurl.Download(string,string).md#CurlDotNet.DotNetCurl.Download(string,string).url 'CurlDotNet\.DotNetCurl\.Download\(string, string\)\.url') to [outputPath](CurlDotNet.DotNetCurl.Download(string,string).md#CurlDotNet.DotNetCurl.Download(string,string).outputPath 'CurlDotNet\.DotNetCurl\.Download\(string, string\)\.outputPath') and blocks until the file is written; same as
[Download\(string, string\)](CurlDotNet.Curl.Download(string,string).md 'CurlDotNet\.Curl\.Download\(string, string\)') \(like `curl -o PATH URL`\)\.

```csharp
public static CurlDotNet.Core.CurlResult Download(string url, string outputPath);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Download(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL of the file, for example `https://example.com/files/report.pdf`\.

<a name='CurlDotNet.DotNetCurl.Download(string,string).outputPath'></a>

`outputPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Where to save it \(absolute, relative or a file name\); its folder must exist\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'); [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles') contains [outputPath](CurlDotNet.DotNetCurl.Download(string,string).md#CurlDotNet.DotNetCurl.Download(string,string).outputPath 'CurlDotNet\.DotNetCurl\.Download\(string, string\)\.outputPath')\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.DotNetCurl.Download(string,string).md#CurlDotNet.DotNetCurl.Download(string,string).url 'CurlDotNet\.DotNetCurl\.Download\(string, string\)\.url') or [outputPath](CurlDotNet.DotNetCurl.Download(string,string).md#CurlDotNet.DotNetCurl.Download(string,string).outputPath 'CurlDotNet\.DotNetCurl\.Download\(string, string\)\.outputPath') is null, empty or whitespace\.

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
The file could not be written \(exit code 23\)\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed\.

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

The folder must exist; an existing file is overwritten; the file is written even for an HTTP error
            status, so check [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess').

Example: `var result = DotNetCurl.Download("https://example.com/a.zip", "downloads/a.zip");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [Async version \(preferred\)](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)')
- [Download\(string, string\)](CurlDotNet.Curl.Download(string,string).md 'CurlDotNet\.Curl\.Download\(string, string\)')