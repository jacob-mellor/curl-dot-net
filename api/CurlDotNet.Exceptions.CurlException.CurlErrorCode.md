#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.CurlErrorCode Property

Gets the curl error code matching the original curl implementation\.

```csharp
public System.Nullable<int> CurlErrorCode { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
The curl error code \(e\.g\., 6 for DNS resolution failure, 28 for timeout\), or null if not a curl\-specific error\.

### Remarks

Error codes match the CURLE_* constants from curl.h

Common codes: 6=DNS failure, 7=connection failed, 28=timeout, 35=SSL error

AI-Usage: Use this to determine the specific type of curl error programmatically.