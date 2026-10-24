#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException')

## CurlTooManyRedirectsException\.RedirectCount Property

How many redirects were followed before the next one was refused; equal to the limit when the chain loops\.

```csharp
public int RedirectCount { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
The number of hops followed\.

### Remarks

Example: `if (ex.RedirectCount >= 50) Console.WriteLine("Redirect loop");`

### See Also
- [RedirectCount](CurlDotNet.Exceptions.CurlRedirectException.RedirectCount.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.RedirectCount')