#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlInterfaceFailedException](CurlDotNet.Exceptions.CurlInterfaceFailedException.md 'CurlDotNet\.Exceptions\.CurlInterfaceFailedException')

## CurlInterfaceFailedException\.InterfaceName Property

The interface name or local address from `--interface` that could not be bound, such as `eth1` or
`10.0.0.5`\.

```csharp
public string InterfaceName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The value as given in the command\.

### Remarks

Compare it with `NetworkInterface.GetAllNetworkInterfaces()`.

### See Also
- [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command')