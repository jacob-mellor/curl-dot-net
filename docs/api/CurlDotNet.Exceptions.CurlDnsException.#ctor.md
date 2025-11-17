#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException')

## CurlDnsException Constructors

| Overloads | |
| :--- | :--- |
| [CurlDnsException\(string, string\)](CurlDotNet.Exceptions.CurlDnsException.#ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string) 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(string, string\)') | Initializes a new instance of the [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException') class\. |
| [CurlDnsException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlDnsException.#ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Initializes a new instance with serialized data\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string)'></a>

## CurlDnsException\(string, string\) Constructor

Initializes a new instance of the [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException') class\.

```csharp
public CurlDnsException(string hostname, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string).hostname'></a>

`hostname` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The hostname that could not be resolved\.

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing\.

<a name='ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlDnsException\(SerializationInfo, StreamingContext\) Constructor

Initializes a new instance with serialized data\.

```csharp
protected CurlDnsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')