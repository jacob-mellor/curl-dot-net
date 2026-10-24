#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlReceiveErrorException](CurlDotNet.Exceptions.CurlReceiveErrorException.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException')

## CurlReceiveErrorException Constructors

| Overloads | |
| :--- | :--- |
| [CurlReceiveErrorException\(string, string\)](CurlDotNet.Exceptions.CurlReceiveErrorException.#ctor.md#CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string) 'CurlDotNet\.Exceptions\.CurlReceiveErrorException\.CurlReceiveErrorException\(string, string\)') | Creates the receive error \(exit code 56\), prefixing `Receive error:` to the explanation of what broke\. |
| [CurlReceiveErrorException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlReceiveErrorException.#ctor.md#CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlReceiveErrorException\.CurlReceiveErrorException\(string, string, System\.Exception\)') | Creates the receive error \(exit code 56\) and keeps the socket, I/O or [System\.Net\.Http\.HttpRequestException](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestexception 'System\.Net\.Http\.HttpRequestException') that broke the transfer as [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string)'></a>

## CurlReceiveErrorException\(string, string\) Constructor

Creates the receive error \(exit code 56\), prefixing `Receive error:` to the explanation of what broke\.

```csharp
public CurlReceiveErrorException(string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What broke while receiving, naming the host; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlReceiveErrorException.md#CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string).message 'CurlDotNet\.Exceptions\.CurlReceiveErrorException\.CurlReceiveErrorException\(string, string\)\.message') has no fix, the suggestion recommends a retry and `-v`.

### See Also
- [CurlSendErrorException](CurlDotNet.Exceptions.CurlSendErrorException.md 'CurlDotNet\.Exceptions\.CurlSendErrorException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string,System.Exception)'></a>

## CurlReceiveErrorException\(string, string, Exception\) Constructor

Creates the receive error \(exit code 56\) and keeps the socket, I/O or [System\.Net\.Http\.HttpRequestException](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestexception 'System\.Net\.Http\.HttpRequestException')
that broke the transfer as [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\.

```csharp
public CurlReceiveErrorException(string message, string? command, System.Exception? innerException);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What broke while receiving, naming the host and the inner cause in words; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

<a name='CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The \.NET exception that broke the transfer, or null\.

### Remarks

Example: `catch (IOException io) { throw new CurlReceiveErrorException($"the connection to api.example.com was reset after 1200 bytes ({io.Message}).\nRetry with --retry 3.", command, io); }`

### See Also
- [CurlReceiveErrorException\(string, string\)](CurlDotNet.Exceptions.CurlReceiveErrorException.#ctor.md#CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string) 'CurlDotNet\.Exceptions\.CurlReceiveErrorException\.CurlReceiveErrorException\(string, string\)')