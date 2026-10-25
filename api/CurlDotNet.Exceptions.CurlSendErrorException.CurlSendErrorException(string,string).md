#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSendErrorException](CurlDotNet.Exceptions.CurlSendErrorException.md 'CurlDotNet\.Exceptions\.CurlSendErrorException')

## CurlSendErrorException\(string, string\) Constructor

Creates the send error \(exit code 55\), prefixing `Send error:` to the socket layer's explanation\.

```csharp
public CurlSendErrorException(string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSendErrorException.CurlSendErrorException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the network layer reported, naming the host; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlSendErrorException.CurlSendErrorException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlSendErrorException.CurlSendErrorException(string,string).md#CurlDotNet.Exceptions.CurlSendErrorException.CurlSendErrorException(string,string).message 'CurlDotNet\.Exceptions\.CurlSendErrorException\.CurlSendErrorException\(string, string\)\.message') has no fix, the suggestion recommends a retry and checking upload limits.

### See Also
- [CurlReceiveErrorException](CurlDotNet.Exceptions.CurlReceiveErrorException.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException')