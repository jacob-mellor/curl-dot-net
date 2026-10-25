#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithBasicAuth\(string, string\) Method

Sends HTTP Basic credentials \(curl's `-u user:password`\) with every request that does not set
its own [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials')\.

```csharp
public CurlDotNet.Lib.LibCurl WithBasicAuth(string username, string password);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithBasicAuth(string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The user name \(may contain `@`\)\.

<a name='CurlDotNet.Lib.LibCurl.WithBasicAuth(string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The password or API token; may be empty\.

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

### Remarks

Basic auth is only base64-encoded, so use it over `https://`. As in curl, the credentials are not
            sent to a different host after a redirect. A wrong password gives a result with `StatusCode == 401`.

Example: `using var jenkins = new LibCurl().WithBasicAuth("ci-bot", apiToken);`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-basic-auth-csharp/README.md

### See Also
- [WithBasicAuth\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBasicAuth\(string, string\)')
- [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials')