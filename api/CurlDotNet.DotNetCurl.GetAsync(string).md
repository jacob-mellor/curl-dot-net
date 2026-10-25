#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.GetAsync\(string\) Method

Sends a plain GET request to [url](CurlDotNet.DotNetCurl.GetAsync(string).md#CurlDotNet.DotNetCurl.GetAsync(string).url 'CurlDotNet\.DotNetCurl\.GetAsync\(string\)\.url') asynchronously; same as
[GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)') \(like `curl URL`\)\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> GetAsync(string url);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.GetAsync(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL including the scheme, for example `https://api.example.com/users?page=1`\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with status code, body and headers\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.DotNetCurl.GetAsync(string).md#CurlDotNet.DotNetCurl.GetAsync(string).url 'CurlDotNet\.DotNetCurl\.GetAsync\(string\)\.url') is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Remarks

For headers or authentication use [CurlAsync\(string\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)') with `-H`/`-u`, or
            [Get\(string\)](CurlDotNet.Core.CurlRequestBuilder.Get(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)').

Example: `var user = (await DotNetCurl.GetAsync("https://api.github.com/users/octocat")).ParseJson<GitHubUser>();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.DotNetCurl.Get(string).md 'CurlDotNet\.DotNetCurl\.Get\(string\)')
- [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)')