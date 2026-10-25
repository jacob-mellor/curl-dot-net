#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.CurlErrorCode Property

curl's exit code for this failure \(the `CURLE_*` value from `curl.h`\), for example 6 when the
host name cannot be resolved, 7 when the connection is refused, 22 for `-f` HTTP errors and 28 for a timeout\.

```csharp
public System.Nullable<int> CurlErrorCode { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
An integer from 1 to 100 that matches what the curl command\-line tool would return as `$?`; null only
            for exceptions that have no curl equivalent \(such as a JSON parsing error in `ParseJson`\)\.

### Remarks

Common codes: 3 malformed URL, 5 proxy not resolved, 6 host not resolved, 7 connection failed, 18 partial body,
            22 HTTP error with `-f`, 23 write error, 26 read error, 28 timeout, 35 TLS handshake failed, 47 too many redirects,
            52 empty reply, 56 receive error, 60 certificate not trusted. [GetCurlErrorName\(\)](CurlDotNet.Exceptions.CurlException.GetCurlErrorName().md 'CurlDotNet\.Exceptions\.CurlException\.GetCurlErrorName\(\)') returns the
            `CURLE_` name.

Example: `catch (CurlException ex) when (ex.CurlErrorCode == 28) { /* timed out: retry with a longer -m */ }`

Full table: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

### See Also
- [GetCurlErrorName\(\)](CurlDotNet.Exceptions.CurlException.GetCurlErrorName().md 'CurlDotNet\.Exceptions\.CurlException\.GetCurlErrorName\(\)')
- [IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)')