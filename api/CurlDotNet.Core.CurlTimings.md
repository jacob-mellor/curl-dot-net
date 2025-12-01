#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlTimings Class


<b>Detailed timing breakdown of the curl operation.</b>

See where time was spent (like curl -w):

```csharp
if (result.Timings.Total > 2000)
{
    Console.WriteLine("Slow request! Let's see why:");
    Console.WriteLine($"DNS: {result.Timings.NameLookup}ms");
    Console.WriteLine($"Connect: {result.Timings.Connect}ms");
    Console.WriteLine($"SSL: {result.Timings.AppConnect}ms");
    Console.WriteLine($"Wait: {result.Timings.StartTransfer}ms");
}
```

```csharp
public class CurlTimings
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlTimings

| Properties | |
| :--- | :--- |
| [AppConnect](CurlDotNet.Core.CurlTimings.AppConnect.md 'CurlDotNet\.Core\.CurlTimings\.AppConnect') | SSL/TLS handshake time in milliseconds |
| [Connect](CurlDotNet.Core.CurlTimings.Connect.md 'CurlDotNet\.Core\.CurlTimings\.Connect') | TCP connection time in milliseconds |
| [NameLookup](CurlDotNet.Core.CurlTimings.NameLookup.md 'CurlDotNet\.Core\.CurlTimings\.NameLookup') | DNS resolution time in milliseconds |
| [PreTransfer](CurlDotNet.Core.CurlTimings.PreTransfer.md 'CurlDotNet\.Core\.CurlTimings\.PreTransfer') | Time until request was sent in milliseconds |
| [Redirect](CurlDotNet.Core.CurlTimings.Redirect.md 'CurlDotNet\.Core\.CurlTimings\.Redirect') | Time spent on redirects in milliseconds |
| [StartTransfer](CurlDotNet.Core.CurlTimings.StartTransfer.md 'CurlDotNet\.Core\.CurlTimings\.StartTransfer') | Time until first byte received in milliseconds |
| [Total](CurlDotNet.Core.CurlTimings.Total.md 'CurlDotNet\.Core\.CurlTimings\.Total') | Total time in milliseconds |
