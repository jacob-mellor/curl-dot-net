#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.CurlCompatVersion Field

The curl release \(`8.7.1`\) whose command\-line behaviour CurlDotNet reproduces; it is also the
version in the default `User-Agent: curl/8.7.1` header\.

```csharp
public const string CurlCompatVersion = "8.7.1";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

### Remarks

Option parsing, exit codes, `-w` variables and wire behaviour follow this curl version. When a
            newer curl behaves differently, CurlDotNet behaves like this one.

Example: `Console.WriteLine($"Compatible with curl {Curl.CurlCompatVersion}");`. To change the
            User-Agent use [DefaultUserAgent](CurlDotNet.Curl.DefaultUserAgent.md 'CurlDotNet\.Curl\.DefaultUserAgent') or `-A`; this constant does not change.

### See Also
- [DefaultUserAgent](CurlDotNet.Curl.DefaultUserAgent.md 'CurlDotNet\.Curl\.DefaultUserAgent')