#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslConnectErrorException](CurlDotNet.Exceptions.CurlSslConnectErrorException.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException')

## CurlSslConnectErrorException\(string, string\) Constructor

Creates the TLS handshake error \(exit code 35\), prefixing `SSL connect error:` to the TLS layer's
explanation\.

```csharp
public CurlSslConnectErrorException(string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslConnectErrorException.CurlSslConnectErrorException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the TLS layer reported, naming the host; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlSslConnectErrorException.CurlSslConnectErrorException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlSslConnectErrorException.CurlSslConnectErrorException(string,string).md#CurlDotNet.Exceptions.CurlSslConnectErrorException.CurlSslConnectErrorException(string,string).message 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException\.CurlSslConnectErrorException\(string, string\)\.message') has no fix, the suggestion covers ports, TLS versions and client certificates.

### See Also
- [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')