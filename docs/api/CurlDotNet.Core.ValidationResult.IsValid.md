#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult')

## ValidationResult\.IsValid Property

True when the command parsed, has a URL, uses only known options and a supported scheme, so
executing it would at least start a transfer\.

```csharp
public bool IsValid { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
`true` for a runnable command; `false` with [Error](CurlDotNet.Core.ValidationResult.Error.md 'CurlDotNet\.Core\.ValidationResult\.Error') set otherwise\.

### Remarks

Example: `Curl.Validate("curl https://example.com").IsValid` is `true`;
            `Curl.Validate("curl --no-such-flag https://example.com").IsValid` is `false`.

### See Also
- [Error](CurlDotNet.Core.ValidationResult.Error.md 'CurlDotNet\.Core\.ValidationResult\.Error')