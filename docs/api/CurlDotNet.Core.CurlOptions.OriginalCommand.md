#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.OriginalCommand Property

The curl command text these options were parsed from \(or an equivalent command built by the fluent builder\),
kept for logging, error messages and [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command')\.

```csharp
public string? OriginalCommand { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The command string, or null for options you built yourself\.

### Remarks

It is copied into every exception's [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command'). It may contain
            secrets (`-u user:password`, tokens), so treat it like the credentials themselves when logging. A non-null
            value also marks the options as "parsed", which makes [HeadOnly](CurlDotNet.Core.CurlOptions.HeadOnly.md 'CurlDotNet\.Core\.CurlOptions\.HeadOnly') print headers like the curl tool.

Example: `Curl.Validate("curl -I https://example.com").ParsedOptions.OriginalCommand` is `"curl -I https://example.com"`.

### See Also
- [Flavor](CurlDotNet.Core.CurlOptions.Flavor.md 'CurlDotNet\.Core\.CurlOptions\.Flavor')
- [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command')
- [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)')