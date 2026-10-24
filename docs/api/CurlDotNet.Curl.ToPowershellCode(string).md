#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ToPowershellCode\(string\) Method

Translates a curl command into a PowerShell `Invoke-RestMethod` call that sends the same request, for
Windows admins and build scripts\.

```csharp
public static string ToPowershellCode(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToPowershellCode(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line to translate, read with [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
One PowerShell command line using `Invoke-RestMethod`\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[command](CurlDotNet.Curl.ToPowershellCode(string).md#CurlDotNet.Curl.ToPowershellCode(string).command 'CurlDotNet\.Curl\.ToPowershellCode\(string\)\.command') is null, empty or whitespace\.

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
The command text cannot be read; the message names the problem\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Remarks

The generated command covers `-Uri`, `-Method`, headers and `-Body`. Nothing is sent.
            To go the other way (run an `Invoke-WebRequest` command), use
            [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)') with [InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest').

Example: `Curl.ToPowershellCode("curl -X POST https://api.example.com/data -d '{\"key\":\"value\"}'")`
            returns `Invoke-RestMethod -Uri "https://api.example.com/data" -Method POST ...`.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md

### See Also
- [ToHttpClient\(string\)](CurlDotNet.Curl.ToHttpClient(string).md 'CurlDotNet\.Curl\.ToHttpClient\(string\)')
- [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)')