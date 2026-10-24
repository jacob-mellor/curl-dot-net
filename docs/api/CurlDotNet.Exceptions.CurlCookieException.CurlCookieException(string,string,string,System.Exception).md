#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCookieException](CurlDotNet.Exceptions.CurlCookieException.md 'CurlDotNet\.Exceptions\.CurlCookieException')

## CurlCookieException\(string, string, string, Exception\) Constructor

Initializes a new instance of the [CurlCookieException](CurlDotNet.Exceptions.CurlCookieException.md 'CurlDotNet\.Exceptions\.CurlCookieException') class\.

```csharp
public CurlCookieException(string message, string cookieJarPath=null, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the cookie operation failure\.

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).cookieJarPath'></a>

`cookieJarPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The path to the cookie jar file, if applicable\.

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing\.

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The underlying exception that caused this error\.