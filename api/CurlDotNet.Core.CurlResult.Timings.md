#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Timings Property


<b>Detailed timing information (like curl -w).</b>

See how long each phase took:

```csharp
Console.WriteLine($"DNS lookup: {result.Timings.NameLookup}ms");
Console.WriteLine($"Connect: {result.Timings.Connect}ms");
Console.WriteLine($"Total: {result.Timings.Total}ms");
```

```csharp
public CurlDotNet.Core.CurlTimings Timings { get; set; }
```

#### Property Value
[CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')