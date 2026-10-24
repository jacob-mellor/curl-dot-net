#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlAuthErrorException](CurlDotNet.Exceptions.CurlAuthErrorException.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException')

## CurlAuthErrorException\(string, bool, string, string\) Constructor

Creates the authentication\-negotiation error \(exit code 94\), naming the scheme, whether it was the proxy, and
what the authentication code reported\.

```csharp
public CurlAuthErrorException(string scheme, bool proxy, string detail, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlAuthErrorException.CurlAuthErrorException(string,bool,string,string).scheme'></a>

`scheme` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The scheme that failed: digest, ntlm, negotiate or anyauth\.

<a name='CurlDotNet.Exceptions.CurlAuthErrorException.CurlAuthErrorException(string,bool,string,string).proxy'></a>

`proxy` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

True when the failing negotiation was with the proxy rather than the server\.

<a name='CurlDotNet.Exceptions.CurlAuthErrorException.CurlAuthErrorException(string,bool,string,string).detail'></a>

`detail` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the authentication code reported, in words\.

<a name='CurlDotNet.Exceptions.CurlAuthErrorException.CurlAuthErrorException(string,bool,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

The fix suggests `--basic`/`--proxy-basic` when the server also offers Basic, and `-v` to see
            the challenge. Example: `throw new CurlAuthErrorException("digest", false, "the challenge has two realm= parameters", command);`

### See Also
- [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException')