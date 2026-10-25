#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlOutOfMemoryException Class

Memory ran out while building the request or holding the response \(exit code 27, CURLE\_OUT\_OF\_MEMORY\),
typically for a very large download read into `Body`\.

```csharp
public class CurlOutOfMemoryException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlOutOfMemoryException

### Remarks

CurlDotNet keeps the response in memory unless it is written to a file. For large downloads use `-o file`
            (or `Curl.DownloadAsync`) and avoid reading `Body`; limit sizes with `--max-filesize`.
            Example: `catch (CurlOutOfMemoryException) { await Curl.ExecuteAsync($"curl -o big.bin {url}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [CurlFileSizeExceededException](CurlDotNet.Exceptions.CurlFileSizeExceededException.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException')

| Constructors | |
| :--- | :--- |
| [CurlOutOfMemoryException\(string\)](CurlDotNet.Exceptions.CurlOutOfMemoryException.CurlOutOfMemoryException(string).md 'CurlDotNet\.Exceptions\.CurlOutOfMemoryException\.CurlOutOfMemoryException\(string\)') | Creates the out\-of\-memory error \(exit code 27\) with advice to stream large responses to a file instead of memory\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlOutOfMemoryException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlOutOfMemoryException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlOutOfMemoryException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlOutOfMemoryException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
