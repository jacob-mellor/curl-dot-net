#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.AwsSigV4 Property

Signs the request with AWS Signature Version 4 \(S3, API Gateway, other AWS services\), like curl's
`--aws-sigv4 provider1[:provider2[:region[:service]]]`\.

```csharp
public CurlDotNet.Core.AwsSigV4Config? AwsSigV4 { get; set; }
```

#### Property Value
[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')  
The signing configuration, or null \(default\) for no signing\.

### Example

```csharp
// Equivalent to: curl --aws-sigv4 "aws:amz:us-east-1:s3" --user "AKID:SECRET" https://s3.amazonaws.com/bucket
var result = await Curl.ExecuteAsync("curl --aws-sigv4 \"aws:amz:us-east-1:s3\" -u \"AKID:SECRET\" https://s3.amazonaws.com/bucket");
```

### Remarks

Credentials come from [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials') (`-u ACCESS_KEY:SECRET_KEY`); without them nothing is signed.
            The signature covers the method, path, query, signed headers and a hash of the body, so it is computed after the
            body is set. [OAuth2Bearer](CurlDotNet.Core.CurlOptions.OAuth2Bearer.md 'CurlDotNet\.Core\.CurlOptions\.OAuth2Bearer') takes precedence. The signature is dropped on a redirect to another host.

Common values: `"aws:amz:us-east-1:s3"` (S3 in us-east-1), `"aws:amz:eu-west-1:execute-api"`
            (API Gateway), `"gcp:goog"` (Google Cloud). curl manual: https://curl.se/libcurl/c/CURLOPT_AWS_SIGV4.html

Example: `curl --aws-sigv4 "aws:amz:us-east-1:s3" -u AKID:SECRET https://my-bucket.s3.amazonaws.com/key.txt`
            as a command string: `await Curl.ExecuteAsync("curl --aws-sigv4 \"aws:amz:us-east-1:s3\" -u AKID:SECRET https://my-bucket.s3.amazonaws.com/key.txt")`.

### See Also
- [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials')
- [AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')