#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[TraceWriter](CurlDotNet.Core.TraceWriter.md 'CurlDotNet\.Core\.TraceWriter')

## TraceWriter\.MaxDumpBytes Field

Maximum number of payload bytes dumped for a single send/receive block\.
Larger payloads are truncated with a note so trace files stay manageable\.

```csharp
private const int MaxDumpBytes = 102400;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')