#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithMaxRedirects\(int\) Method

Sets how many redirects [FollowRedirects\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FollowRedirects(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FollowRedirects\(bool\)') may follow before failing \(curl's
`--max-redirs`, default 50\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithMaxRedirects(int maxRedirects);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithMaxRedirects(int).maxRedirects'></a>

`maxRedirects` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Maximum number of redirects, such as 5; \-1 for unlimited\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Use a small number such as 5 to detect redirect loops quickly; `-1` means unlimited. When the
            limit is exceeded the request fails with [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException') (exit code 47).
            Has no effect unless redirects are followed. [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') omits the flag for the default 50.

Example: `.FollowRedirects().WithMaxRedirects(5)` is `curl -L --max-redirs 5 URL`.

### See Also
- [FollowRedirects\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FollowRedirects(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FollowRedirects\(bool\)')
- [MaxRedirects](CurlDotNet.Core.CurlOptions.MaxRedirects.md 'CurlDotNet\.Core\.CurlOptions\.MaxRedirects')