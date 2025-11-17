#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFileException Class

Thrown when file operations fail

```csharp
public class CurlFileException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFileException

| Constructors | |
| :--- | :--- |
| [CurlFileException\(string, string, FileOperation, string, Exception\)](CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).md 'CurlDotNet\.Exceptions\.CurlFileException\.CurlFileException\(string, string, CurlDotNet\.Exceptions\.CurlFileException\.FileOperation, string, System\.Exception\)') | |

| Properties | |
| :--- | :--- |
| [FilePath](CurlDotNet.Exceptions.CurlFileException.FilePath.md 'CurlDotNet\.Exceptions\.CurlFileException\.FilePath') | Gets the file path that caused the error\. |
| [Operation](CurlDotNet.Exceptions.CurlFileException.Operation.md 'CurlDotNet\.Exceptions\.CurlFileException\.Operation') | Gets the file operation that failed\. |
