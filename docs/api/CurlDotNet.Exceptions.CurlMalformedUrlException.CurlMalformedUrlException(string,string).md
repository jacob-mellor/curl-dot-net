#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException')

## CurlMalformedUrlException\(string, string\) Constructor

Initializes a new instance of the [CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException') class\.

```csharp
public CurlMalformedUrlException(string url, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The malformed URL that caused the error\.

<a name='CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.