#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.SaveToFile\(string\) Method

Writes the response body to [filePath](CurlDotNet.Core.CurlRequestBuilder.SaveToFile(string).md#CurlDotNet.Core.CurlRequestBuilder.SaveToFile(string).filePath 'CurlDotNet\.Core\.CurlRequestBuilder\.SaveToFile\(string\)\.filePath') \(curl's `-o`\); same as
[WithOutput\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithOutput(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithOutput\(string\)'), named for readability in download code\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder SaveToFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.SaveToFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Destination file path, absolute or relative to the current directory\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Overwrites an existing file; the folder must exist (otherwise exit code 23 with the path in the
            message). The saved path appears in [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles').

Example: `await CurlRequestBuilder.Get(url).SaveToFile("logo.png").ExecuteAsync();` is `curl -o logo.png URL`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [WithOutput\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithOutput(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithOutput\(string\)')
- [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)')