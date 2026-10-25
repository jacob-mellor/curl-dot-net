#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlCookieException Class

Reading or writing cookies failed: the cookie jar file \(`-c`\) could not be saved, the cookie file
\(`-b file`\) could not be read, or a cookie could not be parsed; [CookieJarPath](CurlDotNet.Exceptions.CurlCookieException.CookieJarPath.md 'CurlDotNet\.Exceptions\.CurlCookieException\.CookieJarPath') names the file\.

```csharp
public class CurlCookieException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlCookieException

### Example

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl --cookie-jar state/cookies.txt https://example.com");
}
catch (CurlWriteErrorException ex)              // what the engine throws when -c cannot be saved (exit 23)
{
    Console.WriteLine($"Cookie jar not saved: {ex.FilePath} - {ex.Suggestions[0]}");
}
catch (CurlCookieException ex)                  // raised by your own cookie-file code
{
    Console.WriteLine($"Cookie error: {ex.WhatHappened} ({ex.CookieJarPath})");
}
```

### Remarks

CurlDotNet reports a cookie jar that cannot be written as [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException') (23), like curl;
            this type is for application code that manages cookie files. Cookie files use the Netscape format written by
            `-c`; `-b` also accepts a literal `name=value; other=2` string.

Example: `catch (CurlCookieException ex) { Console.WriteLine($"Cookie file problem: {ex.CookieJarPath}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/cookie-errors.md
            cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md

### See Also
- [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')

| Constructors | |
| :--- | :--- |
| [CurlCookieException\(string, string, string, Exception\)](CurlDotNet.Exceptions.CurlCookieException.CurlCookieException(string,string,string,System.Exception).md 'CurlDotNet\.Exceptions\.CurlCookieException\.CurlCookieException\(string, string, string, System\.Exception\)') | Creates a cookie error naming the cookie file, keeping the original I/O or parsing exception as the inner exception\. |

| Properties | |
| :--- | :--- |
| [CookieJarPath](CurlDotNet.Exceptions.CurlCookieException.CookieJarPath.md 'CurlDotNet\.Exceptions\.CurlCookieException\.CookieJarPath') | The cookie file that could not be read or written, as given to `-b`/`--cookie` or `-c`/`--cookie-jar`\. |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlCookieException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlCookieException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlCookieException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlCookieException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
