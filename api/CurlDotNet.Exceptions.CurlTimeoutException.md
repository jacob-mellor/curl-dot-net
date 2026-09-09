#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlTimeoutException Class

Thrown when an operation exceeds the configured timeout period\.

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
    // Set a 5 second timeout
    var result = await curl.ExecuteAsync("curl --max-time 5 https://slow-server.com/large-file");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Operation timed out after {ex.Timeout?.TotalSeconds ?? 0} seconds");

    // Retry with longer timeout
    Console.WriteLine("Retrying with 30 second timeout...");
    result = await curl.ExecuteAsync("curl --max-time 30 https://slow-server.com/large-file");
}
```

### Remarks

This can occur for connection timeout or total operation timeout.

Curl error code: CURLE_OPERATION_TIMEDOUT (28)

AI-Usage: Catch this to implement retry with longer timeout or fail fast.

AI-Pattern: Log timeout value to help diagnose if timeout is too short.

| Constructors | |
| :--- | :--- |
| [CurlTimeoutException\(string, string, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Exceptions.CurlTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_) 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(string, string, System\.Nullable\<System\.TimeSpan\>\)') | Initializes a new instance of the [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException') class\. |
| [CurlTimeoutException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Initializes a new instance with serialized data\. |

| Properties | |
| :--- | :--- |
| [Timeout](CurlDotNet.Exceptions.CurlTimeoutException.Timeout.md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.Timeout') | Gets the timeout duration that was exceeded\. |

| Methods | |
| :--- | :--- |
| [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlTimeoutException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Sets the [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo') with information about the exception\. |
