#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlUploadFailedException](CurlDotNet.Exceptions.CurlUploadFailedException.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException')

## CurlUploadFailedException\.FileName Property

The name of the file whose upload the server refused, as given with `-T` or `-F name=@file`\.

```csharp
public string FileName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A local file name or path\.

### Remarks

Check its size against the server's upload limit.

### See Also
- [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command')