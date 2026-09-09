#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')

## CurlTimeoutException Constructors

| Overloads | |
| :--- | :--- |
| [CurlTimeoutException\(string, string, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Exceptions.CurlTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_) 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(string, string, System\.Nullable\<System\.TimeSpan\>\)') | Initializes a new instance of the [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException') class\. |
| [CurlTimeoutException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Initializes a new instance with serialized data\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_)'></a>

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

<a name='ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

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