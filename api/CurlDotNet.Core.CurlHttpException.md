#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlHttpException Class

Thrown by [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)') and [EnsureStatus\(int\)](CurlDotNet.Core.CurlResult.EnsureStatus(int).md 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)') when the server answered
with a status you did not accept; carries the status, the response body and the headers so you can react\.

```csharp
public class CurlHttpException : System.Exception
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; CurlHttpException

### Remarks

This is `CurlDotNet.Core.CurlHttpException`, a plain [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception'). It is a different type from
            [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException'), which is a [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') thrown during the transfer
            (for example [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException') when the command uses `-f`). With both namespaces
            imported, write the namespace you mean.

Its message says what happened (URL and status), how to fix it for that status, and links the error-handling guide.

Example: `try { result.EnsureSuccess(); } catch (CurlDotNet.Core.CurlHttpException ex) when (ex.StatusCode == 401) { await RefreshTokenAsync(); }`

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)')
- [EnsureStatus\(int\)](CurlDotNet.Core.CurlResult.EnsureStatus(int).md 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)')
- [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

| Constructors | |
| :--- | :--- |
| [CurlHttpException\(string, int\)](CurlDotNet.Core.CurlHttpException.CurlHttpException(string,int).md 'CurlDotNet\.Core\.CurlHttpException\.CurlHttpException\(string, int\)') | Creates the exception with your message and status; [ResponseBody](CurlDotNet.Core.CurlHttpException.ResponseBody.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseBody') and [ResponseHeaders](CurlDotNet.Core.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseHeaders') can be set with an object initializer\. |

| Properties | |
| :--- | :--- |
| [ResponseBody](CurlDotNet.Core.CurlHttpException.ResponseBody.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseBody') | The response text \([Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\) of the failed request \- APIs usually explain the error here\. |
| [ResponseHeaders](CurlDotNet.Core.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseHeaders') | The response headers \([Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')\) of the failed request, such as `Retry-After` or `WWW-Authenticate`\. |
| [StatusCode](CurlDotNet.Core.CurlHttpException.StatusCode.md 'CurlDotNet\.Core\.CurlHttpException\.StatusCode') | The HTTP status the server answered with \(for example 404 or 503\), copied from [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode')\. |
