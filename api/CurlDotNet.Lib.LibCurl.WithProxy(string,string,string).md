#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithProxy\(string, string, string\) Method

Set proxy for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithProxy(string proxyUrl, string username=null, string password=null);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy URL \(e\.g\., "http://proxy\.example\.com:8080"\)

<a name='CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Optional proxy username

<a name='CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Optional proxy password

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining