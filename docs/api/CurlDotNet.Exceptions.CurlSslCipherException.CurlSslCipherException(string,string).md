#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslCipherException](CurlDotNet.Exceptions.CurlSslCipherException.md 'CurlDotNet\.Exceptions\.CurlSslCipherException')

## CurlSslCipherException\(string, string\) Constructor

Creates the error \(exit code 59\) for a cipher that cannot be used, with advice to remove or change
`--ciphers`\.

```csharp
public CurlSslCipherException(string cipher, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslCipherException.CurlSslCipherException(string,string).cipher'></a>

`cipher` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the SSL cipher \(or cipher list\) that could not be used\.

<a name='CurlDotNet.Exceptions.CurlSslCipherException.CurlSslCipherException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Could not use SSL cipher: NAME`.

### See Also
- [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')