#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.RetryCount Property

How many times to retry a failed transfer \(any failure, not only transient ones\), like curl's
`--retry N --retry-all-errors`; 0 \(the default\) means no retries\.

```csharp
public int RetryCount { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Number of extra attempts after the first; 0 or negative disables retrying\.

### Remarks

When greater than 0 it sets [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry'), [RetryDelay](CurlDotNet.Core.CurlOptions.RetryDelay.md 'CurlDotNet\.Core\.CurlOptions\.RetryDelay') (from
            [RetryDelayMs](CurlDotNet.Core.CurlSettings.RetryDelayMs.md 'CurlDotNet\.Core\.CurlSettings\.RetryDelayMs')) and [RetryAllErrors](CurlDotNet.Core.CurlOptions.RetryAllErrors.md 'CurlDotNet\.Core\.CurlOptions\.RetryAllErrors'). The last failure is thrown when all
            attempts fail.

Example: `new CurlSettings { RetryCount = 3, RetryDelayMs = 2000 }` tries up to four times, two seconds apart.

### See Also
- [WithRetries\(int, int\)](CurlDotNet.Core.CurlSettings.WithRetries(int,int).md 'CurlDotNet\.Core\.CurlSettings\.WithRetries\(int, int\)')
- [RetryDelayMs](CurlDotNet.Core.CurlSettings.RetryDelayMs.md 'CurlDotNet\.Core\.CurlSettings\.RetryDelayMs')
- [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry')