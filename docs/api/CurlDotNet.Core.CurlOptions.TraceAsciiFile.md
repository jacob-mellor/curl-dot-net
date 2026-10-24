#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TraceAsciiFile Property

Writes a readable diagnostic trace of the transfer \(payloads as text, no hex column\) to this file,
like curl's `--trace-ascii FILE`; `"-"` writes the trace to standard output\.

```csharp
public string? TraceAsciiFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path, or `"-"`\. Default: null \(no trace\)\.

### Remarks

Identical to [TraceFile](CurlDotNet.Core.CurlOptions.TraceFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceFile') except the payload dumps omit the hexadecimal column, producing a more
            compact log. When both options are supplied, the ASCII format wins.

Example: `curl --trace-ascii - https://example.com/` is
            `new CurlOptions { Url = "https://example.com/", TraceAsciiFile = "-" }`. curl manual:
            https://curl.se/docs/manpage.html#--trace-ascii

### See Also
- [TraceFile](CurlDotNet.Core.CurlOptions.TraceFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceFile')
- [TraceTime](CurlDotNet.Core.CurlOptions.TraceTime.md 'CurlDotNet\.Core\.CurlOptions\.TraceTime')
- [WithTraceAscii\(string, bool\)](CurlDotNet.Core.CurlRequestBuilder.WithTraceAscii(string,bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTraceAscii\(string, bool\)')