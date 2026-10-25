#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')

## AwsSigV4Config\.Region Property

The region in the credential scope, such as `us-east-1` or `eu-west-1`\. The third part of the
`--aws-sigv4` value; when empty it is taken from the host name\.

```csharp
public string Region { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An AWS region code, or empty to detect it: the first host label that looks like a region
            \(`s3.eu-west-2.amazonaws.com` gives `eu-west-2`\), else `us-east-1`\.

### Remarks

A wrong region makes AWS answer 403 with "The request signature we calculated does not match"
            or "credential should be scoped to a valid region"; set it explicitly for custom domains and
            S3-compatible stores.

Example: `--aws-sigv4 "aws:amz:ap-southeast-2:s3"`.

### See Also
- [Service](CurlDotNet.Core.AwsSigV4Config.Service.md 'CurlDotNet\.Core\.AwsSigV4Config\.Service')