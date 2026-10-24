#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.WriteOut Property

A curl `-w`/`--write-out` format whose expansion is appended to [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') after the
transfer, with variables such as `%{http_code}`, `%{time_total}` and `%{url_effective}`\.

```csharp
public string? WriteOut { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A format string; `\n` and `\t` are escapes, `%{json}` prints every variable as JSON,
            `%{stderr}` switches the following text to [StderrFile](CurlDotNet.Core.CurlOptions.StderrFile.md 'CurlDotNet\.Core\.CurlOptions\.StderrFile')\. Default: null\.

### Remarks

The text is expanded for failed transfers too (with `%{exitcode}` and `%{errormsg}`); it then travels on
            the exception under [WriteOutDataKey](CurlDotNet.Exceptions.CurlException.WriteOutDataKey.md 'CurlDotNet\.Exceptions\.CurlException\.WriteOutDataKey'). In C# the same values are
            usually easier to read from [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode'), [Timings](CurlDotNet.Core.CurlResult.Timings.md 'CurlDotNet\.Core\.CurlResult\.Timings') and
            [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers').

Example: `curl -s -o /dev/null -w "%{http_code} %{time_total}\n" URL` is
            `new CurlOptions { Url = url, Silent = true, OutputFile = "/dev/null", WriteOut = "%{http_code} %{time_total}\\n" }`.

### See Also
- [Timings](CurlDotNet.Core.CurlResult.Timings.md 'CurlDotNet\.Core\.CurlResult\.Timings')
- [StderrFile](CurlDotNet.Core.CurlOptions.StderrFile.md 'CurlDotNet\.Core\.CurlOptions\.StderrFile')
- [WriteOutDataKey](CurlDotNet.Exceptions.CurlException.WriteOutDataKey.md 'CurlDotNet\.Exceptions\.CurlException\.WriteOutDataKey')