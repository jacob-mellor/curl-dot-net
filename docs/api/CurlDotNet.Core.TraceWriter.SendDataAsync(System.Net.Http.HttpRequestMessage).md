#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[TraceWriter](CurlDotNet.Core.TraceWriter.md 'CurlDotNet\.Core\.TraceWriter')

## TraceWriter\.SendDataAsync\(HttpRequestMessage\) Method

Dumps the outgoing request body, if any\. Best\-effort: bodies that cannot be
re\-read for inspection are skipped without affecting the request itself\.

```csharp
public System.Threading.Tasks.Task SendDataAsync(System.Net.Http.HttpRequestMessage request);
```
#### Parameters

<a name='CurlDotNet.Core.TraceWriter.SendDataAsync(System.Net.Http.HttpRequestMessage).request'></a>

`request` [System\.Net\.Http\.HttpRequestMessage](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage 'System\.Net\.Http\.HttpRequestMessage')

The request being sent\.

#### Returns
[System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task')