#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TraceFile Property

Path to write a full diagnostic trace \(request/response headers and bodies
dumped as hex \+ ASCII\), set by the `--trace` option\. Use `"-"` to
write the trace to standard output\.

```csharp
public string? TraceFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

### Remarks

The trace captures connection and proxy selection, TLS verification,
            the exact bytes sent and received, transfer timings, and the precise error
            when a request is blocked or fails. This is the recommended option for
            diagnosing why requests are being rejected behind a proxy.

See: [https://curl\.se/docs/manpage\.html\#\-\-trace](https://curl.se/docs/manpage.html#--trace 'https://curl\.se/docs/manpage\.html\#\-\-trace')