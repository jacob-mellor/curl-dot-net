#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')

## CurlSslException\(string, string, string\) Constructor

Initializes a new instance of CurlSslException\.

```csharp
public CurlSslException(string message, string? certError=null, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).certError'></a>

`certError` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Details about the certificate error\.

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed\.