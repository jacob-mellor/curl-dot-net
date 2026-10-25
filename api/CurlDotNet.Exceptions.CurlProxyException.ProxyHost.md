#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')

## CurlProxyException\.ProxyHost Property

The proxy's host name or IP address, from `-x/--proxy` or a proxy environment variable\.

```csharp
public string ProxyHost { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A host such as `proxy.corp.local`; also available as [Host](CurlDotNet.Exceptions.CurlConnectionException.Host.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Host')\.

### Remarks

Compare it with `Environment.GetEnvironmentVariable("https_proxy")` to find out where it came from.

### See Also
- [ProxyPort](CurlDotNet.Exceptions.CurlProxyException.ProxyPort.md 'CurlDotNet\.Exceptions\.CurlProxyException\.ProxyPort')