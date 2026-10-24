#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlRangeErrorException Class

`-C/--continue-at` asked to resume, but the server sent the whole document \(or an error\) instead of the
            requested byte range \(exit code 33, CURLE\_RANGE\_ERROR\)\.

```csharp
public class CurlRangeErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlRangeErrorException

### Remarks

Thrown by the HTTP handler; curl refuses to append a full response to a partial download, which would corrupt
            it. [Offset](CurlDotNet.Exceptions.CurlRangeErrorException.Offset.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException\.Offset') is the resume position and [StatusCode](CurlDotNet.Exceptions.CurlRangeErrorException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException\.StatusCode') what the server answered (usually 200
            instead of 206). Download from the start (drop `-C`) or use a server that supports byte ranges
            (`Accept-Ranges: bytes`).

Example: `catch (CurlRangeErrorException ex) { File.Delete(partialFile); /* start over without -C */ }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [CurlBadDownloadResumeException](CurlDotNet.Exceptions.CurlBadDownloadResumeException.md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException')
- [CurlPartialFileException](CurlDotNet.Exceptions.CurlPartialFileException.md 'CurlDotNet\.Exceptions\.CurlPartialFileException')

| Constructors | |
| :--- | :--- |
| [CurlRangeErrorException\(long, int, string\)](CurlDotNet.Exceptions.CurlRangeErrorException.CurlRangeErrorException(long,int,string).md 'CurlDotNet\.Exceptions\.CurlRangeErrorException\.CurlRangeErrorException\(long, int, string\)') | Creates the range error \(exit code 33\) with curl's wording `HTTP server doesn't seem to support byte ranges. Cannot resume.` and the offset and status involved\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlRangeErrorException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlRangeErrorException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Offset](CurlDotNet.Exceptions.CurlRangeErrorException.Offset.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException\.Offset') | The resume offset that was requested, in bytes \(the size of the partial file for `-C -`\)\. |
| [StatusCode](CurlDotNet.Exceptions.CurlRangeErrorException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException\.StatusCode') | The HTTP status of the response that did not honour the range, usually 200 \(whole document\) instead of 206\. |
