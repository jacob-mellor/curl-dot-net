#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithBearerToken\(string\) Method

Sends `Authorization: Bearer TOKEN` \(OAuth 2 / JWT\) with every request from this client, like
`curl -H 'Authorization: Bearer TOKEN'` or `--oauth2-bearer TOKEN`\.

```csharp
public CurlDotNet.Lib.LibCurl WithBearerToken(string token);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithBearerToken(string).token'></a>

`token` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The access token, such as `"eyJhbGciOi..."` or `"ghp_..."`\.

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

### Remarks

Pass the token alone, without the word `Bearer`. Calling it again replaces the token, so a
            refreshed token can be set on the same instance (do it between requests, not while they run).

Example: `using var github = new LibCurl().WithBearerToken(Environment.GetEnvironmentVariable("GITHUB_TOKEN")).WithUserAgent("MyApp/1.0");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md

### See Also
- [WithBearerToken\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBearerToken\(string\)')
- [WithBasicAuth\(string, string\)](CurlDotNet.Lib.LibCurl.WithBasicAuth(string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithBasicAuth\(string, string\)')