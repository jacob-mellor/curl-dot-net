#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## AwsSigV4Signer Class

Implements AWS Signature Version 4 request signing in pure C\#\.

```csharp
internal static class AwsSigV4Signer
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; AwsSigV4Signer

### Remarks

This signer produces the same Authorization header that curl generates
             with `--aws-sigv4`. It supports any SigV4-compatible provider by parameterizing
             the algorithm prefix, header names, and credential scope based on the provider values.

<b>Signing Process (4 steps):</b>
1. Create a canonical request from the HTTP method, URL, headers, and payload hash
2. Create a string-to-sign from the algorithm, timestamp, credential scope, and canonical request hash
3. Derive a signing key using HMAC-SHA256 chain: secret → date → region → service → request type
4. Calculate the signature and build the Authorization header

Specification: [https://docs\.aws\.amazon\.com/general/latest/gr/signature\-version\-4\.html](https://docs.aws.amazon.com/general/latest/gr/signature-version-4.html 'https://docs\.aws\.amazon\.com/general/latest/gr/signature\-version\-4\.html')

| Methods | |
| :--- | :--- |
| [BuildCanonicalQueryString\(string\)](CurlDotNet.Core.AwsSigV4Signer.BuildCanonicalQueryString(string).md 'CurlDotNet\.Core\.AwsSigV4Signer\.BuildCanonicalQueryString\(string\)') | Builds the canonical query string by sorting parameters alphabetically\. |
| [CreateCanonicalRequest\(HttpRequestMessage, string\)](CurlDotNet.Core.AwsSigV4Signer.CreateCanonicalRequest(System.Net.Http.HttpRequestMessage,string).md 'CurlDotNet\.Core\.AwsSigV4Signer\.CreateCanonicalRequest\(System\.Net\.Http\.HttpRequestMessage, string\)') | Creates the canonical request string per the SigV4 specification\. |
| [CreateStringToSign\(string, string, string, string\)](CurlDotNet.Core.AwsSigV4Signer.CreateStringToSign(string,string,string,string).md 'CurlDotNet\.Core\.AwsSigV4Signer\.CreateStringToSign\(string, string, string, string\)') | Creates the string to sign from the algorithm, timestamp, credential scope, and canonical request hash\. |
| [DeriveSigningKey\(string, string, string, string, string\)](CurlDotNet.Core.AwsSigV4Signer.DeriveSigningKey(string,string,string,string,string).md 'CurlDotNet\.Core\.AwsSigV4Signer\.DeriveSigningKey\(string, string, string, string, string\)') | Derives the signing key using the HMAC\-SHA256 chain\. |
| [ExtractRegion\(Uri\)](CurlDotNet.Core.AwsSigV4Signer.ExtractRegion(System.Uri).md 'CurlDotNet\.Core\.AwsSigV4Signer\.ExtractRegion\(System\.Uri\)') | Attempts to extract the AWS region from the hostname\. Handles common patterns like: \.\.\. |
| [ExtractService\(Uri\)](CurlDotNet.Core.AwsSigV4Signer.ExtractService(System.Uri).md 'CurlDotNet\.Core\.AwsSigV4Signer\.ExtractService\(System\.Uri\)') | Attempts to extract the AWS service name from the hostname\. Handles common patterns like: \.\.\. |
| [HashPayload\(HttpRequestMessage\)](CurlDotNet.Core.AwsSigV4Signer.HashPayload(System.Net.Http.HttpRequestMessage).md 'CurlDotNet\.Core\.AwsSigV4Signer\.HashPayload\(System\.Net\.Http\.HttpRequestMessage\)') | Hashes the request payload \(body\) using SHA\-256\. Returns the hex\-encoded hash, or the hash of an empty string if no body\. |
| [HexEncode\(byte\[\]\)](CurlDotNet.Core.AwsSigV4Signer.HexEncode(byte[]).md 'CurlDotNet\.Core\.AwsSigV4Signer\.HexEncode\(byte\[\]\)') | Hex\-encodes a byte array to a lowercase string\. |
| [HmacSha256\(byte\[\], string\)](CurlDotNet.Core.AwsSigV4Signer.HmacSha256(byte[],string).md 'CurlDotNet\.Core\.AwsSigV4Signer\.HmacSha256\(byte\[\], string\)') | Computes HMAC\-SHA256 of a string using a byte array key\. |
| [IsRegionLike\(string\)](CurlDotNet.Core.AwsSigV4Signer.IsRegionLike(string).md 'CurlDotNet\.Core\.AwsSigV4Signer\.IsRegionLike\(string\)') | Checks whether a hostname part looks like an AWS region identifier\. |
| [Sha256Hash\(string\)](CurlDotNet.Core.AwsSigV4Signer.Sha256Hash(string).md 'CurlDotNet\.Core\.AwsSigV4Signer\.Sha256Hash\(string\)') | Computes SHA\-256 hash of a string\. |
| [Sha256HashBytes\(byte\[\]\)](CurlDotNet.Core.AwsSigV4Signer.Sha256HashBytes(byte[]).md 'CurlDotNet\.Core\.AwsSigV4Signer\.Sha256HashBytes\(byte\[\]\)') | Computes SHA\-256 hash of a byte array\. |
| [SignRequest\(HttpRequestMessage, AwsSigV4Config, string, string, Nullable&lt;DateTime&gt;\)](CurlDotNet.Core.AwsSigV4Signer.SignRequest(System.Net.Http.HttpRequestMessage,CurlDotNet.Core.AwsSigV4Config,string,string,System.Nullable_System.DateTime_).md 'CurlDotNet\.Core\.AwsSigV4Signer\.SignRequest\(System\.Net\.Http\.HttpRequestMessage, CurlDotNet\.Core\.AwsSigV4Config, string, string, System\.Nullable\<System\.DateTime\>\)') | Signs an [System\.Net\.Http\.HttpRequestMessage](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage 'System\.Net\.Http\.HttpRequestMessage') using AWS Signature Version 4\. Adds the required Authorization, date, and content hash headers to the request\. |
