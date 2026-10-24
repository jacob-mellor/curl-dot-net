#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlUploadFailedException](CurlDotNet.Exceptions.CurlUploadFailedException.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException')

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