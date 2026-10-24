#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRedirectException](CurlDotNet.Exceptions.CurlRedirectException.md 'CurlDotNet\.Exceptions\.CurlRedirectException')

## CurlRedirectException\(string, int, string, string\) Constructor

Creates a too\-many\-redirects error \(exit code 47\) with the number of hops and the last URL, which the advice in
the message uses\.

```csharp
public CurlRedirectException(string message, int redirectCount, string lastUrl=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What happened, naming the start URL; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string).redirectCount'></a>

`redirectCount` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of redirects that were followed\.

<a name='CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string).lastUrl'></a>

`lastUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The last URL in the redirect chain, or null\.

<a name='CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

Example: `throw new CurlRedirectException("Login keeps redirecting.", 20, lastUrl, command);`

### See Also
- [CurlTooManyRedirectsException\(int, int, string\)](CurlDotNet.Exceptions.CurlTooManyRedirectsException.#ctor.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,int,string) 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.CurlTooManyRedirectsException\(int, int, string\)')