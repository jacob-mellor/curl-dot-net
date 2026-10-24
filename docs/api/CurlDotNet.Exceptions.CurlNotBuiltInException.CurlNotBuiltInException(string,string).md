#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException')

## CurlNotBuiltInException\(string, string\) Constructor

Initializes a new instance of the [CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException') class\.

```csharp
public CurlNotBuiltInException(string feature, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).feature'></a>

`feature` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The feature that is not available in this build\.

<a name='CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.