#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlTooLargeException](CurlDotNet.Exceptions.CurlTooLargeException.md 'CurlDotNet\.Exceptions\.CurlTooLargeException')

## CurlTooLargeException\(string, string\) Constructor

Creates the error \(exit code 100\) describing which header limit was exceeded, with advice that the server must
be fixed\.

```csharp
public CurlTooLargeException(string detail, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTooLargeException.CurlTooLargeException(string,string).detail'></a>

`detail` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What exceeded the limit, with the sizes involved\.

<a name='CurlDotNet.Exceptions.CurlTooLargeException.CurlTooLargeException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

Example: `throw new CurlTooLargeException("the header block is 412000 bytes (limit 307200)", command);`

### See Also
- [CurlWeirdServerReplyException](CurlDotNet.Exceptions.CurlWeirdServerReplyException.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException')