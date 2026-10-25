#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Verbose Property

Records the request and response headers \(curl's `>`/`<` lines\) and connection notes,
like curl's `-v`/`--verbose`, and puts that log at the start of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

```csharp
public bool Verbose { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to capture the verbose log\. Default: false\.

### Remarks

curl prints this to standard error; CurlDotNet has no terminal, so the log is placed in front of the
            output text, followed by the headers ([IncludeHeaders](CurlDotNet.Core.CurlOptions.IncludeHeaders.md 'CurlDotNet\.Core\.CurlOptions\.IncludeHeaders')) and body. For a complete wire log
            (bytes, TLS, timings) written to a file use [TraceFile](CurlDotNet.Core.CurlOptions.TraceFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceFile') or [TraceAsciiFile](CurlDotNet.Core.CurlOptions.TraceAsciiFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceAsciiFile').

Example: `curl -v https://example.com/` is `new CurlOptions { Url = "https://example.com/", Verbose = true }`.
            Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md

### See Also
- [TraceFile](CurlDotNet.Core.CurlOptions.TraceFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceFile')
- [TraceAsciiFile](CurlDotNet.Core.CurlOptions.TraceAsciiFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceAsciiFile')
- [Verbose\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Verbose(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Verbose\(bool\)')