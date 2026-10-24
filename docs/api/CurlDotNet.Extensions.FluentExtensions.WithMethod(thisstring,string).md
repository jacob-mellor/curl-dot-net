#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[FluentExtensions](CurlDotNet.Extensions.FluentExtensions.md 'CurlDotNet\.Extensions\.FluentExtensions')

## FluentExtensions\.WithMethod\(this string, string\) Method

Creates a [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') for this URL with the HTTP method you name \(like `-X METHOD`\), such as
DELETE, PUT, PATCH or OPTIONS\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder WithMethod(this string url, string method);
```
#### Parameters

<a name='CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The absolute URL, such as `https://api.example.com/items/42`\.

<a name='CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).method'></a>

`method` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The HTTP method name, in any case, such as `delete` or `PATCH`\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
A new builder for [method](CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).md#CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).method 'CurlDotNet\.Extensions\.FluentExtensions\.WithMethod\(this string, string\)\.method') to [url](CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).md#CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).url 'CurlDotNet\.Extensions\.FluentExtensions\.WithMethod\(this string, string\)\.url')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).md#CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).url 'CurlDotNet\.Extensions\.FluentExtensions\.WithMethod\(this string, string\)\.url') is null\.

### Remarks

Same as [Request\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.Request(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Request\(string, string\)'); the method is upper-cased.

Example: `var r = await "https://api.example.com/items/42".WithMethod("DELETE").ExecuteAsync();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [Request\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.Request(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Request\(string, string\)')
- [ToCurl\(this string\)](CurlDotNet.Extensions.FluentExtensions.ToCurl(thisstring).md 'CurlDotNet\.Extensions\.FluentExtensions\.ToCurl\(this string\)')