#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRedirectException](CurlDotNet.Exceptions.CurlRedirectException.md 'CurlDotNet\.Exceptions\.CurlRedirectException')

## CurlRedirectException\.LastUrl Property

Gets the last URL that was attempted before the redirect limit was exceeded\.

```csharp
public string LastUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The final URL in the redirect chain, or null if not available\.

### Remarks

This URL can help identify where the redirect loop or chain ends.

AI-Usage: Use this to debug redirect chains and identify problematic URLs.