#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TraceTime Property

Whether to prefix every trace line with a high\-resolution timestamp
\(`--trace-time`\)\. Useful for spotting where time is spent or where a
request stalls\.

```csharp
public bool TraceTime { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

### Remarks

See: [https://curl\.se/docs/manpage\.html\#\-\-trace\-time](https://curl.se/docs/manpage.html#--trace-time 'https://curl\.se/docs/manpage\.html\#\-\-trace\-time')