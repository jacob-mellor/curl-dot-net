#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')

## CurlInvalidCommandException Constructors

| Overloads | |
| :--- | :--- |
| [CurlInvalidCommandException\(string, string, string\)](CurlDotNet.Exceptions.CurlInvalidCommandException.#ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string) 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.CurlInvalidCommandException\(string, string, string\)') | Creates the exception for a command that cannot be parsed, naming the part that is wrong so the caller can point at it\. |
| [CurlInvalidCommandException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlInvalidCommandException.#ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.CurlInvalidCommandException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Restores the exception, including [InvalidPart](CurlDotNet.Exceptions.CurlInvalidCommandException.InvalidPart.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.InvalidPart'), from serialized data on \.NET Framework\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string)'></a>

## CurlInvalidCommandException\(string, string, string\) Constructor

Creates the exception for a command that cannot be parsed, naming the part that is wrong so the caller
can point at it\.

```csharp
public CurlInvalidCommandException(string message, string invalidPart=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What is wrong with the command, optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string).invalidPart'></a>

`invalidPart` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The token that could not be read \(an option or quoted text\), or null for the whole command\.

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The full command text that failed to parse, or null\.

### Remarks

The exit code stays null because the command never ran. When [message](CurlDotNet.Exceptions.CurlInvalidCommandException.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string).message 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.CurlInvalidCommandException\(string, string, string\)\.message') carries no fix, the
            suggestion tells the reader to correct [invalidPart](CurlDotNet.Exceptions.CurlInvalidCommandException.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string).invalidPart 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.CurlInvalidCommandException\(string, string, string\)\.invalidPart') and to quote values; the link points at
            the error reference.

Example: `throw new CurlInvalidCommandException("Option -H needs a value.", "-H", command);`

### See Also
- [CurlUnknownOptionException](CurlDotNet.Exceptions.CurlUnknownOptionException.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlInvalidCommandException\(SerializationInfo, StreamingContext\) Constructor

Restores the exception, including [InvalidPart](CurlDotNet.Exceptions.CurlInvalidCommandException.InvalidPart.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.InvalidPart'), from serialized data on \.NET Framework\.

```csharp
protected CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The serialized object data written by [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlInvalidCommandException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)')\.

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The source of the serialized stream, supplied by the serializer\.

### Remarks

Called by the serializer; not used directly.

### See Also
- [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlInvalidCommandException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)')