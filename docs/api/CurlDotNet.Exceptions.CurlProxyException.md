#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlProxyException Class

The proxy could not be used \(curl exit code 5, CURLE\_COULDNT\_RESOLVE\_PROXY\): its name did not resolve or it
could not be reached; [ProxyHost](CurlDotNet.Exceptions.CurlProxyException.ProxyHost.md 'CurlDotNet\.Exceptions\.CurlProxyException\.ProxyHost') and [ProxyPort](CurlDotNet.Exceptions.CurlProxyException.ProxyPort.md 'CurlDotNet\.Exceptions\.CurlProxyException\.ProxyPort') say which proxy\.

```csharp
public class CurlProxyException : CurlDotNet.Exceptions.CurlConnectionException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') &#129106; CurlProxyException

### Remarks

The transport reports proxy problems as [CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException') (5) and
            [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException') with `ViaProxy` set (7); this type is for application code.
            The proxy comes from `-x/--proxy` or the `http_proxy`, `https_proxy` and `all_proxy`
            environment variables.

Fixes: correct the proxy URL (`-x http://proxy.corp:3128`), check it is running, or bypass it for a host
            with `--noproxy host`. Example: `catch (CurlProxyException ex) { Console.WriteLine($"Proxy {ex.ProxyHost}:{ex.ProxyPort} is down"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md

### See Also
- [CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException')
- [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')

| Constructors | |
| :--- | :--- |
| [CurlProxyException\(string, string, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).md 'CurlDotNet\.Exceptions\.CurlProxyException\.CurlProxyException\(string, string, System\.Nullable\<int\>, string\)') | Creates a proxy failure \(exit code 5\) naming the proxy host and port, with advice to check the proxy setting or bypass it\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlProxyException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlProxyException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlProxyException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlProxyException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [ProxyHost](CurlDotNet.Exceptions.CurlProxyException.ProxyHost.md 'CurlDotNet\.Exceptions\.CurlProxyException\.ProxyHost') | The proxy's host name or IP address, from `-x/--proxy` or a proxy environment variable\. |
| [ProxyPort](CurlDotNet.Exceptions.CurlProxyException.ProxyPort.md 'CurlDotNet\.Exceptions\.CurlProxyException\.ProxyPort') | The proxy's port, such as 3128 or 8080, or null when the proxy URL did not give one \(curl then uses 1080\)\. |
