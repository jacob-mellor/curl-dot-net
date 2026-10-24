#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.RetryMaxTime Property

Stops retrying once this many seconds have passed since the first attempt \(or when the server's
`Retry-After` wait would pass it\), like curl's `--retry-max-time`; the last attempt's outcome then
stands: its result is returned, or its error thrown\.

```csharp
public int RetryMaxTime { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Whole seconds\. Default: 0 \(no limit beyond [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry')\)\.

### Remarks

Example: `curl --retry 100 --retry-max-time 60 URL` is `new CurlOptions { Url = url, Retry = 100, RetryMaxTime = 60 }`.

### See Also
- [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry')
- [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime')