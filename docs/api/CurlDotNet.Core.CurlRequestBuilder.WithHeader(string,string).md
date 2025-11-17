#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithHeader\(string, string\) Method

Add a header to the request\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithHeader(string name, string value);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header name \(e\.g\., "Content\-Type"\)

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header value \(e\.g\., "application/json"\)

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining

### Example

```csharp
// Add Content-Type header
var result = await CurlRequestBuilder
    .Post("https://api.example.com/data")
    .WithHeader("Content-Type", "application/json")
    .WithJson(new { key = "value" })
    .ExecuteAsync();

// Add custom API key header
var result = await CurlRequestBuilder
    .Get("https://api.example.com/protected")
    .WithHeader("X-API-Key", "your-api-key-here")
    .WithHeader("X-Client-Version", "1.0.0")
    .ExecuteAsync();

// Add Accept header for API versioning
var result = await CurlRequestBuilder
    .Get("https://api.github.com/user")
    .WithHeader("Accept", "application/vnd.github.v3+json")
    .WithBearerToken("your-token")
    .ExecuteAsync();
```