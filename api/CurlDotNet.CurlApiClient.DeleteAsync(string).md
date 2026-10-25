#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[CurlApiClient](CurlDotNet.CurlApiClient.md 'CurlDotNet\.CurlApiClient')

## CurlApiClient\.DeleteAsync\(string\) Method

Sends `DELETE baseUrl/path` and returns the response \(often 204 No Content\) \- like
`curl -X DELETE baseUrl/path`\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> DeleteAsync(string path);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.DeleteAsync(string).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path of the resource relative to the base URL, such as `users/42`\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with the [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see the message for the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Example: `(await api.DeleteAsync("users/42")).EnsureSuccess();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [GetAsync\(string\)](CurlDotNet.CurlApiClient.GetAsync(string).md 'CurlDotNet\.CurlApiClient\.GetAsync\(string\)')