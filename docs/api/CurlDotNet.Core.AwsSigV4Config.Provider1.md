#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')

## AwsSigV4Config\.Provider1 Property

The provider that names the signing algorithm and key prefix: `aws` gives `AWS4-HMAC-SHA256`,
`gcp` gives `GCP4-HMAC-SHA256`\. The first part of the `--aws-sigv4` value; required\.

```csharp
public string Provider1 { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A lowercase identifier such as `aws` or `gcp`; empty only for an instance you built
            yourself without setting it \(signing then produces an invalid signature\)\.

### Remarks

It is upper-cased for the algorithm name (`AWS4-HMAC-SHA256`), prefixed to the secret when
            deriving the signing key (`AWS4` + secret) and lower-cased in the credential scope suffix
            (`aws4_request`).

Example: `AwsSigV4Config.Parse("aws:amz").Provider1` is `"aws"`.

### See Also
- [Provider2](CurlDotNet.Core.AwsSigV4Config.Provider2.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider2')
- [Parse\(string\)](CurlDotNet.Core.AwsSigV4Config.Parse(string).md 'CurlDotNet\.Core\.AwsSigV4Config\.Parse\(string\)')