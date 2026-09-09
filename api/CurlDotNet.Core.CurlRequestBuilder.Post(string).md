#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Post\(string\) Method

Create a POST request builder\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Post(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Post(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to POST to

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining

### Example

```csharp
// POST with JSON
var result = await CurlRequestBuilder
    .Post("https://api.example.com/users")
    .WithJson(new { name = "John", email = "john@example.com" })
    .ExecuteAsync();

// POST with form data
var result = await CurlRequestBuilder
    .Post("https://api.example.com/login")
    .WithFormData(new Dictionary<string, string> {
        { "username", "user123" },
        { "password", "pass456" }
    })
    .ExecuteAsync();

// POST with raw string data
var result = await CurlRequestBuilder
    .Post("https://api.example.com/data")
    .WithData("key1=value1&key2=value2")
    .WithHeader("Content-Type", "application/x-www-form-urlencoded")
    .ExecuteAsync();
```