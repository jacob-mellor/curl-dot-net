#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[IProtocolHandler](CurlDotNet.Core.IProtocolHandler.md 'CurlDotNet\.Core\.IProtocolHandler')

## IProtocolHandler\.SupportsProtocol\(string\) Method

Check if this handler supports the given protocol\.

```csharp
bool SupportsProtocol(string protocol);
```
#### Parameters

<a name='CurlDotNet.Core.IProtocolHandler.SupportsProtocol(string).protocol'></a>

`protocol` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Protocol scheme \(http, ftp, file, etc\.\)

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True if supported