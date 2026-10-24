#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlInterfaceFailedException](CurlDotNet.Exceptions.CurlInterfaceFailedException.md 'CurlDotNet\.Exceptions\.CurlInterfaceFailedException')

## CurlInterfaceFailedException\(string, string\) Constructor

Initializes a new instance of the [CurlInterfaceFailedException](CurlDotNet.Exceptions.CurlInterfaceFailedException.md 'CurlDotNet\.Exceptions\.CurlInterfaceFailedException') class\.

```csharp
public CurlInterfaceFailedException(string interfaceName, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlInterfaceFailedException.CurlInterfaceFailedException(string,string).interfaceName'></a>

`interfaceName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the network interface that failed\.

<a name='CurlDotNet.Exceptions.CurlInterfaceFailedException.CurlInterfaceFailedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.