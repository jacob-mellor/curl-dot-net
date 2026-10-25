#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.GetCurlErrorName\(\) Method

Returns the `CURLE_*` constant name for [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode'), such as `CURLE_COULDNT_RESOLVE_HOST`
for 6, so logs can be searched with the same names as libcurl's documentation\.

```csharp
public string GetCurlErrorName();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The constant name, or null when [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') is null\.

### Remarks

Unknown codes are reported as `CURLE_UNKNOWN_n`; null when there is no exit code. Example:
            `Console.WriteLine($"{ex.CurlErrorCode} {ex.GetCurlErrorName()}");` prints `28 CURLE_OPERATION_TIMEDOUT`.

Meaning of each code: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

### See Also
- [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')