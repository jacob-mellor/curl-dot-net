#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlCouldntResolveProxyException Class

CURLE\_COULDNT\_RESOLVE\_PROXY \(5\) \- Couldn't resolve proxy

```csharp
public class CurlCouldntResolveProxyException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlCouldntResolveProxyException
### Constructors

<a name='CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string)'></a>

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
### Properties

<a name='CurlDotNet.Exceptions.CurlCouldntResolveProxyException.ProxyHost'></a>

## CurlCouldntResolveProxyException\.ProxyHost Property

Gets the proxy host that could not be resolved

```csharp
public string ProxyHost { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')