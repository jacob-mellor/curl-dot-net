#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCookieException](CurlDotNet.Exceptions.CurlCookieException.md 'CurlDotNet\.Exceptions\.CurlCookieException')

## CurlCookieException\.CookieJarPath Property

Gets the path to the cookie jar file that caused the error, if applicable\.

```csharp
public string CookieJarPath { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The file path to the cookie jar, or null if not file\-related\.

### Remarks

This property helps identify file access issues with cookie storage.

AI-Usage: Use this to check file permissions or suggest alternative paths.