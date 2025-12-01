#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')

## CurlInvalidCommandException Constructors

| Overloads | |
| :--- | :--- |
| [CurlInvalidCommandException\(string, string, string\)](CurlDotNet.Exceptions.CurlInvalidCommandException.#ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string) 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.CurlInvalidCommandException\(string, string, string\)') | Initializes a new instance of the [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException') class\. |
| [CurlInvalidCommandException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlInvalidCommandException.#ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.CurlInvalidCommandException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Initializes a new instance with serialized data\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string)'></a>

## CurlInvalidCommandException\(string, string, string\) Constructor

Initializes a new instance of the [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException') class\.

```csharp
public CurlInvalidCommandException(string message, string invalidPart=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the invalid syntax\.

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string).invalidPart'></a>

`invalidPart` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The specific part of the command that is invalid\.

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The full curl command that failed to parse\.

<a name='ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlInvalidCommandException\(SerializationInfo, StreamingContext\) Constructor

Initializes a new instance with serialized data\.

```csharp
protected CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')