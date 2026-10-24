#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFileCouldntReadException](CurlDotNet.Exceptions.CurlFileCouldntReadException.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException')

## CurlFileCouldntReadException\(string, string\) Constructor

Initializes a new instance of the [CurlFileCouldntReadException](CurlDotNet.Exceptions.CurlFileCouldntReadException.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException') class\.

```csharp
public CurlFileCouldntReadException(string filePath, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path that could not be read\.

<a name='CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.