#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.GetHeader\(string\) Method


<b>Get a specific header value (case-insensitive).</b>

Easy header access with null safety. This matches curl's header behavior exactly.

```csharp
public string GetHeader(string headerName);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.GetHeader(string).headerName'></a>

`headerName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Name of the header \(case doesn't matter\)

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Header value or null if not found

### Example

```csharp
// Get content type
var contentType = result.GetHeader("Content-Type");
if (contentType?.Contains("json") == true)
{
    var data = result.ParseJson<MyData>();
}

// Check rate limits (common in APIs)
var remaining = result.GetHeader("X-RateLimit-Remaining");
if (remaining != null && int.Parse(remaining) < 10)
{
    Console.WriteLine("⚠️ Only {0} API calls left!", remaining);
}

// Check cache control
var cacheControl = result.GetHeader("Cache-Control");
if (cacheControl?.Contains("no-cache") == true)
{
    Console.WriteLine("Response should not be cached");
}

// Get redirect location
var location = result.GetHeader("Location");
if (location != null)
{
    Console.WriteLine($"Redirected to: {location}");
}
```

### See Also
- [Check if header exists](CurlDotNet.Core.CurlResult.HasHeader(string).md 'CurlDotNet\.Core\.CurlResult\.HasHeader\(string\)')
- [Access all headers as dictionary](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')