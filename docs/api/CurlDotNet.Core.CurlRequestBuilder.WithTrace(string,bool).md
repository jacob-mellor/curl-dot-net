#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithTrace\(string, bool\) Method

Write a full diagnostic trace of the transfer to a file \(like curl `--trace`\)\.

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

### Remarks

The trace records connection and proxy selection, TLS verification, the
            exact bytes sent and received, transfer timings, and the precise error if the
            request is blocked or fails - ideal for troubleshooting requests behind a proxy.