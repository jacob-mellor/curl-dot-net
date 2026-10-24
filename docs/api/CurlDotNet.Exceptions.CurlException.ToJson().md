#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.ToJson\(\) Method

Serializes the exception to indented JSON \(type, message, whatHappened, documentationUrl, command, exit code and
name, context, suggestions, diagnostics, retryability, stack trace, inner message\) for structured logs\.

```csharp
public virtual string ToJson();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An indented JSON object such as `{ "type": "CurlOperationTimeoutException", "curlErrorCode": 28, ... }`\.

### Remarks

Uses System.Text.Json, or Newtonsoft.Json on the .NET Standard 2.0 build. Values in [Context](CurlDotNet.Exceptions.CurlException.Context.md 'CurlDotNet\.Exceptions\.CurlException\.Context') must
            be serializable. Example: `catch (CurlException ex) { telemetry.TrackTrace(ex.ToJson()); }`

### See Also
- [GetDiagnosticInfo\(\)](CurlDotNet.Exceptions.CurlException.GetDiagnosticInfo().md 'CurlDotNet\.Exceptions\.CurlException\.GetDiagnosticInfo\(\)')
- [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)')