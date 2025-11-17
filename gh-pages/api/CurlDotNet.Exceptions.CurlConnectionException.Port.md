#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')

## CurlConnectionException\.Port Property

Gets the port number that was attempted\.

```csharp
public System.Nullable<int> Port { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
The TCP port number, or null if using the default port for the protocol\.

### Remarks

Default ports: HTTP=80, HTTPS=443, FTP=21, FTPS=990.

AI-Usage: Check if non-standard ports might be blocked by firewalls.