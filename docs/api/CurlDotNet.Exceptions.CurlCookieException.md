#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlCookieException Class

Thrown when cookie operations fail during HTTP requests\.

```csharp
public class CurlCookieException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlCookieException

### Example

```csharp
try
{
    var result = await curl.ExecuteAsync("curl --cookie-jar cookies.txt https://example.com");
}
catch (CurlCookieException ex)
{
    Console.WriteLine($"Cookie error: {ex.Message}");
    if (!string.IsNullOrEmpty(ex.CookieJarPath))
    {
        Console.WriteLine($"Cookie jar path: {ex.CookieJarPath}");
        // Check file permissions or try alternative path
    }
}
```

### Remarks

This exception indicates problems with cookie handling, including cookie jar file access, cookie parsing, or cookie storage.

Common causes: invalid cookie jar path, file permissions, malformed cookies, or disk space issues.

AI-Usage: Catch this to handle cookie-related failures separately from other HTTP errors.

AI-Pattern: Check CookieJarPath property to identify file access issues.

For more information, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/cookie-errors.md

| Constructors | |
| :--- | :--- |
| [CurlCookieException\(string, string, string, Exception\)](CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).md 'CurlDotNet\.Exceptions\.CurlCookieException\.CurlCookieException\(string, string, string, System\.Exception\)') | Initializes a new instance of the [CurlCookieException](CurlDotNet.Exceptions.CurlCookieException.md 'CurlDotNet\.Exceptions\.CurlCookieException') class\. |

| Properties | |
| :--- | :--- |
| [CookieJarPath](CurlDotNet.Exceptions.CurlCookieException.CookieJarPath.md 'CurlDotNet\.Exceptions\.CurlCookieException\.CookieJarPath') | Gets the path to the cookie jar file that caused the error, if applicable\. |
