#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFileSizeExceededException Class

The response is larger than `--max-filesize` allows \(exit code 63, CURLE\_FILESIZE\_EXCEEDED\);
[MaxSize](CurlDotNet.Exceptions.CurlFileSizeExceededException.MaxSize.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.MaxSize') and [ActualSize](CurlDotNet.Exceptions.CurlFileSizeExceededException.ActualSize.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.ActualSize') give both sizes in bytes\.

```csharp
public class CurlFileSizeExceededException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFileSizeExceededException

### Remarks

Thrown by the HTTP handler either before reading (when Content-Length is announced) or while reading. Raise the
            limit or download something smaller. Example: `catch (CurlFileSizeExceededException ex) { Console.WriteLine($"{ex.ActualSize} > {ex.MaxSize}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [CurlOutOfMemoryException](CurlDotNet.Exceptions.CurlOutOfMemoryException.md 'CurlDotNet\.Exceptions\.CurlOutOfMemoryException')

| Constructors | |
| :--- | :--- |
| [CurlFileSizeExceededException\(long, long, string\)](CurlDotNet.Exceptions.CurlFileSizeExceededException.CurlFileSizeExceededException(long,long,string).md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.CurlFileSizeExceededException\(long, long, string\)') | Creates the error \(exit code 63\) with curl's wording `Exceeded the maximum allowed file size (MAX)` and the actual size\. |

| Properties | |
| :--- | :--- |
| [ActualSize](CurlDotNet.Exceptions.CurlFileSizeExceededException.ActualSize.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.ActualSize') | The size that exceeded the limit, in bytes: the announced Content\-Length, or the bytes read so far\. |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlFileSizeExceededException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlFileSizeExceededException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [MaxSize](CurlDotNet.Exceptions.CurlFileSizeExceededException.MaxSize.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.MaxSize') | The size limit set with `--max-filesize`, in bytes, that the response was not allowed to exceed\. |
