#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[CurlApiClient](CurlDotNet.CurlApiClient.md 'CurlDotNet\.CurlApiClient')

## CurlApiClient\.PostJsonAsync\(string, object\) Method

Sends `POST baseUrl/path` with [data](CurlDotNet.CurlApiClient.PostJsonAsync(string,object).md#CurlDotNet.CurlApiClient.PostJsonAsync(string,object).data 'CurlDotNet\.CurlApiClient\.PostJsonAsync\(string, object\)\.data') serialized as JSON and returns the response \- like
`curl --json '{...}' baseUrl/path`\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostJsonAsync(string path, object data);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.PostJsonAsync(string,object).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path relative to the base URL, such as `users`\.

<a name='CurlDotNet.CurlApiClient.PostJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Any serializable object \(class, anonymous object, dictionary\); null sends the JSON literal `null`\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with the [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed \(DNS, connection, timeout, TLS\); see the message for the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Sets `Content-Type: application/json`. Property names are kept as written in C#.

Example: `var created = await api.PostJsonAsync("users", new { name = "Ada", email = "ada@example.com" }); created.EnsureStatus(201);`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [PutJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PutJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PutJsonAsync\(string, object\)')
- [PatchJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PatchJsonAsync\(string, object\)')
- [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)')