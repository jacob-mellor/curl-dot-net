#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')

## CurlTimings\.Connect Property

Milliseconds from the start until the TCP connection to the server \(or proxy\) was established;
curl's `-w '%{time_connect}'`\.

```csharp
public double Connect { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')  
Cumulative milliseconds since the start, so it is always at least [NameLookup](CurlDotNet.Core.CurlTimings.NameLookup.md 'CurlDotNet\.Core\.CurlTimings\.NameLookup');
            0 for a reused connection or on \.NET Standard 2\.0 / \.NET Framework\.

### Remarks

The TCP handshake alone took `Connect - NameLookup` milliseconds. A large value usually
            means network latency or a firewall dropping packets; see `--connect-timeout`.

### See Also
- [NameLookup](CurlDotNet.Core.CurlTimings.NameLookup.md 'CurlDotNet\.Core\.CurlTimings\.NameLookup')
- [AppConnect](CurlDotNet.Core.CurlTimings.AppConnect.md 'CurlDotNet\.Core\.CurlTimings\.AppConnect')