#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.RetryAllErrors Property

`--retry-all-errors`: retry on every failure, not only on the transient ones curl
            retries by default \(timeouts and HTTP 408/429/500/502/503/504\)\.

```csharp
public bool RetryAllErrors { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')