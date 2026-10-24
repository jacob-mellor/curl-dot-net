#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

## StringExtensions\.CurlPostJsonAsync\(this string, string\) Method

POSTs [json](CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).md#CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).json 'CurlDotNet\.Extensions\.StringExtensions\.CurlPostJsonAsync\(this string, string\)\.json') to this URL with `Content-Type: application/json` and returns the result \- like
`curl -X POST -H 'Content-Type: application/json' -d JSON URL`, without any shell quoting\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlPostJsonAsync(this string url, string json);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The absolute URL to POST to, such as `https://api.example.com/users`\.

<a name='CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).json'></a>

`json` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The request body as JSON text, such as `{"name":"John"}`; null sends an empty body\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with the [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).md#CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).url 'CurlDotNet\.Extensions\.StringExtensions\.CurlPostJsonAsync\(this string, string\)\.url') is null\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The URL is malformed or the transfer failed; see the message for the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

The JSON text is sent exactly as given (it is not validated or re-serialized), so apostrophes, spaces and
            newlines need no escaping. To send an object instead of text, use [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)')
            or [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)'). The URL is used literally.

Example: `var r = await "https://api.example.com/users".CurlPostJsonAsync("{\"name\":\"O'Brien\"}"); r.EnsureSuccess();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)')
- [CurlGetAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlGetAsync\(this string\)')