#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.RemoteHeaderName Property

Names the `-O` output file after the server's `Content-Disposition: attachment; filename=...` header,
like curl's `-J`/`--remote-header-name`\.

```csharp
public bool RemoteHeaderName { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to use the server's file name\. Default: false\. Needs [UseRemoteFileName](CurlDotNet.Core.CurlOptions.UseRemoteFileName.md 'CurlDotNet\.Core\.CurlOptions\.UseRemoteFileName')\.

### Remarks

Directory parts of the suggested name are stripped. When the header is missing, the name comes from the (final) URL as
            with plain `-O`. An existing file is never overwritten: the transfer fails with
            [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException') (exit 23) instead.

Example: `curl -OJ https://example.com/download?id=42` is
            `new CurlOptions { Url = url, UseRemoteFileName = true, RemoteHeaderName = true }`.

### See Also
- [UseRemoteFileName](CurlDotNet.Core.CurlOptions.UseRemoteFileName.md 'CurlDotNet\.Core\.CurlOptions\.UseRemoteFileName')
- [OutputDir](CurlDotNet.Core.CurlOptions.OutputDir.md 'CurlDotNet\.Core\.CurlOptions\.OutputDir')