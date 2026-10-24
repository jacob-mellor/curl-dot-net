#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlSslCaCertBadFileException Class

The CA bundle given with `--cacert` or the folder given with `--capath` could not be used \(exit code 77,
CURLE\_SSL\_CACERT\_BADFILE\): it does not exist, cannot be read, or contains no certificate\.

```csharp
public class CurlSslCaCertBadFileException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslCaCertBadFileException

### Remarks

Thrown before connecting, when the transfer's TLS settings are built (.NET 5 or later; on .NET Standard 2.0 and
            .NET Framework `--cacert` is listed in [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions') instead).
            Fixes: pass an existing PEM file with one or more `-----BEGIN CERTIFICATE-----` blocks, or a folder of such files.

Example: `catch (CurlSslCaCertBadFileException ex) { Console.WriteLine($"Bad CA bundle: {ex.FilePath}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md

### See Also
- [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')
- [CaCertFile](CurlDotNet.Core.CurlOptions.CaCertFile.md 'CurlDotNet\.Core\.CurlOptions\.CaCertFile')

| Constructors | |
| :--- | :--- |
| [CurlSslCaCertBadFileException\(string, string, string\)](CurlDotNet.Exceptions.CurlSslCaCertBadFileException.CurlSslCaCertBadFileException(string,string,string).md 'CurlDotNet\.Exceptions\.CurlSslCaCertBadFileException\.CurlSslCaCertBadFileException\(string, string, string\)') | Creates the error \(exit code 77\) for an unusable CA bundle or folder\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlSslCaCertBadFileException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlSslCaCertBadFileException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [FilePath](CurlDotNet.Exceptions.CurlSslCaCertBadFileException.FilePath.md 'CurlDotNet\.Exceptions\.CurlSslCaCertBadFileException\.FilePath') | The `--cacert` file or `--capath` folder that could not be used, as given in the command\. |
