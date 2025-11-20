#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlFileException Class

Thrown when file operations fail

```csharp
public class CurlFileException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFileException
### Constructors

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception)'></a>

## CurlFileException\(string, string, FileOperation, string, Exception\) Constructor

Initializes a new instance of the [CurlFileException](CurlDotNet.Exceptions.CurlFileException.md 'CurlDotNet\.Exceptions\.CurlFileException') class

```csharp
public CurlFileException(string message, string filePath, CurlDotNet.Exceptions.CurlFileException.FileOperation operation, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the file operation failure

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The path to the file that caused the error

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).operation'></a>

`operation` [FileOperation](CurlDotNet.Exceptions.CurlFileException.FileOperation.md 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation')

The type of file operation that failed

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The underlying exception that caused this error
### Properties

<a name='CurlDotNet.Exceptions.CurlFileException.FilePath'></a>

## CurlFileException\.FilePath Property

Gets the file path that caused the error\.

```csharp
public string FilePath { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Exceptions.CurlFileException.Operation'></a>

## CurlFileException\.Operation Property

Gets the file operation that failed\.

```csharp
public CurlDotNet.Exceptions.CurlFileException.FileOperation Operation { get; }
```

#### Property Value
[FileOperation](CurlDotNet.Exceptions.CurlFileException.FileOperation.md 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation')