#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException')

## CurlAuthenticationException\(string, string, string\) Constructor

Initializes a new instance of the CurlAuthenticationException class\.

```csharp
public CurlAuthenticationException(string message, string authMethod=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).authMethod'></a>

`authMethod` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The authentication method that failed\.

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the error\.