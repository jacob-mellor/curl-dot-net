#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException')

## CurlUnsupportedProtocolException\(string, string\) Constructor

Initializes a new instance of the [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException') class\.

```csharp
public CurlUnsupportedProtocolException(string protocol, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string).protocol'></a>

`protocol` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The protocol that is not supported \(e\.g\., "gopher", "telnet"\)\.

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.