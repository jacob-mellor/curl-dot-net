#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

## CurlContext\.GetProperty\<T\>\(string\) Method

Reads a value from [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties') as [T](CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).md#CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).T 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)\.T'), returning `default(T)` when
the key is missing or the value is of another type \(it never throws for a bad cast\)\.

```csharp
public T GetProperty<T>(string key);
```
#### Type parameters

<a name='CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).T'></a>

`T`

The type the value was stored as\.
#### Parameters

<a name='CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The property name used with [WithProperty\(string, object\)](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)')\. Must not be null\.

#### Returns
[T](CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).md#CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).T 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)\.T')  
The stored value, or `default(T)` \(null, 0, false\.\.\.\) when it is missing or not a [T](CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).md#CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).T 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)\.T')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[key](CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).md#CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).key 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)\.key') is null\. Fix: pass a non\-null key such as `"tenant"`\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/logging\-observability\.md

### Remarks

The value is cast, not converted: an `int` stored under a key cannot be read as `long`
            (you get 0). Use `Properties.ContainsKey(key)` when you need to tell "missing" from a stored default.

Example: `var duration = context.GetProperty<TimeSpan>("TimingDuration");` after
            [TimingMiddleware](CurlDotNet.Middleware.TimingMiddleware.md 'CurlDotNet\.Middleware\.TimingMiddleware') has run.

### See Also
- [WithProperty\(string, object\)](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)')