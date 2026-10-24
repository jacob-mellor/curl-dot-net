#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.GetOptions\(\) Method

Returns a copy of the [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') this builder has assembled, to inspect it in tests or
to tweak options that have no builder method and run it with [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)')\.

```csharp
public CurlDotNet.Core.CurlOptions GetOptions();
```

#### Returns
[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')  
A new [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') with this builder's settings\.

### Remarks

The copy is independent: changing it does not change the builder. Every property set by the
            builder methods is copied (URL, method, headers, body, form fields and files, credentials, timeouts,
            redirects, proxy, cookies, output, trace, HTTP version, range and binary/text type lists).

Example: `var options = CurlRequestBuilder.Get(url).WithBearerToken(t).GetOptions(); options.Retry = 3; var result = await Curl.ExecuteAsync(options);`

### See Also
- [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')
- [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)')
- [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)')