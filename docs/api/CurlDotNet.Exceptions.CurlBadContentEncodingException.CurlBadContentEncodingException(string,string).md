#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlBadContentEncodingException](CurlDotNet.Exceptions.CurlBadContentEncodingException.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException')

## CurlBadContentEncodingException\(string, string\) Constructor

Initializes a new instance of the [CurlBadContentEncodingException](CurlDotNet.Exceptions.CurlBadContentEncodingException.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException') class\.

```csharp
public CurlBadContentEncodingException(string encoding, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string).encoding'></a>

`encoding` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The content encoding that was not recognized or could not be decoded\.

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.