#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.WithContext Method

| Overloads | |
| :--- | :--- |
| [WithContext\(string, object\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(string,object) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(string, object\)') | Attaches one key/value fact \(a request id, a tenant, a retry number\) to this exception and returns the same exception, so you can enrich and rethrow it in one expression\. |
| [WithContext\(Dictionary&lt;string,object&gt;\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(System\.Collections\.Generic\.Dictionary\<string,object\>\)') | Attaches several key/value facts at once \(copied into [Context](CurlDotNet.Exceptions.CurlException.Context.md 'CurlDotNet\.Exceptions\.CurlException\.Context')\) and returns the same exception for chaining, for example everything your logging scope already knows\. |

<a name='CurlDotNet.Exceptions.CurlException.WithContext(string,object)'></a>

## CurlException\.WithContext\(string, object\) Method

Attaches one key/value fact \(a request id, a tenant, a retry number\) to this exception and returns the same
exception, so you can enrich and rethrow it in one expression\.

```csharp
public CurlDotNet.Exceptions.CurlException WithContext(string key, object value);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.WithContext(string,object).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Name of the fact, such as `"orderId"`; any non\-null string\.

<a name='CurlDotNet.Exceptions.CurlException.WithContext(string,object).value'></a>

`value` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The value to record; any object, printed with `ToString()` in the detailed text\.

#### Returns
[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
This same exception instance, for chaining\.

### Remarks

The value is stored in [Context](CurlDotNet.Exceptions.CurlException.Context.md 'CurlDotNet\.Exceptions\.CurlException\.Context'), replacing an existing value with the same key, and appears in
            [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)'), [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)') and [GetDiagnosticInfo\(\)](CurlDotNet.Exceptions.CurlException.GetDiagnosticInfo().md 'CurlDotNet\.Exceptions\.CurlException\.GetDiagnosticInfo\(\)'). It does not change
            [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message').

Example: `catch (CurlException ex) { throw ex.WithContext("orderId", order.Id).WithContext("attempt", 3); }`
            (use `throw;` instead when you want to keep the original stack trace).

### See Also
- [WithContext\(Dictionary&lt;string,object&gt;\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(System\.Collections\.Generic\.Dictionary\<string,object\>\)')
- [Context](CurlDotNet.Exceptions.CurlException.Context.md 'CurlDotNet\.Exceptions\.CurlException\.Context')

<a name='CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_)'></a>

## CurlException\.WithContext\(Dictionary\<string,object\>\) Method

Attaches several key/value facts at once \(copied into [Context](CurlDotNet.Exceptions.CurlException.Context.md 'CurlDotNet\.Exceptions\.CurlException\.Context')\) and returns the same exception for
chaining, for example everything your logging scope already knows\.

```csharp
public CurlDotNet.Exceptions.CurlException WithContext(System.Collections.Generic.Dictionary<string,object> context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_).context'></a>

`context` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

The facts to copy; each key/value pair is added to [Context](CurlDotNet.Exceptions.CurlException.Context.md 'CurlDotNet\.Exceptions\.CurlException\.Context')\. Must not be null\.

#### Returns
[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
This same exception instance, for chaining\.

### Remarks

Existing keys are overwritten. Example: `throw ex.WithContext(new Dictionary<string, object> { ["user"] = userId, ["region"] = "eu" });`

### See Also
- [WithContext\(string, object\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(string,object) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(string, object\)')
- [Context](CurlDotNet.Exceptions.CurlException.Context.md 'CurlDotNet\.Exceptions\.CurlException\.Context')