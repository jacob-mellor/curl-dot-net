#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.StderrFile Property

File that receives what curl would print on standard error, mainly the `%{stderr}` part of
[WriteOut](CurlDotNet.Core.CurlOptions.WriteOut.md 'CurlDotNet\.Core\.CurlOptions\.WriteOut') and warnings, like curl's `--stderr FILE`\.

```csharp
public string? StderrFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path \(appended to\), or null/`"-"` \(default\) for the process's standard error \(`Console.Error`\)\.

### Remarks

Errors themselves are thrown as exceptions, not written here. If the file cannot be written, the text goes to
            `Console.Error` instead, like curl.

Example: `curl --stderr errors.log -w "%{stderr}%{http_code}\n" URL` is
            `new CurlOptions { Url = url, StderrFile = "errors.log", WriteOut = "%{stderr}%{http_code}\\n" }`.

### See Also
- [WriteOut](CurlDotNet.Core.CurlOptions.WriteOut.md 'CurlDotNet\.Core\.CurlOptions\.WriteOut')
- [Silent](CurlDotNet.Core.CurlOptions.Silent.md 'CurlDotNet\.Core\.CurlOptions\.Silent')