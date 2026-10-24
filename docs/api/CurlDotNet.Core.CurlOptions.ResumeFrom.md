#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ResumeFrom Property

Resumes a download at a byte offset by sending `Range: bytes=N-`, like curl's `-C`/`--continue-at N`;
`-1` \(curl's `-C -`\) continues from the size of the existing [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')\.

```csharp
public System.Nullable<long> ResumeFrom { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
A byte offset, `-1` for "from the existing file", or null\. Default: null \(no resume\)\. `0` sends no Range\.

### Remarks

A resumed download is appended to [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile'). When the server ignores the range and sends the whole
            document the transfer fails with [CurlRangeErrorException](CurlDotNet.Exceptions.CurlRangeErrorException.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException') (exit 33) rather than
            corrupt the file; a `416` means the file is already complete and is not an error. Resuming an upload
            (with [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data'), [FormData](CurlDotNet.Core.CurlOptions.FormData.md 'CurlDotNet\.Core\.CurlOptions\.FormData') or [UploadFile](CurlDotNet.Core.CurlOptions.UploadFile.md 'CurlDotNet\.Core\.CurlOptions\.UploadFile')) is refused with exit code 43.
            The parser also sets [Range](CurlDotNet.Core.CurlOptions.Range.md 'CurlDotNet\.Core\.CurlOptions\.Range') to `N-`.

Example: `curl -C - -o big.iso https://example.com/big.iso` is
            `new CurlOptions { Url = url, OutputFile = "big.iso", ResumeFrom = -1 }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [Range](CurlDotNet.Core.CurlOptions.Range.md 'CurlDotNet\.Core\.CurlOptions\.Range')
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')