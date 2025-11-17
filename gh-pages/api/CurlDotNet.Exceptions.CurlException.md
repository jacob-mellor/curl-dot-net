#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlException Class

Base exception for all curl operations\. This is the base class for all curl\-specific exceptions\.

```csharp
public class CurlException : System.Exception
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; CurlException

Derived  
&#8627; [CurlFtpException](CurlDotNet.Core.CurlFtpException.md 'CurlDotNet\.Core\.CurlFtpException')  
&#8627; [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')  
&#8627; [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException')  
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
&#8627; [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')  
&#8627; [CurlReceiveErrorException](CurlDotNet.Exceptions.CurlReceiveErrorException.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException')  
&#8627; [CurlRedirectException](CurlDotNet.Exceptions.CurlRedirectException.md 'CurlDotNet\.Exceptions\.CurlRedirectException')  
&#8627; [CurlRemoteAccessDeniedException](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException')  
&#8627; [CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException')  
&#8627; [CurlSendErrorException](CurlDotNet.Exceptions.CurlSendErrorException.md 'CurlDotNet\.Exceptions\.CurlSendErrorException')  
&#8627; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')  
&#8627; [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')  
&#8627; [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException')  
&#8627; [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException')  
&#8627; [CurlUploadFailedException](CurlDotNet.Exceptions.CurlUploadFailedException.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException')  
&#8627; [CurlWeirdServerReplyException](CurlDotNet.Exceptions.CurlWeirdServerReplyException.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException')  
&#8627; [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')

### Example

```csharp
try
{
    var result = await curl.ExecuteAsync("curl https://api.example.com");
}
catch (CurlConnectionException ex)
{
    // Handle connection-specific issues
    Console.WriteLine($"Failed to connect to {ex.Host}:{ex.Port}");
}
catch (CurlException ex)
{
    // Handle any other curl error
    Console.WriteLine($"Curl failed: {ex.Message}");
    Console.WriteLine($"Command: {ex.Command}");
    Console.WriteLine($"Error code: {ex.CurlErrorCode}");
}
```

### Remarks

This exception provides common properties for all curl errors including the command that was executed and the curl error code.

Curl error codes match the original curl error codes from the C implementation.

AI-Usage: Catch this exception type to handle any curl-related error generically.

AI-Pattern: Use specific derived exceptions for targeted error handling.

| Constructors | |
| :--- | :--- |
| [CurlException\(string, int, string\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string\)') | Initializes a new instance of the [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') class with a curl error code\. |
| [CurlException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, string, System\.Exception\)') | Initializes a new instance of the [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') class with a specified error message\. |
| [CurlException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Initializes a new instance of the [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') class with serialized data\. |

| Properties | |
| :--- | :--- |
| [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') | Gets the curl command that was being executed when the exception occurred\. |
| [Context](CurlDotNet.Exceptions.CurlException.Context.md 'CurlDotNet\.Exceptions\.CurlException\.Context') | Gets additional context information added via fluent methods\. |
| [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') | Gets the curl error code matching the original curl implementation\. |
| [DiagnosticInfo](CurlDotNet.Exceptions.CurlException.DiagnosticInfo.md 'CurlDotNet\.Exceptions\.CurlException\.DiagnosticInfo') | Gets or sets custom diagnostic information\. |
| [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') | Gets suggestions for resolving this error\. |

| Methods | |
| :--- | :--- |
| [GetCurlErrorName\(\)](CurlDotNet.Exceptions.CurlException.GetCurlErrorName().md 'CurlDotNet\.Exceptions\.CurlException\.GetCurlErrorName\(\)') | Get the CURLE\_\* constant name for the error code\. |
| [GetDiagnosticInfo\(\)](CurlDotNet.Exceptions.CurlException.GetDiagnosticInfo().md 'CurlDotNet\.Exceptions\.CurlException\.GetDiagnosticInfo\(\)') | Get diagnostic information for debugging\. |
| [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Sets the [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo') with information about the exception\. |
| [IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)') | Check if this error is potentially retryable\. |
| [Log\(Action&lt;string,Dictionary&lt;string,object&gt;&gt;\)](CurlDotNet.Exceptions.CurlException.Log(System.Action_string,System.Collections.Generic.Dictionary_string,object__).md 'CurlDotNet\.Exceptions\.CurlException\.Log\(System\.Action\<string,System\.Collections\.Generic\.Dictionary\<string,object\>\>\)') | Log this exception with structured data\. |
| [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)') | Get a detailed string representation of the exception\. |
| [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)') | Get the exception as a structured JSON object for logging\. |
| [ToUserFriendlyMessage\(\)](CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage().md 'CurlDotNet\.Exceptions\.CurlException\.ToUserFriendlyMessage\(\)') | Create a user\-friendly error message\. |
| [WithContext\(string, object\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(string,object) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(string, object\)') | Add contextual information to the exception \(fluent\)\. |
| [WithContext\(Dictionary&lt;string,object&gt;\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(System\.Collections\.Generic\.Dictionary\<string,object\>\)') | Add multiple context values \(fluent\)\. |
| [WithDiagnostics\(string\)](CurlDotNet.Exceptions.CurlException.WithDiagnostics(string).md 'CurlDotNet\.Exceptions\.CurlException\.WithDiagnostics\(string\)') | Add diagnostic information \(fluent\)\. |
| [WithSuggestion\(string\)](CurlDotNet.Exceptions.CurlException.WithSuggestion(string).md 'CurlDotNet\.Exceptions\.CurlException\.WithSuggestion\(string\)') | Add a suggestion for resolving this error \(fluent\)\. |
