#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlCouldntResolveProxyException Class

The proxy's host name could not be resolved \(exit code 5, CURLE\_COULDNT\_RESOLVE\_PROXY\), so no request was sent\.

```csharp
public class CurlCouldntResolveProxyException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlCouldntResolveProxyException

### Remarks

The proxy comes from `-x/--proxy` or the `http_proxy`, `https_proxy`, `all_proxy` environment
            variables. [ProxyHost](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.ProxyHost.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException\.ProxyHost') names it. Fixes: correct the proxy host, check DNS/VPN, or bypass the proxy with
            `--noproxy HOST` (or `--noproxy '*'`).
            Example: `catch (CurlCouldntResolveProxyException ex) { Console.WriteLine($"Proxy {ex.ProxyHost} unknown"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md

### See Also
- [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')
- [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')

| Constructors | |
| :--- | :--- |
| [CurlCouldntResolveProxyException\(string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException\.CurlCouldntResolveProxyException\(string, string\)') | Creates the error with curl's wording `Could not resolve proxy: HOST` and advice to check the proxy setting\. |
| [CurlCouldntResolveProxyException\(string, string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveProxyException.CurlCouldntResolveProxyException(string,string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException\.CurlCouldntResolveProxyException\(string, string, string\)') | Creates the error with the resolver's explanation of why the proxy name failed, for example "nodename nor servname provided"\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [ProxyHost](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.ProxyHost.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException\.ProxyHost') | The proxy host name that could not be resolved, as given with `-x` or in a proxy environment variable\. |
