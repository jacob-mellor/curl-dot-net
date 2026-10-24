#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.KeyType Property

The format of [KeyFile](CurlDotNet.Core.CurlOptions.KeyFile.md 'CurlDotNet\.Core\.CurlOptions\.KeyFile'), from curl's `--key-type`; CurlDotNet reads PEM keys\.

```csharp
public string? KeyType { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
`PEM` \(the default\) or another curl value\. Default: null\.

### Remarks

Only PEM keys are read: `--key-type DER` or `ENG` is recorded in [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions').
            Example: `curl --key k.pem --key-type PEM URL` parses to `KeyType = "PEM"`.

### See Also
- [KeyFile](CurlDotNet.Core.CurlOptions.KeyFile.md 'CurlDotNet\.Core\.CurlOptions\.KeyFile')