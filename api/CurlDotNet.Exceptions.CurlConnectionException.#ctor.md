#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')

## CurlConnectionException Constructors

| Overloads | |
| :--- | :--- |
| [CurlConnectionException\(string, int, string, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, int, string, System\.Nullable\<int\>, string\)') | Initializes a new instance of the [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') class with an error code\. |
| [CurlConnectionException\(string, string, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)') | Initializes a new instance of the [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') class\. |
| [CurlConnectionException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Initializes a new instance with serialized data\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string)'></a>

## CurlConnectionException\(string, int, string, Nullable\<int\>, string\) Constructor

Initializes a new instance of the [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') class with an error code\.

```csharp
protected CurlConnectionException(string message, int curlErrorCode, string host, System.Nullable<int> port=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the connection failure\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).curlErrorCode'></a>

`curlErrorCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The curl error code\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).host'></a>

`host` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host that could not be connected to\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).port'></a>

`port` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The port number that was attempted\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing\.

<a name='ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string)'></a>

## CurlConnectionException\(string, string, Nullable\<int\>, string\) Constructor

Initializes a new instance of the [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') class\.

```csharp
public CurlConnectionException(string message, string host, System.Nullable<int> port=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the connection failure\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).host'></a>

`host` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host that could not be connected to\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).port'></a>

`port` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The port number that was attempted\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing\.

<a name='ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlConnectionException\(SerializationInfo, StreamingContext\) Constructor

Initializes a new instance with serialized data\.

```csharp
protected CurlConnectionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')