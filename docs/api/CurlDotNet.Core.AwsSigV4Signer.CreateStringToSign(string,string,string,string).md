#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Signer](CurlDotNet.Core.AwsSigV4Signer.md 'CurlDotNet\.Core\.AwsSigV4Signer')

## AwsSigV4Signer\.CreateStringToSign\(string, string, string, string\) Method

Creates the string to sign from the algorithm, timestamp, credential scope, and canonical request hash\.

```csharp
private static string CreateStringToSign(string algorithm, string amzDate, string credentialScope, string canonicalRequest);
```
#### Parameters

<a name='CurlDotNet.Core.AwsSigV4Signer.CreateStringToSign(string,string,string,string).algorithm'></a>

`algorithm` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.AwsSigV4Signer.CreateStringToSign(string,string,string,string).amzDate'></a>

`amzDate` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.AwsSigV4Signer.CreateStringToSign(string,string,string,string).credentialScope'></a>

`credentialScope` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.AwsSigV4Signer.CreateStringToSign(string,string,string,string).canonicalRequest'></a>

`canonicalRequest` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')