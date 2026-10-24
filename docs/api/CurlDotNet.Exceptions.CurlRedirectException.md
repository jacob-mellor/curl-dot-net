#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlRedirectException Class

Thrown when redirect limit is exceeded during HTTP requests\.

```csharp
public class CurlRedirectException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlRedirectException

### Example

```csharp
try
{
    // Default limit is usually 50 redirects
    var result = await curl.ExecuteAsync("curl -L https://example.com/redirect-loop");
}
catch (CurlRedirectException ex)
{
    Console.WriteLine($"Too many redirects: {ex.RedirectCount}");
    Console.WriteLine($"Last URL attempted: {ex.LastUrl}");

    // Try again with limited redirects
    result = await curl.ExecuteAsync("curl --max-redirs 5 https://example.com/redirect-loop");
}
```

### Remarks

This exception indicates that the maximum number of HTTP redirects has been exceeded.

Curl error code: CURLE_TOO_MANY_REDIRECTS (47)

Common causes: redirect loops, misconfigured servers, or intentional redirect chains.

AI-Usage: Catch this to handle redirect issues separately from other HTTP errors.

AI-Pattern: Check RedirectCount and LastUrl to debug redirect chains.

For more information, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md

| Constructors | |
| :--- | :--- |
| [CurlRedirectException\(string, int, string, string\)](CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string).md 'CurlDotNet\.Exceptions\.CurlRedirectException\.CurlRedirectException\(string, int, string, string\)') | Initializes a new instance of the [CurlRedirectException](CurlDotNet.Exceptions.CurlRedirectException.md 'CurlDotNet\.Exceptions\.CurlRedirectException') class\. |

| Properties | |
| :--- | :--- |
| [LastUrl](CurlDotNet.Exceptions.CurlRedirectException.LastUrl.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.LastUrl') | Gets the last URL that was attempted before the redirect limit was exceeded\. |
| [RedirectCount](CurlDotNet.Exceptions.CurlRedirectException.RedirectCount.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.RedirectCount') | Gets the number of redirects that were followed before the limit was exceeded\. |
