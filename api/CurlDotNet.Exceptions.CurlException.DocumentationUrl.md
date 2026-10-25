#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.DocumentationUrl Property

The documentation page that explains this error and how to fix it: the `See https://...` link
at the end of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\.

```csharp
public string? DocumentationUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An absolute `https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/...` URL, or null
            for an exception you constructed yourself without a link and without an exit code\.

### Remarks

When the message carries no link but [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') is set, the page for that exit
            code is used (for example the DNS page for 6, the timeout page for 28, the TLS page for 60).

Example: `Process.Start(new ProcessStartInfo(ex.DocumentationUrl) { UseShellExecute = true });`
            opens the page in the browser from a desktop tool.

### See Also
- [WhatHappened](CurlDotNet.Exceptions.CurlException.WhatHappened.md 'CurlDotNet\.Exceptions\.CurlException\.WhatHappened')
- [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions')