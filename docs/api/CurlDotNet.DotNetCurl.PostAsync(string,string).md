#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.PostAsync\(string, string\) Method

Sends a POST request with [data](CurlDotNet.DotNetCurl.PostAsync(string,string).md#CurlDotNet.DotNetCurl.PostAsync(string,string).data 'CurlDotNet\.DotNetCurl\.PostAsync\(string, string\)\.data') as the body asynchronously; same as
[PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)') \(like `curl -d DATA URL`\)\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostAsync(string url, string data);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.PostAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL to post to, for example `https://api.example.com/users`\.

<a name='CurlDotNet.DotNetCurl.PostAsync(string,string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The request body as text; null sends an empty body\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the server's status, headers and body\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.DotNetCurl.PostAsync(string,string).md#CurlDotNet.DotNetCurl.PostAsync(string,string).url 'CurlDotNet\.DotNetCurl\.PostAsync\(string, string\)\.url') is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Remarks

Quotes, apostrophes and spaces in the data need no escaping; the data is not URL-encoded for you.

Example: `var result = await DotNetCurl.PostAsync("https://api.example.com/notes", "It's \"quoted\"");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.DotNetCurl.Post(string,string).md 'CurlDotNet\.DotNetCurl\.Post\(string, string\)')
- [PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)')