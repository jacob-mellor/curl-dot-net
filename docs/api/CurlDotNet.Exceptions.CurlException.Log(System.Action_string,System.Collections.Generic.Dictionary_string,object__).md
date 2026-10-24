#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.Log\(Action\<string,Dictionary\<string,object\>\>\) Method

Sends this exception to your logging callback as two values: the human\-readable [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)')
text and the structured [GetDiagnosticInfo\(\)](CurlDotNet.Exceptions.CurlException.GetDiagnosticInfo().md 'CurlDotNet\.Exceptions\.CurlException\.GetDiagnosticInfo\(\)') dictionary\.

```csharp
public void Log(System.Action<string,System.Collections.Generic.Dictionary<string,object>> logger);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.Log(System.Action_string,System.Collections.Generic.Dictionary_string,object__).logger'></a>

`logger` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')

Callback receiving the detailed text and the diagnostic dictionary; must not be null\.

### Remarks

Example with Microsoft.Extensions.Logging:
            `ex.Log((text, data) => { using (logger.BeginScope(data)) logger.LogError(text); });`

### See Also
- [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)')
- [GetDiagnosticInfo\(\)](CurlDotNet.Exceptions.CurlException.GetDiagnosticInfo().md 'CurlDotNet\.Exceptions\.CurlException\.GetDiagnosticInfo\(\)')