#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')

## AwsSigV4Config\.Provider2 Property

Secondary provider identifier used in header names\.
For AWS this is "amz" \(producing "x\-amz\-date", "x\-amz\-content\-sha256"\)\.
Defaults to [Provider1](CurlDotNet.Core.AwsSigV4Config.Provider1.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider1') if not specified\.

```csharp
public string Provider2 { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

### Example

```csharp
// Produces headers: x-amz-date, x-amz-content-sha256
config.Provider2 = "amz";
```