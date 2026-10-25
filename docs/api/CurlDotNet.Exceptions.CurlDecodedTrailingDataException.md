#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlDecodedTrailingDataException Class

A `--compressed` body went on after its gzip stream had ended \(exit code 23, CURLE\_WRITE\_ERROR, as in curl\):
either a second gzip member followed the first \(`Multi-member gzip response not supported`\) or other bytes did\.

```csharp
public class CurlDecodedTrailingDataException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlDecodedTrailingDataException

### Remarks

curl's gzip decoder (lib/content_encoding.c) decodes exactly one gzip member. When the decoded member is followed
            by the gzip magic bytes `1f 8b` it fails with `Multi-member gzip response not supported`; any other bytes
            after the member's 8-byte trailer fail with curl's plain write error text
            `Failed writing received data to disk/application`. CurlDotNet reports both with the same exit code and the same
            first message line, so scripts that parse curl's `curl: (23) ...` line keep working.

Fixes: drop `--compressed` to save the raw bytes and decompress them with a tool that reads concatenated
            gzip members (`gzip -dc`), or have the server send a single gzip member.

Example: `catch (CurlDecodedTrailingDataException ex) when (ex.AnotherGzipMember) { /* fetch again without --compressed */ }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md

### See Also
- [CurlBadContentEncodingException](CurlDotNet.Exceptions.CurlBadContentEncodingException.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException')
- [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')

| Constructors | |
| :--- | :--- |
| [CurlDecodedTrailingDataException\(bool, long, string\)](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.CurlDecodedTrailingDataException(bool,long,string).md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.CurlDecodedTrailingDataException\(bool, long, string\)') | Creates the error \(exit code 23\) for [trailingBytes](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.CurlDecodedTrailingDataException(bool,long,string).md#CurlDotNet.Exceptions.CurlDecodedTrailingDataException.CurlDecodedTrailingDataException(bool,long,string).trailingBytes 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.CurlDecodedTrailingDataException\(bool, long, string\)\.trailingBytes') bytes found after the end of a gzip member in a body decoded for `--compressed`\. |

| Properties | |
| :--- | :--- |
| [AnotherGzipMember](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.AnotherGzipMember.md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.AnotherGzipMember') | True when the bytes after the first gzip member are a second gzip member \(the server concatenated several compressed pieces\), false when they are something else\. |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [TrailingBytes](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.TrailingBytes.md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.TrailingBytes') | How many body bytes followed the end of the first gzip member\. |
