#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.PostJsonAsync\(string, object\) Method


<b>POST with JSON data - automatically serializes objects to JSON.</b>

The easiest way to POST JSON data. Pass any object and it's automatically
             serialized to JSON with the correct Content-Type header.

<b>Example:</b>

```csharp
// Create your data object
var newUser = new
{
    name = "John Smith",
    email = "john@example.com",
    age = 30
};

// Post it as JSON automatically
var response = await Curl.PostJson("https://api.example.com/users", newUser);

// Check if successful
if (response.IsSuccess)
{
    var created = response.ParseJson<User>();
    Console.WriteLine($"User created with ID: {created.Id}");
}
```

<b>Works with any object:</b>

```csharp
// Anonymous objects
await Curl.PostJson(url, new { key = "value" });

// Your classes
await Curl.PostJson(url, myUserObject);

// Collections
await Curl.PostJson(url, new[] { item1, item2, item3 });

// Dictionaries
await Curl.PostJson(url, new Dictionary<string, object> { ["key"] = "value" });
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostJsonAsync(string url, object data);
```
#### Parameters

<a name='CurlDotNet.Curl.PostJsonAsync(string,object).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to POST to\.

<a name='CurlDotNet.Curl.PostJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Any object to serialize as JSON\. Can be:
- Anonymous objects: `new { name = "John" }`
- Your classes: `new User { Name = "John" }`
- Collections: `new[] { 1, 2, 3 }`
- Dictionaries: `Dictionary<string, object>`

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the response\.

### Remarks

Automatically adds: `Content-Type: application/json` header

Uses System.Text.Json on .NET 6+ or Newtonsoft.Json on older frameworks