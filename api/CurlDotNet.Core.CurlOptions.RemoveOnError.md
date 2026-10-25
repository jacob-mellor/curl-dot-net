#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.RemoveOnError Property

Leaves no output file behind when the transfer fails \(for example a connection cut mid\-download\), like curl's
`--remove-on-error`, so a partial file is never mistaken for a complete one\.

```csharp
public bool RemoveOnError { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to drop the output of a failed transfer\. Default: false \(whatever arrived is written\)\.

### Remarks

Example: `curl --remove-on-error -o big.iso URL` is `new CurlOptions { Url = url, OutputFile = "big.iso", RemoveOnError = true }`.
            Without it, resume an interrupted download with [ResumeFrom](CurlDotNet.Core.CurlOptions.ResumeFrom.md 'CurlDotNet\.Core\.CurlOptions\.ResumeFrom') = -1.

### See Also
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')
- [ResumeFrom](CurlDotNet.Core.CurlOptions.ResumeFrom.md 'CurlDotNet\.Core\.CurlOptions\.ResumeFrom')