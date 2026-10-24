#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')

## CurlHttpException\(string, int\) Constructor

Creates the exception with your message and status; [ResponseBody](CurlDotNet.Core.CurlHttpException.ResponseBody.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseBody') and [ResponseHeaders](CurlDotNet.Core.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseHeaders')
can be set with an object initializer\.

```csharp
public CurlHttpException(string message, int statusCode);
```
#### Parameters

<a name='CurlDotNet.Core.CurlHttpException.CurlHttpException(string,int).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The text returned by [System\.Exception\.Message](https://learn.microsoft.com/en-us/dotnet/api/system.exception.message 'System\.Exception\.Message')\.

<a name='CurlDotNet.Core.CurlHttpException.CurlHttpException(string,int).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code, such as 404, stored in [StatusCode](CurlDotNet.Core.CurlHttpException.StatusCode.md 'CurlDotNet\.Core\.CurlHttpException\.StatusCode')\.

### Remarks

The message is used exactly as given. Write it as what happened, how to fix it and a link, like the
            library's own messages.

Example: `throw new CurlHttpException($"Order API answered {r.StatusCode}.", r.StatusCode) { ResponseBody = r.Body, ResponseHeaders = r.Headers };`

### See Also
- [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)')