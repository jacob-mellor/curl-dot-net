#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.Validate Method

| Overloads | |
| :--- | :--- |
| [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)') | Checks a curl command without sending anything and reports whether it can run, what is wrong with it, and exactly which URL, method, headers and body it would send\. |
| [Validate\(string, CurlFlavor\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Validate\(string, CurlDotNet\.CurlFlavor\)') | Checks a curl command copied from a specific shell without executing it, reading it with that shell's quoting, escaping, line\-continuation and variable rules\. |

<a name='CurlDotNet.Curl.Validate(string)'></a>

## Curl\.Validate\(string\) Method

Checks a curl command without sending anything and reports whether it can run, what is wrong with it, and
exactly which URL, method, headers and body it would send\.

```csharp
public static CurlDotNet.Core.ValidationResult Validate(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.Validate(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line to check, as for [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. Null or empty
            text is reported as invalid\.

#### Returns
[ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult')  
A [ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult'): `IsValid` \(true when the command can run\), `Error` \(null
            when valid; otherwise the reason, such as `No URL specified`, `Unknown option: --heder`,
            `Unsupported protocol: gopher` or the parser's message with its fix\) and `ParsedOptions` \(the
            [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') the command produced; set when valid and for unknown options, null otherwise\)\.

### Example

```csharp
var validation = Curl.Validate("curl -X POST --heder 'A: b' https://api.example.com");
if (!validation.IsValid)
    Console.WriteLine($"Invalid command: {validation.Error}");   // Unknown option: --heder
else
    Console.WriteLine(validation.ParsedOptions.Url);
```

### Remarks

Validation parses the command with [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor') and checks that it has a URL, that every
            option is known (typos such as `--heder` are reported here, while [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')
            ignores them), that the URL parses and that its scheme is supported. It does not contact the server, so a URL
            that does not exist is still "valid".

Never throws for bad input: null or empty text gives `IsValid == false` with the reason in
            `Error`.

Example: `var v = Curl.Validate(userInput);` then
            `if (!v.IsValid) ShowError(v.Error); else Console.WriteLine($"{v.ParsedOptions.Method ?? "GET"} {v.ParsedOptions.Url}");`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md

### See Also
- [Validate\(string, CurlFlavor\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Validate\(string, CurlDotNet\.CurlFlavor\)')
- [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')
- [Validate\(string\)](CurlDotNet.DotNetCurl.Validate(string).md 'CurlDotNet\.DotNetCurl\.Validate\(string\)')

<a name='CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor)'></a>

## Curl\.Validate\(string, CurlFlavor\) Method

Checks a curl command copied from a specific shell without executing it, reading it with that shell's
quoting, escaping, line\-continuation and variable rules\.

```csharp
public static CurlDotNet.Core.ValidationResult Validate(string command, CurlDotNet.CurlFlavor flavor);
```
#### Parameters

<a name='CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The command exactly as pasted\. Null or empty text is reported as invalid\.

<a name='CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor).flavor'></a>

`flavor` [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')

The shell it was written for \([Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') detects it\); overrides
            [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor') for this call\.

#### Returns
[ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult')  
A [ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult'): `IsValid`, `Error` \(for example an unterminated quote, an unknown
option or an untranslatable `Invoke-WebRequest` parameter, with the fix in the text\) and
`ParsedOptions`, whose [Flavor](CurlDotNet.Core.CurlOptions.Flavor.md 'CurlDotNet\.Core\.CurlOptions\.Flavor') is the flavor that was used and whose
`Data`, `Headers` and `Url` show exactly what would be sent\. Never throws for bad input\.

### Remarks

Example: `var v = Curl.Validate("curl -d '{\"a\":1}' https://x.example", CurlFlavor.Posix);`
            gives `v.ParsedOptions.Data == "{\\\"a\\\":1}"` (backslashes kept), while the same text with
            [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell') gives `{"a":1}`. Comparing the two is the quickest way to explain
            why a command behaves differently on another machine.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md

### See Also
- [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)')
- [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)')
- [DetectFlavor\(string\)](CurlDotNet.Curl.DetectFlavor(string).md 'CurlDotNet\.Curl\.DetectFlavor\(string\)')