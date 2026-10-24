#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.GetDiagnosticInfo\(\) Method

Returns the facts about this error as a dictionary \(type, message, command, exit code and name, timestamp,
machine, OS, retryability, context, suggestions, inner exception\) for logging scopes and telemetry\.

```csharp
public virtual System.Collections.Generic.Dictionary<string,object> GetDiagnosticInfo();
```

#### Returns
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
A new dictionary with keys such as `ExceptionType`, `CurlErrorCode`, `CurlErrorName`,
            `IsRetryable` and `Suggestions`\.

### Remarks

Missing values are reported as `"N/A"`. Example:
            `using (logger.BeginScope(ex.GetDiagnosticInfo())) logger.LogError(ex.WhatHappened);`

### See Also
- [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)')
- [Log\(Action&lt;string,Dictionary&lt;string,object&gt;&gt;\)](CurlDotNet.Exceptions.CurlException.Log(System.Action_string,System.Collections.Generic.Dictionary_string,object__).md 'CurlDotNet\.Exceptions\.CurlException\.Log\(System\.Action\<string,System\.Collections\.Generic\.Dictionary\<string,object\>\>\)')