#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')

## CurlWriteErrorException Constructors

| Overloads | |
| :--- | :--- |
| [CurlWriteErrorException\(string, string, string\)](CurlDotNet.Exceptions.CurlWriteErrorException.#ctor.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string) 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string\)') | Creates the write error \(exit code 23\) for [filePath](CurlDotNet.Exceptions.CurlWriteErrorException.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string).filePath 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string\)\.filePath'), prefixing curl's `Write error for PATH:` to the caller's explanation\. |
| [CurlWriteErrorException\(string, string, string, Exception\)](CurlDotNet.Exceptions.CurlWriteErrorException.#ctor.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string, System\.Exception\)') | Creates the write error \(exit code 23\) for [filePath](CurlDotNet.Exceptions.CurlWriteErrorException.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string,System.Exception).filePath 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string, System\.Exception\)\.filePath') and keeps the \.NET exception that caused it \(an [System\.IO\.IOException](https://learn.microsoft.com/en-us/dotnet/api/system.io.ioexception 'System\.IO\.IOException'), [System\.UnauthorizedAccessException](https://learn.microsoft.com/en-us/dotnet/api/system.unauthorizedaccessexception 'System\.UnauthorizedAccessException') and so on\) as [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string)'></a>

## CurlWriteErrorException\(string, string, string\) Constructor

Creates the write error \(exit code 23\) for [filePath](CurlDotNet.Exceptions.CurlWriteErrorException.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string).filePath 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string\)\.filePath'), prefixing curl's
`Write error for PATH:` to the caller's explanation\.

```csharp
public CurlWriteErrorException(string filePath, string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path where the error occurred\.

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Why the write failed and, optionally, how to fix it \(newline\-separated\)\.

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlWriteErrorException.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string).message 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string\)\.message') has no fix, the suggestion names the folder and mentions
            `--create-dirs`, permissions and disk space.
            Example: `throw new CurlWriteErrorException(path, "the directory 'out' does not exist.", command);`

### See Also
- [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string,System.Exception)'></a>

## CurlWriteErrorException\(string, string, string, Exception\) Constructor

Creates the write error \(exit code 23\) for [filePath](CurlDotNet.Exceptions.CurlWriteErrorException.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string,System.Exception).filePath 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string, System\.Exception\)\.filePath') and keeps the \.NET exception that
caused it \(an [System\.IO\.IOException](https://learn.microsoft.com/en-us/dotnet/api/system.io.ioexception 'System\.IO\.IOException'), [System\.UnauthorizedAccessException](https://learn.microsoft.com/en-us/dotnet/api/system.unauthorizedaccessexception 'System\.UnauthorizedAccessException') and so on\) as
[System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\.

```csharp
public CurlWriteErrorException(string filePath, string message, string? command, System.Exception? innerException);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string,System.Exception).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file that could not be written\.

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Why the write failed and how to fix it \(newline\-separated\)\.

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The \.NET exception that caused the failure, or null\.

### Remarks

The message should still say in words what the inner exception reported; if it does not, the first
            line of the inner message is appended as `(caused by IOException: ...)`.
            Example: `catch (IOException io) { throw new CurlWriteErrorException(path, $"the disk is full ({io.Message}).\nFree space or write elsewhere.", command, io); }`

### See Also
- [CurlWriteErrorException\(string, string, string\)](CurlDotNet.Exceptions.CurlWriteErrorException.#ctor.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string) 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string\)')