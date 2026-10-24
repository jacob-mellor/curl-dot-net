#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.EnsureSuccess\(\) Method

Throws [CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException') unless [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') is 200\-299, otherwise returns this result \-
the CurlDotNet version of `HttpResponseMessage.EnsureSuccessStatusCode()`\.

```csharp
public CurlDotNet.Core.CurlResult EnsureSuccess();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result when the status is 2xx, for chaining\.

#### Exceptions

[CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')  
\(status not 200\-299\) The message names the URL, the status and a fix for that status \(credentials
            for 401, permissions for 403, the path for 404, waiting for 429, retrying for 5xx, `-L` for 3xx\);
            [ResponseBody](CurlDotNet.Core.CurlHttpException.ResponseBody.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseBody') and [ResponseHeaders](CurlDotNet.Core.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseHeaders') carry the response\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Example

```csharp
try
{
    var data = result.EnsureSuccess().ParseJson<Data>();
}
catch (CurlHttpException ex) when (ex.StatusCode >= 500)
{
    Console.WriteLine($"Server error {ex.StatusCode} - retry later");
}
```

### Remarks

Use it to fail fast before parsing: `var user = (await Curl.ExecuteAsync("curl https://api.example.com/users/42")).EnsureSuccess().ParseJson<User>();`

It is the after-the-fact equivalent of curl's `-f`/`--fail`. With `-f` in the command the
            transfer itself throws [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException') (exit code 22) and you never get a result; with
            EnsureSuccess you get the result first and decide. The thrown exception is [CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')
            (namespace `CurlDotNet.Core`), which carries the body and headers.

Example: `try { result.EnsureSuccess(); } catch (CurlHttpException ex) when (ex.StatusCode == 404) { return null; }`

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess')
- [EnsureStatus\(int\)](CurlDotNet.Core.CurlResult.EnsureStatus(int).md 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)')
- [EnsureContains\(string\)](CurlDotNet.Core.CurlResult.EnsureContains(string).md 'CurlDotNet\.Core\.CurlResult\.EnsureContains\(string\)')
- [EnsureSuccessStatusCode\(this CurlResult\)](CurlDotNet.ErgonomicExtensions.EnsureSuccessStatusCode(thisCurlDotNet.Core.CurlResult).md 'CurlDotNet\.ErgonomicExtensions\.EnsureSuccessStatusCode\(this CurlDotNet\.Core\.CurlResult\)')