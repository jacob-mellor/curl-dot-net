#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[FluentExtensions](CurlDotNet.Extensions.FluentExtensions.md 'CurlDotNet\.Extensions\.FluentExtensions')

## FluentExtensions\.ToCurl\(this string\) Method

Creates a GET [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') for this URL so you can chain headers, data and options before
`ExecuteAsync()`\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder ToCurl(this string url);
```
#### Parameters

<a name='CurlDotNet.Extensions.FluentExtensions.ToCurl(thisstring).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The absolute URL, such as `https://api.example.com/items`\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
A new builder for a GET to [url](CurlDotNet.Extensions.FluentExtensions.ToCurl(thisstring).md#CurlDotNet.Extensions.FluentExtensions.ToCurl(thisstring).url 'CurlDotNet\.Extensions\.FluentExtensions\.ToCurl\(this string\)\.url')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Extensions.FluentExtensions.ToCurl(thisstring).md#CurlDotNet.Extensions.FluentExtensions.ToCurl(thisstring).url 'CurlDotNet\.Extensions\.FluentExtensions\.ToCurl\(this string\)\.url') is null\.

### Remarks

Same as [Get\(string\)](CurlDotNet.Core.CurlRequestBuilder.Get(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)'); calling `WithData`/`WithJson` later switches the
            method to POST.

Example: `var r = await "https://api.example.com/items".ToCurl().WithJson(new { name = "x" }).ExecuteAsync();`

### See Also
- [Get\(string\)](CurlDotNet.Core.CurlRequestBuilder.Get(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)')
- [WithMethod\(this string, string\)](CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).md 'CurlDotNet\.Extensions\.FluentExtensions\.WithMethod\(this string, string\)')