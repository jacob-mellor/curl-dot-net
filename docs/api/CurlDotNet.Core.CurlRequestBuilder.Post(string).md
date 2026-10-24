#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Post\(string\) Method

Starts a POST request \(`curl -X POST URL`\); add the body with [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)'),
[WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)'), [WithFormData\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFormData\(System\.Collections\.Generic\.Dictionary\<string,string\>\)') or [WithMultipartForm\(Dictionary&lt;string,string&gt;, Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)')\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Post(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Post(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute URL to post to, such as `https://api.example.com/users`\. Must not be null\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
A new builder whose method is POST\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Core.CurlRequestBuilder.Post(string).md#CurlDotNet.Core.CurlRequestBuilder.Post(string).url 'CurlDotNet\.Core\.CurlRequestBuilder\.Post\(string\)\.url') is null\. Pass the URL string; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

### Example

```csharp
var result = await CurlRequestBuilder
    .Post("https://api.example.com/login")
    .WithFormData(new Dictionary<string, string> { ["username"] = "user123", ["password"] = "pass456" })
    .ExecuteAsync();
```

### Remarks

A POST with no body sends `Content-Length: 0`. The body methods set the matching
            `Content-Type`: [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)') uses `application/json`, [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)') and
            [WithFormData\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFormData\(System\.Collections\.Generic\.Dictionary\<string,string\>\)') use `application/x-www-form-urlencoded` (like `curl -d`),
            [WithMultipartForm\(Dictionary&lt;string,string&gt;, Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)') and the file methods use `multipart/form-data` (like `curl -F`).

Example: `var created = await CurlRequestBuilder.Post("https://api.example.com/users").WithJson(new { name = "Ada", email = "ada@example.com" }).ExecuteAsync();`
            then check `created.StatusCode == 201`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)')
- [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)')
- [PostAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PostAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')