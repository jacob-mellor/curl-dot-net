#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlUploadFailedException](CurlDotNet.Exceptions.CurlUploadFailedException.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException')

## CurlUploadFailedException\(string, string, string\) Constructor

Creates the upload error \(exit code 25\) for [fileName](CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).md#CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).fileName 'CurlDotNet\.Exceptions\.CurlUploadFailedException\.CurlUploadFailedException\(string, string, string\)\.fileName'), prefixing `Upload failed for FILE:`
to the caller's explanation\.

```csharp
public CurlUploadFailedException(string fileName, string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).fileName'></a>

`fileName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the file that failed to upload\.

<a name='CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Why the upload failed and, optionally, how to fix it \(newline\-separated\)\.

<a name='CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Example: `throw new CurlUploadFailedException("big.iso", "the server answered 552 (quota exceeded).", command);`

### See Also
- [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')