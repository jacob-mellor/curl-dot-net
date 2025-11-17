#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.AsJson\<T\>\(\) Method


<b>Parse JSON response (alternative name for <see cref="M:CurlDotNet.Core.CurlResult.ParseJson``1"/>).</b>

Some people prefer `AsJson`, some prefer `ParseJson`. Both methods are identical and produce the same result.

```csharp
var data = result.AsJson<MyData>();
// Exactly the same as: result.ParseJson<MyData>()
```

```csharp
public T AsJson<T>();
```
#### Type parameters

<a name='CurlDotNet.Core.CurlResult.AsJson_T_().T'></a>

`T`

The type to deserialize to\. See [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') for details\.

#### Returns
[T](CurlDotNet.Core.CurlResult.AsJson_T_().md#CurlDotNet.Core.CurlResult.AsJson_T_().T 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)\.T')  
An instance of [T](CurlDotNet.Core.CurlResult.AsJson_T_().md#CurlDotNet.Core.CurlResult.AsJson_T_().T 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)\.T') with data from the JSON [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
Thrown when [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') is null or empty\.

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
Thrown when JSON deserialization fails\.

[JsonException](https://learn.microsoft.com/en-us/dotnet/api/jsonexception 'JsonException')  
Thrown when the JSON syntax is invalid\.

### Remarks

This is simply an alias for [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)'). Use whichever method name you prefer.

### See Also
- [Primary method for parsing JSON](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
- [Parse JSON as dynamic without a class](CurlDotNet.Core.CurlResult.AsJsonDynamic().md 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)')