#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.OutputFiles Property

Paths of the files this transfer wrote: the `-o`/`-O` output \(with `--output-dir` and
`--no-clobber` renaming applied\), plus any files saved later with [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)') and friends\.

```csharp
public System.Collections.Generic.List<string> OutputFiles { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
Paths as written \(relative paths stay relative to the current directory\); empty when the body went to
            [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')/[BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') only\. `-D`, `-c` and `--trace` files are not listed\.

### Remarks

Example: `var r = await Curl.ExecuteAsync("curl -O https://example.com/files/report.pdf"); Console.WriteLine(r.OutputFiles[0]); // report.pdf`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)')
- [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')
- [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')