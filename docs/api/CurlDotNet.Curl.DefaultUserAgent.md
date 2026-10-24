#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultUserAgent Property


<b>The User-Agent sent when a command does not set one.</b>

Real curl always identifies itself as `curl/<version>`, and many APIs
            (GitHub's, for example) reject requests with no User-Agent at all, so CurlDotNet sends
            `curl/8.7.1` by default. Override per request with `-A` / `--user-agent` or
            `-H "User-Agent: ..."`, suppress it with `-H "User-Agent:"`, or change the default
            for the whole process here.

```csharp
Curl.DefaultUserAgent = "MyApp/2.0 (+https://example.com)";
```

```csharp
public static string DefaultUserAgent { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')