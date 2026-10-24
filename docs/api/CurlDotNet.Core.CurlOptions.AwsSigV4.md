#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.AwsSigV4 Property

AWS Signature Version 4 configuration for authenticating requests to AWS services\.
Set by the `--aws-sigv4` flag\.

```csharp
public CurlDotNet.Core.AwsSigV4Config? AwsSigV4 { get; set; }
```

#### Property Value
[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')

### Example

```csharp
// Equivalent to: curl --aws-sigv4 "aws:amz:us-east-1:s3" --user "AKID:SECRET" https://s3.amazonaws.com/bucket
var result = await Curl.RunAsync("curl --aws-sigv4 \"aws:amz:us-east-1:s3\" -u \"AKID:SECRET\" https://s3.amazonaws.com/bucket");
```

### Remarks

When set, the request will be signed using the AWS Signature Version 4 algorithm.
            Credentials must be provided via the [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials') property (`--user ACCESS_KEY:SECRET_KEY`).

The config format is `provider1[:provider2[:region[:service]]]`.

Common values:
- `"aws:amz:us-east-1:s3"` — AWS S3 in us-east-1
- `"aws:amz:eu-west-1:execute-api"` — AWS API Gateway in eu-west-1
- `"gcp:goog"` — Google Cloud Platform

See: [https://curl\.se/libcurl/c/CURLOPT\_AWS\_SIGV4\.html](https://curl.se/libcurl/c/CURLOPT_AWS_SIGV4.html 'https://curl\.se/libcurl/c/CURLOPT\_AWS\_SIGV4\.html')