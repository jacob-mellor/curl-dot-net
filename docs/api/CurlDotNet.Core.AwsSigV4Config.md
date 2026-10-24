#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## AwsSigV4Config Class

Configuration parsed from the \-\-aws\-sigv4 parameter value\.

```csharp
public class AwsSigV4Config
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; AwsSigV4Config

### Remarks

The parameter format is `provider1[:provider2[:region[:service]]]`.

Only `provider1` is required; all other components are optional and will
             be inferred from defaults or the request URL when omitted.

<b>Provider Roles:</b>
- <b>provider1</b> — Used in the algorithm name (uppercased + "4-HMAC-SHA256"),
               the signing key derivation prefix, and the credential scope suffix.
- <b>provider2</b> — Used in the date header name ("x-{provider2}-date"),
               the content hash header, and the security token header. Defaults to provider1 if omitted.

<b>Examples:</b>
- `"aws:amz"` — AWS with standard Amazon header prefix
- `"aws:amz:us-east-1:s3"` — AWS S3 in us-east-1
- `"gcp:goog"` — Google Cloud Platform

See curl documentation: [https://curl\.se/libcurl/c/CURLOPT\_AWS\_SIGV4\.html](https://curl.se/libcurl/c/CURLOPT_AWS_SIGV4.html 'https://curl\.se/libcurl/c/CURLOPT\_AWS\_SIGV4\.html')

| Properties | |
| :--- | :--- |
| [Provider1](CurlDotNet.Core.AwsSigV4Config.Provider1.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider1') | Primary provider identifier used in the signing algorithm name, signing key prefix, and credential scope suffix\. For AWS this is "aws", for GCP this is "gcp"\. |
| [Provider2](CurlDotNet.Core.AwsSigV4Config.Provider2.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider2') | Secondary provider identifier used in header names\. For AWS this is "amz" \(producing "x\-amz\-date", "x\-amz\-content\-sha256"\)\. Defaults to [Provider1](CurlDotNet.Core.AwsSigV4Config.Provider1.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider1') if not specified\. |
| [Region](CurlDotNet.Core.AwsSigV4Config.Region.md 'CurlDotNet\.Core\.AwsSigV4Config\.Region') | AWS region for signing \(e\.g\., "us\-east\-1", "eu\-west\-1"\)\. When empty, the signer attempts to extract the region from the request URL hostname\. |
| [Service](CurlDotNet.Core.AwsSigV4Config.Service.md 'CurlDotNet\.Core\.AwsSigV4Config\.Service') | AWS service name for signing \(e\.g\., "s3", "execute\-api", "dynamodb"\)\. When empty, the signer attempts to extract the service from the request URL hostname\. |

| Methods | |
| :--- | :--- |
| [Parse\(string\)](CurlDotNet.Core.AwsSigV4Config.Parse(string).md 'CurlDotNet\.Core\.AwsSigV4Config\.Parse\(string\)') | Parses a \-\-aws\-sigv4 parameter value into an [AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')\. |
