#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRedirectException](CurlDotNet.Exceptions.CurlRedirectException.md 'CurlDotNet\.Exceptions\.CurlRedirectException')

## CurlRedirectException\.LastUrl Property

The last URL in the chain before it was stopped, which shows where the loop or the long chain goes\.

```csharp
public string LastUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An absolute URL, or null when not recorded\.

### Remarks

Open it with `curl -i` (without `-L`) to see its `Location` header.

### See Also
- [RedirectCount](CurlDotNet.Exceptions.CurlRedirectException.RedirectCount.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.RedirectCount')