#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Silent Property

Suppresses the warnings curl would print on standard error, like curl's `-s`/`--silent`; errors
are still thrown as exceptions, so this never hides a failure\.

```csharp
public bool Silent { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for silent mode\. Default: false\.

### Remarks

CurlDotNet draws no progress meter, so the only visible effect is that warnings (for example
            `--etag-save` folder missing, skipping the transfer) are not written to standard error.

Example: `curl -s https://example.com/` is `new CurlOptions { Url = "https://example.com/", Silent = true }`.

### See Also
- [ShowError](CurlDotNet.Core.CurlOptions.ShowError.md 'CurlDotNet\.Core\.CurlOptions\.ShowError')
- [Silent\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Silent(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Silent\(bool\)')