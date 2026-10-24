#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DownloadAsync\(string, string\) Method

Downloads [url](CurlDotNet.Curl.DownloadAsync(string,string).md#CurlDotNet.Curl.DownloadAsync(string,string).url 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)\.url') and saves the response body to [outputPath](CurlDotNet.Curl.DownloadAsync(string,string).md#CurlDotNet.Curl.DownloadAsync(string,string).outputPath 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)\.outputPath') \(like
`curl -o PATH URL`\); paths with spaces or quotes need no escaping\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> DownloadAsync(string url, string outputPath);
```
#### Parameters

<a name='CurlDotNet.Curl.DownloadAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL of the file, for example `https://example.com/files/report.pdf`\.

<a name='CurlDotNet.Curl.DownloadAsync(string,string).outputPath'></a>

`outputPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Where to save it: an absolute path \(`@"C:\Downloads\file.pdf"`,
            `/tmp/file.pdf`\), a relative path \(`downloads/file.pdf`, relative to the current directory\) or just a
            file name\. Its folder must already exist\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'): [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode'), [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers'),
            [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles') \(contains [outputPath](CurlDotNet.Curl.DownloadAsync(string,string).md#CurlDotNet.Curl.DownloadAsync(string,string).outputPath 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)\.outputPath')\)\. The body is in the file, not in
            [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') \(empty\); [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') also holds it for binary downloads\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.Curl.DownloadAsync(string,string).md#CurlDotNet.Curl.DownloadAsync(string,string).url 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)\.url') or [outputPath](CurlDotNet.Curl.DownloadAsync(string,string).md#CurlDotNet.Curl.DownloadAsync(string,string).outputPath 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)\.outputPath') is null, empty or whitespace\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
The file could not be written: its folder does not exist, the path is
            invalid, or the disk is full or read\-only \(exit code 23\)\. Fix: create the folder first\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed \(DNS, connect, timeout, TLS\.\.\.\); see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Example

```csharp
try
{
    var result = await Curl.DownloadAsync("https://example.com/manual.pdf", "downloads/manual.pdf");
    if (!result.IsSuccess) Console.WriteLine($"Server answered {result.StatusCode}");
}
catch (CurlWriteErrorException ex)
{
    Console.WriteLine(ex.Message); // folder missing or not writable, and how to fix it
}
```

### Remarks

The folder of [outputPath](CurlDotNet.Curl.DownloadAsync(string,string).md#CurlDotNet.Curl.DownloadAsync(string,string).outputPath 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)\.outputPath') must exist (curl's `--create-dirs` is not added); an
            existing file is overwritten. The file is written even for an HTTP error status, exactly like
            `curl -o` without `-f`, so check [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess') afterwards (and delete the file
            if it holds an error page). Redirects are followed only when [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects') is true; for
            download links that redirect, use `await Curl.ExecuteAsync("curl -L -f -o file.zip URL")`.

The whole body is also kept in memory ([BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') or [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')).
            For a progress bar use [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)') with
            [WithProgress\(Action&lt;double,long,long&gt;\)](CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_).md 'CurlDotNet\.Core\.CurlSettings\.WithProgress\(System\.Action\<double,long,long\>\)').

Example: `var result = await Curl.DownloadAsync("https://example.com/manual.pdf", @"C:\Downloads\manual.pdf");`
            then `result.OutputFiles[0]` is the path written.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.Curl.Download(string,string).md 'CurlDotNet\.Curl\.Download\(string, string\)')
- [Builder equivalent](CurlDotNet.Core.CurlRequestBuilder.WithOutput(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithOutput\(string\)')
- [libcurl\-style equivalent](CurlDotNet.Lib.LibCurl.WithOutputFile(string).md 'CurlDotNet\.Lib\.LibCurl\.WithOutputFile\(string\)')
- [Save a response you already have](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)')
- [DownloadAsync\(string, string\)](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)')