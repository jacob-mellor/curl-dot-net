#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')

## CurlTimings\.AppConnect Property

Milliseconds from the start until the TLS handshake finished; curl's `-w '%{time_appconnect}'`\.
Always 0 for plain `http://`, exactly like curl\.

```csharp
public double AppConnect { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')  
Cumulative milliseconds since the start \(at least [Connect](CurlDotNet.Core.CurlTimings.Connect.md 'CurlDotNet\.Core\.CurlTimings\.Connect')\) for `https://`
            on a new connection; 0 for `http://`, reused connections and \.NET Standard 2\.0 / \.NET Framework\.

### Remarks

The TLS handshake alone took `AppConnect - Connect` milliseconds.

### See Also
- [Connect](CurlDotNet.Core.CurlTimings.Connect.md 'CurlDotNet\.Core\.CurlTimings\.Connect')
- [PreTransfer](CurlDotNet.Core.CurlTimings.PreTransfer.md 'CurlDotNet\.Core\.CurlTimings\.PreTransfer')