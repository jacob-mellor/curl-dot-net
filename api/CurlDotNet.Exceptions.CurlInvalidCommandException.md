#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

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

| Constructors | |
| :--- | :--- |
| [CurlInvalidCommandException\(string, string, string\)](CurlDotNet.Exceptions.CurlInvalidCommandException.#ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string) 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.CurlInvalidCommandException\(string, string, string\)') | Initializes a new instance of the [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException') class\. |
| [CurlInvalidCommandException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlInvalidCommandException.#ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.CurlInvalidCommandException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Initializes a new instance with serialized data\. |

| Properties | |
| :--- | :--- |
| [InvalidPart](CurlDotNet.Exceptions.CurlInvalidCommandException.InvalidPart.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.InvalidPart') | Gets the part of the command that is invalid\. |

| Methods | |
| :--- | :--- |
| [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlInvalidCommandException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Sets the [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo') with information about the exception\. |
