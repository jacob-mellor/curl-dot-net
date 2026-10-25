#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCookieException](CurlDotNet.Exceptions.CurlCookieException.md 'CurlDotNet\.Exceptions\.CurlCookieException')

## CurlCookieException\.CookieJarPath Property

The cookie file that could not be read or written, as given to `-b`/`--cookie` or
`-c`/`--cookie-jar`\.

```csharp
public string CookieJarPath { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path, or null when the problem is a cookie string rather than a file\.

### Remarks

Example: `File.Exists(ex.CookieJarPath)` tells whether the file is missing or unreadable.

### See Also
- [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command')