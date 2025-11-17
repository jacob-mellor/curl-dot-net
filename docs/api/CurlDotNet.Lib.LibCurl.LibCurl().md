#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\(\) Constructor

Creates a new LibCurl instance\.

```csharp
public LibCurl();
```

### Example

```csharp
using (var curl = new LibCurl())
{
    var result = await curl.GetAsync("https://api.example.com/data");
}
```