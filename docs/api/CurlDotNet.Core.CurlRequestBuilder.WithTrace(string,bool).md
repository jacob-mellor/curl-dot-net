#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithTrace\(string, bool\) Method

Writes a full diagnostic trace of the transfer to a file \(curl's `--trace FILE`\): every byte sent
and received as hex plus text, connection and TLS notes, and the exact error if it fails\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithTrace(string filePath, bool includeTimestamps=false);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path of the trace file to write\. Use `"-"` for standard output\.

<a name='CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).includeTimestamps'></a>

`includeTimestamps` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

When `true`, prefixes every line with a timestamp \(like curl `--trace-time`\)\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

The trace records proxy selection, TLS verification, request and response bytes, timings and the
            precise failure, which makes it the first thing to capture for a request that works in curl but not
            here (or behind a proxy). `"-"` writes the trace to standard output. The folder must exist.

[includeTimestamps](CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).md#CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).includeTimestamps 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTrace\(string, bool\)\.includeTimestamps') adds curl's `--trace-time` (a time prefix on every line).
            [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') prints `--trace FILE` and, when set, `--trace-time`.

Example: `CurlRequestBuilder.Get(url).WithTrace("trace.log", includeTimestamps: true)` is
            `curl --trace trace.log --trace-time URL`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md

### See Also
- [WithTraceAscii\(string, bool\)](CurlDotNet.Core.CurlRequestBuilder.WithTraceAscii(string,bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTraceAscii\(string, bool\)')
- [Verbose\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Verbose(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Verbose\(bool\)')
- [TraceFile](CurlDotNet.Core.CurlOptions.TraceFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceFile')
- [TraceTime](CurlDotNet.Core.CurlOptions.TraceTime.md 'CurlDotNet\.Core\.CurlOptions\.TraceTime')