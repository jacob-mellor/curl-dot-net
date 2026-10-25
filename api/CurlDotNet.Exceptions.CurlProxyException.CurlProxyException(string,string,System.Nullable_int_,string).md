#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')

## CurlProxyException\(string, string, Nullable\<int\>, string\) Constructor

Creates a proxy failure \(exit code 5\) naming the proxy host and port, with advice to check the proxy setting
or bypass it\.

```csharp
public CurlProxyException(string message, string proxyHost, System.Nullable<int> proxyPort=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What failed with the proxy; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).proxyHost'></a>

`proxyHost` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The proxy host name or IP address\.

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).proxyPort'></a>

`proxyPort` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The proxy port number, or null when none was given\.

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed, or null\.

### Remarks

Example: `throw new CurlProxyException("Proxy proxy.corp did not answer.", "proxy.corp", 3128, command);`

### See Also
- [CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException')