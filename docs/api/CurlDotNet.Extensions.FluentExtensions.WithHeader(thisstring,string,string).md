#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[FluentExtensions](CurlDotNet.Extensions.FluentExtensions.md 'CurlDotNet\.Extensions\.FluentExtensions')

## FluentExtensions\.WithHeader\(this string, string, string\) Method

Creates a GET [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') for this URL with one header already set \(like `-H "key: value"`\),
ready for more chaining\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder WithHeader(this string url, string key, string value);
```
#### Parameters

<a name='CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The absolute URL, such as `https://api.example.com/items`\.

<a name='CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header name without the colon, such as `Accept`\.

<a name='CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header value, such as `application/json`\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
A new builder for a GET to [url](CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).md#CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).url 'CurlDotNet\.Extensions\.FluentExtensions\.WithHeader\(this string, string, string\)\.url') with the header set\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).md#CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).url 'CurlDotNet\.Extensions\.FluentExtensions\.WithHeader\(this string, string, string\)\.url') is null\.

### Remarks

Shorthand for `CurlRequestBuilder.Get(url).WithHeader(key, value)`.

Example: `var r = await "https://api.github.com/user".WithHeader("Authorization", "Bearer " + token).ExecuteAsync();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md

### See Also
- [WithHeader\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)')
- [ToCurl\(this string\)](CurlDotNet.Extensions.FluentExtensions.ToCurl(thisstring).md 'CurlDotNet\.Extensions\.FluentExtensions\.ToCurl\(this string\)')