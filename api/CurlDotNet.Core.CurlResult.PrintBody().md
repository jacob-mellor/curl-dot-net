#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.PrintBody\(\) Method

Writes [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') to the console \(standard output\) followed by a newline, and returns this result \-
handy in scripts and quick experiments\.

```csharp
public CurlDotNet.Core.CurlResult PrintBody();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result, for chaining\.

### Remarks

Prints exactly [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'), so binary responses print an empty line. Not for production logging; use
            your logger with [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') instead.

Example: `(await Curl.ExecuteAsync("curl https://api.github.com/zen")).PrintBody();`

### See Also
- [Print\(\)](CurlDotNet.Core.CurlResult.Print().md 'CurlDotNet\.Core\.CurlResult\.Print\(\)')
- [PrintVerbose\(\)](CurlDotNet.Core.CurlResult.PrintVerbose().md 'CurlDotNet\.Core\.CurlResult\.PrintVerbose\(\)')