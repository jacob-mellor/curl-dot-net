#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlUnknownOptionException Class

The command uses an option curl does not have \(CURLE\_UNKNOWN\_OPTION\); [OptionName](CurlDotNet.Exceptions.CurlUnknownOptionException.OptionName.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException\.OptionName') names it, usually
a typo or an option from a newer curl\.

```csharp
public class CurlUnknownOptionException : CurlDotNet.Exceptions.CurlInvalidCommandException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException') &#129106; CurlUnknownOptionException

### Remarks

A [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException'); nothing was sent. Fixes: check the spelling (`--header`, not
            `--headers`), and the option list in `curl --help all`. `Curl.Validate(command)` reports unknown options
            without throwing. Example: `catch (CurlUnknownOptionException ex) { Console.WriteLine($"Unknown: {ex.OptionName}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/curl-cli-compatibility.md

### See Also
- [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')
- [CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException')

| Constructors | |
| :--- | :--- |
| [CurlUnknownOptionException\(string, string\)](CurlDotNet.Exceptions.CurlUnknownOptionException.CurlUnknownOptionException(string,string).md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException\.CurlUnknownOptionException\(string, string\)') | Creates the error for an unknown option, with curl's wording `option NAME: is unknown` and advice to check the spelling\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlUnknownOptionException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlUnknownOptionException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [OptionName](CurlDotNet.Exceptions.CurlUnknownOptionException.OptionName.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException\.OptionName') | The option that curl does not recognise, exactly as written, such as `--headers`\. |
