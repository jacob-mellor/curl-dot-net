#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithRedirectHandler\(Action\<string\>\) Method

Sets [OnRedirect](CurlDotNet.Core.CurlSettings.OnRedirect.md 'CurlDotNet\.Core\.CurlSettings\.OnRedirect'), called with the absolute URL of every redirect that is followed, and returns
these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithRedirectHandler(System.Action<string> callback);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithRedirectHandler(System.Action_string_).callback'></a>

`callback` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Receives each redirect target URL; null removes it\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Redirects are only followed with `-L` or [WithFollowRedirects\(bool\)](CurlDotNet.Core.CurlSettings.WithFollowRedirects(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithFollowRedirects\(bool\)'); throwing from the
            callback stops the transfer with exit code 42.

Example: `settings.WithFollowRedirects().WithRedirectHandler(url => Console.WriteLine("-> " + url));`

### See Also
- [OnRedirect](CurlDotNet.Core.CurlSettings.OnRedirect.md 'CurlDotNet\.Core\.CurlSettings\.OnRedirect')
- [WithFollowRedirects\(bool\)](CurlDotNet.Core.CurlSettings.WithFollowRedirects(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithFollowRedirects\(bool\)')