#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.BinaryMagicNumbers Field

Well\-known binary file signatures\. Servers frequently serve binary files under
text/\* or application/xml Content\-Types \(misconfigured MIME maps default unknown
extensions to text/plain\); real curl is immune because it never decodes bytes\.
Sniffing the payload lets us match that behavior\.

```csharp
private static readonly byte[][] BinaryMagicNumbers;
```

#### Field Value
[System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')