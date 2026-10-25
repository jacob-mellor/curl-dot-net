#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult')

## ValidationResult\.Error Property

Why the command is not valid, worded for the person who wrote it; null when [IsValid](CurlDotNet.Core.ValidationResult.IsValid.md 'CurlDotNet\.Core\.ValidationResult\.IsValid') is true\.

```csharp
public string? Error { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
One of: `No URL specified`; `Unknown option: --a, --b` \(every unknown option, comma
            separated\); `Unsupported protocol: gopher`; `Invalid URL: ...`; or, when parsing itself
            failed \(an unbalanced quote, an option without its value\), the parser's exception message, which
            spans several lines: what is wrong, how to fix it and a `See https://...` documentation link\.

### Remarks

Show it as-is to the user, or take the first line for a short status bar.

Example: `Curl.Validate("curl -H").Error` explains that `-H` needs a header value.

### See Also
- [IsValid](CurlDotNet.Core.ValidationResult.IsValid.md 'CurlDotNet\.Core\.ValidationResult\.IsValid')