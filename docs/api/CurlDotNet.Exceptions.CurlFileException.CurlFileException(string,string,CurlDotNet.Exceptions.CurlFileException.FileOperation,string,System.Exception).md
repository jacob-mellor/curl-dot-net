#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFileException](CurlDotNet.Exceptions.CurlFileException.md 'CurlDotNet\.Exceptions\.CurlFileException')

## CurlFileException\(string, string, FileOperation, string, Exception\) Constructor

Creates a file error naming the path and the operation, keeping the original I/O exception as the inner
exception so its details are not lost\.

```csharp
public CurlFileException(string message, string filePath, CurlDotNet.Exceptions.CurlFileException.FileOperation operation, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What failed, naming the file; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The path of the file that failed, as it was given\.

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).operation'></a>

`operation` [FileOperation](CurlDotNet.Exceptions.CurlFileException.FileOperation.md 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation')

What was being done with the file: read, write, create, delete, upload or download\.

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed, or null\.

<a name='CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The underlying I/O exception, kept as InnerException; may be null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).md#CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).message 'CurlDotNet\.Exceptions\.CurlFileException\.CurlFileException\(string, string, CurlDotNet\.Exceptions\.CurlFileException\.FileOperation, string, System\.Exception\)\.message') has no fix, the advice depends on [operation](CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).md#CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).operation 'CurlDotNet\.Exceptions\.CurlFileException\.CurlFileException\(string, string, CurlDotNet\.Exceptions\.CurlFileException\.FileOperation, string, System\.Exception\)\.operation') (check that
            the file exists for reads, that the folder exists and is writable for writes) and names
            [filePath](CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).md#CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).filePath 'CurlDotNet\.Exceptions\.CurlFileException\.CurlFileException\(string, string, CurlDotNet\.Exceptions\.CurlFileException\.FileOperation, string, System\.Exception\)\.filePath'); the inner exception's message is added to the first line when the message does
            not already contain it.

Example: `throw new CurlFileException($"Could not save {path}", path, CurlFileException.FileOperation.Download, cmd, ioException);`

### See Also
- [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')