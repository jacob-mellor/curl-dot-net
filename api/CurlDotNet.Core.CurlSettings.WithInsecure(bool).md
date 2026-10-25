#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithInsecure\(bool\) Method

Turns TLS certificate verification off \(curl `-k`\) or back on, and returns these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithInsecure(bool insecure=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithInsecure(bool).insecure'></a>

`insecure` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

true \(default\) to skip verification; false to verify certificates\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Sets [Insecure](CurlDotNet.Core.CurlSettings.Insecure.md 'CurlDotNet\.Core\.CurlSettings\.Insecure'). Use only for development against self-signed certificates; in production
            trust the CA instead.

Example: `await Curl.ExecuteAsync("curl https://localhost:5001/health", new CurlSettings().WithInsecure());`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md

### See Also
- [Insecure](CurlDotNet.Core.CurlSettings.Insecure.md 'CurlDotNet\.Core\.CurlSettings\.Insecure')
- [Insecure\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Insecure(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Insecure\(bool\)')