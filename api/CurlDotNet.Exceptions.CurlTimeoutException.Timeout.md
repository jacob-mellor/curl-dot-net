#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')

## CurlTimeoutException\.Timeout Property

The limit that expired: the `-m/--max-time` or `--connect-timeout` value, so you can decide
whether raising it makes sense\.

```csharp
public System.Nullable<System.TimeSpan> Timeout { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
The configured limit as a [System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan'); [System\.TimeSpan\.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.zero 'System\.TimeSpan\.Zero') or null when no explicit
            limit was set \(for example a caller\-supplied HttpClient timed out\)\.

### Remarks

Example: `var next = (ex.Timeout ?? TimeSpan.FromSeconds(30)).TotalSeconds * 2;` then run the command
            again with `-m {next}`.

### See Also
- [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')