#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlConnectionException Class

Base type for network\-level failures raised by your own code or older callers \(name resolution, refused or
unreachable connection\); the transport throws the per\-exit\-code types listed in the remarks instead\.

```csharp
public class CurlConnectionException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlConnectionException

Derived  
&#8627; [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException')  
&#8627; [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')

### Example

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://internal-server:8443/api");
}
catch (CurlCouldntConnectException ex)          // what the transport throws (exit code 7)
{
    Console.WriteLine($"Cannot connect to {ex.Host}:{ex.Port} - {ex.Suggestions[0]}");
}
catch (CurlConnectionException ex)              // raised by your own code or middleware
{
    Console.WriteLine($"Cannot connect to {ex.Host}:{ex.Port}");
}
```

### Remarks

No HTTP exchange happened: the request was never answered. [Host](CurlDotNet.Exceptions.CurlConnectionException.Host.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Host') and [Port](CurlDotNet.Exceptions.CurlConnectionException.Port.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Port') say
            what was tried. Derived types: [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException') (exit code 6) and [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')
            (exit code 5). The transport itself throws the per-exit-code types [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')
            (6), [CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException') (5) and [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException') (7), which
            are not subclasses of this type, so catch those too (or catch [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') and check
            [IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)')).

Usual causes: a typo in the host, the service is down, the port is wrong, a firewall or VPN blocks the
            connection, or a proxy is required. Example: `catch (CurlConnectionException ex) { Console.WriteLine($"Cannot reach {ex.Host}:{ex.Port}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md

### See Also
- [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException')
- [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')
- [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')
- [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')

| Constructors | |
| :--- | :--- |
| [CurlConnectionException\(string, int, string, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, int, string, System\.Nullable\<int\>, string\)') | Creates a connection failure that carries curl's exit code, for derived types such as [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException') \(6\) and [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException') \(5\)\. |
| [CurlConnectionException\(string, string, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)') | Creates a connection failure for [host](CurlDotNet.Exceptions.CurlConnectionException.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).host 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)\.host') \(and [port](CurlDotNet.Exceptions.CurlConnectionException.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).port 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)\.port')\) without a curl exit code, for code of your own that reports an unreachable endpoint\. |
| [CurlConnectionException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Restores the exception, including [Host](CurlDotNet.Exceptions.CurlConnectionException.Host.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Host') and [Port](CurlDotNet.Exceptions.CurlConnectionException.Port.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Port'), from serialized data\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlConnectionException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlConnectionException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Host](CurlDotNet.Exceptions.CurlConnectionException.Host.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Host') | The host name or IP address that could not be reached, exactly as it appears in the URL \(or the proxy's host for [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')\)\. |
| [Port](CurlDotNet.Exceptions.CurlConnectionException.Port.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Port') | The TCP port that was tried, or null when the URL used the scheme's default \(80 for http, 443 for https, 21 for ftp\)\. |

| Methods | |
| :--- | :--- |
| [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlConnectionException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlConnectionException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Writes the exception, including [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') and [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode'), into serialization data so it can cross an AppDomain or remoting boundary on \.NET Framework\. |
