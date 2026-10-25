#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Get\(string\) Method

Starts a GET request for [url](CurlDotNet.Core.CurlRequestBuilder.Get(string).md#CurlDotNet.Core.CurlRequestBuilder.Get(string).url 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)\.url'), the default curl request \(no method flag\);
chain options and call [ExecuteAsync\(CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(System\.Threading\.CancellationToken\)')\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Get(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Get(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute URL to fetch, such as `https://api.example.com/users?page=2`\. Must not be null;
            an invalid URL is reported when the request runs \([CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException'), exit code 3\)\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
A new builder whose method is GET; nothing is sent until you execute it\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Core.CurlRequestBuilder.Get(string).md#CurlDotNet.Core.CurlRequestBuilder.Get(string).url 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)\.url') is null\. Pass the URL string; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

### Example

```csharp
var result = await CurlRequestBuilder
    .Get("https://api.example.com/protected")
    .WithBearerToken("your-token")
    .FollowRedirects()
    .ExecuteAsync();
```

### Remarks

Equivalent curl: `curl URL`. Adding a body later with [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)'),
            [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)'), [WithFormData\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFormData\(System\.Collections\.Generic\.Dictionary\<string,string\>\)'), [WithMultipartForm\(Dictionary&lt;string,string&gt;, Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)'),
            [WithBinaryData\(byte\[\]\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryData(byte[]).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryData\(byte\[\]\)') or a file upload turns the request into a POST, exactly as curl does when
            `-d` or `-F` is added.

The URL is used as given (curl syntax): include the scheme (`https://`, `http://`,
            `ftp://`, `file://`), percent-encode spaces as `%20`, and add query strings yourself,
            for example `$"https://api.example.com/search?q={Uri.EscapeDataString(term)}"`. A URL without a
            scheme is treated as `http://`, like curl.

Example: `var user = await CurlRequestBuilder.Get("https://api.github.com/users/octocat").WithHeader("Accept", "application/json").ExecuteAsync();`
            then `user.ParseJson<GitHubUser>()`.

### See Also
- [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)')
- [GetAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.GetAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')
- [Head\(string\)](CurlDotNet.Core.CurlRequestBuilder.Head(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Head\(string\)')