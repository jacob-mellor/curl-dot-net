#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlReadErrorException Class

CURLE\_READ\_ERROR \(26\) \- Read error

```csharp
public class CurlReadErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlReadErrorException
### Constructors

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string)'></a>

## CurlReadErrorException\(string, string, string\) Constructor

Initializes a new instance of CurlReadErrorException\.

```csharp
public CurlReadErrorException(string filePath, string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path where the error occurred\.

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed\.
### Properties

<a name='CurlDotNet.Exceptions.CurlReadErrorException.FilePath'></a>

## CurlReadErrorException\.FilePath Property

The file path where the read error occurred\.

```csharp
public string FilePath { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')