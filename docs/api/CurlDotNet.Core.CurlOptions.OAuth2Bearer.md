#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.OAuth2Bearer Property

OAuth 2\.0 bearer token sent as `Authorization: Bearer TOKEN`, like curl's `--oauth2-bearer TOKEN`;
the token is only sent to the original host, not to redirect targets elsewhere\.

```csharp
public string? OAuth2Bearer { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The token text without the `Bearer ` prefix\. Default: null\.

### Remarks

The token is sent verbatim and takes precedence over Basic credentials from `-u` or the URL, and over
            [AwsSigV4](CurlDotNet.Core.CurlOptions.AwsSigV4.md 'CurlDotNet\.Core\.CurlOptions\.AwsSigV4'), as in curl. An explicit `-H "Authorization: ..."` header still wins over it. After a
            redirect to another host, port or scheme it is dropped unless [LocationTrusted](CurlDotNet.Core.CurlOptions.LocationTrusted.md 'CurlDotNet\.Core\.CurlOptions\.LocationTrusted') is true.

Example: `curl --oauth2-bearer eyJhbGciOi... https://api.example.com/me` is
            `new CurlOptions { Url = "https://api.example.com/me", OAuth2Bearer = token }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md

### See Also
- [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials')
- [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers')
- [WithBearerToken\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBearerToken\(string\)')