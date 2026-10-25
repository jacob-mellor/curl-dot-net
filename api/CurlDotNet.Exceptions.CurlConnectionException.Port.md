#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')

## CurlConnectionException\.Port Property

The TCP port that was tried, or null when the URL used the scheme's default \(80 for http, 443 for https,
21 for ftp\)\.

```csharp
public System.Nullable<int> Port { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
A port number from 1 to 65535, or null for the default port\.

### Remarks

Non-standard ports such as 8443 are often blocked by corporate firewalls; try the service on 443 first.

### See Also
- [Host](CurlDotNet.Exceptions.CurlConnectionException.Host.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Host')