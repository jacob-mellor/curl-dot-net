#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Signer](CurlDotNet.Core.AwsSigV4Signer.md 'CurlDotNet\.Core\.AwsSigV4Signer')

## AwsSigV4Signer\.HashPayload\(HttpRequestMessage\) Method

Hashes the request payload \(body\) using SHA\-256\.
Returns the hex\-encoded hash, or the hash of an empty string if no body\.

```csharp
private static string HashPayload(System.Net.Http.HttpRequestMessage request);
```
#### Parameters

<a name='CurlDotNet.Core.AwsSigV4Signer.HashPayload(System.Net.Http.HttpRequestMessage).request'></a>

`request` [System\.Net\.Http\.HttpRequestMessage](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage 'System\.Net\.Http\.HttpRequestMessage')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')