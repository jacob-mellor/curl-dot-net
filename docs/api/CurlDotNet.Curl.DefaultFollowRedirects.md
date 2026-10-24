#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultFollowRedirects Property

When true, every request follows HTTP redirects \(301, 302, 303, 307, 308\) as if `-L`/`--location`
were in every command\. False \(the default\) returns the redirect response itself, like curl\.

```csharp
public static bool DefaultFollowRedirects { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true to follow redirects everywhere; false \(default\) to follow only with `-L`\.

### Remarks

While this is true it cannot be switched off for a single command; leave it false and add `-L`
            to the commands that need it if only some should follow redirects. The redirect limit is
            `--max-redirs` (curl's default is 50); exceeding it throws [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException')
            (exit code 47). As in curl, credentials given with `-u` are not sent to a different host after a
            redirect unless the command has `--location-trusted`.

Process-wide and thread-safe; read at the start of every transfer.

Example: `Curl.DefaultFollowRedirects = true;` then `(await Curl.ExecuteAsync("curl https://bit.ly/example")).StatusCode`
            is the status of the final page instead of 301.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md

### See Also
- [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation')
- [WithFollowRedirects\(bool\)](CurlDotNet.Core.CurlSettings.WithFollowRedirects(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithFollowRedirects\(bool\)')
- [FollowRedirects\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FollowRedirects(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FollowRedirects\(bool\)')
- [DefaultFollowRedirects](CurlDotNet.DotNetCurl.DefaultFollowRedirects.md 'CurlDotNet\.DotNetCurl\.DefaultFollowRedirects')