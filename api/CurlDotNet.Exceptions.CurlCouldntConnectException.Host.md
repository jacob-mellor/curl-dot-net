#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')

## CurlCouldntConnectException\.Host Property

The host that refused or did not answer: the URL's host, or the proxy's host when [ViaProxy](CurlDotNet.Exceptions.CurlCouldntConnectException.ViaProxy.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.ViaProxy') is
true\.

```csharp
public string Host { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A host name or IP address such as `127.0.0.1`\.

### Remarks

Example: `Console.WriteLine($"Is anything listening on {ex.Host}:{ex.Port}?");`

### See Also
- [Port](CurlDotNet.Exceptions.CurlCouldntConnectException.Port.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.Port')