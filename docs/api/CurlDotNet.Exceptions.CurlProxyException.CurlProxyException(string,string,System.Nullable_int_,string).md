#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')

## CurlProxyException\(string, string, Nullable\<int\>, string\) Constructor

Initializes a new instance of the [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException') class

```csharp
public CurlProxyException(string message, string proxyHost, System.Nullable<int> proxyPort=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the proxy connection failure

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).proxyHost'></a>

`proxyHost` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The proxy hostname or IP address

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).proxyPort'></a>

`proxyPort` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The proxy port number, if applicable

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed