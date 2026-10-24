#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.UserAgent Property

The `User-Agent` header to send, like curl's `-A`/`--user-agent`; it replaces the command's
`-A` and any `-H "User-Agent: ..."`\.

```csharp
public string UserAgent { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Null by default \(the command decides; without one curl sends `curl/VERSION`, see
            [DefaultUserAgent](CurlDotNet.Curl.DefaultUserAgent.md 'CurlDotNet\.Curl\.DefaultUserAgent')\)\. An empty string sends no User\-Agent at all\.

### Remarks

Copied to [UserAgent](CurlDotNet.Core.CurlOptions.UserAgent.md 'CurlDotNet\.Core\.CurlOptions\.UserAgent'). A [Headers](CurlDotNet.Core.CurlSettings.Headers.md 'CurlDotNet\.Core\.CurlSettings\.Headers') entry named `User-Agent` is
            applied after it and wins.

Example: `settings.WithUserAgent("MyApp/1.0 (+https://myapp.example)")`.

### See Also
- [WithUserAgent\(string\)](CurlDotNet.Core.CurlSettings.WithUserAgent(string).md 'CurlDotNet\.Core\.CurlSettings\.WithUserAgent\(string\)')
- [UserAgent](CurlDotNet.Core.CurlOptions.UserAgent.md 'CurlDotNet\.Core\.CurlOptions\.UserAgent')