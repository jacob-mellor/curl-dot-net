#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')

## CurlSslException Constructors

| Overloads | |
| :--- | :--- |
| [CurlSslException\(string, string, string\)](CurlDotNet.Exceptions.CurlSslException.#ctor.md#CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string) 'CurlDotNet\.Exceptions\.CurlSslException\.CurlSslException\(string, string, string\)') | Creates a certificate\-verification failure \(exit code 60\) for a server certificate this machine does not trust, with the TLS layer's detail\. |
| [CurlSslException\(string, string, string, int\)](CurlDotNet.Exceptions.CurlSslException.#ctor.md#CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string,int) 'CurlDotNet\.Exceptions\.CurlSslException\.CurlSslException\(string, string, string, int\)') | Creates a TLS failure with a specific curl exit code, such as 35 \(CURLE\_SSL\_CONNECT\_ERROR\) for a handshake that failed for a reason other than certificate verification\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string)'></a>

## CurlSslException\(string, string, string\) Constructor

Creates a certificate\-verification failure \(exit code 60\) for a server certificate this machine does not
trust, with the TLS layer's detail\.

```csharp
public CurlSslException(string message, string? certError=null, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What was rejected, naming the host; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).certError'></a>

`certError` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The TLS layer's description of the certificate problem, or null\.

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlSslException.md#CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).message 'CurlDotNet\.Exceptions\.CurlSslException\.CurlSslException\(string, string, string\)\.message') has no fix, the suggestion explains `--cacert`, fixing the server and
            `-k` for testing. Example: `throw new CurlSslException("Certificate for intranet.local is self-signed.", "UntrustedRoot", command);`

### See Also
- [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string,int)'></a>

## CurlSslException\(string, string, string, int\) Constructor

Creates a TLS failure with a specific curl exit code, such as 35 \(CURLE\_SSL\_CONNECT\_ERROR\) for a handshake
that failed for a reason other than certificate verification\.

```csharp
protected CurlSslException(string message, string? certError, string? command, int curlErrorCode);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string,int).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What happened and how to fix it, separated by newlines\.

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string,int).certError'></a>

`certError` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Details about the certificate error, if any, or null\.

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string,int).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string,int).curlErrorCode'></a>

`curlErrorCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The curl exit code, for example 35, 53, 54, 58, 59, 60 or 64\.

### Remarks

Used by the derived TLS types to set [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode').

### See Also
- [CurlSslConnectErrorException](CurlDotNet.Exceptions.CurlSslConnectErrorException.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException')