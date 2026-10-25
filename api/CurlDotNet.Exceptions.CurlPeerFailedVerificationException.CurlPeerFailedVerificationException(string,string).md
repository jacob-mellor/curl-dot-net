#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')

## CurlPeerFailedVerificationException\(string, string\) Constructor

Creates the certificate\-rejected error \(exit code 60\), prefixing `Peer certificate verification failed:`
to the TLS layer's reason\.

```csharp
public CurlPeerFailedVerificationException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlPeerFailedVerificationException.CurlPeerFailedVerificationException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Why the certificate was rejected, naming the host; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlPeerFailedVerificationException.CurlPeerFailedVerificationException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.CurlPeerFailedVerificationException(string,string).md#CurlDotNet.Exceptions.CurlPeerFailedVerificationException.CurlPeerFailedVerificationException(string,string).message 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException\.CurlPeerFailedVerificationException\(string, string\)\.message') has no fix, the suggestion covers `--cacert`, host names and `-k`.

### See Also
- [CurlSslException\(string, string, string\)](CurlDotNet.Exceptions.CurlSslException.#ctor.md#CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string) 'CurlDotNet\.Exceptions\.CurlSslException\.CurlSslException\(string, string, string\)')