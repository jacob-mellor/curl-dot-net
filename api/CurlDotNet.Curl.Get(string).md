#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.Get\(string\) Method


<b>⚠️ SYNCHRONOUS GET request (blocks thread).</b>

```csharp
var result = Curl.Get("https://api.example.com"); // Blocks!
```

```csharp
public static CurlDotNet.Core.CurlResult Get(string url);
```
#### Parameters

<a name='CurlDotNet.Curl.Get(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')