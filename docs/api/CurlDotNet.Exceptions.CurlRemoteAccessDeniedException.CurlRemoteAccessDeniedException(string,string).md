#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRemoteAccessDeniedException](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException')

## CurlRemoteAccessDeniedException\(string, string\) Constructor

Initializes a new instance of the [CurlRemoteAccessDeniedException](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException') class\.

```csharp
public CurlRemoteAccessDeniedException(string resource, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.CurlRemoteAccessDeniedException(string,string).resource'></a>

`resource` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The resource that was denied access\.

<a name='CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.CurlRemoteAccessDeniedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.