#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithBearerToken\(string\) Method

Sends OAuth 2 / JWT bearer authentication: the header `Authorization: Bearer TOKEN`
\(curl's `-H 'Authorization: Bearer TOKEN'` or `--oauth2-bearer TOKEN`\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithBearerToken(string token);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string).token'></a>

`token` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The access token, such as `"eyJhbGciOi..."` or `"ghp_..."`\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Pass the token alone, without the word `Bearer`. It replaces any Authorization header set
            earlier (including one from [WithAuth\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithAuth(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithAuth\(string\)')). Expired tokens usually give a 401 result; refresh
            the token and build the request again.

Example: `CurlRequestBuilder.Get("https://api.github.com/user").WithBearerToken(Environment.GetEnvironmentVariable("GITHUB_TOKEN"))`
            is `curl -H 'Authorization: Bearer ghp_...' https://api.github.com/user`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md

### See Also
- [WithAuth\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithAuth(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithAuth\(string\)')
- [WithBasicAuth\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBasicAuth\(string, string\)')
- [WithBearerToken\(string\)](CurlDotNet.Lib.LibCurl.WithBearerToken(string).md 'CurlDotNet\.Lib\.LibCurl\.WithBearerToken\(string\)')