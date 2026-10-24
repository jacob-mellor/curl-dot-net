#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithTraceAscii\(string, bool\) Method

Writes a readable text trace of the transfer to a file \(curl's `--trace-ascii FILE`\): the same
content as [WithTrace\(string, bool\)](CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTrace\(string, bool\)') without the hexadecimal column\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithTraceAscii(string filePath, bool includeTimestamps=false);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithTraceAscii(string,bool).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path of the trace file to write\. Use `"-"` for standard output\.

<a name='CurlDotNet.Core.CurlRequestBuilder.WithTraceAscii(string,bool).includeTimestamps'></a>

`includeTimestamps` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

When `true`, prefixes every line with a timestamp \(like curl `--trace-time`\)\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Best for reading headers and JSON bodies in a log; use [WithTrace\(string, bool\)](CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTrace\(string, bool\)') when you need to
            see exact bytes (encodings, line endings). `"-"` writes to standard output.
            [includeTimestamps](CurlDotNet.Core.CurlRequestBuilder.WithTraceAscii(string,bool).md#CurlDotNet.Core.CurlRequestBuilder.WithTraceAscii(string,bool).includeTimestamps 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTraceAscii\(string, bool\)\.includeTimestamps') adds `--trace-time`.

Example: `CurlRequestBuilder.Post(url).WithJson(order).WithTraceAscii("order-call.txt")` is
            `curl -X POST -H 'Content-Type: application/json' -d '...' --trace-ascii order-call.txt URL`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md

### See Also
- [WithTrace\(string, bool\)](CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTrace\(string, bool\)')
- [TraceAsciiFile](CurlDotNet.Core.CurlOptions.TraceAsciiFile.md 'CurlDotNet\.Core\.CurlOptions\.TraceAsciiFile')