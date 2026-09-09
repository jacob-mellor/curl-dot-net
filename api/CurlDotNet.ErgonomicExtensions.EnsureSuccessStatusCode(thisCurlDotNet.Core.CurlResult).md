#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions')

## ErgonomicExtensions\.EnsureSuccessStatusCode\(this CurlResult\) Method

Throw an exception if the request was not successful\.
Similar to HttpResponseMessage\.EnsureSuccessStatusCode\(\)\.

```csharp
public static CurlDotNet.Core.CurlResult EnsureSuccessStatusCode(this CurlDotNet.Core.CurlResult result);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.EnsureSuccessStatusCode(thisCurlDotNet.Core.CurlResult).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult to check

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The same CurlResult for chaining

#### Exceptions

[CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')  
If the status code indicates failure