#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')

## AwsSigV4Config\.Service Property

The service in the credential scope, such as `s3`, `execute-api` or `dynamodb`\. The fourth
part of the `--aws-sigv4` value; when empty it is taken from the host name\.

```csharp
public string Service { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An AWS service code, or empty to use the first label of the host
            \(`dynamodb.us-east-1.amazonaws.com` gives `dynamodb`\)\.

### Remarks

Set it explicitly whenever the first host label is not the service: API Gateway URLs such as
            `abc123.execute-api.us-east-1.amazonaws.com` need `execute-api`, and bucket-style S3 URLs such
            as `my-bucket.s3.amazonaws.com` need `s3`.

Example: `--aws-sigv4 "aws:amz:us-east-1:execute-api"`.

### See Also
- [Region](CurlDotNet.Core.AwsSigV4Config.Region.md 'CurlDotNet\.Core\.AwsSigV4Config\.Region')