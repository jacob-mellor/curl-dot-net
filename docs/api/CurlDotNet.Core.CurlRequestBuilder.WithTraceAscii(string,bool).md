#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithTraceAscii\(string, bool\) Method

Write an ASCII\-only diagnostic trace of the transfer to a file \(like curl `--trace-ascii`\)\.

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

### Remarks

Identical to [WithTrace\(string, bool\)](CurlDotNet.Core.CurlRequestBuilder.WithTrace(string,bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTrace\(string, bool\)') but the payload dumps
            omit the hexadecimal column, producing a more compact, readable log.