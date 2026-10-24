#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlReadErrorException Class

A local file the command needs could not be read \(exit code 26, CURLE\_READ\_ERROR\): the file after `@` in
`-d @file`, `--data-binary @file`, `-F name=@file`, `-T file`, `-K file` or `.netrc`\.

```csharp
public class CurlReadErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlReadErrorException

### Remarks

Thrown before the request is sent. [FilePath](CurlDotNet.Exceptions.CurlReadErrorException.FilePath.md 'CurlDotNet\.Exceptions\.CurlReadErrorException\.FilePath') names the file. Fixes: check the path (relative paths
            start in the process's current directory, not next to your .exe), the permissions, and that no other program holds
            it locked. Example: `catch (CurlReadErrorException ex) { Console.WriteLine($"Cannot read {Path.GetFullPath(ex.FilePath)}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')
- [CurlFileCouldntReadException](CurlDotNet.Exceptions.CurlFileCouldntReadException.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException')

| Constructors | |
| :--- | :--- |
| [CurlReadErrorException\(string, string, string\)](CurlDotNet.Exceptions.CurlReadErrorException.#ctor.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string) 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string\)') | Creates the read error \(exit code 26\) for [filePath](CurlDotNet.Exceptions.CurlReadErrorException.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string).filePath 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string\)\.filePath'), prefixing `Read error for PATH:` to the caller's explanation\. |
| [CurlReadErrorException\(string, string, string, Exception\)](CurlDotNet.Exceptions.CurlReadErrorException.#ctor.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string, System\.Exception\)') | Creates the read error \(exit code 26\) for [filePath](CurlDotNet.Exceptions.CurlReadErrorException.md#CurlDotNet.Exceptions.CurlReadErrorException.CurlReadErrorException(string,string,string,System.Exception).filePath 'CurlDotNet\.Exceptions\.CurlReadErrorException\.CurlReadErrorException\(string, string, string, System\.Exception\)\.filePath') and keeps the \.NET exception that caused it \([System\.IO\.FileNotFoundException](https://learn.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception 'System\.IO\.FileNotFoundException'), [System\.UnauthorizedAccessException](https://learn.microsoft.com/en-us/dotnet/api/system.unauthorizedaccessexception 'System\.UnauthorizedAccessException')\.\.\.\) as [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlReadErrorException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlReadErrorException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlReadErrorException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlReadErrorException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [FilePath](CurlDotNet.Exceptions.CurlReadErrorException.FilePath.md 'CurlDotNet\.Exceptions\.CurlReadErrorException\.FilePath') | The local file that could not be read, as given in the command \(relative to the process's current directory\)\. |
