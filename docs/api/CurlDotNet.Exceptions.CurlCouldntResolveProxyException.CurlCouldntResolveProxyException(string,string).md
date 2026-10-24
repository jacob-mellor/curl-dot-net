#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException')

## CurlCouldntResolveProxyException\(string, string\) Constructor

Initializes a new instance of the [CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException') class\.

```csharp
public CurlCouldntResolveProxyException(string proxyHost, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string).proxyHost'></a>

`proxyHost` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The proxy hostname that could not be resolved\.

<a name='CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.