#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.LocationTrusted Property

Follows redirects and keeps sending [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials') \(and bearer tokens\) when a redirect goes to another
host, like curl's `--location-trusted`; only use it with hosts you trust\.

```csharp
public bool LocationTrusted { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to trust redirect targets\. Default: false: credentials are only sent to the original host\.

### Remarks

The parser also sets [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation'). Example: `curl --location-trusted -u ada:s3cret https://example.com/api` is
            `new CurlOptions { Url = url, FollowLocation = true, LocationTrusted = true, Credentials = new NetworkCredential("ada", "s3cret") }`.

### See Also
- [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation')
- [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials')