#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')

## CurlConnectionException\.Host Property

Gets the host that could not be connected to\.

```csharp
public string Host { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The hostname or IP address that failed to connect\.

### Remarks

This may be a hostname (e.g., "api.example.com") or IP address (e.g., "192.168.1.1").

AI-Usage: Use this to implement host-specific retry or fallback logic.