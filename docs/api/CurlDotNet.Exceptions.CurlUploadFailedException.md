#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlUploadFailedException Class

An upload was rejected by the server \(exit code 25, CURLE\_UPLOAD\_FAILED\), for example an FTP STOR refused or an
upload stopped by the server; [FileName](CurlDotNet.Exceptions.CurlUploadFailedException.FileName.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException\.FileName') names the file\.

```csharp
public class CurlUploadFailedException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlUploadFailedException

### Remarks

Fixes: check the target path and that the account may write there, the server's size limit and free space.
            Example: `catch (CurlUploadFailedException ex) { Console.WriteLine($"Upload of {ex.FileName} refused"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')

| Constructors | |
| :--- | :--- |
| [CurlUploadFailedException\(string, string, string\)](CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).md 'CurlDotNet\.Exceptions\.CurlUploadFailedException\.CurlUploadFailedException\(string, string, string\)') | Creates the upload error \(exit code 25\) for [fileName](CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).md#CurlDotNet.Exceptions.CurlUploadFailedException.CurlUploadFailedException(string,string,string).fileName 'CurlDotNet\.Exceptions\.CurlUploadFailedException\.CurlUploadFailedException\(string, string, string\)\.fileName'), prefixing `Upload failed for FILE:` to the caller's explanation\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlUploadFailedException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlUploadFailedException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [FileName](CurlDotNet.Exceptions.CurlUploadFailedException.FileName.md 'CurlDotNet\.Exceptions\.CurlUploadFailedException\.FileName') | The name of the file whose upload the server refused, as given with `-T` or `-F name=@file`\. |
