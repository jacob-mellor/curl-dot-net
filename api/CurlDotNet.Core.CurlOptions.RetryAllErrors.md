#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.RetryAllErrors Property

Retries on every failure \(any HTTP error with `-f`, refused connections, DNS errors\.\.\.\), not only the
transient ones curl retries by default; curl's `--retry-all-errors`\.

```csharp
public bool RetryAllErrors { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to retry everything\. Default: false\. Only used when [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry') is above 0\.

### Remarks

`CurlSettings.WithRetries` turns this on. Example: `curl --retry 3 --retry-all-errors URL` is
            `new CurlOptions { Url = url, Retry = 3, RetryAllErrors = true }`.

### See Also
- [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry')
- [RetryConnRefused](CurlDotNet.Core.CurlOptions.RetryConnRefused.md 'CurlDotNet\.Core\.CurlOptions\.RetryConnRefused')