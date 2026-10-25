#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')

## CurlProxyException\.ProxyPort Property

The proxy's port, such as 3128 or 8080, or null when the proxy URL did not give one \(curl then uses 1080\)\.

```csharp
public System.Nullable<int> ProxyPort { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
A TCP port number, or null\.

### Remarks

Also available as [Port](CurlDotNet.Exceptions.CurlConnectionException.Port.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Port').

### See Also
- [ProxyHost](CurlDotNet.Exceptions.CurlProxyException.ProxyHost.md 'CurlDotNet\.Exceptions\.CurlProxyException\.ProxyHost')