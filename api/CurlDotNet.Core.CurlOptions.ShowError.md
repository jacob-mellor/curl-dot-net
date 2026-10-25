#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ShowError Property

curl's `-S`/`--show-error` \(print errors even with `-s`\); recorded for compatibility only,
because CurlDotNet reports every error as an exception anyway\.

```csharp
public bool ShowError { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when `-S` was given\. Default: false\. Not acted on\.

### Remarks

Example: `curl -sS URL` parses to `Silent = true, ShowError = true`; catch
            [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') to see the error.

### See Also
- [Silent](CurlDotNet.Core.CurlOptions.Silent.md 'CurlDotNet\.Core\.CurlOptions\.Silent')