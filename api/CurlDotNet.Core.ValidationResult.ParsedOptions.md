#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult')

## ValidationResult\.ParsedOptions Property

The options the command parsed into \(URL, method, headers, data, flags\), for inspection or for
running later with [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)')\.

```csharp
public CurlDotNet.Core.CurlOptions? ParsedOptions { get; set; }
```

#### Property Value
[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')  
Set when the command is valid and also when its only problem is unknown options; null when
            there is no URL, the scheme is unsupported, the URL is malformed or parsing failed\.

### Remarks

Example: `var opts = Curl.Validate("curl -X POST -d a=1 https://example.com").ParsedOptions;`
            gives `opts.Method == "POST"` and `opts.Data == "a=1"`.

### See Also
- [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')
- [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)')