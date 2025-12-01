#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlHttpException Class

Thrown for HTTP error responses when using ThrowOnError\(\)

```csharp
public class CurlHttpException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlHttpException

Derived  
&#8627; [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')  
&#8627; [CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException')

| Constructors | |
| :--- | :--- |
| [CurlHttpException\(string, int, string, string, string\)](CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).md 'CurlDotNet\.Exceptions\.CurlHttpException\.CurlHttpException\(string, int, string, string, string\)') | Initializes a new instance of the CurlHttpException class\. |

| Properties | |
| :--- | :--- |
| [IsClientError](CurlDotNet.Exceptions.CurlHttpException.IsClientError.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsClientError') | Check if this is a client error \(4xx\) |
| [IsForbidden](CurlDotNet.Exceptions.CurlHttpException.IsForbidden.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsForbidden') | Check if this is forbidden \(403\)\. |
| [IsNotFound](CurlDotNet.Exceptions.CurlHttpException.IsNotFound.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsNotFound') | Check if this is not found \(404\)\. |
| [IsRateLimited](CurlDotNet.Exceptions.CurlHttpException.IsRateLimited.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsRateLimited') | Check if this is a rate limit error \(429\)\. |
| [IsServerError](CurlDotNet.Exceptions.CurlHttpException.IsServerError.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsServerError') | Check if this is a server error \(5xx\) |
| [IsUnauthorized](CurlDotNet.Exceptions.CurlHttpException.IsUnauthorized.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsUnauthorized') | Check if this is unauthorized \(401\)\. |
| [ResponseBody](CurlDotNet.Exceptions.CurlHttpException.ResponseBody.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ResponseBody') | Gets the response body content\. |
| [ResponseHeaders](CurlDotNet.Exceptions.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ResponseHeaders') | Gets or sets the response headers\. |
| [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode') | Gets the HTTP status code of the response\. |
| [StatusText](CurlDotNet.Exceptions.CurlHttpException.StatusText.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusText') | Gets the HTTP status text of the response\. |

| Methods | |
| :--- | :--- |
| [GetRetryAfter\(\)](CurlDotNet.Exceptions.CurlHttpException.GetRetryAfter().md 'CurlDotNet\.Exceptions\.CurlHttpException\.GetRetryAfter\(\)') | Get retry\-after value if present in headers\. |
| [IsRetryable\(\)](CurlDotNet.Exceptions.CurlHttpException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsRetryable\(\)') | Determines whether the HTTP error is retryable\. |
| [IsStatus\(int\)](CurlDotNet.Exceptions.CurlHttpException.IsStatus(int).md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsStatus\(int\)') | Check if this is a specific status code\. |
| [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlHttpException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlHttpException\.ToDetailedString\(\)') | Returns a detailed string representation of the exception including HTTP status information\. |
| [WithHeaders\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Exceptions.CurlHttpException.WithHeaders(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Exceptions\.CurlHttpException\.WithHeaders\(System\.Collections\.Generic\.Dictionary\<string,string\>\)') | Add response headers \(fluent\)\. |
