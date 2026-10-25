#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Transform\<T\>\(Func\<CurlResult,T\>\) Method

Passes this result to [transformer](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).transformer 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.transformer') and returns whatever it returns \- a way to end a fluent chain
with your own projection\.

```csharp
public T Transform<T>(System.Func<CurlDotNet.Core.CurlResult,T> transformer);
```
#### Type parameters

<a name='CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).T'></a>

`T`

The type your function returns\.
#### Parameters

<a name='CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).transformer'></a>

`transformer` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).T 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

A function of this result, such as `r => r.ParseJson<User>().Name`; must not be null\.

#### Returns
[T](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).T 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.T')  
The value returned by [transformer](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).transformer 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.transformer')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[transformer](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).transformer 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.transformer') is null; pass a function\.

### Remarks

Exceptions thrown by [transformer](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).transformer 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.transformer') propagate unchanged.

Example: `var summary = result.Transform(r => new { r.StatusCode, Size = r.Body?.Length ?? 0, Type = r.GetHeader("Content-Type") });`

### See Also
- [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')