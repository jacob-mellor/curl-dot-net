#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## ValidationResult Class

What [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)') found when it parsed a curl command without sending it:
whether it would run \([IsValid](CurlDotNet.Core.ValidationResult.IsValid.md 'CurlDotNet\.Core\.ValidationResult\.IsValid')\), why not \([Error](CurlDotNet.Core.ValidationResult.Error.md 'CurlDotNet\.Core\.ValidationResult\.Error')\) and the parsed options\.

```csharp
public class ValidationResult
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ValidationResult

### Remarks

Validation parses the command exactly as [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') would (same shell
            flavor rules), then checks that there is a URL, that every option is known and that the URL's scheme is
            one CurlDotNet implements (http, https, ftp, ftps, file). Nothing is sent over the network, so a
            valid command can still fail at run time (unknown host, refused connection, HTTP errors).

Example: `var check = Curl.Validate(userInput); if (!check.IsValid) { ShowError(check.Error); return; }`
            then `var result = await Curl.ExecuteAsync(userInput);`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md

### See Also
- [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)')
- [Validate\(string, CurlFlavor\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Validate\(string, CurlDotNet\.CurlFlavor\)')
- [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

| Properties | |
| :--- | :--- |
| [Error](CurlDotNet.Core.ValidationResult.Error.md 'CurlDotNet\.Core\.ValidationResult\.Error') | Why the command is not valid, worded for the person who wrote it; null when [IsValid](CurlDotNet.Core.ValidationResult.IsValid.md 'CurlDotNet\.Core\.ValidationResult\.IsValid') is true\. |
| [IsValid](CurlDotNet.Core.ValidationResult.IsValid.md 'CurlDotNet\.Core\.ValidationResult\.IsValid') | True when the command parsed, has a URL, uses only known options and a supported scheme, so executing it would at least start a transfer\. |
| [ParsedOptions](CurlDotNet.Core.ValidationResult.ParsedOptions.md 'CurlDotNet\.Core\.ValidationResult\.ParsedOptions') | The options the command parsed into \(URL, method, headers, data, flags\), for inspection or for running later with [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)')\. |
