#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')

## CurlConnectionException\.Host Property

The host name or IP address that could not be reached, exactly as it appears in the URL \(or the proxy's
host for [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')\)\.

```csharp
public string Host { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A name such as `api.example.com` or an address such as `192.168.1.10`; null when unknown\.

### Remarks

Use it for per-host fallback: `catch (CurlConnectionException ex) when (ex.Host == primary) { url = backupUrl; }`

### See Also
- [Port](CurlDotNet.Exceptions.CurlConnectionException.Port.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Port')