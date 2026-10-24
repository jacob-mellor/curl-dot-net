#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')

## CurlCouldntResolveHostException\.Hostname Property

The host name from the URL that could not be resolved, such as `api.exmaple.com`\.

```csharp
public string Hostname { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The host part of the URL, without scheme or port\.

### Remarks

Example: `Dns.GetHostAddresses(ex.Hostname)` reproduces the lookup outside CurlDotNet.

### See Also
- [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException')