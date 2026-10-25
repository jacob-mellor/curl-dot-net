#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')

## CurlConnectionException\.GetObjectData\(SerializationInfo, StreamingContext\) Method

Writes the exception, including [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') and [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode'), into serialization data
so it can cross an AppDomain or remoting boundary on \.NET Framework\.

```csharp
public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The object that receives the serialized values \(message, command, exit code\)\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The destination of the serialized stream, supplied by the serializer\.

Implements [GetObjectData\(SerializationInfo, StreamingContext\)](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable.getobjectdata#system-runtime-serialization-iserializable-getobjectdata(system-runtime-serialization-serializationinfo-system-runtime-serialization-streamingcontext) 'System\.Runtime\.Serialization\.ISerializable\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo,System\.Runtime\.Serialization\.StreamingContext\)')

### Remarks

Called by the serializer, not by your code. On .NET 8 and later binary serialization is obsolete; use
            [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)') for logs and message queues instead.

### See Also
- [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)')