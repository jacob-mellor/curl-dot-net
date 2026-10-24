#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.EnsureStatus\(int\) Method

Throws [CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException') unless [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') equals [expectedStatus](CurlDotNet.Core.CurlResult.EnsureStatus(int).md#CurlDotNet.Core.CurlResult.EnsureStatus(int).expectedStatus 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)\.expectedStatus') exactly,
otherwise returns this result \- for APIs where only one code is right \(201 Created, 204 No Content\)\.

```csharp
public CurlDotNet.Core.CurlResult EnsureStatus(int expectedStatus);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.EnsureStatus(int).expectedStatus'></a>

`expectedStatus` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The exact status you require, such as 200, 201 or 204\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result when the status matches, for chaining\.

#### Exceptions

[CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')  
\(status differs from [expectedStatus](CurlDotNet.Core.CurlResult.EnsureStatus(int).md#CurlDotNet.Core.CurlResult.EnsureStatus(int).expectedStatus 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)\.expectedStatus')\) The message names the URL, the status and a fix for that status \(credentials
            for 401, permissions for 403, the path for 404, waiting for 429, retrying for 5xx, `-L` for 3xx\);
            [ResponseBody](CurlDotNet.Core.CurlHttpException.ResponseBody.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseBody') and [ResponseHeaders](CurlDotNet.Core.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseHeaders') carry the response\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Example: `(await Curl.ExecuteAsync("curl -X POST --json '{\"name\":\"x\"}' https://api.example.com/items")).EnsureStatus(201);`
            throws if the API answered 200 or 409 instead of 201.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)')
- [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode')