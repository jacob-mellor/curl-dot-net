#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')

## CurlTimings\.NameLookup Property

Milliseconds from the start of the transfer until the host name was resolved to an address;
curl's `-w '%{time_namelookup}'` \(which prints seconds\)\.

```csharp
public double NameLookup { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')  
Milliseconds since the transfer started; 0 when the request reused a pooled connection
            \(no lookup happened\) or on \.NET Standard 2\.0 / \.NET Framework, where the connection is not observable\.

### Remarks

Recorded by the CurlDotNet transport on .NET 5 and later for every new connection, for
            `http://` and `https://` alike. A literal IP address still gets a (near-zero) value.

Example: `if (result.Timings.NameLookup > 500) Console.WriteLine("Slow DNS - try --resolve host:443:1.2.3.4");`.

### See Also
- [Connect](CurlDotNet.Core.CurlTimings.Connect.md 'CurlDotNet\.Core\.CurlTimings\.Connect')
- [Total](CurlDotNet.Core.CurlTimings.Total.md 'CurlDotNet\.Core\.CurlTimings\.Total')