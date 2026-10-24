#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlException Class

Base class of every transfer error CurlDotNet throws: catch it to handle any failed curl command, read
[CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') for curl's exit code and [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message') for what happened and how to fix it\.

```csharp
public class CurlException : System.Exception
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; CurlException

Derived  
&#8627; [CurlFtpException](CurlDotNet.Core.CurlFtpException.md 'CurlDotNet\.Core\.CurlFtpException')  
&#8627; [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')  
&#8627; [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException')  
&#8627; [CurlAuthErrorException](CurlDotNet.Exceptions.CurlAuthErrorException.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException')  
&#8627; [CurlBadContentEncodingException](CurlDotNet.Exceptions.CurlBadContentEncodingException.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException')  
&#8627; [CurlBadDownloadResumeException](CurlDotNet.Exceptions.CurlBadDownloadResumeException.md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException')  
&#8627; [CurlBadFunctionArgumentException](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException')  
&#8627; [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')  
&#8627; [CurlCookieException](CurlDotNet.Exceptions.CurlCookieException.md 'CurlDotNet\.Exceptions\.CurlCookieException')  
&#8627; [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')  
&#8627; [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')  
&#8627; [CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException')  
&#8627; [CurlExecutionException](CurlDotNet.Exceptions.CurlExecutionException.md 'CurlDotNet\.Exceptions\.CurlExecutionException')  
&#8627; [CurlFailedInitException](CurlDotNet.Exceptions.CurlFailedInitException.md 'CurlDotNet\.Exceptions\.CurlFailedInitException')  
&#8627; [CurlFileCouldntReadException](CurlDotNet.Exceptions.CurlFileCouldntReadException.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException')  
&#8627; [CurlFileException](CurlDotNet.Exceptions.CurlFileException.md 'CurlDotNet\.Exceptions\.CurlFileException')  
&#8627; [CurlFileSizeExceededException](CurlDotNet.Exceptions.CurlFileSizeExceededException.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException')  
&#8627; [CurlFtpAcceptFailedException](CurlDotNet.Exceptions.CurlFtpAcceptFailedException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptFailedException')  
&#8627; [CurlFtpException](CurlDotNet.Exceptions.CurlFtpException.md 'CurlDotNet\.Exceptions\.CurlFtpException')  
&#8627; [CurlFtpWeirdPassReplyException](CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.md 'CurlDotNet\.Exceptions\.CurlFtpWeirdPassReplyException')  
&#8627; [CurlFunctionNotFoundException](CurlDotNet.Exceptions.CurlFunctionNotFoundException.md 'CurlDotNet\.Exceptions\.CurlFunctionNotFoundException')  
&#8627; [CurlGotNothingException](CurlDotNet.Exceptions.CurlGotNothingException.md 'CurlDotNet\.Exceptions\.CurlGotNothingException')  
&#8627; [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')  
&#8627; [CurlHttpPostErrorException](CurlDotNet.Exceptions.CurlHttpPostErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpPostErrorException')  
&#8627; [CurlInterfaceFailedException](CurlDotNet.Exceptions.CurlInterfaceFailedException.md 'CurlDotNet\.Exceptions\.CurlInterfaceFailedException')  
&#8627; [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
&#8627; [CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException')  
&#8627; [CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException')  
&#8627; [CurlNotSupportedException](CurlDotNet.Exceptions.CurlNotSupportedException.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException')  
&#8627; [CurlOutOfMemoryException](CurlDotNet.Exceptions.CurlOutOfMemoryException.md 'CurlDotNet\.Exceptions\.CurlOutOfMemoryException')  
&#8627; [CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  
&#8627; [CurlPartialFileException](CurlDotNet.Exceptions.CurlPartialFileException.md 'CurlDotNet\.Exceptions\.CurlPartialFileException')  
&#8627; [CurlRangeErrorException](CurlDotNet.Exceptions.CurlRangeErrorException.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException')  
&#8627; [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')  
&#8627; [CurlReceiveErrorException](CurlDotNet.Exceptions.CurlReceiveErrorException.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException')  
&#8627; [CurlRedirectException](CurlDotNet.Exceptions.CurlRedirectException.md 'CurlDotNet\.Exceptions\.CurlRedirectException')  
&#8627; [CurlRemoteAccessDeniedException](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException')  
&#8627; [CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException')  
&#8627; [CurlSendErrorException](CurlDotNet.Exceptions.CurlSendErrorException.md 'CurlDotNet\.Exceptions\.CurlSendErrorException')  
&#8627; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')  
&#8627; [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')  
&#8627; [CurlTooLargeException](CurlDotNet.Exceptions.CurlTooLargeException.md 'CurlDotNet\.Exceptions\.CurlTooLargeException')  
&#8627; [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException')  
&#8627; [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException')  
&#8627; [CurlUploadFailedException](CurlDotNet.Exceptions.CurlUploadFailedException.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException')  
&#8627; [CurlWeirdServerReplyException](CurlDotNet.Exceptions.CurlWeirdServerReplyException.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException')  
&#8627; [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')

### Example

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
catch (CurlConnectionException ex)
{
    Console.WriteLine($"Failed to connect to {ex.Host}:{ex.Port}");
}
catch (CurlException ex)
{
    Console.WriteLine(ex.WhatHappened);
    foreach (var fix in ex.Suggestions) Console.WriteLine("  fix: " + fix);
    Console.WriteLine($"exit code {ex.CurlErrorCode}, docs: {ex.DocumentationUrl}");
}
```

### Remarks

Every transfer failure of `Curl.ExecuteAsync`, `Curl.Execute`, [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')
            and [LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl') surfaces as a [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') subclass. Three things do not:
            cancelling your own token ([System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')), invalid arguments such as a null command
            ([System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException') with the same what/fix/link text), and `CurlResult.EnsureSuccess()` /
            `EnsureStatus()`, which throw [CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException') (a plain [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception'),
            unlike [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') in this namespace). There is one subclass
            per curl exit code (for example [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException') for exit code 6,
            [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') for 28, [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException') for 60), so
            you can catch exactly the failures you can handle and let the rest bubble up.

An HTTP error status (404, 500...) is not an exception: it comes back as a normal
            [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with `StatusCode` set, unless the command uses `-f`/`--fail`,
            in which case [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException') (exit code 22) is thrown.

The message has three parts: what happened (first line, also [WhatHappened](CurlDotNet.Exceptions.CurlException.WhatHappened.md 'CurlDotNet\.Exceptions\.CurlException\.WhatHappened')), how to fix it (the
            following lines, also [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions')) and a `See https://...` documentation link (last line, also
            [DocumentationUrl](CurlDotNet.Exceptions.CurlException.DocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlException\.DocumentationUrl')). [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') holds the command that failed and
            [StandardOutput](CurlDotNet.Exceptions.CurlException.StandardOutput.md 'CurlDotNet\.Exceptions\.CurlException\.StandardOutput') what curl would already have printed.

Example: `try { var r = await Curl.ExecuteAsync("curl https://api.example.com/users"); }
            catch (CurlCouldntResolveHostException ex) { Console.WriteLine($"Bad host {ex.Hostname}"); }
            catch (CurlTimeoutException) { /* retry later */ } catch (CurlException ex) { Console.WriteLine($"curl exit {ex.CurlErrorCode}: {ex.Message}"); }`

Exit code table and handling patterns: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md
            Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')
- [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')
- [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')
- [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')

| Constructors | |
| :--- | :--- |
| [CurlException\(string, int, string\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string\)') | Creates an exception that carries curl's exit code, so callers can switch on [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') exactly like a shell script switches on `$?`\. |
| [CurlException\(string, int, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string, System\.Exception\)') | Creates an exception that carries curl's exit code and wraps the \.NET exception that caused it \(for example an [System\.IO\.IOException](https://learn.microsoft.com/en-us/dotnet/api/system.io.ioexception 'System\.IO\.IOException') while writing `-o` output\)\. |
| [CurlException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, string, System\.Exception\)') | Creates an exception with a message and, optionally, the command that failed and the exception that caused it\. |
| [CurlException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Restores an exception \(its message, [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') and [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')\) from serialized data; used by remoting and binary serialization on \.NET Framework\. |

| Fields | |
| :--- | :--- |
| [StandardOutputDataKey](CurlDotNet.Exceptions.CurlException.StandardOutputDataKey.md 'CurlDotNet\.Exceptions\.CurlException\.StandardOutputDataKey') | Key in [System\.Exception\.Data](https://learn.microsoft.com/en-us/dotnet/api/system.exception.data 'System\.Exception\.Data') under which a failed command keeps the text curl would already have printed to standard output\. |
| [WriteOutDataKey](CurlDotNet.Exceptions.CurlException.WriteOutDataKey.md 'CurlDotNet\.Exceptions\.CurlException\.WriteOutDataKey') | Key in [System\.Exception\.Data](https://learn.microsoft.com/en-us/dotnet/api/system.exception.data 'System\.Exception\.Data') holding the standard\-output part of `-w`/`--write-out` for a failed transfer\. |

| Properties | |
| :--- | :--- |
| [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') | The curl command \(or a description of the API call\) that was running when the error happened, exactly as it was passed in, so you can log it or re\-run it in a terminal\. |
| [Context](CurlDotNet.Exceptions.CurlException.Context.md 'CurlDotNet\.Exceptions\.CurlException\.Context') | Extra key/value facts attached with [WithContext\(string, object\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(string,object) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(string, object\)'), such as a request id or the tenant a call was made for, so that logs show them next to the error\. |
| [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') | curl's exit code for this failure \(the `CURLE_*` value from `curl.h`\), for example 6 when the host name cannot be resolved, 7 when the connection is refused, 22 for `-f` HTTP errors and 28 for a timeout\. |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [DiagnosticInfo](CurlDotNet.Exceptions.CurlException.DiagnosticInfo.md 'CurlDotNet\.Exceptions\.CurlException\.DiagnosticInfo') | Free\-form diagnostic text attached with [WithDiagnostics\(string\)](CurlDotNet.Exceptions.CurlException.WithDiagnostics(string).md 'CurlDotNet\.Exceptions\.CurlException\.WithDiagnostics\(string\)'), such as the output of `-v` or a trace file path, printed by [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)')\. |
| [DocumentationUrl](CurlDotNet.Exceptions.CurlException.DocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlException\.DocumentationUrl') | The documentation page that explains this error and how to fix it: the `See https://...` link at the end of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\. |
| [ExtraSuggestions](CurlDotNet.Exceptions.CurlException.ExtraSuggestions.md 'CurlDotNet\.Exceptions\.CurlException\.ExtraSuggestions') | Further fixes appended to [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') after the ones in the message, for errors where several remedies apply \(for example the status\-specific advice of [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')\)\. |
| [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message') | The full error text: what happened \(first line\), how to fix it \(following lines\) and a `See https://...` documentation link \(last line\)\. |
| [StandardOutput](CurlDotNet.Exceptions.CurlException.StandardOutput.md 'CurlDotNet\.Exceptions\.CurlException\.StandardOutput') | The text curl would have written to standard output before this failure, or null when nothing was written\. |
| [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') | How to fix this error, one sentence per entry, taken from the message \(every line between the first line and the `See https://...` link\) or, when the message has none, from curl's exit code\. |
| [WhatHappened](CurlDotNet.Exceptions.CurlException.WhatHappened.md 'CurlDotNet\.Exceptions\.CurlException\.WhatHappened') | The first line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message'): what went wrong, with the concrete values involved \(URL, host, option, file path, status code\), but without the fix or the documentation link\. |

| Methods | |
| :--- | :--- |
| [GetCurlErrorName\(\)](CurlDotNet.Exceptions.CurlException.GetCurlErrorName().md 'CurlDotNet\.Exceptions\.CurlException\.GetCurlErrorName\(\)') | Returns the `CURLE_*` constant name for [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode'), such as `CURLE_COULDNT_RESOLVE_HOST` for 6, so logs can be searched with the same names as libcurl's documentation\. |
| [GetDiagnosticInfo\(\)](CurlDotNet.Exceptions.CurlException.GetDiagnosticInfo().md 'CurlDotNet\.Exceptions\.CurlException\.GetDiagnosticInfo\(\)') | Returns the facts about this error as a dictionary \(type, message, command, exit code and name, timestamp, machine, OS, retryability, context, suggestions, inner exception\) for logging scopes and telemetry\. |
| [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Writes the exception, including [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') and [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode'), into serialization data so it can cross an AppDomain or remoting boundary on \.NET Framework\. |
| [IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)') | Tells whether running the same command again might succeed: true for transient network failures \(DNS, refused or reset connections, timeouts, empty replies\), false for problems that need a change first \(bad URL, TLS, auth\)\. |
| [Log\(Action&lt;string,Dictionary&lt;string,object&gt;&gt;\)](CurlDotNet.Exceptions.CurlException.Log(System.Action_string,System.Collections.Generic.Dictionary_string,object__).md 'CurlDotNet\.Exceptions\.CurlException\.Log\(System\.Action\<string,System\.Collections\.Generic\.Dictionary\<string,object\>\>\)') | Sends this exception to your logging callback as two values: the human\-readable [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)') text and the structured [GetDiagnosticInfo\(\)](CurlDotNet.Exceptions.CurlException.GetDiagnosticInfo().md 'CurlDotNet\.Exceptions\.CurlException\.GetDiagnosticInfo\(\)') dictionary\. |
| [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)') | Returns a multi\-line report for logs: type, what happened, documentation link, command, exit code and name, context, how\-to\-fix list, diagnostics, inner exception and stack trace\. |
| [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)') | Serializes the exception to indented JSON \(type, message, whatHappened, documentationUrl, command, exit code and name, context, suggestions, diagnostics, retryability, stack trace, inner message\) for structured logs\. |
| [ToUserFriendlyMessage\(\)](CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage().md 'CurlDotNet\.Exceptions\.CurlException\.ToUserFriendlyMessage\(\)') | Returns short text for end users: `Error:` what happened, a bullet list of possible solutions, a note when a retry may help, and the documentation link, without stack traces or the command\. |
| [WithContext\(string, object\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(string,object) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(string, object\)') | Attaches one key/value fact \(a request id, a tenant, a retry number\) to this exception and returns the same exception, so you can enrich and rethrow it in one expression\. |
| [WithContext\(Dictionary&lt;string,object&gt;\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(System\.Collections\.Generic\.Dictionary\<string,object\>\)') | Attaches several key/value facts at once \(copied into [Context](CurlDotNet.Exceptions.CurlException.Context.md 'CurlDotNet\.Exceptions\.CurlException\.Context')\) and returns the same exception for chaining, for example everything your logging scope already knows\. |
| [WithDiagnostics\(string\)](CurlDotNet.Exceptions.CurlException.WithDiagnostics(string).md 'CurlDotNet\.Exceptions\.CurlException\.WithDiagnostics\(string\)') | Stores free\-form diagnostic text \(a verbose log, a trace excerpt, environment details\) in [DiagnosticInfo](CurlDotNet.Exceptions.CurlException.DiagnosticInfo.md 'CurlDotNet\.Exceptions\.CurlException\.DiagnosticInfo') and returns the same exception for chaining\. |
| [WithSuggestion\(string\)](CurlDotNet.Exceptions.CurlException.WithSuggestion(string).md 'CurlDotNet\.Exceptions\.CurlException\.WithSuggestion\(string\)') | Adds your own advice on how to fix the problem to [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(ignored if the same text is already there\) and returns the same exception for chaining\. |
