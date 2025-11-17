#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException Constructors

| Overloads | |
| :--- | :--- |
| [CurlException\(string, int, string\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string\)') | Initializes a new instance of the [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') class with a curl error code\. |
| [CurlException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, string, System\.Exception\)') | Initializes a new instance of the [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') class with a specified error message\. |
| [CurlException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Initializes a new instance of the [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') class with serialized data\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string)'></a>

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

<a name='ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception)'></a>

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

<a name='ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

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