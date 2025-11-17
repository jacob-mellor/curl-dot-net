#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Get\(string\) Method

Create a GET request builder\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Get(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Get(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to GET

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining

### Example

```csharp
// Simple GET request
var result = await CurlRequestBuilder
    .Get("https://api.github.com/users/octocat")
    .ExecuteAsync();

// GET with headers
var result = await CurlRequestBuilder
    .Get("https://api.example.com/data")
    .WithHeader("Accept", "application/json")
    .WithHeader("X-API-Key", "your-key")
    .ExecuteAsync();

// GET with authentication
var result = await CurlRequestBuilder
    .Get("https://api.example.com/protected")
    .WithBearerToken("your-token")
    .FollowRedirects()
    .ExecuteAsync();
```