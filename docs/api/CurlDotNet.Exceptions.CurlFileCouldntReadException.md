#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFileCouldntReadException Class

A local file could not be opened \(exit code 37, CURLE\_FILE\_COULDNT\_READ\_FILE\): the path of a `file://`
URL, or a missing file after `-d @`/`--data-binary @`; [FilePath](CurlDotNet.Exceptions.CurlFileCouldntReadException.FilePath.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.FilePath') names it\.

```csharp
public class CurlFileCouldntReadException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFileCouldntReadException

### Remarks

Thrown by the file handler for `file://` URLs it may not read, and by the HTTP handler when the file named
            in `-d @file` does not exist (a file that exists but cannot be read is [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException'), exit 26).
            Fixes: check the path (use three slashes for absolute paths:
            `file:///C:/data/x.txt` or `file:///home/me/x.txt`), that it is a file and not a folder, and the
            permissions. Example: `catch (CurlFileCouldntReadException ex) { Console.WriteLine($"No such file: {ex.FilePath}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

### See Also
- [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')

| Constructors | |
| :--- | :--- |
| [CurlFileCouldntReadException\(string, string\)](CurlDotNet.Exceptions.CurlFileCouldntReadException.#ctor.md#CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string) 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.CurlFileCouldntReadException\(string, string\)') | Creates the error \(exit code 37\) for a `file://` path that cannot be opened, with curl's wording `Couldn't open file PATH`\. |
| [CurlFileCouldntReadException\(string, string, string\)](CurlDotNet.Exceptions.CurlFileCouldntReadException.#ctor.md#CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string,string) 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.CurlFileCouldntReadException\(string, string, string\)') | Creates the error \(exit code 37\) for [filePath](CurlDotNet.Exceptions.CurlFileCouldntReadException.md#CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string,string).filePath 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.CurlFileCouldntReadException\(string, string, string\)\.filePath') with the caller's own explanation and fix, for situations other than a `file://` URL \(such as `-d @missing.json`\)\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlFileCouldntReadException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlFileCouldntReadException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [FilePath](CurlDotNet.Exceptions.CurlFileCouldntReadException.FilePath.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.FilePath') | The local path from the `file://` URL that could not be opened or read by the current user\. |
