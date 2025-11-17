#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](index.md#CurlDotNet.Extensions 'CurlDotNet\.Extensions')

## FluentExtensions Class

Extension methods for fluent curl building\.

```csharp
public static class FluentExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; FluentExtensions
### Methods

<a name='CurlDotNet.Extensions.FluentExtensions.ToCurl(thisstring)'></a>

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

<a name='CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string)'></a>

## FluentExtensions\.WithHeader\(this string, string, string\) Method

Adds a header to the curl builder\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder WithHeader(this string url, string key, string value);
```
#### Parameters

<a name='CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Extensions.FluentExtensions.WithHeader(thisstring,string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string)'></a>

## FluentExtensions\.WithMethod\(this string, string\) Method

Sets the HTTP method\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder WithMethod(this string url, string method);
```
#### Parameters

<a name='CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Extensions.FluentExtensions.WithMethod(thisstring,string).method'></a>

`method` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')