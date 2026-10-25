#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.PostJsonAsync\(string, object\) Method

Serializes [data](CurlDotNet.DotNetCurl.PostJsonAsync(string,object).md#CurlDotNet.DotNetCurl.PostJsonAsync(string,object).data 'CurlDotNet\.DotNetCurl\.PostJsonAsync\(string, object\)\.data') to JSON and POSTs it with `Content-Type: application/json`
asynchronously; same as [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)')\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostJsonAsync(string url, object data);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.PostJsonAsync(string,object).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL to post to, for example `https://api.example.com/users`\.

<a name='CurlDotNet.DotNetCurl.PostJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The object to send as JSON\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the server's status, headers and body\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.DotNetCurl.PostJsonAsync(string,object).md#CurlDotNet.DotNetCurl.PostJsonAsync(string,object).url 'CurlDotNet\.DotNetCurl\.PostJsonAsync\(string, object\)\.url') is null, empty or whitespace\.

[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  
[data](CurlDotNet.DotNetCurl.PostJsonAsync(string,object).md#CurlDotNet.DotNetCurl.PostJsonAsync(string,object).data 'CurlDotNet\.DotNetCurl\.PostJsonAsync\(string, object\)\.data') cannot be serialized to JSON\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-post\-json\-csharp/README\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed\.

### Remarks

Apostrophes and non-ASCII characters are sent literally. Parse the answer with
            [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)').

Example: `var created = (await DotNetCurl.PostJsonAsync("https://api.example.com/users", new { name = "John" })).ParseJson<User>();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.DotNetCurl.PostJson(string,object).md 'CurlDotNet\.DotNetCurl\.PostJson\(string, object\)')
- [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)')