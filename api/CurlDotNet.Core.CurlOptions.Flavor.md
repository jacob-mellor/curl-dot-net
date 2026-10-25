#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Flavor Property

The shell flavor the command text was read with: which quoting, escaping, line\-continuation and
variable rules turned [OriginalCommand](CurlDotNet.Core.CurlOptions.OriginalCommand.md 'CurlDotNet\.Core\.CurlOptions\.OriginalCommand') into curl arguments\.

```csharp
public CurlDotNet.CurlFlavor Flavor { get; set; }
```

#### Property Value
[CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')  
[Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') \(the default\) on options you build yourself\. After a command string
            has been parsed it is the flavor actually used: the one you asked for, or, when you asked for
            [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto'), the one Auto detected \([Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix'),
            [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd'), [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell') or
            [InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest')\)\. Every transfer of a multi\-URL command carries the same value\.

### Remarks

Setting it on a [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object you pass to [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)')
            has no effect, because nothing is parsed there; choose the flavor when you pass a command string, with
            [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)'), [WithFlavor\(CurlFlavor\)](CurlDotNet.Core.CurlSettings.WithFlavor(CurlDotNet.CurlFlavor).md 'CurlDotNet\.Core\.CurlSettings\.WithFlavor\(CurlDotNet\.CurlFlavor\)')
            or [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor').

Example: after `var v = Curl.Validate("curl.exe -d '{\"a\":1}' https://x.example", CurlFlavor.PowerShell);``v.ParsedOptions.Flavor` is `CurlFlavor.PowerShell`; log it next to the command when a pasted
            request does not behave the way it did in the terminal.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md

### See Also
- [DetectFlavor\(string\)](CurlDotNet.Curl.DetectFlavor(string).md 'CurlDotNet\.Curl\.DetectFlavor\(string\)')
- [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')
- [WithFlavor\(CurlFlavor\)](CurlDotNet.Core.CurlSettings.WithFlavor(CurlDotNet.CurlFlavor).md 'CurlDotNet\.Core\.CurlSettings\.WithFlavor\(CurlDotNet\.CurlFlavor\)')