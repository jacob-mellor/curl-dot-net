#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.WithContext Method

| Overloads | |
| :--- | :--- |
| [WithContext\(string, object\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(string,object) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(string, object\)') | Add contextual information to the exception \(fluent\)\. |
| [WithContext\(Dictionary&lt;string,object&gt;\)](CurlDotNet.Exceptions.CurlException.WithContext.md#CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_) 'CurlDotNet\.Exceptions\.CurlException\.WithContext\(System\.Collections\.Generic\.Dictionary\<string,object\>\)') | Add multiple context values \(fluent\)\. |

<a name='CurlDotNet.Exceptions.CurlException.WithContext(string,object)'></a>

## CurlException\.WithContext\(string, object\) Method

Add contextual information to the exception \(fluent\)\.

```csharp
public CurlDotNet.Exceptions.CurlException WithContext(string key, object value);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.WithContext(string,object).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Context key

<a name='CurlDotNet.Exceptions.CurlException.WithContext(string,object).value'></a>

`value` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Context value

#### Returns
[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
This exception for chaining

<a name='CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_)'></a>

## CurlException\.WithContext\(Dictionary\<string,object\>\) Method

Add multiple context values \(fluent\)\.

```csharp
public CurlDotNet.Exceptions.CurlException WithContext(System.Collections.Generic.Dictionary<string,object> context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.WithContext(System.Collections.Generic.Dictionary_string,object_).context'></a>

`context` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

Dictionary of context values

#### Returns
[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
This exception for chaining