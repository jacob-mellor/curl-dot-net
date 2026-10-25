#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Warnings Property

The warnings and notes the curl tool would have printed on standard error for this command, each with curl's own
`Warning: ` or `Note: ` prefix and in the order curl prints them\.

```csharp
public System.Collections.Generic.IReadOnlyList<string> Warnings { get; }
```

#### Property Value
[System\.Collections\.Generic\.IReadOnlyList&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')  
A read\-only list, empty \(never null\) when curl would have printed none\. Each entry is one message on one
            line, without the line wrapping the curl tool applies at the terminal width \(`COLUMNS`\)\.

### Remarks

Warnings point at a command that probably does not do what its author meant. Examples, in curl's words:
            `Warning: --fail deselects --fail-with-body here`, `Warning: --follow overrides --location`,
            `Warning: Got more output options than URLs`, `Warning: The argument '“host:' starts with a Unicode
            character. Maybe ASCII was intended?` (a typographic quote pasted from a web page) and, for a `-K` file,
            `Warning: app.curlrc:3 Option 'data' uses argument with unquoted whitespace. This may cause side-effects. Consider
            double quotes.` Like curl, a warning found while the command line is read is left out when
            `-s`/`--silent` was already given at that point (so `curl -H “x: y” -s URL` still warns); the ones
            curl prints while it sets up or runs a transfer, such as "Got more output options than URLs", follow the final
            `-s` state.

Notes are informational and, like curl's, only collected when the command asks for `-v`,
            `--trace` or `--trace-ascii` and not `-s`: `Note: Unnecessary use of -X or --request, POST is
            already inferred.`, `Note: skips transfer, "out.json" exists locally` (`--skip-existing`) and
            `Note: Read config file from '...'`.

CurlDotNet never prints warnings to the console by itself. When the command gives `--stderr FILE`,
            warnings and notes are appended to that file as the curl tool would write them (wrapped at the `COLUMNS`
            width, 79 when unset); without it, notes go to [System\.Console\.Error](https://learn.microsoft.com/en-us/dotnet/api/system.console.error 'System\.Console\.Error') because the command asked for verbose
            output, the same place `-w '%{stderr}...'` text goes.

Example: `var r = await Curl.ExecuteAsync("curl -f --fail-with-body https://example.com"); foreach (var w in
            r.Warnings) Console.Error.WriteLine(w);` prints `Warning: --fail-with-body deselects --fail here`.

### See Also
- [Warnings](CurlDotNet.Exceptions.CurlException.Warnings.md 'CurlDotNet\.Exceptions\.CurlException\.Warnings')
- [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command')