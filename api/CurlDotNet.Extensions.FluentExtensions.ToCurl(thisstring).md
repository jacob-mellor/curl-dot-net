#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[FluentExtensions](CurlDotNet.Extensions.FluentExtensions.md 'CurlDotNet\.Extensions\.FluentExtensions')

## FluentExtensions\.ToCurl\(this string\) Method

Starts building a curl command from a URL\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder ToCurl(this string url);
```
#### Parameters

<a name='CurlDotNet.Extensions.FluentExtensions.ToCurl(thisstring).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

### Example
var result = await "https://api\.example\.com"
    \.WithHeader\("Authorization", "Bearer token"\)
    \.WithData\(@"\{""key"":""value""\}"\)
    \.ExecuteAsync\(\);