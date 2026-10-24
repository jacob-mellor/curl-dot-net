#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslCipherException](CurlDotNet.Exceptions.CurlSslCipherException.md 'CurlDotNet\.Exceptions\.CurlSslCipherException')

## CurlSslCipherException\(string, string\) Constructor

Initializes a new instance of the [CurlSslCipherException](CurlDotNet.Exceptions.CurlSslCipherException.md 'CurlDotNet\.Exceptions\.CurlSslCipherException') class\.

```csharp
public CurlSslCipherException(string cipher, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslCipherException.CurlSslCipherException(string,string).cipher'></a>

`cipher` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the SSL cipher that could not be used\.

<a name='CurlDotNet.Exceptions.CurlSslCipherException.CurlSslCipherException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.