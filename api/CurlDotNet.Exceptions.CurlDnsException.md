#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlDnsException Class

A host name that could not be turned into an IP address \(curl exit code 6, CURLE\_COULDNT\_RESOLVE\_HOST\):
the name is misspelled, does not exist, or DNS is unreachable\.

```csharp
public class CurlDnsException : CurlDotNet.Exceptions.CurlConnectionException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') &#129106; CurlDnsException

### Example

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://non-existent-domain.invalid");
}
catch (CurlCouldntResolveHostException ex)      // what the transport throws (exit code 6)
{
    Console.WriteLine($"DNS lookup failed for: {ex.Hostname}");
}
catch (CurlDnsException ex)                     // raised by your own code
{
    Console.WriteLine($"DNS lookup failed for: {ex.Host}");
}
```

### Remarks

A [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') with [Host](CurlDotNet.Exceptions.CurlConnectionException.Host.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Host') set to the name. The
            transport reports DNS failures as [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException'); this type is for your own code
            and older callers that catch it.

Fixes: check the spelling of the host in the URL, try `nslookup host` on the same machine, check VPN or
            proxy settings, or pin the name with `--resolve host:443:1.2.3.4`.
            Example: `catch (CurlDnsException ex) { Console.WriteLine($"Unknown host {ex.Host}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md

### See Also
- [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')
- [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')

| Constructors | |
| :--- | :--- |
| [CurlDnsException\(string, string\)](CurlDotNet.Exceptions.CurlDnsException.#ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string) 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(string, string\)') | Creates the exception for [hostname](CurlDotNet.Exceptions.CurlDnsException.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string).hostname 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(string, string\)\.hostname'), with curl's message `Could not resolve host: NAME` and exit code 6\. |
| [CurlDnsException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlDnsException.#ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Restores the exception from serialized data on \.NET Framework \(host name and exit code included\)\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlDnsException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlDnsException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
