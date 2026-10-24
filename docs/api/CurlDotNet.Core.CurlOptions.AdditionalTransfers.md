#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.AdditionalTransfers Property

Further transfers described by the same command line: extra URLs, URLs produced by globbing \(`{a,b}`,
`[1-3]`\) and groups separated by `--next`/`-:`; executing the options runs them all in order\.

```csharp
public System.Collections.Generic.List<CurlDotNet.Core.CurlOptions> AdditionalTransfers { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
Complete, independent [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') for the second and later transfers; empty for a single transfer\.

### Remarks

The options object returned by the parser describes the first transfer; every entry here is the next one, in
            command-line order. The options of a `--next` group apply to every URL of that group wherever they appear, and the
            nth `-o`/`-O`/`-T` belongs to the nth URL, exactly as in curl:
            `curl -H "X: 1" https://a.example/ https://b.example/ --next -d x=1 https://c.example/` yields a first transfer
            for `a` and two additional ones, `b` (with the header) and `c` (a POST without it).

Executing the first options (or the command string) runs every transfer serially and shares cookies between
            them, like curl. [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') is the concatenated output, the status and headers are those of the
            last transfer, and [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles') lists every file. As in curl, a failed transfer does not stop
            the rest (unless [FailEarly](CurlDotNet.Core.CurlOptions.FailEarly.md 'CurlDotNet\.Core\.CurlOptions\.FailEarly'), or a fatal error); the command fails only when the last transfer failed.
            To run one transfer on its own, pass that entry to `Curl.ExecuteAsync(CurlOptions)` after clearing its own list.

### See Also
- [GlobOff](CurlDotNet.Core.CurlOptions.GlobOff.md 'CurlDotNet\.Core\.CurlOptions\.GlobOff')
- [FailEarly](CurlDotNet.Core.CurlOptions.FailEarly.md 'CurlDotNet\.Core\.CurlOptions\.FailEarly')
- [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)')