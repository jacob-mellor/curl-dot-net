#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslCaCertBadFileException](CurlDotNet.Exceptions.CurlSslCaCertBadFileException.md 'CurlDotNet\.Exceptions\.CurlSslCaCertBadFileException')

## CurlSslCaCertBadFileException\(string, string, string\) Constructor

Creates the error \(exit code 77\) for an unusable CA bundle or folder\.

```csharp
public CurlSslCaCertBadFileException(string filePath, string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslCaCertBadFileException.CurlSslCaCertBadFileException(string,string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The `--cacert` file or `--capath` folder\.

<a name='CurlDotNet.Exceptions.CurlSslCaCertBadFileException.CurlSslCaCertBadFileException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What is wrong with it and how to fix it \(newline\-separated\), optionally with a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlSslCaCertBadFileException.CurlSslCaCertBadFileException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

Example: `throw new CurlSslCaCertBadFileException("ca.pem", "--cacert names \"ca.pem\", which does not exist", command);`

### See Also
- [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')