#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Transform\<T\>\(Func\<CurlResult,T\>\) Method


<b>Transform the result using your own function.</b>

Extract or convert data however you need:

```csharp
// Extract just what you need
var name = result.Transform(r =>
{
    var user = r.ParseJson<User>();
    return user.Name;
});

// Convert to your own type
var summary = result.Transform(r => new
{
    Success = r.IsSuccess,
    Size = r.Body?.Length ?? 0,
    Type = r.GetHeader("Content-Type")
});
```

```csharp
public T Transform<T>(System.Func<CurlDotNet.Core.CurlResult,T> transformer);
```
#### Type parameters

<a name='CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).T'></a>

`T`
#### Parameters

<a name='CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).transformer'></a>

`transformer` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).T 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

#### Returns
[T](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).T 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.T')