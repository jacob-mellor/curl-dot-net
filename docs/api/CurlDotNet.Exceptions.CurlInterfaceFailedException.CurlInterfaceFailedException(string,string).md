#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlInterfaceFailedException](CurlDotNet.Exceptions.CurlInterfaceFailedException.md 'CurlDotNet\.Exceptions\.CurlInterfaceFailedException')

## CurlInterfaceFailedException\(string, string\) Constructor

Creates the error \(exit code 45\) for an interface that cannot be used, with advice to pick an existing one\.

```csharp
public CurlInterfaceFailedException(string interfaceName, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlInterfaceFailedException.CurlInterfaceFailedException(string,string).interfaceName'></a>

`interfaceName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name or address of the network interface that failed\.

<a name='CurlDotNet.Exceptions.CurlInterfaceFailedException.CurlInterfaceFailedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Interface failed: NAME`.

### See Also
- [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')