#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions')

## ErgonomicExtensions\.EnsureSuccessStatusCode\(this CurlResult\) Method

Throws [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') unless the status is 200\-299, otherwise returns the
result \- named like `HttpResponseMessage.EnsureSuccessStatusCode()` for code moved from HttpClient\.

```csharp
public static CurlDotNet.Core.CurlResult EnsureSuccessStatusCode(this CurlDotNet.Core.CurlResult result);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.EnsureSuccessStatusCode(thisCurlDotNet.Core.CurlResult).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The response to check; must not be null\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The same result when the status is 2xx, for chaining\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[result](CurlDotNet.ErgonomicExtensions.EnsureSuccessStatusCode(thisCurlDotNet.Core.CurlResult).md#CurlDotNet.ErgonomicExtensions.EnsureSuccessStatusCode(thisCurlDotNet.Core.CurlResult).result 'CurlDotNet\.ErgonomicExtensions\.EnsureSuccessStatusCode\(this CurlDotNet\.Core\.CurlResult\)\.result') is null\.

[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')  
The status is not 200\-299\. The message names the URL, the
            status and how to fix it for that status\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Differs from [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)') only in the exception type: this one throws the
            [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')-based [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') (so a single
            `catch (CurlException)` covers transport and HTTP failures), with `StatusCode`, `ResponseBody`,
            `IsNotFound`, `IsRateLimited` and status-specific `Suggestions`.

Example: `var data = (await Curl.ExecuteAsync("curl https://api.example.com/items")).EnsureSuccessStatusCode().ParseJson<List<Item>>();`

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)')
- [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess')