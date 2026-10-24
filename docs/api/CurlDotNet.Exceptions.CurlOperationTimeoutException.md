#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlOperationTimeoutException Class

The transfer ran out of time \(exit code 28, CURLE\_OPERATION\_TIMEDOUT\): `--connect-timeout` expired while
connecting, or `-m/--max-time` expired before the whole response arrived\.

```csharp
public class CurlOperationTimeoutException : CurlDotNet.Exceptions.CurlTimeoutException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException') &#129106; CurlOperationTimeoutException

### Remarks

Thrown by the HTTP handler and the transport; the first line uses curl's wording
            (`Operation timed out after 5000 milliseconds with 1024 bytes received`,
            `Connection timed out after 2000 milliseconds`). [Timeout](CurlDotNet.Exceptions.CurlTimeoutException.Timeout.md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.Timeout') holds the limit.
            Cancelling your own token throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException') instead (with a
            [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException') as its InnerException).

Example: `catch (CurlOperationTimeoutException ex) { Console.WriteLine($"Timed out ({ex.Timeout?.TotalSeconds} s) - raising -m"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/timeout-errors.md

### See Also
- [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')
- [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')

| Constructors | |
| :--- | :--- |
| [CurlOperationTimeoutException\(double, string\)](CurlDotNet.Exceptions.CurlOperationTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string) 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string\)') | Creates the timeout error with curl's wording `Operation timed out after N milliseconds` for the limit [timeoutSeconds](CurlDotNet.Exceptions.CurlOperationTimeoutException.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string).timeoutSeconds 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string\)\.timeoutSeconds')\. |
| [CurlOperationTimeoutException\(double, string, string\)](CurlDotNet.Exceptions.CurlOperationTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string,string) 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string, string\)') | Creates the timeout error with a message describing which limit expired and what had been received, such as curl's `Connection timed out after 2000 milliseconds`\. |
