#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Signer](CurlDotNet.Core.AwsSigV4Signer.md 'CurlDotNet\.Core\.AwsSigV4Signer')

## AwsSigV4Signer\.ExtractRegion\(Uri\) Method

Attempts to extract the AWS region from the hostname\.
Handles common patterns like:
- s3.us-east-1.amazonaws.com → us-east-1
- dynamodb.eu-west-1.amazonaws.com → eu-west-1
- my-api.execute-api.ap-southeast-2.amazonaws.com → ap-southeast-2

```csharp
internal static string ExtractRegion(System.Uri uri);
```
#### Parameters

<a name='CurlDotNet.Core.AwsSigV4Signer.ExtractRegion(System.Uri).uri'></a>

`uri` [System\.Uri](https://learn.microsoft.com/en-us/dotnet/api/system.uri 'System\.Uri')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')