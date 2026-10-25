#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')

## CurlReadErrorException Constructors

| Overloads | |
| :--- | :--- |
| [CurlReadErrorException\(string, string, string\)](CurlDotNet.Exceptions.CurlReadErrorException.#ctor.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string) 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string\)') | Creates the read error \(exit code 26\) for [filePath](CurlDotNet.Exceptions.CurlReadErrorException.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string).filePath 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string\)\.filePath'), prefixing `Read error for PATH:` to the caller's explanation\. |
| [CurlReadErrorException\(string, string, string, Exception\)](CurlDotNet.Exceptions.CurlReadErrorException.#ctor.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string, System\.Exception\)') | Creates the read error \(exit code 26\) for [filePath](CurlDotNet.Exceptions.CurlReadErrorException.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string,System.Exception).filePath 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string, System\.Exception\)\.filePath') and keeps the \.NET exception that caused it \([System\.IO\.FileNotFoundException](https://learn.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception 'System\.IO\.FileNotFoundException'), [System\.UnauthorizedAccessException](https://learn.microsoft.com/en-us/dotnet/api/system.unauthorizedaccessexception 'System\.UnauthorizedAccessException')\.\.\.\) as [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string)'></a>

## CurlReadErrorException\(string, string, string\) Constructor

Creates the read error \(exit code 26\) for [filePath](CurlDotNet.Exceptions.CurlReadErrorException.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string).filePath 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string\)\.filePath'), prefixing `Read error for PATH:` to
the caller's explanation\.

```csharp
public CurlReadErrorException(string filePath, string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path where the error occurred\.

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Why the read failed and, optionally, how to fix it \(newline\-separated\)\.

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlReadErrorException.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string).message 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string\)\.message') has no fix, the suggestion names the full path and the current directory.
            Example: `throw new CurlReadErrorException("body.json", "the file does not exist.", command);`

### See Also
- [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string,System.Exception)'></a>

## CurlReadErrorException\(string, string, string, Exception\) Constructor

Creates the read error \(exit code 26\) for [filePath](CurlDotNet.Exceptions.CurlReadErrorException.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string,System.Exception).filePath 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string, System\.Exception\)\.filePath') and keeps the \.NET exception that
caused it \([System\.IO\.FileNotFoundException](https://learn.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception 'System\.IO\.FileNotFoundException'), [System\.UnauthorizedAccessException](https://learn.microsoft.com/en-us/dotnet/api/system.unauthorizedaccessexception 'System\.UnauthorizedAccessException')\.\.\.\) as
[System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\.

```csharp
public CurlReadErrorException(string filePath, string message, string? command, System.Exception? innerException);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string,System.Exception).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file that could not be read\.

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Why the read failed and how to fix it \(newline\-separated\)\.

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The \.NET exception that caused the failure, or null\.

### Remarks

Example: `catch (UnauthorizedAccessException denied) { throw new CurlReadErrorException(path, $"access is denied ({denied.Message}).\nGrant read permission or copy the file somewhere readable.", command, denied); }`

### See Also
- [CurlReadErrorException\(string, string, string\)](CurlDotNet.Exceptions.CurlReadErrorException.#ctor.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string) 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string\)')