#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.EtagSave Property

Saves the response's `ETag` to this file \(one line\), like curl's `--etag-save FILE`, so a later request can
ask "only if changed" with [EtagCompare](CurlDotNet.Core.CurlOptions.EtagCompare.md 'CurlDotNet\.Core\.CurlOptions\.EtagCompare'); `"-"` appends it to [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

```csharp
public string? EtagSave { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path, or `"-"`\. Default: null\.

### Remarks

A response without an ETag writes an empty file. When the folder does not exist (and [CreateDirs](CurlDotNet.Core.CurlOptions.CreateDirs.md 'CurlDotNet\.Core\.CurlOptions\.CreateDirs') is not
            set) curl skips the whole transfer with a warning, and so does CurlDotNet: nothing is sent and the result is empty.

Example: `curl --etag-save etag.txt -o feed.xml URL` is `new CurlOptions { Url = url, OutputFile = "feed.xml", EtagSave = "etag.txt" }`.

### See Also
- [EtagCompare](CurlDotNet.Core.CurlOptions.EtagCompare.md 'CurlDotNet\.Core\.CurlOptions\.EtagCompare')