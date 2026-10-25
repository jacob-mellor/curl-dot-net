#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithInsecureSsl\(\) Method

Skips TLS certificate verification for every request \(curl's `-k` / `--insecure`\), accepting
self\-signed and expired certificates\. Development and testing only\.

```csharp
public CurlDotNet.Lib.LibCurl WithInsecureSsl();
```

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

### Remarks

With verification off anyone on the network path can impersonate the server; never ship it. Without it,
            an untrusted certificate gives a [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException') (exit code 60) in
            [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception').

Example: `using var local = new LibCurl().WithInsecureSsl();` then
            `await local.GetAsync("https://localhost:5001/health");`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md

### See Also
- [Insecure\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Insecure(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Insecure\(bool\)')
- [Insecure](CurlDotNet.Core.CurlOptions.Insecure.md 'CurlDotNet\.Core\.CurlOptions\.Insecure')