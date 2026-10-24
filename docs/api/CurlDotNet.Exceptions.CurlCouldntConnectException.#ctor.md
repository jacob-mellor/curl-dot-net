#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')

## CurlCouldntConnectException Constructors

| Overloads | |
| :--- | :--- |
| [CurlCouldntConnectException\(string, int, string\)](CurlDotNet.Exceptions.CurlCouldntConnectException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string) 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.CurlCouldntConnectException\(string, int, string\)') | Creates the error with curl's wording `Failed to connect to HOST port PORT` and advice to check the service, port and firewall\. |
| [CurlCouldntConnectException\(string, int, string, bool, string\)](CurlDotNet.Exceptions.CurlCouldntConnectException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string) 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.CurlCouldntConnectException\(string, int, string, bool, string\)') | Creates the error with the socket layer's reason and whether the unreachable host was the server or the proxy, which changes the advice\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string)'></a>

## CurlCouldntConnectException\(string, int, string\) Constructor

Creates the error with curl's wording `Failed to connect to HOST port PORT` and advice to check the
service, port and firewall\.

```csharp
public CurlCouldntConnectException(string host, int port, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string).host'></a>

`host` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host that could not be connected to\.

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string).port'></a>

`port` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The port number that was attempted\.

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred, or null\.

### Remarks

Example: `throw new CurlCouldntConnectException("db.internal", 5432, command);`

### See Also
- [CurlCouldntConnectException\(string, int, string, bool, string\)](CurlDotNet.Exceptions.CurlCouldntConnectException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string) 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.CurlCouldntConnectException\(string, int, string, bool, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string)'></a>

## CurlCouldntConnectException\(string, int, string, bool, string\) Constructor

Creates the error with the socket layer's reason and whether the unreachable host was the server or the proxy,
which changes the advice\.

```csharp
public CurlCouldntConnectException(string host, int port, string detail, bool viaProxy, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string).host'></a>

`host` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host that refused or did not answer \(the proxy when [viaProxy](CurlDotNet.Exceptions.CurlCouldntConnectException.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string).viaProxy 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.CurlCouldntConnectException\(string, int, string, bool, string\)\.viaProxy') is true\)\.

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string).port'></a>

`port` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The port that was tried\.

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string).detail'></a>

`detail` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the socket layer reported, such as `Connection refused`\.

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string).viaProxy'></a>

`viaProxy` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

True when [host](CurlDotNet.Exceptions.CurlCouldntConnectException.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string).host 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.CurlCouldntConnectException\(string, int, string, bool, string\)\.host') is the proxy rather than the URL's host\.

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

When a proxy is in use (`-x`, `--socks5` or a proxy environment variable) curl connects to the
            proxy, not to the URL's host, so [host](CurlDotNet.Exceptions.CurlCouldntConnectException.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string).host 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.CurlCouldntConnectException\(string, int, string, bool, string\)\.host') and [port](CurlDotNet.Exceptions.CurlCouldntConnectException.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string).port 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.CurlCouldntConnectException\(string, int, string, bool, string\)\.port') name the proxy and
            [ViaProxy](CurlDotNet.Exceptions.CurlCouldntConnectException.ViaProxy.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.ViaProxy') is true.

### See Also
- [CurlCouldntConnectException\(string, int, string\)](CurlDotNet.Exceptions.CurlCouldntConnectException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string) 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.CurlCouldntConnectException\(string, int, string\)')