#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFileException Class

A local file could not be read, written, created, deleted, uploaded or downloaded; [FilePath](CurlDotNet.Exceptions.CurlFileException.FilePath.md 'CurlDotNet\.Exceptions\.CurlFileException\.FilePath') and
[Operation](CurlDotNet.Exceptions.CurlFileException.Operation.md 'CurlDotNet\.Exceptions\.CurlFileException\.Operation') say which file and what was being done\.

```csharp
public class CurlFileException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFileException

### Remarks

CurlDotNet's own transfers report file problems with curl's exit codes instead:
            [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException') (23) for `-o`, `-D`, `-c`, `--trace` output and
            [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException') (26) for `-d @file`, `-F name=@file`, `-T` and `-K`. This
            type has no exit code and is meant for application code (for example a download helper) that wants to say
            which file failed. Keep the original `IOException` as the inner exception.

Example: `catch (IOException io) { throw new CurlFileException($"Could not save {path}", path, CurlFileException.FileOperation.Write, cmd, io); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md and
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')
- [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')

| Constructors | |
| :--- | :--- |
| [CurlFileException\(string, string, FileOperation, string, Exception\)](CurlDotNet.Exceptions.CurlFileException.CurlFileException(string,string,CurlDotNet.Exceptions.CurlFileException.FileOperation,string,System.Exception).md 'CurlDotNet\.Exceptions\.CurlFileException\.CurlFileException\(string, string, CurlDotNet\.Exceptions\.CurlFileException\.FileOperation, string, System\.Exception\)') | Creates a file error naming the path and the operation, keeping the original I/O exception as the inner exception so its details are not lost\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlFileException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlFileException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlFileException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlFileException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [FilePath](CurlDotNet.Exceptions.CurlFileException.FilePath.md 'CurlDotNet\.Exceptions\.CurlFileException\.FilePath') | The path of the file that failed, as it was given \(relative paths are relative to the process's current directory\)\. |
| [Operation](CurlDotNet.Exceptions.CurlFileException.Operation.md 'CurlDotNet\.Exceptions\.CurlFileException\.Operation') | What was being done with [FilePath](CurlDotNet.Exceptions.CurlFileException.FilePath.md 'CurlDotNet\.Exceptions\.CurlFileException\.FilePath') when it failed \(read, write, create, delete, upload or download\), so you can word the fix\. |
