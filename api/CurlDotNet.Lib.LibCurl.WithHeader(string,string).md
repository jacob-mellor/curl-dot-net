#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithHeader\(string, string\) Method

Set a default header for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithHeader(string key, string value);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithHeader(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header name

<a name='CurlDotNet.Lib.LibCurl.WithHeader(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header value

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

### Example

```csharp
using (var curl = new LibCurl())
{
    curl.WithHeader("Accept", "application/json")
        .WithHeader("X-API-Key", "your-key");
    
    // All subsequent requests will include these headers
    var result = await curl.GetAsync("https://api.example.com/data");
}
```