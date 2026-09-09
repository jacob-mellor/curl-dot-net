#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## IProtocolHandler Interface

Interface for protocol\-specific handlers \(HTTP, FTP, FILE, etc\.\)\.

```csharp
internal interface IProtocolHandler
```

Derived  
&#8627; [FileHandler](CurlDotNet.Core.FileHandler.md 'CurlDotNet\.Core\.FileHandler')  
&#8627; [FtpHandler](CurlDotNet.Core.FtpHandler.md 'CurlDotNet\.Core\.FtpHandler')  
&#8627; [HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlOptions, CancellationToken\)](CurlDotNet.Core.IProtocolHandler.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).md 'CurlDotNet\.Core\.IProtocolHandler\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)') | Execute a request with the given options\. |
| [SupportsProtocol\(string\)](CurlDotNet.Core.IProtocolHandler.SupportsProtocol(string).md 'CurlDotNet\.Core\.IProtocolHandler\.SupportsProtocol\(string\)') | Check if this handler supports the given protocol\. |
