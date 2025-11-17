#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions')

## ErgonomicExtensions\.SaveToFile\(this CurlResult, string\) Method

Save the response body to a file\.

```csharp
public static long SaveToFile(this CurlDotNet.Core.CurlResult result, string filePath);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult containing the data

<a name='CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path to save to

#### Returns
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
The number of bytes written