#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')

## AwsSigV4Config\.Parse\(string\) Method

Parses a \-\-aws\-sigv4 parameter value into an [AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')\.

```csharp
public static CurlDotNet.Core.AwsSigV4Config Parse(string value);
```
#### Parameters

<a name='CurlDotNet.Core.AwsSigV4Config.Parse(string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The parameter value in format `provider1[:provider2[:region[:service]]]`\.

#### Returns
[AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')  
A configured [AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config') instance\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
Thrown when the value is null or empty\.

### Example

```csharp
var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");
// config.Provider1 == "aws"
// config.Provider2 == "amz"
// config.Region == "us-east-1"
// config.Service == "s3"
```