#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Expect100TimeoutSeconds Property

Seconds to wait for the server's `100 Continue` before sending a request body anyway, like curl's
`--expect100-timeout SECONDS` \(decimals allowed\)\.

```csharp
public System.Nullable<double> Expect100TimeoutSeconds { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
Seconds, or null \(default\), which keeps HttpClient's default of 1 second\.

### Remarks

Only matters for requests that send `Expect: 100-continue` (large uploads). Setting it makes the request use a
            dedicated connection. Example: `curl --expect100-timeout 0.5 -T big.bin URL` is
            `new CurlOptions { Url = url, UploadFile = "big.bin", BinaryData = bytes, Method = "PUT", Expect100TimeoutSeconds = 0.5 }`.

### See Also
- [UploadFile](CurlDotNet.Core.CurlOptions.UploadFile.md 'CurlDotNet\.Core\.CurlOptions\.UploadFile')
- [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime')