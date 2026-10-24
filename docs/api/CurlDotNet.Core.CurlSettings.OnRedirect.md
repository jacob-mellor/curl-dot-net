#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.OnRedirect Property

Called with the absolute target URL of every redirect that is followed \(each `Location` hop with
`-L`\), before the next request is sent\.

```csharp
public System.Action<string> OnRedirect { get; set; }
```

#### Property Value
[System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')  
Null by default \(no callback\)\.

### Remarks

Only followed redirects are reported: without `-L` (or [FollowRedirects](CurlDotNet.Core.CurlSettings.FollowRedirects.md 'CurlDotNet\.Core\.CurlSettings\.FollowRedirects')) the 3xx response
            is returned and nothing is called. The URL is resolved against the previous hop, so relative `Location`
            values arrive as absolute URLs. It runs on the thread performing the transfer; keep it fast. If it throws, the
            transfer stops with [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException') (exit code 42) carrying your exception as
            [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException'), which is how to refuse a redirect target.

Example: `var hops = new List<string>(); await Curl.ExecuteAsync("curl -L http://example.com", new CurlSettings().WithRedirectHandler(hops.Add));`

### See Also
- [WithRedirectHandler\(Action&lt;string&gt;\)](CurlDotNet.Core.CurlSettings.WithRedirectHandler(System.Action_string_).md 'CurlDotNet\.Core\.CurlSettings\.WithRedirectHandler\(System\.Action\<string\>\)')
- [FollowRedirects](CurlDotNet.Core.CurlSettings.FollowRedirects.md 'CurlDotNet\.Core\.CurlSettings\.FollowRedirects')