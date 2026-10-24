#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Signer](CurlDotNet.Core.AwsSigV4Signer.md 'CurlDotNet\.Core\.AwsSigV4Signer')

## AwsSigV4Signer\.DeriveSigningKey\(string, string, string, string, string\) Method

Derives the signing key using the HMAC\-SHA256 chain\.

```csharp
private static byte[] DeriveSigningKey(string secretKey, string dateStamp, string region, string service, string provider1);
```
#### Parameters

<a name='CurlDotNet.Core.AwsSigV4Signer.DeriveSigningKey(string,string,string,string,string).secretKey'></a>

`secretKey` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.AwsSigV4Signer.DeriveSigningKey(string,string,string,string,string).dateStamp'></a>

`dateStamp` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.AwsSigV4Signer.DeriveSigningKey(string,string,string,string,string).region'></a>

`region` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.AwsSigV4Signer.DeriveSigningKey(string,string,string,string,string).service'></a>

`service` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.AwsSigV4Signer.DeriveSigningKey(string,string,string,string,string).provider1'></a>

`provider1` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

### Remarks
The derivation follows this chain:

```csharp
kDate    = HMAC("PROVIDER4" + secretKey, dateStamp)
kRegion  = HMAC(kDate, region)
kService = HMAC(kRegion, service)
kSigning = HMAC(kService, "provider4_request")
```