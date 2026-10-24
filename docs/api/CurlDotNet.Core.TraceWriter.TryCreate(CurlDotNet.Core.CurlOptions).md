#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[TraceWriter](CurlDotNet.Core.TraceWriter.md 'CurlDotNet\.Core\.TraceWriter')

## TraceWriter\.TryCreate\(CurlOptions\) Method

Creates a [TraceWriter](CurlDotNet.Core.TraceWriter.md 'CurlDotNet\.Core\.TraceWriter') for the supplied options, or returns
`null` when neither `--trace` nor `--trace-ascii` was requested\.

```csharp
public static CurlDotNet.Core.TraceWriter TryCreate(CurlDotNet.Core.CurlOptions options);
```
#### Parameters

<a name='CurlDotNet.Core.TraceWriter.TryCreate(CurlDotNet.Core.CurlOptions).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

The parsed curl options\.

#### Returns
[TraceWriter](CurlDotNet.Core.TraceWriter.md 'CurlDotNet\.Core\.TraceWriter')  
A configured writer, or `null` if tracing is not enabled\.