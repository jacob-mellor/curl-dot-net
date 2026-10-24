#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Credentials Property

User name and password for server authentication, like curl's `-u user:password`; sent as Basic
authentication on the first request unless [AuthScheme](CurlDotNet.Core.CurlOptions.AuthScheme.md 'CurlDotNet\.Core\.CurlOptions\.AuthScheme') picks a challenge scheme\.

```csharp
public System.Net.NetworkCredential? Credentials { get; set; }
```

#### Property Value
[System\.Net\.NetworkCredential](https://learn.microsoft.com/en-us/dotnet/api/system.net.networkcredential 'System\.Net\.NetworkCredential')  
A [System\.Net\.NetworkCredential](https://learn.microsoft.com/en-us/dotnet/api/system.net.networkcredential 'System\.Net\.NetworkCredential')\. Default: null\. The parser splits `-u user:password` at the first
            colon; `-u user` gives an empty password \(curl would prompt for one\)\.

### Remarks

With no [AuthScheme](CurlDotNet.Core.CurlOptions.AuthScheme.md 'CurlDotNet\.Core\.CurlOptions\.AuthScheme') (or `basic`) an `Authorization: Basic` header is sent preemptively.
            `digest`, `ntlm`, `negotiate` and `anyauth` answer the server's `401` challenge instead.
            Credentials are not sent to another host after a redirect unless [LocationTrusted](CurlDotNet.Core.CurlOptions.LocationTrusted.md 'CurlDotNet\.Core\.CurlOptions\.LocationTrusted') is true. An explicit
            `Authorization` header or [OAuth2Bearer](CurlDotNet.Core.CurlOptions.OAuth2Bearer.md 'CurlDotNet\.Core\.CurlOptions\.OAuth2Bearer') wins over them. With [AwsSigV4](CurlDotNet.Core.CurlOptions.AwsSigV4.md 'CurlDotNet\.Core\.CurlOptions\.AwsSigV4') they are the
            access key and secret key. User info in [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url') (`https://user:pass@host/`) has the same effect.

Example: `curl -u ada:s3cret https://api.example.com/me` is
            `new CurlOptions { Url = "https://api.example.com/me", Credentials = new NetworkCredential("ada", "s3cret") }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-basic-auth-csharp/README.md

### See Also
- [AuthScheme](CurlDotNet.Core.CurlOptions.AuthScheme.md 'CurlDotNet\.Core\.CurlOptions\.AuthScheme')
- [OAuth2Bearer](CurlDotNet.Core.CurlOptions.OAuth2Bearer.md 'CurlDotNet\.Core\.CurlOptions\.OAuth2Bearer')
- [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials')
- [WithBasicAuth\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBasicAuth\(string, string\)')