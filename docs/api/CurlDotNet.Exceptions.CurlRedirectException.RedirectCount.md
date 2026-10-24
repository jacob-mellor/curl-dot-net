#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRedirectException](CurlDotNet.Exceptions.CurlRedirectException.md 'CurlDotNet\.Exceptions\.CurlRedirectException')

## CurlRedirectException\.RedirectCount Property

Gets the number of redirects that were followed before the limit was exceeded\.

```csharp
public int RedirectCount { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
The count of redirects attempted\.

### Remarks

This helps identify potential redirect loops or excessive redirect chains.

AI-Usage: If count is high, suspect a redirect loop.