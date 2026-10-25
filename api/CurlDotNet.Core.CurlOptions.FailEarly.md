#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.FailEarly Property

Stops a multi\-URL command at the first failed transfer instead of running the rest, like curl's
`--fail-early`; without it a failure only ends the command when it is the last transfer\.

```csharp
public bool FailEarly { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to stop at the first failure\. Default: false\.

### Remarks

Only matters when the command has [AdditionalTransfers](CurlDotNet.Core.CurlOptions.AdditionalTransfers.md 'CurlDotNet\.Core\.CurlOptions\.AdditionalTransfers') (several URLs, globs or `--next`). Fatal errors
            (unknown option, bad argument, out of memory) stop the command regardless.

Example: `curl --fail-early -f https://a.example/ https://b.example/` does not request `b` when `a` returns 404.

### See Also
- [AdditionalTransfers](CurlDotNet.Core.CurlOptions.AdditionalTransfers.md 'CurlDotNet\.Core\.CurlOptions\.AdditionalTransfers')
- [FailOnError](CurlDotNet.Core.CurlOptions.FailOnError.md 'CurlDotNet\.Core\.CurlOptions\.FailOnError')