#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException')

## CurlAuthenticationException\(string, string, string\) Constructor

Creates a login\-denied error \(exit code 67\) naming the authentication method that was refused\.

```csharp
public CurlAuthenticationException(string message, string authMethod=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What was refused, naming the server and the user \(never the password\)\.

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).authMethod'></a>

`authMethod` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The scheme that failed, such as `Basic` or `Digest`, or null\.

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the error, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).md#CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).message 'CurlDotNet\.Exceptions\.CurlAuthenticationException\.CurlAuthenticationException\(string, string, string\)\.message') has no fix, the suggestion tells the reader to check `-u user:password`
            and the scheme. Example: `throw new CurlAuthenticationException("ftp.example.com refused user 'bob'.", "USER/PASS", command);`

### See Also
- [CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException')