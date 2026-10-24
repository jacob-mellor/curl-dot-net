#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TraceTime Property

Prefixes every line of the [TraceFile](CurlDotNet.Core.CurlOptions.TraceFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceFile')/[TraceAsciiFile](CurlDotNet.Core.CurlOptions.TraceAsciiFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceAsciiFile') output with a high\-resolution
timestamp, like curl's `--trace-time`, to show where a request stalls\.

```csharp
public bool TraceTime { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to add timestamps\. Default: false\. Has no effect without a trace file\.

### Remarks

Example: `curl --trace-ascii t.log --trace-time URL` is
            `new CurlOptions { Url = url, TraceAsciiFile = "t.log", TraceTime = true }`. curl manual:
            https://curl.se/docs/manpage.html#--trace-time

### See Also
- [TraceFile](CurlDotNet.Core.CurlOptions.TraceFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceFile')
- [TraceAsciiFile](CurlDotNet.Core.CurlOptions.TraceAsciiFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceAsciiFile')