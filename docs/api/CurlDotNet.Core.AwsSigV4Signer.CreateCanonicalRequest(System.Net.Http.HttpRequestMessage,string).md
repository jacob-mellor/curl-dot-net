#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Signer](CurlDotNet.Core.AwsSigV4Signer.md 'CurlDotNet\.Core\.AwsSigV4Signer')

## AwsSigV4Signer\.CreateCanonicalRequest\(HttpRequestMessage, string\) Method

Creates the canonical request string per the SigV4 specification\.

```csharp
private static (string CanonicalRequest,string SignedHeaders) CreateCanonicalRequest(System.Net.Http.HttpRequestMessage request, string payloadHash);
```
#### Parameters

<a name='CurlDotNet.Core.AwsSigV4Signer.CreateCanonicalRequest(System.Net.Http.HttpRequestMessage,string).request'></a>

`request` [System\.Net\.Http\.HttpRequestMessage](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage 'System\.Net\.Http\.HttpRequestMessage')

<a name='CurlDotNet.Core.AwsSigV4Signer.CreateCanonicalRequest(System.Net.Http.HttpRequestMessage,string).payloadHash'></a>

`payloadHash` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')  
A tuple of \(canonical request string, signed headers string\)\.