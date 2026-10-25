#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')

## CurlCouldntConnectException\.Port Property

The TCP port that was tried: the URL's port \(80/443 by default\) or the proxy's port when
[ViaProxy](CurlDotNet.Exceptions.CurlCouldntConnectException.ViaProxy.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.ViaProxy') is true\.

```csharp
public int Port { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
A port number from 1 to 65535\.

### Remarks

A refused connection on a non-standard port usually means the service listens elsewhere.

### See Also
- [Host](CurlDotNet.Exceptions.CurlCouldntConnectException.Host.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException\.Host')