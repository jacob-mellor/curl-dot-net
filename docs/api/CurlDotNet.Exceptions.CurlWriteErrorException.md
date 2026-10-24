#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlWriteErrorException Class

Output could not be written \(exit code 23, CURLE\_WRITE\_ERROR\): the file for `-o`, `-O`, `-D`,
`-c` or `--etag-save` could not be created or written; [FilePath](CurlDotNet.Exceptions.CurlWriteErrorException.FilePath.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.FilePath') names it\.

```csharp
public class CurlWriteErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlWriteErrorException

### Remarks

Thrown when the folder does not exist (without `--create-dirs`), the user may not write there, the disk is
            full, the name is invalid on this OS, `--no-clobber` ran out of names, or `-O -J` would overwrite a file.
            `-D` is checked before connecting, like curl. Not covered: `--trace`/`--trace-ascii` create their
            folder themselves, `--stderr` falls back to the console, and an `--etag-save` folder that does not exist
            skips the transfer with a warning instead of failing.

Example: `catch (CurlWriteErrorException ex) { Console.WriteLine($"Cannot write {ex.FilePath}: {ex.Suggestions[0]}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')
- [CurlFileException](CurlDotNet.Exceptions.CurlFileException.md 'CurlDotNet\.Exceptions\.CurlFileException')

| Constructors | |
| :--- | :--- |
| [CurlWriteErrorException\(string, string, string\)](CurlDotNet.Exceptions.CurlWriteErrorException.#ctor.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string) 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string\)') | Creates the write error \(exit code 23\) for [filePath](CurlDotNet.Exceptions.CurlWriteErrorException.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string).filePath 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string\)\.filePath'), prefixing curl's `Write error for PATH:` to the caller's explanation\. |
| [CurlWriteErrorException\(string, string, string, Exception\)](CurlDotNet.Exceptions.CurlWriteErrorException.#ctor.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string, System\.Exception\)') | Creates the write error \(exit code 23\) for [filePath](CurlDotNet.Exceptions.CurlWriteErrorException.md#CurlDotNet.Exceptions.CurlWriteErrorException.CurlWriteErrorException(string,string,string,System.Exception).filePath 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.CurlWriteErrorException\(string, string, string, System\.Exception\)\.filePath') and keeps the \.NET exception that caused it \(an [System\.IO\.IOException](https://learn.microsoft.com/en-us/dotnet/api/system.io.ioexception 'System\.IO\.IOException'), [System\.UnauthorizedAccessException](https://learn.microsoft.com/en-us/dotnet/api/system.unauthorizedaccessexception 'System\.UnauthorizedAccessException') and so on\) as [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlWriteErrorException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlWriteErrorException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [FilePath](CurlDotNet.Exceptions.CurlWriteErrorException.FilePath.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.FilePath') | The file that could not be written, as given in the command \(relative to the process's current directory\)\. |
