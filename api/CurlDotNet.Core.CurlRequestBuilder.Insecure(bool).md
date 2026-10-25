#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Insecure\(bool\) Method

Skips TLS certificate verification \(curl's `-k` / `--insecure`\) so self\-signed or expired
certificates are accepted\. For local development and testing only\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder Insecure(bool insecure=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Insecure(bool).insecure'></a>

`insecure` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

True \(the default\) to skip verification; false to verify \(the normal behaviour\)\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

With verification off anyone on the network path can impersonate the server; never ship this in
            production code. The better fix for an internal CA is to trust its certificate (`--cacert` in a
            command string). Without this option an untrusted certificate fails with
            [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException') (exit code 60).

Example: `CurlRequestBuilder.Get("https://localhost:5001/api/health").Insecure()` is
            `curl -k https://localhost:5001/api/health`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md

### See Also
- [Insecure](CurlDotNet.Core.CurlOptions.Insecure.md 'CurlDotNet\.Core\.CurlOptions\.Insecure')
- [WithInsecureSsl\(\)](CurlDotNet.Lib.LibCurl.WithInsecureSsl().md 'CurlDotNet\.Lib\.LibCurl\.WithInsecureSsl\(\)')