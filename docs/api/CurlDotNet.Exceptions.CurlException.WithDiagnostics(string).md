#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.WithDiagnostics\(string\) Method

Stores free\-form diagnostic text \(a verbose log, a trace excerpt, environment details\) in
[DiagnosticInfo](CurlDotNet.Exceptions.CurlException.DiagnosticInfo.md 'CurlDotNet\.Exceptions\.CurlException\.DiagnosticInfo') and returns the same exception for chaining\.

```csharp
public CurlDotNet.Exceptions.CurlException WithDiagnostics(string diagnosticInfo);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.WithDiagnostics(string).diagnosticInfo'></a>

`diagnosticInfo` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The text to attach; any string, may span several lines; null clears it\.

#### Returns
[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
This same exception instance, for chaining\.

### Remarks

Replaces any previous value. Printed by [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)') and included in [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)').

Example: run the command with `--trace-ascii trace.txt`, then `throw ex.WithDiagnostics(File.ReadAllText("trace.txt"));`

### See Also
- [DiagnosticInfo](CurlDotNet.Exceptions.CurlException.DiagnosticInfo.md 'CurlDotNet\.Exceptions\.CurlException\.DiagnosticInfo')
- [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)')