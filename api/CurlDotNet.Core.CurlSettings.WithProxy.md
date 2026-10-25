#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithProxy Method

| Overloads | |
| :--- | :--- |
| [WithProxy\(string\)](CurlDotNet.Core.CurlSettings.WithProxy.md#CurlDotNet.Core.CurlSettings.WithProxy(string) 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string\)') | Sends the request through the proxy at [proxyUrl](CurlDotNet.Core.CurlSettings.md#CurlDotNet.Core.CurlSettings.WithProxy(string).proxyUrl 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string\)\.proxyUrl') \(sets [Proxy](CurlDotNet.Core.CurlSettings.Proxy.md 'CurlDotNet\.Core\.CurlSettings\.Proxy') to a [System\.Net\.WebProxy](https://learn.microsoft.com/en-us/dotnet/api/system.net.webproxy 'System\.Net\.WebProxy')\), like curl's `-x URL`, and returns these settings for chaining\. |
| [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlSettings.WithProxy.md#CurlDotNet.Core.CurlSettings.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string, string, string\)') | Sends the request through the proxy at [proxyUrl](CurlDotNet.Core.CurlSettings.md#CurlDotNet.Core.CurlSettings.WithProxy(string,string,string).proxyUrl 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string, string, string\)\.proxyUrl') with Basic proxy credentials, like curl's `-x URL -U user:pass`, and returns these settings for chaining\. |

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string)'></a>

## CurlSettings\.WithProxy\(string\) Method

Sends the request through the proxy at [proxyUrl](CurlDotNet.Core.CurlSettings.md#CurlDotNet.Core.CurlSettings.WithProxy(string).proxyUrl 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string\)\.proxyUrl') \(sets [Proxy](CurlDotNet.Core.CurlSettings.Proxy.md 'CurlDotNet\.Core\.CurlSettings\.Proxy') to a
[System\.Net\.WebProxy](https://learn.microsoft.com/en-us/dotnet/api/system.net.webproxy 'System\.Net\.WebProxy')\), like curl's `-x URL`, and returns these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithProxy(string proxyUrl);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute proxy URL such as `http://proxy.local:3128`\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

#### Exceptions

[CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException')  
[proxyUrl](CurlDotNet.Core.CurlSettings.md#CurlDotNet.Core.CurlSettings.WithProxy(string).proxyUrl 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string\)\.proxyUrl') is null or not a valid URL \(exit code 3\);
            write it as `http://host:port`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

### Remarks

The URL is validated immediately and replaces a `-x` in the command.

Example: `settings.WithProxy("http://proxy.local:3128")`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md

### See Also
- [Proxy](CurlDotNet.Core.CurlSettings.Proxy.md 'CurlDotNet\.Core\.CurlSettings\.Proxy')
- [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlSettings.WithProxy.md#CurlDotNet.Core.CurlSettings.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string, string, string\)')
- [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy')

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string,string,string)'></a>

## CurlSettings\.WithProxy\(string, string, string\) Method

Sends the request through the proxy at [proxyUrl](CurlDotNet.Core.CurlSettings.md#CurlDotNet.Core.CurlSettings.WithProxy(string,string,string).proxyUrl 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string, string, string\)\.proxyUrl') with Basic proxy credentials, like curl's
`-x URL -U user:pass`, and returns these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithProxy(string proxyUrl, string username, string password);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string,string,string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute proxy URL such as `http://proxy.corp:8080`\.

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string,string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy user name \(curl `-U user:...`\)\.

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string,string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy password\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

#### Exceptions

[CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException')  
[proxyUrl](CurlDotNet.Core.CurlSettings.md#CurlDotNet.Core.CurlSettings.WithProxy(string,string,string).proxyUrl 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string, string, string\)\.proxyUrl') is null or not a valid URL \(exit code 3\)\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

### Remarks

The credentials are sent preemptively as `Proxy-Authorization: Basic ...` (never logged or put into
            exception messages).

Example: `settings.WithProxy("http://proxy.corp:8080", "alice", secret)`.

### See Also
- [WithProxy\(string\)](CurlDotNet.Core.CurlSettings.WithProxy.md#CurlDotNet.Core.CurlSettings.WithProxy(string) 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string\)')
- [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials')