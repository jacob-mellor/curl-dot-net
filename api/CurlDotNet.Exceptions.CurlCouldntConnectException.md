#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlCouldntConnectException Class

The TCP connection to the server or proxy failed \(exit code 7, CURLE\_COULDNT\_CONNECT\): refused, unreachable,
or blocked; [Host](CurlDotNet.Exceptions.CurlCouldntConnectException.Host.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.Host'), [Port](CurlDotNet.Exceptions.CurlCouldntConnectException.Port.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.Port') and [ViaProxy](CurlDotNet.Exceptions.CurlCouldntConnectException.ViaProxy.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.ViaProxy') say where\.

```csharp
public class CurlCouldntConnectException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlCouldntConnectException

### Remarks

The name resolved but nothing accepted the connection. Usual causes: the service is not running or listens on
            another port, a firewall drops the traffic, the URL uses `http` on an `https`-only port (or the reverse),
            or a proxy is down. Retrying may help (`--retry 3 --retry-connrefused`).

Example: `catch (CurlCouldntConnectException ex) when (ex.ViaProxy) { Console.WriteLine($"Proxy {ex.Host}:{ex.Port} is down"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md

### See Also
- [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')
- [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException')

| Constructors | |
| :--- | :--- |
| [CurlCouldntConnectException\(string, int, string\)](CurlDotNet.Exceptions.CurlCouldntConnectException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string) 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.CurlCouldntConnectException\(string, int, string\)') | Creates the error with curl's wording `Failed to connect to HOST port PORT` and advice to check the service, port and firewall\. |
| [CurlCouldntConnectException\(string, int, string, bool, string\)](CurlDotNet.Exceptions.CurlCouldntConnectException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string,bool,string) 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.CurlCouldntConnectException\(string, int, string, bool, string\)') | Creates the error with the socket layer's reason and whether the unreachable host was the server or the proxy, which changes the advice\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlCouldntConnectException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlCouldntConnectException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Host](CurlDotNet.Exceptions.CurlCouldntConnectException.Host.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.Host') | The host that refused or did not answer: the URL's host, or the proxy's host when [ViaProxy](CurlDotNet.Exceptions.CurlCouldntConnectException.ViaProxy.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.ViaProxy') is true\. |
| [Port](CurlDotNet.Exceptions.CurlCouldntConnectException.Port.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.Port') | The TCP port that was tried: the URL's port \(80/443 by default\) or the proxy's port when [ViaProxy](CurlDotNet.Exceptions.CurlCouldntConnectException.ViaProxy.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.ViaProxy') is true\. |
| [ViaProxy](CurlDotNet.Exceptions.CurlCouldntConnectException.ViaProxy.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.ViaProxy') | True when the failed connection was to a proxy rather than to the URL's host, so the proxy setting \(not the server\) needs fixing\. |
