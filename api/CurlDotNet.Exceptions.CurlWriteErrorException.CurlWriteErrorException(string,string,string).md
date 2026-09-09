#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')

## CurlWriteErrorException\(string, string, string\) Constructor

Initializes a new instance of CurlWriteErrorException\.

```csharp
public CurlWriteErrorException(string filePath, string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path where the error occurred\.

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed\.