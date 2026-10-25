#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlPartialFileException Class

The server closed the connection before the whole body arrived \(exit code 18, CURLE\_PARTIAL\_FILE\):
fewer bytes than its Content\-Length, or a chunked body without its final chunk\.

```csharp
public class CurlPartialFileException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlPartialFileException

### Remarks

Thrown by the transport while reading the body. Whatever did arrive is still written to the `-o` file
            (unless `--remove-on-error` is used), exactly like curl, so a large download can be resumed with
            `-C -`. [BytesReceived](CurlDotNet.Exceptions.CurlPartialFileException.BytesReceived.md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.BytesReceived') and [BytesExpected](CurlDotNet.Exceptions.CurlPartialFileException.BytesExpected.md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.BytesExpected') say how far it got. Retrying helps when a
            proxy or flaky network cut the transfer; `--ignore-content-length` accepts what arrived when the server's length
            is known to be wrong.

Example: `catch (CurlPartialFileException ex) { Console.WriteLine($"{ex.BytesReceived} of {ex.BytesExpected} bytes"); await Curl.ExecuteAsync($"curl -C - -o big.iso {url}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [CurlReceiveErrorException](CurlDotNet.Exceptions.CurlReceiveErrorException.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException')
- [CurlRangeErrorException](CurlDotNet.Exceptions.CurlRangeErrorException.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException')

| Constructors | |
| :--- | :--- |
| [CurlPartialFileException\(string, long, Nullable&lt;long&gt;, string\)](CurlDotNet.Exceptions.CurlPartialFileException.CurlPartialFileException(string,long,System.Nullable_long_,string).md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.CurlPartialFileException\(string, long, System\.Nullable\<long\>, string\)') | Creates the partial\-transfer error \(exit code 18\) with curl's wording `transfer closed with N bytes remaining to read` and advice to retry or resume\. |

| Properties | |
| :--- | :--- |
| [BytesExpected](CurlDotNet.Exceptions.CurlPartialFileException.BytesExpected.md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.BytesExpected') | Body bytes the server announced with Content\-Length, or null for chunked bodies, where the total is unknown\. |
| [BytesReceived](CurlDotNet.Exceptions.CurlPartialFileException.BytesReceived.md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.BytesReceived') | Body bytes that arrived before the connection closed; also the resume offset for `-C -` when they were written to a file\. |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlPartialFileException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlPartialFileException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Url](CurlDotNet.Exceptions.CurlPartialFileException.Url.md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.Url') | The URL whose body was cut short, as requested \(it may contain credentials; the message shows a safe version\)\. |
