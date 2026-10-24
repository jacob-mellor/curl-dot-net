#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlOptionSyntaxException Class

An option's value has the wrong syntax \(CURLE\_SETOPT\_OPTION\_SYNTAX, exit code 49 for the overload that sets it\),
for example `--resolve` or `--connect-to` entries with a port that is not a number\.

```csharp
public class CurlOptionSyntaxException : CurlDotNet.Exceptions.CurlInvalidCommandException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException') &#129106; CurlOptionSyntaxException

### Remarks

A [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException'); nothing was sent. The message explains the expected format, such
            as `--resolve HOST:PORT:ADDRESS`. Example: `catch (CurlOptionSyntaxException ex) { Console.WriteLine(ex.Suggestions[0]); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

### See Also
- [CurlUnknownOptionException](CurlDotNet.Exceptions.CurlUnknownOptionException.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException')
- [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')

| Constructors | |
| :--- | :--- |
| [CurlOptionSyntaxException\(string, string\)](CurlDotNet.Exceptions.CurlOptionSyntaxException.#ctor.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string) 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException\.CurlOptionSyntaxException\(string, string\)') | Creates a syntax error for [option](CurlDotNet.Exceptions.CurlOptionSyntaxException.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string).option 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException\.CurlOptionSyntaxException\(string, string\)\.option') without an exit code, with advice to check the value's format in curl's manual\. |
| [CurlOptionSyntaxException\(string, string, string\)](CurlDotNet.Exceptions.CurlOptionSyntaxException.#ctor.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string,string) 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException\.CurlOptionSyntaxException\(string, string, string\)') | Creates the error for an option value curl refuses to parse \(exit code 49\), such as `--resolve` or `--connect-to` entries with a port that is not a number\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlOptionSyntaxException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
