#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.SpeedTime Property

How long, in seconds, a transfer may stay below [SpeedLimit](CurlDotNet.Core.CurlOptions.SpeedLimit.md 'CurlDotNet\.Core\.CurlOptions\.SpeedLimit') before it is aborted, like curl's
`-y`/`--speed-time`\.

```csharp
public int SpeedTime { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Whole seconds\. Default: 0, which means curl's 30 seconds when [SpeedLimit](CurlDotNet.Core.CurlOptions.SpeedLimit.md 'CurlDotNet\.Core\.CurlOptions\.SpeedLimit') is set \(and nothing
            otherwise\)\.

### Remarks

curl also applies `--speed-time` alone with a limit of 1 byte per second; CurlDotNet does the same, so
            `--speed-time 20` aborts a transfer that receives nothing for 20 seconds (exit 28).

Example: `curl -Y 500 -y 10 URL` is `new CurlOptions { Url = url, SpeedLimit = 500, SpeedTime = 10 }`.

### See Also
- [SpeedLimit](CurlDotNet.Core.CurlOptions.SpeedLimit.md 'CurlDotNet\.Core\.CurlOptions\.SpeedLimit')
- [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime')