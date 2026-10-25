#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRedirectException](CurlDotNet.Exceptions.CurlRedirectException.md 'CurlDotNet\.Exceptions\.CurlRedirectException')

## CurlRedirectException\.RedirectCount Property

How many redirects were followed before the limit stopped the chain; a value equal to the limit usually
means a redirect loop\.

```csharp
public int RedirectCount { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
The number of hops followed\.

### Remarks

Example: `if (ex.RedirectCount >= 50) Console.WriteLine("Redirect loop - check cookies and login");`

### See Also
- [LastUrl](CurlDotNet.Exceptions.CurlRedirectException.LastUrl.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.LastUrl')