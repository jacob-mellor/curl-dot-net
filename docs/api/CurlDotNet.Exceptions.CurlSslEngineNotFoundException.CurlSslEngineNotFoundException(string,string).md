#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslEngineNotFoundException](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException')

## CurlSslEngineNotFoundException\(string, string\) Constructor

Initializes a new instance of the [CurlSslEngineNotFoundException](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException') class\.

```csharp
public CurlSslEngineNotFoundException(string engine, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslEngineNotFoundException.CurlSslEngineNotFoundException(string,string).engine'></a>

`engine` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The SSL engine that was not found\.

<a name='CurlDotNet.Exceptions.CurlSslEngineNotFoundException.CurlSslEngineNotFoundException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.