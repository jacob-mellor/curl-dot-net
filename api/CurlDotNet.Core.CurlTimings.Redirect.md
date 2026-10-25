#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')

## CurlTimings\.Redirect Property

Milliseconds from the start until the last redirect was followed \(with `-L`\); curl's
`-w '%{time_redirect}'`\.

```csharp
public double Redirect { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')  
0 when no redirect was followed\.

### Remarks

Example: `curl -L https://bit.ly/x` then `result.Timings.Redirect` tells how long the
            redirect hops took before the final request started.

### See Also
- [Total](CurlDotNet.Core.CurlTimings.Total.md 'CurlDotNet\.Core\.CurlTimings\.Total')