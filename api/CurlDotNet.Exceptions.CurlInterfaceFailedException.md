#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlInterfaceFailedException Class

CURLE\_INTERFACE\_FAILED \(45\) \- Interface failed

```csharp
public class CurlInterfaceFailedException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlInterfaceFailedException
### Constructors

<a name='CurlDotNet.Exceptions.CurlInterfaceFailedException.CurlInterfaceFailedException(string,string)'></a>

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
### Properties

<a name='CurlDotNet.Exceptions.CurlInterfaceFailedException.InterfaceName'></a>

## CurlInterfaceFailedException\.InterfaceName Property

Gets the name of the network interface that failed

```csharp
public string InterfaceName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')