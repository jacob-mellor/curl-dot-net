#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultUserAgent Property

The `User-Agent` header sent when a command does not set one; `curl/8.7.1` unless you change it\.
Set it to null or an empty string to send no User\-Agent at all\.

```csharp
public static string DefaultUserAgent { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Any header value; default `"curl/" + CurlCompatVersion`\. Null or empty sends no header\.

### Remarks

Real curl always identifies itself as `curl/<version>`, and many APIs (GitHub's, for
            example) reject requests with no User-Agent, so CurlDotNet sends one by default. Per request, `-A` /
            `--user-agent "MyApp/1.0"` or `-H "User-Agent: MyApp/1.0"` wins over this default, and
            `-H "User-Agent:"` (empty value) suppresses the header for that request.

Process-wide: read at the start of every HTTP request (and for the CONNECT request of a proxy
            tunnel). Set it once at start-up; it is safe to read from many threads.

Example: `Curl.DefaultUserAgent = "MyApp/2.0 (+https://example.com)";` then every
            `Curl.ExecuteAsync("curl https://api.github.com")` sends `User-Agent: MyApp/2.0 (+https://example.com)`.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md

### See Also
- [CurlCompatVersion](CurlDotNet.Curl.CurlCompatVersion.md 'CurlDotNet\.Curl\.CurlCompatVersion')
- [WithUserAgent\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithUserAgent(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUserAgent\(string\)')
- [UserAgent](CurlDotNet.Core.CurlOptions.UserAgent.md 'CurlDotNet\.Core\.CurlOptions\.UserAgent')