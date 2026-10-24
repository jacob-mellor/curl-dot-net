#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions')

## ErgonomicExtensions\.ToSimple\(this CurlResult\) Method

Collapses the result into `(Success, Body, Error)` for code that just wants "did it work, what came back,
and why not"\.

```csharp
public static (bool Success,string? Body,string? Error) ToSimple(this CurlDotNet.Core.CurlResult result);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.ToSimple(thisCurlDotNet.Core.CurlResult).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The response to summarise; must not be null\.

#### Returns
[&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')  
A tuple: `Success` true for 2xx; `Body` the response text; `Error` null on success, else a description\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[result](CurlDotNet.ErgonomicExtensions.ToSimple(thisCurlDotNet.Core.CurlResult).md#CurlDotNet.ErgonomicExtensions.ToSimple(thisCurlDotNet.Core.CurlResult).result 'CurlDotNet\.ErgonomicExtensions\.ToSimple\(this CurlDotNet\.Core\.CurlResult\)\.result') is null\.

### Remarks

`Success` is [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess') (2xx). On failure `Error` is the message of
            [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception') (set by the non-throwing LibCurl API) or `"Request failed with status N"`;
            `Body` is returned either way, since error pages often explain the problem.

Example: `var (ok, body, error) = (await Curl.ExecuteAsync("curl https://api.example.com/ping")).ToSimple(); if (!ok) Console.WriteLine(error);`

### See Also
- [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess')
- [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception')