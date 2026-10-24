#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithBasicAuth\(string, string\) Method

Sends HTTP Basic authentication \(curl's `-u user:password`\): an
`Authorization: Basic base64(user:password)` header on every request, redirects to the same host included\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithBasicAuth(string username, string password);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The user name \(may contain `@`, for example an email address\)\.

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The password; may be empty\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Basic auth sends the password base64-encoded, not encrypted; only use it over `https://`.
            The credentials are sent preemptively (curl's default); for Digest or NTLM use a command string with
            `--digest` or `--ntlm`.

[ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') prints `-u 'user:password'` with the password in clear text, so
            do not log that string in production.

Example: `CurlRequestBuilder.Get("https://api.example.com/me").WithBasicAuth("ada", "s3cret")`
            is `curl -u 'ada:s3cret' https://api.example.com/me`; a wrong password gives a result with
            `StatusCode == 401`, not an exception.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-basic-auth-csharp/README.md

### See Also
- [WithBearerToken\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBearerToken\(string\)')
- [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials')
- [WithBasicAuth\(string, string\)](CurlDotNet.Lib.LibCurl.WithBasicAuth(string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithBasicAuth\(string, string\)')