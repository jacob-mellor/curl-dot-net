#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions')

## ErgonomicExtensions\.ToSimple\(this CurlResult\) Method

Convert the CurlResult to a simplified success/error tuple\.

```csharp
public static (bool Success,string? Body,string? Error) ToSimple(this CurlDotNet.Core.CurlResult result);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.ToSimple(thisCurlDotNet.Core.CurlResult).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult

#### Returns
[&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')  
A tuple of \(success, body, error\)