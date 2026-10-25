#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DetectFlavor\(string\) Method

Returns the shell flavor [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') would read [command](CurlDotNet.Curl.DetectFlavor(string).md#CurlDotNet.Curl.DetectFlavor(string).command 'CurlDotNet\.Curl\.DetectFlavor\(string\)\.command') with, without
parsing or sending anything\.

```csharp
public static CurlDotNet.CurlFlavor DetectFlavor(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.DetectFlavor(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The command text as pasted\. Null or empty text returns [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix')\.

#### Returns
[CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')  
[InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest') when it starts with `Invoke-WebRequest`,
            `Invoke-RestMethod`, `iwr`, `irm` or `wget`, or is a `curl` command using cmdlet
            parameters such as `-Uri`, `-Method` or `-Headers @{...}`;
            [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell') when a line ends in a backtick or it contains `$env:`,
            ``"` or `--%`; [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd') when a line ends in `^` or it contains
            `^"` or `%NAME%` naming a set environment variable; otherwise [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix')\.
            Never returns [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto')\.

### Remarks

Never throws. Use it in diagnostics, for example
            `logger.LogDebug("Running {Flavor} command {Command}", Curl.DetectFlavor(pasted), pasted);`, or to
            show users which shell rules their command will be read with before running it. When the guess is wrong,
            pass the right flavor to [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)').

Example: `Curl.DetectFlavor("curl -H ^\"Accept: */*^\" https://x.example")` returns
            [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd').

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md

### See Also
- [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')
- [Flavor](CurlDotNet.Core.CurlOptions.Flavor.md 'CurlDotNet\.Core\.CurlOptions\.Flavor')
- [Validate\(string, CurlFlavor\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Validate\(string, CurlDotNet\.CurlFlavor\)')