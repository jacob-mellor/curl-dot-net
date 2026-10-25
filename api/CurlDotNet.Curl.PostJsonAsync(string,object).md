#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.PostJsonAsync\(string, object\) Method

Serializes [data](CurlDotNet.Curl.PostJsonAsync(string,object).md#CurlDotNet.Curl.PostJsonAsync(string,object).data 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)\.data') to JSON and POSTs it with `Content-Type: application/json`
\(like `curl -H 'Content-Type: application/json' -d '{...}' URL`\)\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostJsonAsync(string url, object data);
```
#### Parameters

<a name='CurlDotNet.Curl.PostJsonAsync(string,object).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL to post to, for example `https://api.example.com/users`\.

<a name='CurlDotNet.Curl.PostJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The object to send as JSON: an anonymous object \(`new { name = "John" }`\), an instance
             of your class, an array/list or a `Dictionary<string, object>`\. Null sends the JSON literal `null`\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the server's status, headers and body\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.Curl.PostJsonAsync(string,object).md#CurlDotNet.Curl.PostJsonAsync(string,object).url 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)\.url') is null, empty or whitespace\.

[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  
[data](CurlDotNet.Curl.PostJsonAsync(string,object).md#CurlDotNet.Curl.PostJsonAsync(string,object).data 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)\.data') cannot be serialized to JSON \(for example it
             contains a reference cycle or a type the serializer does not support\); the serializer's exception is the
             [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\. Fix: send a simpler object \(a DTO\) or serialize it yourself\. See
             https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-post\-json\-csharp/README\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Example

```csharp
var response = await Curl.PostJsonAsync("https://api.example.com/users",
    new { name = "John Smith", email = "john@example.com", age = 30 });

if (response.IsSuccess)
{
    var created = response.ParseJson<User>();
    Console.WriteLine($"User created with ID: {created.Id}");
}

await Curl.PostJsonAsync(url, new Dictionary<string, object> { ["key"] = "value" });
```

### Remarks

Serialization: System.Text.Json on .NET 8/9/10 and .NET Framework (property names as declared,
             apostrophes and non-ASCII characters kept literal rather than `\u0027`-escaped), Newtonsoft.Json on the
             .NET Standard 2.0 build. Anonymous objects, your classes, arrays, lists and dictionaries all work. To control
             naming (camelCase) serialize yourself and call [PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)') with the string.

HTTP errors are returned, not thrown; check [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess') and parse the answer
             with [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)').

Example: `var result = await Curl.PostJsonAsync("https://api.example.com/users", new { name = "John", age = 30 });`
             sends the body `{"name":"John","age":30}`; then `var created = result.ParseJson<User>();`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.Curl.PostJson(string,object).md 'CurlDotNet\.Curl\.PostJson\(string, object\)')
- [Post a string body](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)')
- [Builder equivalent](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)')
- [Parse the JSON answer](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
- [PostJsonAsync\(string, object\)](CurlDotNet.DotNetCurl.PostJsonAsync(string,object).md 'CurlDotNet\.DotNetCurl\.PostJsonAsync\(string, object\)')