#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlTimings Class

Where the time of a transfer went \- DNS, TCP connect, TLS handshake, first byte, total \- in milliseconds,
matching curl's `-w '%{time_namelookup}'`, `%{time_connect}` \.\.\. `%{time_total}` \(which are seconds\)\.

```csharp
public class CurlTimings
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlTimings

### Remarks

All values are cumulative from the start of the transfer, as in curl: [NameLookup](CurlDotNet.Core.CurlTimings.NameLookup.md 'CurlDotNet\.Core\.CurlTimings\.NameLookup') <=
            [Connect](CurlDotNet.Core.CurlTimings.Connect.md 'CurlDotNet\.Core\.CurlTimings\.Connect') <= [AppConnect](CurlDotNet.Core.CurlTimings.AppConnect.md 'CurlDotNet\.Core\.CurlTimings\.AppConnect') (HTTPS) <= [StartTransfer](CurlDotNet.Core.CurlTimings.StartTransfer.md 'CurlDotNet\.Core\.CurlTimings\.StartTransfer') <= [Total](CurlDotNet.Core.CurlTimings.Total.md 'CurlDotNet\.Core\.CurlTimings\.Total').
            Subtract neighbours to get the length of one phase. Connection-level values (NameLookup, Connect, AppConnect) are
            recorded on .NET 5 and later for new connections; on .NET Standard 2.0 / .NET Framework and for reused pooled
            connections they are 0.

Example: `var t = result.Timings; Console.WriteLine($"DNS {t.NameLookup:F0} ms, TCP {t.Connect - t.NameLookup:F0} ms, TLS {t.AppConnect - t.Connect:F0} ms, server {t.StartTransfer - Math.Max(t.AppConnect, t.Connect):F0} ms, download {t.Total - t.StartTransfer:F0} ms");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md

### See Also
- [Timings](CurlDotNet.Core.CurlResult.Timings.md 'CurlDotNet\.Core\.CurlResult\.Timings')

| Properties | |
| :--- | :--- |
| [AppConnect](CurlDotNet.Core.CurlTimings.AppConnect.md 'CurlDotNet\.Core\.CurlTimings\.AppConnect') | Milliseconds from the start until the TLS handshake finished; curl's `-w '%{time_appconnect}'`\. Always 0 for plain `http://`, exactly like curl\. |
| [Connect](CurlDotNet.Core.CurlTimings.Connect.md 'CurlDotNet\.Core\.CurlTimings\.Connect') | Milliseconds from the start until the TCP connection to the server \(or proxy\) was established; curl's `-w '%{time_connect}'`\. |
| [NameLookup](CurlDotNet.Core.CurlTimings.NameLookup.md 'CurlDotNet\.Core\.CurlTimings\.NameLookup') | Milliseconds from the start of the transfer until the host name was resolved to an address; curl's `-w '%{time_namelookup}'` \(which prints seconds\)\. |
| [PreTransfer](CurlDotNet.Core.CurlTimings.PreTransfer.md 'CurlDotNet\.Core\.CurlTimings\.PreTransfer') | Milliseconds from the start until the request was about to be sent; curl's `-w '%{time_pretransfer}'`\. |
| [Redirect](CurlDotNet.Core.CurlTimings.Redirect.md 'CurlDotNet\.Core\.CurlTimings\.Redirect') | Milliseconds from the start until the last redirect was followed \(with `-L`\); curl's `-w '%{time_redirect}'`\. |
| [StartTransfer](CurlDotNet.Core.CurlTimings.StartTransfer.md 'CurlDotNet\.Core\.CurlTimings\.StartTransfer') | Milliseconds from the start until the response headers arrived \("time to first byte"\); curl's `-w '%{time_starttransfer}'`\. |
| [Total](CurlDotNet.Core.CurlTimings.Total.md 'CurlDotNet\.Core\.CurlTimings\.Total') | Milliseconds the whole transfer took, body included; curl's `-w '%{time_total}'`\. |
