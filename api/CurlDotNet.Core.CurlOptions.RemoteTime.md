#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.RemoteTime Property

Gives the downloaded file the server's `Last-Modified` time instead of the current time, like curl's
`-R`/`--remote-time`; useful for mirroring\.

```csharp
public bool RemoteTime { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to copy the server time\. Default: false\. Needs an output file\.

### Remarks

Nothing happens when the response has no usable `Last-Modified` header or the time cannot be set.
            Example: `curl -R -O https://example.com/data.csv` is `new CurlOptions { Url = url, UseRemoteFileName = true, RemoteTime = true }`.

### See Also
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')
- [TimeCondition](CurlDotNet.Core.CurlOptions.TimeCondition.md 'CurlDotNet\.Core\.CurlOptions\.TimeCondition')