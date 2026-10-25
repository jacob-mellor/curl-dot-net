#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultFlavor Property

The shell \(bash, cmd\.exe, PowerShell, Invoke\-WebRequest\) whose quoting rules are used to read command strings
when a call does not name one; [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') by default, which detects it\.

```csharp
public static CurlDotNet.CurlFlavor DefaultFlavor { get; set; }
```

#### Property Value
[CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')  
One of the [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor') values\. Default is [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto')\.

#### Exceptions

[System\.ArgumentOutOfRangeException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception 'System\.ArgumentOutOfRangeException')  
The value is not a defined [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor') \(for
            example a number cast to the enum\)\. Fix: assign `CurlFlavor.Auto`, `Native`, `Posix`,
            `WindowsCmd`, `PowerShell` or `InvokeWebRequest`\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-flavors\-bash\-cmd\-powershell/README\.md

### Remarks

A pasted curl command is shell text: bash, Windows cmd.exe and PowerShell quote, escape,
            continue lines and expand variables differently, so the same text can mean different bytes (see
            [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor') for every rule). The default, [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto'), recognises the
            shell from telltales such as backtick or caret line continuations, `$env:`, `%VAR%` or
            `-Uri`, and otherwise reads the command permissively as bash. Set this once at start-up when every
            command your application runs comes from one place, for example
            `Curl.DefaultFlavor = CurlFlavor.WindowsCmd;` for commands typed by Windows users, or
            `Curl.DefaultFlavor = CurlFlavor.Posix;` to get exact bash behaviour with no Windows conveniences, or
            `Curl.DefaultFlavor = CurlFlavor.Native;` to read every command with the rules of the shell of the
            operating system the program runs on (cmd.exe on Windows, bash elsewhere).

Applies to [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)'), [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)'),
            [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)'), the code converters and every other method that takes a command string
            without a flavor. A flavor passed to [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)') or set with
            [WithFlavor\(CurlFlavor\)](CurlDotNet.Core.CurlSettings.WithFlavor(CurlDotNet.CurlFlavor).md 'CurlDotNet\.Core\.CurlSettings\.WithFlavor\(CurlDotNet\.CurlFlavor\)') wins over it. It is process-wide and read at the
            start of each call; reads and writes are thread-safe.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md

### See Also
- [DetectFlavor\(string\)](CurlDotNet.Curl.DetectFlavor(string).md 'CurlDotNet\.Curl\.DetectFlavor\(string\)')
- [Flavor](CurlDotNet.Core.CurlOptions.Flavor.md 'CurlDotNet\.Core\.CurlOptions\.Flavor')
- [WithFlavor\(CurlFlavor\)](CurlDotNet.Core.CurlSettings.WithFlavor(CurlDotNet.CurlFlavor).md 'CurlDotNet\.Core\.CurlSettings\.WithFlavor\(CurlDotNet\.CurlFlavor\)')
- [curl flavors guide](https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md 'https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-flavors\-bash\-cmd\-powershell/README\.md')