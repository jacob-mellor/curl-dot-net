#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')

## CurlTimings\.StartTransfer Property

Milliseconds from the start until the response headers arrived \("time to first byte"\);
curl's `-w '%{time_starttransfer}'`\.

```csharp
public double StartTransfer { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')  
Milliseconds since the start; includes DNS, connect, TLS, sending the request and the
            server's think time\.

### Remarks

`StartTransfer - AppConnect` (or `- Connect` for http) is roughly how long the server
            took to answer. The body download then takes `Total - StartTransfer`.

### See Also
- [Total](CurlDotNet.Core.CurlTimings.Total.md 'CurlDotNet\.Core\.CurlTimings\.Total')
- [PreTransfer](CurlDotNet.Core.CurlTimings.PreTransfer.md 'CurlDotNet\.Core\.CurlTimings\.PreTransfer')