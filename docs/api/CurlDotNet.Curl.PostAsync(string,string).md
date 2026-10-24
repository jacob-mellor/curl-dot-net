#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.PostAsync\(string, string\) Method

Sends a POST request with [data](CurlDotNet.Curl.PostAsync(string,string).md#CurlDotNet.Curl.PostAsync(string,string).data 'CurlDotNet\.Curl\.PostAsync\(string, string\)\.data') as the body \(like `curl -d DATA URL`\), with no quoting or
escaping needed for the data\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostAsync(string url, string data);
```
#### Parameters

<a name='CurlDotNet.Curl.PostAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL to post to, for example `https://api.example.com/users`\.

<a name='CurlDotNet.Curl.PostAsync(string,string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The request body as text: form fields \(`key1=value1&key2=value2`\), JSON
            \(`{"key":"value"}`\), XML or anything else\. Null sends an empty body\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the server's status, headers and body\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.Curl.PostAsync(string,string).md#CurlDotNet.Curl.PostAsync(string,string).url 'CurlDotNet\.Curl\.PostAsync\(string, string\)\.url') is null, empty or whitespace\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Example

```csharp
var response = await Curl.PostAsync("https://api.example.com/login", "username=john&password=secret123");
var created = await Curl.PostAsync("https://api.example.com/notes", "It's \"quoted\" and that's fine");
```

### Remarks

The body is sent exactly as given; quotes, apostrophes, spaces and newlines need no escaping. As with
            `curl -d`, the request has `Content-Type: application/x-www-form-urlencoded` unless you send
            something else; for JSON use [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)'), which sets `Content-Type: application/json`.
            The data is not URL-encoded for you: for form fields write `name=value&other=2` and encode values
            with `Uri.EscapeDataString`.

HTTP errors are returned, not thrown; check [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess').

Example: `var result = await Curl.PostAsync("https://api.example.com/login", "username=john&password=" + Uri.EscapeDataString(pw));`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.Curl.Post(string,string).md 'CurlDotNet\.Curl\.Post\(string, string\)')
- [Post an object as JSON](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)')
- [Builder equivalent](CurlDotNet.Core.CurlRequestBuilder.Post(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Post\(string\)')
- [libcurl\-style equivalent](CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PostAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')
- [PostAsync\(string, string\)](CurlDotNet.DotNetCurl.PostAsync(string,string).md 'CurlDotNet\.DotNetCurl\.PostAsync\(string, string\)')