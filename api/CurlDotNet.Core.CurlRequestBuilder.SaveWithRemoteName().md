#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.SaveWithRemoteName\(\) Method

Saves the response body in the current directory under the file name taken from the URL
\(curl's `-O` / `--remote-name`\), for example `setup.exe` for `https://x.com/dl/setup.exe`\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder SaveWithRemoteName();
```

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

The name is the last path segment of the URL without the query string; a URL ending in `/`
            saves as `curl_response`. An existing file is overwritten. The saved path appears in
            [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles').

Example: `await CurlRequestBuilder.Get("https://example.com/files/data.zip").SaveWithRemoteName().ExecuteAsync();`
            creates `data.zip`, like `curl -O https://example.com/files/data.zip`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [WithOutput\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithOutput(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithOutput\(string\)')
- [UseRemoteFileName](CurlDotNet.Core.CurlOptions.UseRemoteFileName.md 'CurlDotNet\.Core\.CurlOptions\.UseRemoteFileName')