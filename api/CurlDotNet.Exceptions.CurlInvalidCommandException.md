#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlInvalidCommandException Class

Thrown before anything is sent when the curl command string cannot be understood: shell syntax the chosen
[CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor') cannot read, an Invoke\-WebRequest feature with no curl
equivalent, a `--next` group without a URL, or a malformed `--resolve`/`--connect-to` value\.

```csharp
public class CurlInvalidCommandException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlInvalidCommandException

Derived  
&#8627; [CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException')  
&#8627; [CurlUnknownOptionException](CurlDotNet.Exceptions.CurlUnknownOptionException.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException')

### Example

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl --resolve 80:example.com:127.0.0.1 http://example.com/");   // host and port swapped
}
catch (CurlInvalidCommandException ex)
{
    Console.WriteLine(ex.WhatHappened);
    Console.WriteLine($"Problem with: {ex.InvalidPart}");
}
```

### Remarks

Raised by the command parser (`Curl.ExecuteAsync`, `Curl.Execute`, `Curl.Validate`) and by the
            shell-flavor readers (bash, cmd, PowerShell, Invoke-WebRequest). Nothing has touched the network, so this is
            always a problem in the command text, never in the server. [InvalidPart](CurlDotNet.Exceptions.CurlInvalidCommandException.InvalidPart.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.InvalidPart') names the piece that
            could not be read. Derived types: [CurlUnknownOptionException](CurlDotNet.Exceptions.CurlUnknownOptionException.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException') (an option curl does not have,
            exit code 48 in curl) and [CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException') (a `--resolve`/`--connect-to` value
            curl cannot parse, exit code 49). Two other command problems use the base [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') instead: an
            option missing its value (`-H` at the end) or with a non-numeric value (exit code 2), and a command with no
            URL at all (exit code 3); unknown options are ignored by `Execute` and reported only by `Curl.Validate`.

Usual fixes: start with the URL or `curl URL`; quote values with spaces (`-H 'X-Name: a b'`);
            close every quote; when the command was copied from Windows cmd or PowerShell, pass the matching
            [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor') (or `CurlFlavor.Auto`).

Example: `try { await Curl.ExecuteAsync("curl --resolve 80:example.com:127.0.0.1 http://example.com/"); } catch (CurlInvalidCommandException ex) { Console.WriteLine($"Fix the command near '{ex.InvalidPart}': {ex.Suggestions[0]}"); }`.
            Call `Curl.Validate(command)` first to check user-supplied commands without throwing.

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md and
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md

### See Also
- [CurlUnknownOptionException](CurlDotNet.Exceptions.CurlUnknownOptionException.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException')
- [CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException')
- [CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException')

| Constructors | |
| :--- | :--- |
| [CurlInvalidCommandException\(string, string, string\)](CurlDotNet.Exceptions.CurlInvalidCommandException.#ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(string,string,string) 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.CurlInvalidCommandException\(string, string, string\)') | Creates the exception for a command that cannot be parsed, naming the part that is wrong so the caller can point at it\. |
| [CurlInvalidCommandException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlInvalidCommandException.#ctor.md#CurlDotNet.Exceptions.CurlInvalidCommandException.CurlInvalidCommandException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.CurlInvalidCommandException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Restores the exception, including [InvalidPart](CurlDotNet.Exceptions.CurlInvalidCommandException.InvalidPart.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.InvalidPart'), from serialized data on \.NET Framework\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlInvalidCommandException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlInvalidCommandException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [InvalidPart](CurlDotNet.Exceptions.CurlInvalidCommandException.InvalidPart.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.InvalidPart') | The piece of the command that could not be read: an option name such as `--frobnicate`, an option whose value is missing, or the unclosed quoted text\. |

| Methods | |
| :--- | :--- |
| [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlInvalidCommandException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Writes the exception, including [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') and [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode'), into serialization data so it can cross an AppDomain or remoting boundary on \.NET Framework\. |
