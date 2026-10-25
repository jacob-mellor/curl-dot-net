#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlWeirdServerReplyException](CurlDotNet.Exceptions.CurlWeirdServerReplyException.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException')

## CurlWeirdServerReplyException\(string, string\) Constructor

Creates the error with a description of what was wrong with the reply, such as "the status line is not
HTTP/1\.x"\.

```csharp
public CurlWeirdServerReplyException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlWeirdServerReplyException.CurlWeirdServerReplyException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What was wrong with the reply, naming the host; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlWeirdServerReplyException.CurlWeirdServerReplyException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlWeirdServerReplyException.CurlWeirdServerReplyException(string,string).md#CurlDotNet.Exceptions.CurlWeirdServerReplyException.CurlWeirdServerReplyException(string,string).message 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException\.CurlWeirdServerReplyException\(string, string\)\.message') has no fix, the suggestion tells the reader to check the port and scheme and
            to inspect the reply with `-v`. Example: `throw new CurlWeirdServerReplyException($"Weird server reply from {host}: two Content-Length headers.", command);`

### See Also
- [CurlReceiveErrorException](CurlDotNet.Exceptions.CurlReceiveErrorException.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException')