#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlBadDownloadResumeException Class

A resumed download could not continue from the requested offset \(exit code 36, CURLE\_BAD\_DOWNLOAD\_RESUME\);
[ResumeOffset](CurlDotNet.Exceptions.CurlBadDownloadResumeException.ResumeOffset.md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException\.ResumeOffset') says where it tried to resume\.

```csharp
public class CurlBadDownloadResumeException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlBadDownloadResumeException

### Remarks

Happens with `-C OFFSET`/`-C -` when the offset is beyond the remote file's size (the remote file
            changed or is shorter). Delete the partial file and download again without `-C`.
            Example: `catch (CurlBadDownloadResumeException ex) { File.Delete(partial); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [CurlRangeErrorException](CurlDotNet.Exceptions.CurlRangeErrorException.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException')
- [CurlPartialFileException](CurlDotNet.Exceptions.CurlPartialFileException.md 'CurlDotNet\.Exceptions\.CurlPartialFileException')

| Constructors | |
| :--- | :--- |
| [CurlBadDownloadResumeException\(long, string\)](CurlDotNet.Exceptions.CurlBadDownloadResumeException.CurlBadDownloadResumeException(long,string).md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException\.CurlBadDownloadResumeException\(long, string\)') | Creates the resume error \(exit code 36\) for [offset](CurlDotNet.Exceptions.CurlBadDownloadResumeException.CurlBadDownloadResumeException(long,string).md#CurlDotNet.Exceptions.CurlBadDownloadResumeException.CurlBadDownloadResumeException(long,string).offset 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException\.CurlBadDownloadResumeException\(long, string\)\.offset'), with advice to restart the download\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlBadDownloadResumeException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlBadDownloadResumeException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [ResumeOffset](CurlDotNet.Exceptions.CurlBadDownloadResumeException.ResumeOffset.md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException\.ResumeOffset') | The byte offset the download tried to resume from \(the size of the partial local file for `-C -`\)\. |
