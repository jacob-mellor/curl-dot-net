#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlInvalidCommandException Class

Thrown when the curl command syntax is invalid or cannot be parsed\.

```csharp
public class CurlInvalidCommandException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlInvalidCommandException

Derived  
&#8627; [CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException')  
&#8627; [CurlUnknownOptionException](CurlDotNet.Exceptions.CurlUnknownOptionException.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException')

### Example

```csharp
try
{
    // Missing URL will throw CurlInvalidCommandException
    var result = await curl.ExecuteAsync("curl -X POST");
}
catch (CurlInvalidCommandException ex)
{
    Console.WriteLine($"Invalid command syntax: {ex.Message}");
    Console.WriteLine($"Problem with: {ex.InvalidPart}");
    // Suggest correction to user
    Console.WriteLine("Did you forget to specify a URL?");
}
```

### Remarks

This exception indicates a problem with the curl command syntax, not a network or execution error.

AI-Usage: Catch this to handle command syntax errors separately from execution errors.

AI-Pattern: Validate commands before execution to avoid this exception.
### Constructors

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string)'></a>

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

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

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
### Properties

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.InvalidPart'></a>

## CurlInvalidCommandException\.InvalidPart Property

Gets the part of the command that is invalid\.

```csharp
public string InvalidPart { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The specific option, argument, or syntax element that caused the parsing error\.

### Remarks

This helps identify exactly what part of the command is wrong.

AI-Usage: Use this to provide specific feedback about what needs to be corrected.
### Methods

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlInvalidCommandException\.GetObjectData\(SerializationInfo, StreamingContext\) Method

Sets the [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo') with information about the exception\.

```csharp
public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The serialization information\.

<a name='CurlDotNet.Exceptions.CurlInvalidCommandException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The streaming context\.

Implements [GetObjectData\(SerializationInfo, StreamingContext\)](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable.getobjectdata#system-runtime-serialization-iserializable-getobjectdata(system-runtime-serialization-serializationinfo-system-runtime-serialization-streamingcontext) 'System\.Runtime\.Serialization\.ISerializable\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo,System\.Runtime\.Serialization\.StreamingContext\)')