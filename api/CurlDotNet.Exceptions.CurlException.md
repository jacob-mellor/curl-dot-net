#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

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
### Constructors

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string)'></a>

## CurlException\(string, int, string\) Constructor

Initializes a new instance of the [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') class with a curl error code\.

```csharp
public CurlException(string message, int curlErrorCode, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message that explains the reason for the exception\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string).curlErrorCode'></a>

`curlErrorCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The curl error code from the original curl implementation\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception)'></a>

## CurlException\(string, string, Exception\) Constructor

Initializes a new instance of the [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') class with a specified error message\.

```csharp
public CurlException(string message, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message that explains the reason for the exception\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The exception that is the cause of the current exception\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlException\(SerializationInfo, StreamingContext\) Constructor

Initializes a new instance of the [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') class with serialized data\.

```csharp
protected CurlException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The serialization information\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The streaming context\.
### Properties

<a name='CurlDotNet.Exceptions.CurlException.Command'></a>

## CurlException\.Command Property

Gets the curl command that was being executed when the exception occurred\.

```csharp
public string Command { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The full curl command string, or null if not applicable\.

### Remarks

This property contains the exact command that was passed to the Execute method.

AI-Usage: Use this for logging and debugging to understand what command failed.

<a name='CurlDotNet.Exceptions.CurlException.Context'></a>

## CurlException\.Context Property

Gets additional context information added via fluent methods\.

```csharp
public System.Collections.Generic.Dictionary<string,object> Context { get; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Exceptions.CurlException.CurlErrorCode'></a>

## CurlException\.CurlErrorCode Property

Gets the curl error code matching the original curl implementation\.

```csharp
public System.Nullable<int> CurlErrorCode { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
The curl error code \(e\.g\., 6 for DNS resolution failure, 28 for timeout\), or null if not a curl\-specific error\.

### Remarks

Error codes match the CURLE_* constants from curl.h

Common codes: 6=DNS failure, 7=connection failed, 28=timeout, 35=SSL error

AI-Usage: Use this to determine the specific type of curl error programmatically.

<a name='CurlDotNet.Exceptions.CurlException.DiagnosticInfo'></a>

## CurlException\.DiagnosticInfo Property

Gets or sets custom diagnostic information\.

```csharp
public string DiagnosticInfo { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Exceptions.CurlException.Suggestions'></a>

## CurlException\.Suggestions Property

Gets suggestions for resolving this error\.

```csharp
public System.Collections.Generic.List<string> Suggestions { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')
### Methods

<a name='CurlDotNet.Exceptions.CurlException.GetCurlErrorName()'></a>

## CurlException\.GetCurlErrorName\(\) Method

Get the CURLE\_\* constant name for the error code\.

```csharp
public string GetCurlErrorName();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Curl error constant name

<a name='CurlDotNet.Exceptions.CurlException.GetDiagnosticInfo()'></a>

## CurlException\.GetDiagnosticInfo\(\) Method

Get diagnostic information for debugging\.

```csharp
public virtual System.Collections.Generic.Dictionary<string,object> GetDiagnosticInfo();
```

#### Returns
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
Diagnostic details

<a name='CurlDotNet.Exceptions.CurlException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlException\.GetObjectData\(SerializationInfo, StreamingContext\) Method

Sets the [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo') with information about the exception\.

```csharp
public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The serialization information\.

<a name='CurlDotNet.Exceptions.CurlException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The streaming context\.

Implements [GetObjectData\(SerializationInfo, StreamingContext\)](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable.getobjectdata#system-runtime-serialization-iserializable-getobjectdata(system-runtime-serialization-serializationinfo-system-runtime-serialization-streamingcontext) 'System\.Runtime\.Serialization\.ISerializable\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo,System\.Runtime\.Serialization\.StreamingContext\)')

<a name='CurlDotNet.Exceptions.CurlException.IsRetryable()'></a>

## CurlException\.IsRetryable\(\) Method

Check if this error is potentially retryable\.

```csharp
public virtual bool IsRetryable();
```

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True if the error might succeed on retry

<a name='CurlDotNet.Exceptions.CurlException.Log(System.Action_string,System.Collections.Generic.Dictionary_string,object__)'></a>

## CurlException\.Log\(Action\<string,Dictionary\<string,object\>\>\) Method

Log this exception with structured data\.

```csharp
public void Log(System.Action<string,System.Collections.Generic.Dictionary<string,object>> logger);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.Log(System.Action_string,System.Collections.Generic.Dictionary_string,object__).logger'></a>

`logger` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')

Action to perform logging

<a name='CurlDotNet.Exceptions.CurlException.ToDetailedString()'></a>

## CurlException\.ToDetailedString\(\) Method

Get a detailed string representation of the exception\.

```csharp
public virtual string ToDetailedString();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Detailed error information

<a name='CurlDotNet.Exceptions.CurlException.ToJson()'></a>

## CurlException\.ToJson\(\) Method

Get the exception as a structured JSON object for logging\.

```csharp
public virtual string ToJson();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
JSON representation of the exception

<a name='CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage()'></a>

## CurlException\.ToUserFriendlyMessage\(\) Method

Create a user\-friendly error message\.

```csharp
public virtual string ToUserFriendlyMessage();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Simplified error message for end users

<a name='CurlDotNet.Exceptions.CurlException.WithContext(string,object)'></a>

## CurlException\.WithContext\(string, object\) Method

Add contextual information to the exception \(fluent\)\.

```csharp
public CurlDotNet.Exceptions.CurlException WithContext(string key, object value);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.WithContext(string,object).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Context key

<a name='CurlDotNet.Exceptions.CurlException.WithContext(string,object).value'></a>

`value` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Context value

#### Returns
[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
This exception for chaining

<a name='CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_)'></a>

## CurlException\.WithContext\(Dictionary\<string,object\>\) Method

Add multiple context values \(fluent\)\.

```csharp
public CurlDotNet.Exceptions.CurlException WithContext(System.Collections.Generic.Dictionary<string,object> context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_).context'></a>

`context` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

Dictionary of context values

#### Returns
[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
This exception for chaining

<a name='CurlDotNet.Exceptions.CurlException.WithDiagnostics(string)'></a>

## CurlException\.WithDiagnostics\(string\) Method

Add diagnostic information \(fluent\)\.

```csharp
public CurlDotNet.Exceptions.CurlException WithDiagnostics(string diagnosticInfo);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.WithDiagnostics(string).diagnosticInfo'></a>

`diagnosticInfo` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Diagnostic details

#### Returns
[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
This exception for chaining

<a name='CurlDotNet.Exceptions.CurlException.WithSuggestion(string)'></a>

## CurlException\.WithSuggestion\(string\) Method

Add a suggestion for resolving this error \(fluent\)\.

```csharp
public CurlDotNet.Exceptions.CurlException WithSuggestion(string suggestion);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.WithSuggestion(string).suggestion'></a>

`suggestion` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Helpful suggestion text

#### Returns
[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
This exception for chaining