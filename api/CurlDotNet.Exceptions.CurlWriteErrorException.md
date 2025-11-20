#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlWriteErrorException Class

CURLE\_WRITE\_ERROR \(23\) \- Write error

```csharp
public class CurlWriteErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlWriteErrorException
### Constructors

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string)'></a>

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
### Properties

<a name='CurlDotNet.Exceptions.CurlWriteErrorException.FilePath'></a>

## CurlWriteErrorException\.FilePath Property

The file path where the write error occurred\.

```csharp
public string FilePath { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')