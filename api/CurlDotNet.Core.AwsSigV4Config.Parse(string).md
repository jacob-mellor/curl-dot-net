#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')

## AwsSigV4Config\.Parse\(string\) Method

Reads a `--aws-sigv4` value such as `aws:amz:us-east-1:s3` into its provider, header provider,
region and service parts; missing parts fall back to their defaults\.

```csharp
public static CurlDotNet.Core.AwsSigV4Config Parse(string value);
```
#### Parameters

<a name='CurlDotNet.Core.AwsSigV4Config.Parse(string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The `--aws-sigv4` argument, for example `"aws:amz:us-east-1:s3"` or `"gcp:goog"`\.
            Must contain at least the first provider\.

#### Returns
[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')  
A new configuration whose [Provider1](CurlDotNet.Core.AwsSigV4Config.Provider1.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider1') is always set, [Provider2](CurlDotNet.Core.AwsSigV4Config.Provider2.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider2') defaults to
            it, and [Region](CurlDotNet.Core.AwsSigV4Config.Region.md 'CurlDotNet\.Core\.AwsSigV4Config\.Region')/[Service](CurlDotNet.Core.AwsSigV4Config.Service.md 'CurlDotNet\.Core\.AwsSigV4Config\.Service') are empty when not given \(detected from the URL when signing\)\.

#### Exceptions

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
[value](CurlDotNet.Core.AwsSigV4Config.Parse(string).md#CurlDotNet.Core.AwsSigV4Config.Parse(string).value 'CurlDotNet\.Core\.AwsSigV4Config\.Parse\(string\)\.value') is null, empty or
            whitespace, its first provider is empty \(`:amz`\), it has more than four parts, a part is longer than 64
            characters, or it contains white space \(exit code 2, like curl's "first aws\-sigv4 provider cannot be empty"\)\.
            Fix: pass at least the provider, for example `--aws-sigv4 "aws:amz:us-east-1:s3"`\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/cookbook/auth\-schemes\.md

### Example

```csharp
var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");
// config.Provider1 == "aws", config.Provider2 == "amz", config.Region == "us-east-1", config.Service == "s3"
```

### Remarks

Accepted format: `provider1[:provider2[:region[:service]]]`, colon-separated, with empty parts
            allowed (`aws::us-east-1` keeps [Provider2](CurlDotNet.Core.AwsSigV4Config.Provider2.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider2') equal to [Provider1](CurlDotNet.Core.AwsSigV4Config.Provider1.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider1')). Nothing is
            validated against AWS: a typo in the region only shows up as a 403 from the service.

Example: `var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");` gives
            `Provider1 = "aws"`, `Provider2 = "amz"`, `Region = "us-east-1"`, `Service = "s3"`;
            `AwsSigV4Config.Parse("aws")` gives `Provider2 = "aws"` and empty region and service (detected from the URL).

The command line parser calls this for `--aws-sigv4`. Guide:
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md

### See Also
- [AwsSigV4](CurlDotNet.Core.CurlOptions.AwsSigV4.md 'CurlDotNet\.Core\.CurlOptions\.AwsSigV4')