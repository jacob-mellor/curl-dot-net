#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlSslPinnedPublicKeyMismatchException Class

The server's public key did not match any `--pinnedpubkey` pin, or the pin value could not be read \(exit code
90, CURLE\_SSL\_PINNEDPUBKEYNOTMATCH\)\.

```csharp
public class CurlSslPinnedPublicKeyMismatchException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslPinnedPublicKeyMismatchException

### Remarks

Pins are checked after certificate verification and also with `-k`, like curl. A mismatch means the server
            presented a different key than the one you pinned: the certificate was renewed with a new key, you reached another
            server (a TLS-inspecting proxy, for example), or the pin is wrong. Fixes: pin the current key (the message shows the
            presented `sha256//...` value), list the old and new pins separated by `;` during a key rotation, or check
            the network path. Needs .NET 5 or later; on .NET Standard 2.0 and .NET Framework the option is listed in
            [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions').

Example: `catch (CurlSslPinnedPublicKeyMismatchException ex) { alert.Send(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md

### See Also
- [PinnedPublicKey](CurlDotNet.Core.CurlOptions.PinnedPublicKey.md 'CurlDotNet\.Core\.CurlOptions\.PinnedPublicKey')
- [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')

| Constructors | |
| :--- | :--- |
| [CurlSslPinnedPublicKeyMismatchException\(string, string\)](CurlDotNet.Exceptions.CurlSslPinnedPublicKeyMismatchException.CurlSslPinnedPublicKeyMismatchException(string,string).md 'CurlDotNet\.Exceptions\.CurlSslPinnedPublicKeyMismatchException\.CurlSslPinnedPublicKeyMismatchException\(string, string\)') | Creates the pinned\-key error \(exit code 90\)\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlSslPinnedPublicKeyMismatchException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlSslPinnedPublicKeyMismatchException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
