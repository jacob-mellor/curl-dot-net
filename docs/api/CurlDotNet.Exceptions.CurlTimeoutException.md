#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlTimeoutException Class

Base type for every timeout \(curl exit code 28, CURLE\_OPERATION\_TIMEDOUT\): the connection or the whole
transfer took longer than `--connect-timeout` or `-m/--max-time` allowed\.

```csharp
public class CurlTimeoutException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlTimeoutException

Derived  
&#8627; [CurlFtpAcceptTimeoutException](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException')  
&#8627; [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException')

### Example

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl --max-time 5 https://slow-server.com/large-file");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timed out after {ex.Timeout?.TotalSeconds ?? 0} seconds; retrying with 30");
    var retry = await Curl.ExecuteAsync("curl --max-time 30 https://slow-server.com/large-file");
}
```

### Remarks

CurlDotNet throws the derived [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') when `-m/--max-time` or
            `--connect-timeout` (or `Curl.DefaultMaxTimeSeconds` / `Curl.DefaultConnectTimeoutSeconds`)
            expires, and when a caller-supplied `HttpClient` gives up on its own `Timeout`. Cancelling your own
            `CancellationToken` is not a timeout: it ends with [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException') (whose InnerException
            is a [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException'), exit code 42).

Fixes: raise the limit (`-m 120` for large downloads, `--connect-timeout 10` for slow networks),
            check that the server is up, or add `--retry 3`. [Timeout](CurlDotNet.Exceptions.CurlTimeoutException.Timeout.md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.Timeout') tells which limit expired.
            Example: `catch (CurlTimeoutException ex) { Console.WriteLine($"Gave up after {ex.Timeout?.TotalSeconds} s"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/timeout-errors.md
            tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException')
- [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')

| Constructors | |
| :--- | :--- |
| [CurlTimeoutException\(string, string, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Exceptions.CurlTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_) 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(string, string, System\.Nullable\<System\.TimeSpan\>\)') | Creates a timeout error \(exit code 28\) with the limit that expired, for transfers that ran out of time\. |
| [CurlTimeoutException\(string, string, Nullable&lt;TimeSpan&gt;, int\)](CurlDotNet.Exceptions.CurlTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_,int) 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(string, string, System\.Nullable\<System\.TimeSpan\>, int\)') | Creates a timeout error with a curl exit code other than 28, for timeouts curl reports separately such as [CurlFtpAcceptTimeoutException](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException') \(12, the FTP server never connected back\)\. |
| [CurlTimeoutException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Restores the exception, including [Timeout](CurlDotNet.Exceptions.CurlTimeoutException.Timeout.md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.Timeout'), from serialized data on \.NET Framework\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlTimeoutException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlTimeoutException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Timeout](CurlDotNet.Exceptions.CurlTimeoutException.Timeout.md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.Timeout') | The limit that expired: the `-m/--max-time` or `--connect-timeout` value, so you can decide whether raising it makes sense\. |

| Methods | |
| :--- | :--- |
| [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlTimeoutException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Writes the exception, including [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') and [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode'), into serialization data so it can cross an AppDomain or remoting boundary on \.NET Framework\. |
