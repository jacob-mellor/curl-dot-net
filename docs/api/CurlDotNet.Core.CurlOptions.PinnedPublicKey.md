#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.PinnedPublicKey Property

Public key pins the server must match, from curl's `--pinnedpubkey`: `sha256//BASE64` hashes of the
server's SubjectPublicKeyInfo \(several separated by `;`\), or the path of a PEM/DER public key file\.

```csharp
public string? PinnedPublicKey { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The pin text or file path\. Default: null \(no pinning\)\.

### Remarks

Applied on .NET 5 and later, after certificate verification and also with [Insecure](CurlDotNet.Core.CurlOptions.Insecure.md 'CurlDotNet\.Core\.CurlOptions\.Insecure'), like curl.
            A server key that matches no pin, or a pin that cannot be read, fails with
            [CurlSslPinnedPublicKeyMismatchException](CurlDotNet.Exceptions.CurlSslPinnedPublicKeyMismatchException.md 'CurlDotNet\.Exceptions\.CurlSslPinnedPublicKeyMismatchException') (exit 90); the message shows the pin of
            the key that was presented. Where pinning cannot be applied - on .NET Standard 2.0 and .NET Framework (where the
            parser also records the option in [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions')), or when you passed your own `HttpClient` to
            the engine - an https transfer fails with [CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException') (exit 4) instead
            of silently skipping the check, like curl built without pinning support.

Example: `curl --pinnedpubkey "sha256//YhKJKSzoTt2b5FP18fvpHo7fJYqQCjAa3HWY3tvRMwE=" https://api.example.com/`.

### See Also
- [CaCertFile](CurlDotNet.Core.CurlOptions.CaCertFile.md 'CurlDotNet\.Core\.CurlOptions\.CaCertFile')