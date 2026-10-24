#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.AuthScheme Property

Which HTTP authentication scheme [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials') use: `basic`, `digest`, `ntlm`,
`negotiate` or `anyauth`, set by curl's `--basic`, `--digest`, `--ntlm`, `--negotiate`, `--anyauth`\.

```csharp
public string? AuthScheme { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A lower\-case scheme name\. Default: null, which means curl's default: preemptive Basic when credentials are present\.

### Remarks

Challenge-based schemes (digest, ntlm, negotiate, anyauth) send the first request without credentials and answer
            the server's `401 WWW-Authenticate` challenge, so the password never travels in Basic form. `anyauth` picks
            the most secure scheme the server offers. A challenge the scheme cannot process fails with
            [CurlAuthErrorException](CurlDotNet.Exceptions.CurlAuthErrorException.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException') (exit 94); a wrong password simply returns 401.

Example: `curl --digest -u ada:s3cret URL` is
            `new CurlOptions { Url = url, Credentials = new NetworkCredential("ada", "s3cret"), AuthScheme = "digest" }`.
            Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md

### See Also
- [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials')
- [ProxyAuthScheme](CurlDotNet.Core.CurlOptions.ProxyAuthScheme.md 'CurlDotNet\.Core\.CurlOptions\.ProxyAuthScheme')