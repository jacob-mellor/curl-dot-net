#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

## CurlContext\.WithProperty\(string, object\) Method

Stores [value](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md#CurlDotNet.Middleware.CurlContext.WithProperty(string,object).value 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)\.value') in [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties') under [key](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md#CurlDotNet.Middleware.CurlContext.WithProperty(string,object).key 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)\.key') \(replacing any
earlier value\) and returns this context, so calls can be chained\.

```csharp
public CurlDotNet.Middleware.CurlContext WithProperty(string key, object value);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlContext.WithProperty(string,object).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The property name, for example `"tenant"`\. Must not be null\.

<a name='CurlDotNet.Middleware.CurlContext.WithProperty(string,object).value'></a>

`value` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Any value, including null\.

#### Returns
[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')  
This same [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[key](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md#CurlDotNet.Middleware.CurlContext.WithProperty(string,object).key 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)\.key') is null\. Fix: pass a non\-null key such as `"tenant"`\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/logging\-observability\.md

### Remarks

Example: `var ctx = new CurlContext { Command = "curl https://api.example.com" }.WithProperty("tenant", "acme").WithProperty("attempt", 1);`

### See Also
- [GetProperty&lt;T&gt;\(string\)](CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).md 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)')
- [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties')