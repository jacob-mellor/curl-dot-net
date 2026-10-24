#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[CurlApiClient](CurlDotNet.CurlApiClient.md 'CurlDotNet\.CurlApiClient')

## CurlApiClient\.PatchJsonAsync\(string, object\) Method

Sends `PATCH baseUrl/path` with [data](CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).md#CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).data 'CurlDotNet\.CurlApiClient\.PatchJsonAsync\(string, object\)\.data') serialized as JSON \(change some fields\) and returns
the response \- like `curl -X PATCH --json '{...}' baseUrl/path`\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PatchJsonAsync(string path, object data);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path of the resource relative to the base URL, such as `users/42`\.

<a name='CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

An object with only the fields to change, as any serializable object\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with the [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see the message for the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Example: `await api.PatchJsonAsync("users/42", new { email = "new@example.com" });`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [PutJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PutJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PutJsonAsync\(string, object\)')
- [PostJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PostJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PostJsonAsync\(string, object\)')