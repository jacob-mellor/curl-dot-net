#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlSslConnectErrorException Class

CURLE\_SSL\_CONNECT\_ERROR \(35\) \- SSL connect error

```csharp
public class CurlSslConnectErrorException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslConnectErrorException
### Constructors

<a name='CurlDotNet.Exceptions.CurlSslConnectErrorException.CurlSslConnectErrorException(string,string)'></a>

## CurlSslConnectErrorException\(string, string\) Constructor

Initializes a new instance of CurlSslConnectErrorException\.

```csharp
public CurlSslConnectErrorException(string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslConnectErrorException.CurlSslConnectErrorException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlSslConnectErrorException.CurlSslConnectErrorException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed\.