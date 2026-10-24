#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException')

## CurlCouldntResolveProxyException\.ProxyHost Property

The proxy host name that could not be resolved, as given with `-x` or in a proxy environment variable\.

```csharp
public string ProxyHost { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A host name such as `proxy.corp.local`\.

### Remarks

Example: `Console.WriteLine($"https_proxy={Environment.GetEnvironmentVariable("https_proxy")}");` shows where it came from.

### See Also
- [ProxyHost](CurlDotNet.Exceptions.CurlProxyException.ProxyHost.md 'CurlDotNet\.Exceptions\.CurlProxyException\.ProxyHost')