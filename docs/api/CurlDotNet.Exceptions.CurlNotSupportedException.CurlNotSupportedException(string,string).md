#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlNotSupportedException](CurlDotNet.Exceptions.CurlNotSupportedException.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException')

## CurlNotSupportedException\(string, string\) Constructor

Initializes a new instance of the [CurlNotSupportedException](CurlDotNet.Exceptions.CurlNotSupportedException.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException') class

```csharp
public CurlNotSupportedException(string feature, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlNotSupportedException.CurlNotSupportedException(string,string).feature'></a>

`feature` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the unsupported feature

<a name='CurlDotNet.Exceptions.CurlNotSupportedException.CurlNotSupportedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed