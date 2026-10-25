#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[CurlApiClient](CurlDotNet.CurlApiClient.md 'CurlDotNet\.CurlApiClient')

## CurlApiClient\.PutJsonAsync\(string, object\) Method

Sends `PUT baseUrl/path` with [data](CurlDotNet.CurlApiClient.PutJsonAsync(string,object).md#CurlDotNet.CurlApiClient.PutJsonAsync(string,object).data 'CurlDotNet\.CurlApiClient\.PutJsonAsync\(string, object\)\.data') serialized as JSON \(replace a resource\) and returns the
response \- like `curl -X PUT --json '{...}' baseUrl/path`\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PutJsonAsync(string path, object data);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.PutJsonAsync(string,object).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path of the resource relative to the base URL, such as `users/42`\.

<a name='CurlDotNet.CurlApiClient.PutJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The full new representation as any serializable object\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with the [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see the message for the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Example: `await api.PutJsonAsync("users/42", new { name = "Ada Lovelace" });`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [PatchJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PatchJsonAsync\(string, object\)')
- [PostJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PostJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PostJsonAsync\(string, object\)')