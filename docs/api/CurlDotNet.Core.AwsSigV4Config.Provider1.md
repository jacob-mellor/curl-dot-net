#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')

## AwsSigV4Config\.Provider1 Property

Primary provider identifier used in the signing algorithm name,
signing key prefix, and credential scope suffix\.
For AWS this is "aws", for GCP this is "gcp"\.

```csharp
public string Provider1 { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

### Example

```csharp
// For AWS: algorithm becomes "AWS4-HMAC-SHA256"
// For GCP: algorithm becomes "GCP4-HMAC-SHA256"
config.Provider1 = "aws";
```