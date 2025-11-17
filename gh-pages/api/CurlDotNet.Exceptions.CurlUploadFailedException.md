#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlUploadFailedException Class

CURLE\_UPLOAD\_FAILED \(25\) \- Upload failed

```csharp
public class CurlUploadFailedException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlUploadFailedException
### Constructors

<a name='CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string)'></a>

## CurlUploadFailedException\(string, string, string\) Constructor

Initializes a new instance of the [CurlUploadFailedException](CurlDotNet.Exceptions.CurlUploadFailedException.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException') class\.

```csharp
public CurlUploadFailedException(string fileName, string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).fileName'></a>

`fileName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the file that failed to upload\.

<a name='CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message that explains the reason for the exception\.

<a name='CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.
### Properties

<a name='CurlDotNet.Exceptions.CurlUploadFailedException.FileName'></a>

## CurlUploadFailedException\.FileName Property

Gets the file name that failed to upload

```csharp
public string FileName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')