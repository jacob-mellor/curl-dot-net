#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')

## CurlCouldntResolveHostException\(string, string\) Constructor

Initializes a new instance of the [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException') class\.

```csharp
public CurlCouldntResolveHostException(string hostname, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string).hostname'></a>

`hostname` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The hostname that could not be resolved\.

<a name='CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.