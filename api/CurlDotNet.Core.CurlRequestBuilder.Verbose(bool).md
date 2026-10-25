#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Verbose\(bool\) Method

Records curl's `-v` / `--verbose` log \(request lines with `>`, response headers with
`<`, connection notes with `*`\) for troubleshooting a request\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder Verbose(bool verbose=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Verbose(bool).verbose'></a>

`verbose` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

True \(the default\) to record the verbose log\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

The verbose text is placed at the start of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'), before the response, in the
            same order the curl tool prints it, so use it while debugging rather than when parsing the body.
            For a complete byte-level log written to a file use [WithTrace\(string, bool\)](CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTrace\(string, bool\)') or
            [WithTraceAscii\(string, bool\)](CurlDotNet.Core.CurlRequestBuilder.WithTraceAscii(string,bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTraceAscii\(string, bool\)').

Example: `CurlRequestBuilder.Get(url).Verbose()` is `curl -v URL`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md

### See Also
- [WithTrace\(string, bool\)](CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTrace\(string, bool\)')
- [Verbose](CurlDotNet.Core.CurlOptions.Verbose.md 'CurlDotNet\.Core\.CurlOptions\.Verbose')
- [WithVerbose\(\)](CurlDotNet.Lib.LibCurl.WithVerbose().md 'CurlDotNet\.Lib\.LibCurl\.WithVerbose\(\)')