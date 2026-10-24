#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException')

## CurlLoginDeniedException\(string, string\) Constructor

Creates the login\-denied error \(exit code 67\), prefixing `Login denied:` to the server's answer\.

```csharp
public CurlLoginDeniedException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlLoginDeniedException.CurlLoginDeniedException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the server answered, naming the user \(never the password\); optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlLoginDeniedException.CurlLoginDeniedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlLoginDeniedException.CurlLoginDeniedException(string,string).md#CurlDotNet.Exceptions.CurlLoginDeniedException.CurlLoginDeniedException(string,string).message 'CurlDotNet\.Exceptions\.CurlLoginDeniedException\.CurlLoginDeniedException\(string, string\)\.message') has no fix, the suggestion covers `-u` and quoting.

### See Also
- [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException')