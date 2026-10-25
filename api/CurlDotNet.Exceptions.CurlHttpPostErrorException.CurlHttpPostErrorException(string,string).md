#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpPostErrorException](CurlDotNet.Exceptions.CurlHttpPostErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpPostErrorException')

## CurlHttpPostErrorException\(string, string\) Constructor

Creates the form\-post error \(exit code 34\), prefixing `HTTP POST error:` to the caller's explanation of
which field failed\.

```csharp
public CurlHttpPostErrorException(string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpPostErrorException.CurlHttpPostErrorException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Which form field failed and why; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlHttpPostErrorException.CurlHttpPostErrorException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

Example: `throw new CurlHttpPostErrorException("field 'avatar' names a folder, not a file.", command);`

### See Also
- [CurlUploadFailedException](CurlDotNet.Exceptions.CurlUploadFailedException.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException')