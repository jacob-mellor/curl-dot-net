#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlAbortedByCallbackException Class

The transfer was stopped on purpose \(exit code 42, CURLE\_ABORTED\_BY\_CALLBACK\): your progress callback threw,
or \(seen only as an InnerException\) your `CancellationToken` was cancelled\.

```csharp
public class CurlAbortedByCallbackException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlAbortedByCallbackException

### Remarks

You catch this type directly when a handler passed to `CurlSettings.WithProgress` /
            `CurlOptions.ProgressHandler` throws; the handler's exception is the [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException').

Cancelling your own token is different: the public APIs (`Curl.ExecuteAsync`, `CurlRequestBuilder`,
            `LibCurl`) throw the standard [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException'), with this exception as its InnerException,
            so write `catch (OperationCanceledException) when (cts.IsCancellationRequested) { /* user pressed Cancel */ }`.
            Timeouts set with `-m` are [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') instead.

Example: `catch (CurlAbortedByCallbackException ex) { Console.WriteLine($"Progress handler failed: {ex.InnerException?.Message}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md

### See Also
- [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException')

| Constructors | |
| :--- | :--- |
| [CurlAbortedByCallbackException\(string\)](CurlDotNet.Exceptions.CurlAbortedByCallbackException.#ctor.md#CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string) 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException\.CurlAbortedByCallbackException\(string\)') | Creates the error \(exit code 42\) for a transfer cancelled through the caller's `CancellationToken`\. |
| [CurlAbortedByCallbackException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlAbortedByCallbackException.#ctor.md#CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException\.CurlAbortedByCallbackException\(string, string, System\.Exception\)') | Creates the exception for a transfer that your own callback \(for example a progress handler set with [WithProgress\(Action&lt;double,long,long&gt;\)](CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_).md 'CurlDotNet\.Core\.CurlSettings\.WithProgress\(System\.Action\<double,long,long\>\)')\) stopped by throwing\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlAbortedByCallbackException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlAbortedByCallbackException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
