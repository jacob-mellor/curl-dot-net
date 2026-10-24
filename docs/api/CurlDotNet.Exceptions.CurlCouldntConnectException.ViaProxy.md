#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')

## CurlCouldntConnectException\.ViaProxy Property

True when the failed connection was to a proxy rather than to the URL's host, so the proxy setting \(not the
server\) needs fixing\.

```csharp
public bool ViaProxy { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for proxy connections \(`-x`, `--socks5`, proxy environment variables\); otherwise false\.

### Remarks

Example: `catch (CurlCouldntConnectException ex) when (ex.ViaProxy) { /* try without the proxy: --noproxy '*' */ }`

### See Also
- [CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException')