#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

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
### Constructors

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception)'></a>

## CurlCookieException\(string, string, string, Exception\) Constructor

Initializes a new instance of the [CurlCookieException](CurlDotNet.Exceptions.CurlCookieException.md 'CurlDotNet\.Exceptions\.CurlCookieException') class\.

```csharp
public CurlCookieException(string message, string cookieJarPath=null, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the cookie operation failure\.

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).cookieJarPath'></a>

`cookieJarPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The path to the cookie jar file, if applicable\.

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing\.

<a name='CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The underlying exception that caused this error\.
### Properties

<a name='CurlDotNet.Exceptions.CurlCookieException.CookieJarPath'></a>

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