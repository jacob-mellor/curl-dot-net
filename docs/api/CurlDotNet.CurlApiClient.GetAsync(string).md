#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[CurlApiClient](CurlDotNet.CurlApiClient.md 'CurlDotNet\.CurlApiClient')

## CurlApiClient\.GetAsync\(string\) Method

Sends `GET baseUrl/path` and returns the response, whatever its status \- like `curl -m T baseUrl/path`\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> GetAsync(string path);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.GetAsync(string).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path relative to the base URL, such as `users/42`; null or empty requests the base URL itself\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with the [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') \(status, body, headers\)\.

#### Exceptions

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed \(DNS, connection, timeout after the client's limit, TLS\)\. The
            message says what happened and how to fix it\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

A leading `/` on [path](CurlDotNet.CurlApiClient.GetAsync(string).md#CurlDotNet.CurlApiClient.GetAsync(string).path 'CurlDotNet\.CurlApiClient\.GetAsync\(string\)\.path') is ignored; a query string can be included
            (`"users?page=2"`). Redirects are not followed.

Example: `var users = (await api.GetAsync("users?page=2")).EnsureSuccess().ParseJson<List<User>>();`

### See Also
- [PostJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PostJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PostJsonAsync\(string, object\)')
- [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)')