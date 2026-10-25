#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.RetryConnRefused Property

Also treats "connection refused" \(exit 7\) as a transient error worth retrying, for services that are still
starting up; curl's `--retry-connrefused`\.

```csharp
public bool RetryConnRefused { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to retry refused connections\. Default: false\. Only used when [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry') is above 0\.

### Remarks

Example: waiting for a container: `curl --retry 10 --retry-connrefused http://localhost:8080/health` is
            `new CurlOptions { Url = url, Retry = 10, RetryConnRefused = true }`.

### See Also
- [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry')
- [RetryAllErrors](CurlDotNet.Core.CurlOptions.RetryAllErrors.md 'CurlDotNet\.Core\.CurlOptions\.RetryAllErrors')