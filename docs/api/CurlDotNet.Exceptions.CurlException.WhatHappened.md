#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.WhatHappened Property

The first line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message'): what went wrong, with the concrete values involved
\(URL, host, option, file path, status code\), but without the fix or the documentation link\.

```csharp
public string WhatHappened { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A single line such as `Could not resolve host: api.exmaple.com (nodename nor servname provided)`\.

### Remarks

This is the text curl itself prints after `curl: (6)`, and what `-w '%{errormsg}'`
            expands to. Use it for short status lines; use [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message') or [ToUserFriendlyMessage\(\)](CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage().md 'CurlDotNet\.Exceptions\.CurlException\.ToUserFriendlyMessage\(\)')
            when the reader also needs the fix.

Example: `statusLabel.Text = ex.WhatHappened;` and `helpLink.NavigateUri = ex.DocumentationUrl;`.

### See Also
- [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions')
- [DocumentationUrl](CurlDotNet.Exceptions.CurlException.DocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlException\.DocumentationUrl')