#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.PrintVerbose\(\) Method

Writes the status, every response header, the body and the main timings \(DNS, connect, total\) to the console,
and returns this result \- a quick look at everything that came back\.

```csharp
public CurlDotNet.Core.CurlResult PrintVerbose();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result, for chaining\.

### Remarks

This prints the parsed result; for the raw request and response exchange as curl shows it, run the command
            with `-v` (the log is in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')) or `--trace-ascii trace.txt`.

Example: `(await Curl.ExecuteAsync("curl https://example.com")).PrintVerbose();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md

### See Also
- [Print\(\)](CurlDotNet.Core.CurlResult.Print().md 'CurlDotNet\.Core\.CurlResult\.Print\(\)')
- [Timings](CurlDotNet.Core.CurlResult.Timings.md 'CurlDotNet\.Core\.CurlResult\.Timings')