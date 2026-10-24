#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithAuth\(string\) Method

Sets the whole `Authorization` header value yourself, for schemes other than Basic and Bearer
\(for example `"Token abc123"`, `"ApiKey xyz"` or `"AWS4-HMAC-SHA256 ..."`\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithAuth(string authHeader);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithAuth(string).authHeader'></a>

`authHeader` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The complete header value including the scheme name, such as `"Token abc123"`\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

The value is sent exactly as given, equivalent to `curl -H 'Authorization: VALUE'`, and replaces
            any value set by [WithBearerToken\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBearerToken\(string\)').

Example: `CurlRequestBuilder.Get(url).WithAuth("Token 9944b09199c62bcf")` for Django REST
            Framework token authentication.

Guide to the schemes: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md

### See Also
- [WithBearerToken\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBearerToken\(string\)')
- [WithHeader\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)')