#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Timings Property

How long each phase of the transfer took \(DNS, connect, TLS, first byte, total\) in milliseconds, like curl's
`-w '%{time_total}'` family\.

```csharp
public CurlDotNet.Core.CurlTimings Timings { get; set; }
```

#### Property Value
[CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')  
Filled for HTTP\(S\) transfers; null or all zeros for other protocols and results you created yourself\.
            See [CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings') for what each value means and when it is 0\.

### Remarks

Example: `if (result.Timings.Total > 2000) Console.WriteLine($"slow: DNS {result.Timings.NameLookup:F0} ms, connect {result.Timings.Connect:F0} ms, first byte {result.Timings.StartTransfer:F0} ms");`

### See Also
- [CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')
- [PrintVerbose\(\)](CurlDotNet.Core.CurlResult.PrintVerbose().md 'CurlDotNet\.Core\.CurlResult\.PrintVerbose\(\)')