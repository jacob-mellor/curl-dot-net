#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.IsSuccess Property

True when [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') is 200\-299 \(OK, Created, No Content\.\.\.\); false for 1xx, 3xx, 4xx, 5xx and 0\.

```csharp
public bool IsSuccess { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
Computed from [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') each time; never throws\.

### Remarks

A 304 Not Modified or an unfollowed 301/302 is not "success" here. To accept other codes, compare
            [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') yourself or call [EnsureStatus\(int\)](CurlDotNet.Core.CurlResult.EnsureStatus(int).md 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)').

Example: `if (!result.IsSuccess) logger.LogWarning("{Status} from {Command}", result.StatusCode, result.Command);`

### See Also
- [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode')
- [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)')