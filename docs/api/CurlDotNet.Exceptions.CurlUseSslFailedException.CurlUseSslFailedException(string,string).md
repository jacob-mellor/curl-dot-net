#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlUseSslFailedException](CurlDotNet.Exceptions.CurlUseSslFailedException.md 'CurlDotNet\.Exceptions\.CurlUseSslFailedException')

## CurlUseSslFailedException\(string, string\) Constructor

Creates the error \(exit code 64\) for a server that could not provide the required TLS level, prefixing
`Required SSL level failed:`\.

```csharp
public CurlUseSslFailedException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUseSslFailedException.CurlUseSslFailedException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the server answered, naming it; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlUseSslFailedException.CurlUseSslFailedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlUseSslFailedException.CurlUseSslFailedException(string,string).md#CurlDotNet.Exceptions.CurlUseSslFailedException.CurlUseSslFailedException(string,string).message 'CurlDotNet\.Exceptions\.CurlUseSslFailedException\.CurlUseSslFailedException\(string, string\)\.message') has no fix, the suggestion mentions FTPS and `--ssl-reqd`.

### See Also
- [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')