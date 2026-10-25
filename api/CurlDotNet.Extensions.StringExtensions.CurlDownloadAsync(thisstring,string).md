#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

## StringExtensions\.CurlDownloadAsync\(this string, string\) Method

Downloads this URL to [outputFile](CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).md#CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).outputFile 'CurlDotNet\.Extensions\.StringExtensions\.CurlDownloadAsync\(this string, string\)\.outputFile') \(like `curl -o FILE URL`\) and returns the result \- the same as
[DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)')\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlDownloadAsync(this string url, string outputFile);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The absolute URL of the file, such as `https://example.com/file.zip`\.

<a name='CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).outputFile'></a>

`outputFile` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The local path to write, such as `downloads/file.zip`; must not be empty\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with the [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[outputFile](CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).md#CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).outputFile 'CurlDotNet\.Extensions\.StringExtensions\.CurlDownloadAsync\(this string, string\)\.outputFile') is null or empty\.

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
The file could not be created \(folder missing, no permission\); exit code 23\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed \(DNS, connect, timeout, TLS\); see the message for the fix\.

### Remarks

The file is written as the bytes arrive and is listed in [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles'); the folder must
            exist. An HTTP error page (404, 500) is saved like any other body unless you check the status - use
            `"curl -f -o FILE URL".CurlAsync()` to fail instead.

Example: `await "https://example.com/files/report.pdf".CurlDownloadAsync("report.pdf");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)')
- [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)')