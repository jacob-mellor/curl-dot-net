#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.Warnings Property

The warnings and notes the curl tool would have printed on standard error before this failure, each with curl's
own `Warning: ` or `Note: ` prefix and in the order curl prints them\.

```csharp
public System.Collections.Generic.IReadOnlyList<string> Warnings { get; }
```

#### Property Value
[System\.Collections\.Generic\.IReadOnlyList&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')  
A read\-only list, empty \(never null\) when curl would have printed none\. Each entry is one message on one
            line, without the line wrapping the curl tool applies at the terminal width\.

### Remarks

The same list as [Warnings](CurlDotNet.Core.CurlResult.Warnings.md 'CurlDotNet\.Core\.CurlResult\.Warnings') for a command that succeeds; see there
            for which messages are collected and when they are also written to a `--stderr` file.

Example: `catch (CurlException ex) { foreach (var w in ex.Warnings) logger.LogWarning(w); }` logs
            `Warning: --fail deselects --fail-with-body here` for a command that has both options.

### See Also
- [Warnings](CurlDotNet.Core.CurlResult.Warnings.md 'CurlDotNet\.Core\.CurlResult\.Warnings')
- [CurlErrorMessage](CurlDotNet.Exceptions.CurlException.CurlErrorMessage.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorMessage')