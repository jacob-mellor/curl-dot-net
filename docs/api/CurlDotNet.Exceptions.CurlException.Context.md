#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.Context Property

Extra key/value facts attached with [WithContext\(string, object\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(string,object) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(string, object\)'), such as a request id
or the tenant a call was made for, so that logs show them next to the error\.

```csharp
public System.Collections.Generic.Dictionary<string,object> Context { get; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
A mutable dictionary; empty unless your own code \(or middleware\) adds entries\. CurlDotNet
            itself never adds keys here\.

### Remarks

The entries are printed by [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)'), serialized by [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)') and
            returned by [GetDiagnosticInfo\(\)](CurlDotNet.Exceptions.CurlException.GetDiagnosticInfo().md 'CurlDotNet\.Exceptions\.CurlException\.GetDiagnosticInfo\(\)'). They do not change [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message').

Example: `catch (CurlException ex) { throw ex.WithContext("orderId", 1234); }` then
            `logger.LogError(ex.ToDetailedString())` prints `orderId: 1234` in the Context section.

### See Also
- [WithContext\(string, object\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(string,object) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(string, object\)')
- [WithContext\(Dictionary&lt;string,object&gt;\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(System\.Collections\.Generic\.Dictionary\<string,object\>\)')