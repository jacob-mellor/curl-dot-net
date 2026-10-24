#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ProxyAuthScheme Property

How [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials') are presented to the proxy: `basic`, `digest`, `ntlm`,
`negotiate` or `anyauth`, set by curl's `--proxy-basic`, `--proxy-digest`, `--proxy-ntlm`,
`--proxy-negotiate` and `--proxy-anyauth`\.

```csharp
public string? ProxyAuthScheme { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A lower\-case scheme name\. Default: null, which means Basic\.

### Remarks

With Basic (or null) the `Proxy-Authorization` header is sent on the first request, exactly like curl; the
            other schemes wait for the proxy's `407` challenge and answer it. A challenge that cannot be processed fails with
            [CurlAuthErrorException](CurlDotNet.Exceptions.CurlAuthErrorException.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException') (exit 94).

Example: `curl -x proxy:3128 -U alice:secret --proxy-digest http://example.com/` is
            `new CurlOptions { Url = url, Proxy = "proxy:3128", ProxyCredentials = new NetworkCredential("alice", "secret"), ProxyAuthScheme = "digest" }`.

### See Also
- [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials')
- [AuthScheme](CurlDotNet.Core.CurlOptions.AuthScheme.md 'CurlDotNet\.Core\.CurlOptions\.AuthScheme')
- [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy')