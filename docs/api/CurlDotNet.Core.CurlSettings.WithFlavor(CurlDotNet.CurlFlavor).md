#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithFlavor\(CurlFlavor\) Method

Sets the shell the command string was written for \(see [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')\) and returns these
settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithFlavor(CurlDotNet.CurlFlavor flavor);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithFlavor(CurlDotNet.CurlFlavor).flavor'></a>

`flavor` [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')

[Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') to detect it from the text; [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix') for bash/zsh
            \(macOS, Linux, Git Bash\); [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd') for `curl.exe` in cmd\.exe;
            [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell') for `curl.exe` in PowerShell; or
            [InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest') for PowerShell's `Invoke-WebRequest` / `curl` alias
            syntax\. Overrides [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor') for requests made with these settings\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Example: `await Curl.ExecuteAsync("curl.exe -d '{\"a\":1}' https://api.example.com", new CurlSettings().WithFlavor(CurlFlavor.PowerShell).WithTimeout(30));`
            sends the body `{"a":1}`. For a one-off call without other settings,
            [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)') is shorter.

### See Also
- [Flavor](CurlDotNet.Core.CurlSettings.Flavor.md 'CurlDotNet\.Core\.CurlSettings\.Flavor')
- [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')
- [Flavor](CurlDotNet.Core.CurlOptions.Flavor.md 'CurlDotNet\.Core\.CurlOptions\.Flavor')