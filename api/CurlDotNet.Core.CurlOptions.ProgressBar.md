#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ProgressBar Property

curl's `-#`/`--progress-bar`: draws curl's `#` progress bar for HTTP downloads on
[StderrFile](CurlDotNet.Core.CurlOptions.StderrFile.md 'CurlDotNet\.Core\.CurlOptions\.StderrFile') \(`--stderr FILE`\) or, without one, standard error\.

```csharp
public bool ProgressBar { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when the option was given\. Default: false\.

### Remarks

The bar is curl's own (src/tool_cb_prg.c): a carriage return, `#` marks across the terminal width
            (`COLUMNS`, else the console width, else 79 columns) minus 7, and the percentage such as `100.0%`,
            redrawn at most ten times a second and ended with a newline; a download of unknown size shows curl's moving
            `-=O=-` animation. [Silent](CurlDotNet.Core.CurlOptions.Silent.md 'CurlDotNet\.Core\.CurlOptions\.Silent') (`-s`) turns it off, as in curl. CurlDotNet never draws curl's
            default progress meter.

Example: `await Curl.ExecuteAsync("curl -# -o big.iso https://example.com/big.iso");`. In an
            application, prefer [ProgressHandler](CurlDotNet.Core.CurlOptions.ProgressHandler.md 'CurlDotNet\.Core\.CurlOptions\.ProgressHandler') (or `CurlSettings.WithProgress`) to show your own progress.

### See Also
- [ProgressHandler](CurlDotNet.Core.CurlOptions.ProgressHandler.md 'CurlDotNet\.Core\.CurlOptions\.ProgressHandler')