#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.DefaultInsecure Property

When true, TLS certificates are not verified for any request, as if `-k` were in every command; the same
setting as [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure')\. Development and testing only\.

```csharp
public static bool DefaultInsecure { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true to skip certificate verification \(dangerous\); default false\.

### Remarks

Disabling verification exposes every https:// request to man-in-the-middle attacks; prefer
            `--cacert` for a private CA. Default false. Thread-safe.

Example: `#if DEBUG DotNetCurl.DefaultInsecure = true; #endif`

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md

### See Also
- [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure')