#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

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
### Constructors

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_)'></a>

## CurlTimeoutException\(string, string, Nullable\<TimeSpan\>\) Constructor

Initializes a new instance of the [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException') class\.

```csharp
public CurlTimeoutException(string message, string command=null, System.Nullable<System.TimeSpan> timeout=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the timeout\.

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing\.

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_).timeout'></a>

`timeout` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The timeout duration that was exceeded\.

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlTimeoutException\(SerializationInfo, StreamingContext\) Constructor

Initializes a new instance with serialized data\.

```csharp
protected CurlTimeoutException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')
### Properties

<a name='CurlDotNet.Exceptions.CurlTimeoutException.Timeout'></a>

## CurlTimeoutException\.Timeout Property

Gets the timeout duration that was exceeded\.

```csharp
public System.Nullable<System.TimeSpan> Timeout { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
The timeout duration, or null if not specified\.

### Remarks

This represents the --max-time or --connect-timeout value that was exceeded.

AI-Usage: Use this to determine if timeout should be increased.
### Methods

<a name='CurlDotNet.Exceptions.CurlTimeoutException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlTimeoutException\.GetObjectData\(SerializationInfo, StreamingContext\) Method

Sets the [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo') with information about the exception\.

```csharp
public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTimeoutException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The serialization information\.

<a name='CurlDotNet.Exceptions.CurlTimeoutException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The streaming context\.

Implements [GetObjectData\(SerializationInfo, StreamingContext\)](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable.getobjectdata#system-runtime-serialization-iserializable-getobjectdata(system-runtime-serialization-serializationinfo-system-runtime-serialization-streamingcontext) 'System\.Runtime\.Serialization\.ISerializable\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo,System\.Runtime\.Serialization\.StreamingContext\)')