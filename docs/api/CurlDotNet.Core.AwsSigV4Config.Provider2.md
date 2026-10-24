#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')

## AwsSigV4Config\.Provider2 Property

The provider that names the signing headers: `amz` gives `x-amz-date` and
`x-amz-content-sha256`\. The second part of the `--aws-sigv4` value; defaults to [Provider1](CurlDotNet.Core.AwsSigV4Config.Provider1.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider1')\.

```csharp
public string Provider2 { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A lowercase identifier such as `amz` \(AWS\) or `goog` \(GCP\)\.

### Remarks

AWS needs `aws:amz`: with only `aws` the headers would be `x-aws-date`, which AWS rejects.

Example: `AwsSigV4Config.Parse("aws").Provider2` is `"aws"`; `Parse("aws:amz").Provider2` is `"amz"`.

### See Also
- [Provider1](CurlDotNet.Core.AwsSigV4Config.Provider1.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider1')