#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Headers Property


<b>All HTTP headers from the response - contains metadata about the response.</b>

Headers tell you things like content type, cache rules, rate limits, etc.
             Access them like a dictionary (case-insensitive keys).

<b>Get a specific header:</b>

```csharp
// These all work (case-insensitive):
var type = result.Headers["Content-Type"];
var type = result.Headers["content-type"];
var type = result.Headers["CONTENT-TYPE"];

// Or use the helper:
var type = result.GetHeader("Content-Type");
```

<b>Check rate limits (common in APIs):</b>

```csharp
if (result.Headers.ContainsKey("X-RateLimit-Remaining"))
{
    var remaining = int.Parse(result.Headers["X-RateLimit-Remaining"]);
    if (remaining < 10)
        Console.WriteLine("⚠️ Only {0} API calls left!", remaining);
}
```

<b>Common headers:</b>
- <b>Content-Type</b> - Format of the data (application/json, text/html)
- <b>Content-Length</b> - Size in bytes
- <b>Location</b> - Where you got redirected to
- <b>Set-Cookie</b> - Cookies to store
- <b>Cache-Control</b> - How long to cache

```csharp
public System.Collections.Generic.Dictionary<string,string> Headers { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')