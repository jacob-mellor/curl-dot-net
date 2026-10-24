#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRemoteAccessDeniedException](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException')

## CurlRemoteAccessDeniedException\(string, string\) Constructor

Creates the access\-denied error \(exit code 9\) for [resource](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.CurlRemoteAccessDeniedException(string,string).md#CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.CurlRemoteAccessDeniedException(string,string).resource 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException\.CurlRemoteAccessDeniedException\(string, string\)\.resource'), with advice on credentials,
path and permissions\.

```csharp
public CurlRemoteAccessDeniedException(string resource, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.CurlRemoteAccessDeniedException(string,string).resource'></a>

`resource` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The resource that was denied access, such as a remote path\.

<a name='CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.CurlRemoteAccessDeniedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Access denied to: RESOURCE`. Example: `throw new CurlRemoteAccessDeniedException("/pub/private", command);`

### See Also
- [CurlFtpException](CurlDotNet.Exceptions.CurlFtpException.md 'CurlDotNet\.Exceptions\.CurlFtpException')