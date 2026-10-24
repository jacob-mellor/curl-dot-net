#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException')

## CurlCouldntResolveProxyException Constructors

| Overloads | |
| :--- | :--- |
| [CurlCouldntResolveProxyException\(string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException\.CurlCouldntResolveProxyException\(string, string\)') | Creates the error with curl's wording `Could not resolve proxy: HOST` and advice to check the proxy setting\. |
| [CurlCouldntResolveProxyException\(string, string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException\.CurlCouldntResolveProxyException\(string, string, string\)') | Creates the error with the resolver's explanation of why the proxy name failed, for example "nodename nor servname provided"\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string)'></a>

## CurlCouldntResolveProxyException\(string, string\) Constructor

Creates the error with curl's wording `Could not resolve proxy: HOST` and advice to check the proxy
setting\.

```csharp
public CurlCouldntResolveProxyException(string proxyHost, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string).proxyHost'></a>

`proxyHost` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The proxy hostname that could not be resolved\.

<a name='CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred, or null\.

### Remarks

Example: `throw new CurlCouldntResolveProxyException("proxy.corp.local", command);`

### See Also
- [CurlCouldntResolveProxyException\(string, string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException\.CurlCouldntResolveProxyException\(string, string, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string,string)'></a>

## CurlCouldntResolveProxyException\(string, string, string\) Constructor

Creates the error with the resolver's explanation of why the proxy name failed, for example "nodename nor
servname provided"\.

```csharp
public CurlCouldntResolveProxyException(string proxyHost, string detail, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string,string).proxyHost'></a>

`proxyHost` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The proxy host name \(from `-x`/`--proxy` or a proxy environment variable\)\.

<a name='CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string,string).detail'></a>

`detail` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the resolver reported, in its own words\.

<a name='CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

The resolver's text is shown in parentheses after the proxy name.

### See Also
- [CurlCouldntResolveProxyException\(string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException\.CurlCouldntResolveProxyException\(string, string\)')