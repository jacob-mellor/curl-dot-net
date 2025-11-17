#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlHttpException Class


<b>Exception for HTTP errors (4xx, 5xx status codes).</b>

Thrown by EnsureSuccess() when request fails:

```csharp
try
{
    result.EnsureSuccess();
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
    Console.WriteLine($"Response was: {ex.ResponseBody}");
}
```

```csharp
public class CurlHttpException : System.Exception
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; CurlHttpException

| Constructors | |
| :--- | :--- |
| [CurlHttpException\(string, int\)](CurlDotNet.Core.CurlHttpException.CurlHttpException(string,int).md 'CurlDotNet\.Core\.CurlHttpException\.CurlHttpException\(string, int\)') | Initializes a new instance of the CurlHttpException class\. |

| Properties | |
| :--- | :--- |
| [ResponseBody](CurlDotNet.Core.CurlHttpException.ResponseBody.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseBody') | The response body \(may contain error details\) |
| [ResponseHeaders](CurlDotNet.Core.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseHeaders') | The response headers |
| [StatusCode](CurlDotNet.Core.CurlHttpException.StatusCode.md 'CurlDotNet\.Core\.CurlHttpException\.StatusCode') | The HTTP status code that caused the error |
