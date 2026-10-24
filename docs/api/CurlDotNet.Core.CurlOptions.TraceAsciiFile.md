#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TraceAsciiFile Property

Path to write an ASCII\-only diagnostic trace \(no hex column\), set by the
`--trace-ascii` option\. Use `"-"` to write the trace to standard output\.

```csharp
public string? TraceAsciiFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

### Remarks

Identical to [TraceFile](CurlDotNet.Core.CurlOptions.TraceFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceFile') except the payload dumps omit the
            hexadecimal column, producing a more compact, readable log. When both options
            are supplied, the ASCII format wins.

See: [https://curl\.se/docs/manpage\.html\#\-\-trace\-ascii](https://curl.se/docs/manpage.html#--trace-ascii 'https://curl\.se/docs/manpage\.html\#\-\-trace\-ascii')