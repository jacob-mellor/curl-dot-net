#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithUserAgent\(string\) Method

Sets the `User-Agent` header \(see [UserAgent](CurlDotNet.Core.CurlSettings.UserAgent.md 'CurlDotNet\.Core\.CurlSettings\.UserAgent')\), like curl's `-A`, and returns these
settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithUserAgent(string userAgent);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithUserAgent(string).userAgent'></a>

`userAgent` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The product token string, such as `MyApp/1.0`\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Example: `settings.WithUserAgent("MyApp/2.1 (+https://myapp.example)")`.

### See Also
- [UserAgent](CurlDotNet.Core.CurlSettings.UserAgent.md 'CurlDotNet\.Core\.CurlSettings\.UserAgent')
- [WithHeader\(string, string\)](CurlDotNet.Core.CurlSettings.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlSettings\.WithHeader\(string, string\)')