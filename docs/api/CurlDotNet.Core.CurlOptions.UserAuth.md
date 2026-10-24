#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.UserAuth Property

Same setting as [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials') \(curl's `-u user:password`\) under an alternative name;
reading or writing either property changes both\.

```csharp
public System.Net.NetworkCredential UserAuth { get; set; }
```

#### Property Value
[System\.Net\.NetworkCredential](https://learn.microsoft.com/en-us/dotnet/api/system.net.networkcredential 'System\.Net\.NetworkCredential')  
A [System\.Net\.NetworkCredential](https://learn.microsoft.com/en-us/dotnet/api/system.net.networkcredential 'System\.Net\.NetworkCredential'), or null\.

### Remarks

Example: `options.UserAuth = new NetworkCredential("ada", "s3cret");` is `curl -u ada:s3cret`.
            See [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials') for how the credentials are sent.

### See Also
- [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials')