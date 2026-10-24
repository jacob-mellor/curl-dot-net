#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TraceFile Property

Writes a full diagnostic trace of the transfer \(headers and bodies dumped as hex plus ASCII\) to this file,
like curl's `--trace FILE`; `"-"` writes the trace to standard output\.

```csharp
public string? TraceFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path, or `"-"`\. Default: null \(no trace\)\.

### Remarks

The trace captures connection and proxy selection, TLS verification, the exact bytes sent and received,
            transfer timings, and the precise error when a request is blocked or fails. This is the recommended option for
            diagnosing why requests are rejected behind a proxy. When [TraceAsciiFile](CurlDotNet.Core.CurlOptions.TraceAsciiFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceAsciiFile') is also set, the ASCII
            format wins. [TraceTime](CurlDotNet.Core.CurlOptions.TraceTime.md 'CurlDotNet\.Core\.CurlOptions\.TraceTime') adds timestamps.

Example: `curl --trace trace.log https://example.com/` is
            `new CurlOptions { Url = "https://example.com/", TraceFile = "trace.log" }`. curl manual:
            https://curl.se/docs/manpage.html#--trace

### See Also
- [TraceAsciiFile](CurlDotNet.Core.CurlOptions.TraceAsciiFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceAsciiFile')
- [TraceTime](CurlDotNet.Core.CurlOptions.TraceTime.md 'CurlDotNet\.Core\.CurlOptions\.TraceTime')
- [Verbose](CurlDotNet.Core.CurlOptions.Verbose.md 'CurlDotNet\.Core\.CurlOptions\.Verbose')
- [WithTrace\(string, bool\)](CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTrace\(string, bool\)')