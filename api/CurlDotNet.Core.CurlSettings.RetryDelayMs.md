#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.RetryDelayMs Property

Pause between retries in milliseconds, like curl's `--retry-delay` \(which takes seconds\); only used
when [RetryCount](CurlDotNet.Core.CurlSettings.RetryCount.md 'CurlDotNet\.Core\.CurlSettings\.RetryCount') is greater than 0\.

```csharp
public int RetryDelayMs { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Milliseconds; default 1000\. Converted to seconds for [RetryDelay](CurlDotNet.Core.CurlOptions.RetryDelay.md 'CurlDotNet\.Core\.CurlOptions\.RetryDelay'); 0 lets the
            engine use curl's exponential back\-off \(1, 2, 4 \.\.\. seconds\)\.

### Remarks

Like curl's `--retry-delay`, a fixed delay replaces the server's `Retry-After` header; set it to 0
            to honour `Retry-After` (and back off 1, 2, 4 ... seconds when the server sends none).

Example: `settings.WithRetries(5, delayMs: 500)`.

### See Also
- [RetryCount](CurlDotNet.Core.CurlSettings.RetryCount.md 'CurlDotNet\.Core\.CurlSettings\.RetryCount')
- [WithRetries\(int, int\)](CurlDotNet.Core.CurlSettings.WithRetries(int,int).md 'CurlDotNet\.Core\.CurlSettings\.WithRetries\(int, int\)')
- [RetryDelay](CurlDotNet.Core.CurlOptions.RetryDelay.md 'CurlDotNet\.Core\.CurlOptions\.RetryDelay')