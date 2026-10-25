#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithRetries\(int, int\) Method

Retries any failed transfer up to [count](CurlDotNet.Core.CurlSettings.WithRetries(int,int).md#CurlDotNet.Core.CurlSettings.WithRetries(int,int).count 'CurlDotNet\.Core\.CurlSettings\.WithRetries\(int, int\)\.count') more times with a fixed pause \(curl
`--retry N --retry-delay S --retry-all-errors`\), and returns these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithRetries(int count, int delayMs=1000);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithRetries(int,int).count'></a>

`count` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Extra attempts after the first, for example 3; 0 disables retrying\.

<a name='CurlDotNet.Core.CurlSettings.WithRetries(int,int).delayMs'></a>

`delayMs` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Pause between attempts in milliseconds; default 1000\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Sets [RetryCount](CurlDotNet.Core.CurlSettings.RetryCount.md 'CurlDotNet\.Core\.CurlSettings\.RetryCount') and [RetryDelayMs](CurlDotNet.Core.CurlSettings.RetryDelayMs.md 'CurlDotNet\.Core\.CurlSettings\.RetryDelayMs'). If every attempt fails, the exception of
            the last attempt is thrown.

Example: `new CurlSettings().WithRetries(3, delayMs: 2000)` makes up to four attempts, two seconds apart.

### See Also
- [RetryCount](CurlDotNet.Core.CurlSettings.RetryCount.md 'CurlDotNet\.Core\.CurlSettings\.RetryCount')
- [RetryDelayMs](CurlDotNet.Core.CurlSettings.RetryDelayMs.md 'CurlDotNet\.Core\.CurlSettings\.RetryDelayMs')
- [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry')