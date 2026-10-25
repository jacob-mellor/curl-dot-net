#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlCouldntResolveHostException Class

The host name in the URL could not be resolved to an IP address \(exit code 6, CURLE\_COULDNT\_RESOLVE\_HOST\):
a typo, a name that does not exist, or no DNS \(offline, VPN down\)\.

```csharp
public class CurlCouldntResolveHostException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlCouldntResolveHostException

### Remarks

Thrown by the transport before any connection is made. [Hostname](CurlDotNet.Exceptions.CurlCouldntResolveHostException.Hostname.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.Hostname') holds the name. Fixes: check
            the spelling, run `nslookup NAME` on the same machine, check VPN or corporate DNS, or pin the name with
            `--resolve NAME:443:1.2.3.4`. Retrying helps only for temporary DNS outages
            ([IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)') is true).

Example: `catch (CurlCouldntResolveHostException ex) { Console.WriteLine($"Unknown host {ex.Hostname} - check the URL"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md

### See Also
- [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')
- [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException')

| Constructors | |
| :--- | :--- |
| [CurlCouldntResolveHostException\(string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveHostException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.CurlCouldntResolveHostException\(string, string\)') | Creates the error with curl's wording `Could not resolve host: NAME` and advice on spelling, DNS and `--resolve`\. |
| [CurlCouldntResolveHostException\(string, string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveHostException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.CurlCouldntResolveHostException\(string, string, string\)') | Creates the error with the resolver's explanation of why the name failed, such as "No such host is known"\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlCouldntResolveHostException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlCouldntResolveHostException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Hostname](CurlDotNet.Exceptions.CurlCouldntResolveHostException.Hostname.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.Hostname') | The host name from the URL that could not be resolved, such as `api.exmaple.com`\. |
