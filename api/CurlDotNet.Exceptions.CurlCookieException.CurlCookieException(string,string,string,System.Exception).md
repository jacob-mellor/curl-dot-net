#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCookieException](CurlDotNet.Exceptions.CurlCookieException.md 'CurlDotNet\.Exceptions\.CurlCookieException')

## CurlCookieException\(string, string, string, Exception\) Constructor

Creates a cookie error naming the cookie file, keeping the original I/O or parsing exception as the inner
exception\.

```csharp
public CurlCookieException(string message, string cookieJarPath=null, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What failed with the cookies; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).cookieJarPath'></a>

`cookieJarPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The path to the cookie file, or null when no file is involved\.

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The underlying exception that caused this error, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).md#CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).message 'CurlDotNet\.Exceptions\.CurlCookieException\.CurlCookieException\(string, string, string, System\.Exception\)\.message') has no fix, the advice names the file and its folder; the link points at the
            cookie page. Example: `throw new CurlCookieException("Cookie jar could not be saved.", "state/cookies.txt", command, ioException);`

### See Also
- [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')