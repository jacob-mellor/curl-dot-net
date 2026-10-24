#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ProgressBar Property

curl's `-#`/`--progress-bar` \(draw a bar instead of the meter\); recorded for compatibility only,
because CurlDotNet never draws progress on the terminal\.

```csharp
public bool ProgressBar { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when the option was given\. Default: false\. Not acted on\.

### Remarks

Use [ProgressHandler](CurlDotNet.Core.CurlOptions.ProgressHandler.md 'CurlDotNet\.Core\.CurlOptions\.ProgressHandler') (or `CurlSettings.WithProgress`) to show your own progress.

### See Also
- [ProgressHandler](CurlDotNet.Core.CurlOptions.ProgressHandler.md 'CurlDotNet\.Core\.CurlOptions\.ProgressHandler')