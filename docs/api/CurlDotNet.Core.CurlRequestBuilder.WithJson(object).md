#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithJson\(object\) Method

Add POST/PUT data as JSON \(automatically serializes and sets Content\-Type\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithJson(object data);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithJson(object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The object to serialize as JSON\. Can be any class, anonymous object, or built\-in type\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining

### Example

```csharp
// POST with anonymous object
var result = await CurlRequestBuilder
    .Post("https://api.example.com/users")
    .WithJson(new { name = "John", email = "john@example.com", age = 30 })
    .ExecuteAsync();

// POST with typed class
public class User {
    public string Name { get; set; }
    public string Email { get; set; }
}
var user = new User { Name = "John", Email = "john@example.com" };
var result = await CurlRequestBuilder
    .Post("https://api.example.com/users")
    .WithJson(user)
    .ExecuteAsync();

// PUT with JSON update
var result = await CurlRequestBuilder
    .Put("https://api.example.com/users/123")
    .WithJson(new { name = "Jane", email = "jane@example.com" })
    .WithBearerToken("your-token")
    .ExecuteAsync();
```