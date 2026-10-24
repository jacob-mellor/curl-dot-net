#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Signer](CurlDotNet.Core.AwsSigV4Signer.md 'CurlDotNet\.Core\.AwsSigV4Signer')

## AwsSigV4Signer\.ExtractService\(Uri\) Method

Attempts to extract the AWS service name from the hostname\.
Handles common patterns like:
- s3.us-east-1.amazonaws.com → s3
- dynamodb.eu-west-1.amazonaws.com → dynamodb
- my-api.execute-api.ap-southeast-2.amazonaws.com → execute-api

```csharp
internal static string ExtractService(System.Uri uri);
```
#### Parameters

<a name='CurlDotNet.Core.AwsSigV4Signer.ExtractService(System.Uri).uri'></a>

`uri` [System\.Uri](https://learn.microsoft.com/en-us/dotnet/api/system.uri 'System\.Uri')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')