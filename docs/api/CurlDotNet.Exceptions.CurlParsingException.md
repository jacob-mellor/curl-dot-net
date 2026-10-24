#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlParsingException Class

Thrown when content parsing fails

```csharp
public class CurlParsingException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlParsingException

| Constructors | |
| :--- | :--- |
| [CurlParsingException\(string, string, Type, string, Exception\)](CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).md 'CurlDotNet\.Exceptions\.CurlParsingException\.CurlParsingException\(string, string, System\.Type, string, System\.Exception\)') | Initializes a new instance of the [CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException') class |

| Properties | |
| :--- | :--- |
| [ContentType](CurlDotNet.Exceptions.CurlParsingException.ContentType.md 'CurlDotNet\.Exceptions\.CurlParsingException\.ContentType') | Gets the content type of the data that failed to parse |
| [ExpectedType](CurlDotNet.Exceptions.CurlParsingException.ExpectedType.md 'CurlDotNet\.Exceptions\.CurlParsingException\.ExpectedType') | Gets the expected type that the content could not be parsed into |
