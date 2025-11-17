#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')

## CurlTimeoutException\.Timeout Property

Gets the timeout duration that was exceeded\.

```csharp
public System.Nullable<System.TimeSpan> Timeout { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
The timeout duration, or null if not specified\.

### Remarks

This represents the --max-time or --connect-timeout value that was exceeded.

AI-Usage: Use this to determine if timeout should be increased.