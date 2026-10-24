#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithJson\(object\) Method

Serializes [data](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md#CurlDotNet.Core.CurlRequestBuilder.WithJson(object).data 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)\.data') to JSON and sends it as the body with
`Content-Type: application/json` \(curl's `-H 'Content-Type: application/json' -d JSON`\), switching a GET to POST\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithJson(object data);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithJson(object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The object to serialize: anonymous object, class instance, dictionary, list or value\. Null sends the JSON literal `null`\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

#### Exceptions

[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  
[data](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md#CurlDotNet.Core.CurlRequestBuilder.WithJson(object).data 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)\.data') cannot be serialized to JSON \(a reference cycle,
            a delegate or stream member\); the serializer's exception is the InnerException\. Fix: pass a plain DTO, or
            serialize it yourself and use [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)')\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-post\-json\-csharp/README\.md

### Example

```csharp
var user = new User { Name = "John", Email = "john@example.com" };
var result = await CurlRequestBuilder
    .Put("https://api.example.com/users/123")
    .WithJson(user)
    .WithBearerToken("your-token")
    .ExecuteAsync();
```

### Remarks

Any object works: an anonymous object (`new { name = "Ada" }`), your own class, a dictionary
            or a list. Serialization uses System.Text.Json on .NET 8/9/10 and .NET Framework 4.7.2+/4.8 (the same
            settings as [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)')) and Newtonsoft.Json on .NET Standard 2.0;
            property names are written as declared in C#. A string is serialized as a JSON string literal, so
            to send JSON text you already have, use `.WithData(json).WithHeader("Content-Type", "application/json")`.

The Content-Type header is set to `application/json`, replacing an earlier value; add an
            `Accept` header yourself if the API needs one. PUT and PATCH keep their method.

Example: `var r = await CurlRequestBuilder.Post("https://api.example.com/users").WithJson(new { name = "John", age = 30 }).ExecuteAsync();`
            sends `{"name":"John","age":30}`, the same as `curl -H 'Content-Type: application/json' -d '{"name":"John","age":30}' https://api.example.com/users`
            (curl's `--json` also adds `Accept: application/json`; add `.WithHeader("Accept", "application/json")` for that).

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)')
- [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)')
- [PostAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PostAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')
- [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')