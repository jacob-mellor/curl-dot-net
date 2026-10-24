#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')

## CurlTimings\.Total Property

Milliseconds the whole transfer took, body included; curl's `-w '%{time_total}'`\.

```csharp
public double Total { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')  
Milliseconds since the start of the transfer, measured after the body was read\.

### Remarks

Example: `Console.WriteLine($"{result.Timings.Total:F0} ms (DNS {result.Timings.NameLookup:F0}, connect {result.Timings.Connect:F0}, TLS {result.Timings.AppConnect:F0}, first byte {result.Timings.StartTransfer:F0})");`

To get curl's seconds instead, divide by 1000 or run the command with
            `-w '%{time_total}'` and read it from [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body').

### See Also
- [StartTransfer](CurlDotNet.Core.CurlTimings.StartTransfer.md 'CurlDotNet\.Core\.CurlTimings\.StartTransfer')
- [NameLookup](CurlDotNet.Core.CurlTimings.NameLookup.md 'CurlDotNet\.Core\.CurlTimings\.NameLookup')