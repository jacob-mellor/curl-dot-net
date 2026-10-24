#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.Message Property

The full error text: what happened \(first line\), how to fix it \(following lines\) and a
`See https://...` documentation link \(last line\)\.

```csharp
public override string Message { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
For example:
            `Could not resolve host: api.exmaple.com (No such host is known)`, then
            `Check the spelling of the host in the URL and your DNS settings; --resolve HOST:PORT:ADDRESS pins a name to an address without DNS.`,
            then `See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md`\.

### Remarks

Older message formats such as `For DNS troubleshooting, see: URL` are normalised to a
            single trailing `See URL` line, so a message never carries two links. When a CurlDotNet
            exception has no link of its own, the page for its exit code is appended. A message you pass to a
            constructor yourself is returned unchanged unless it contains such a link, an exit code is set, or an inner
            exception is given (its first message line is then appended as `(caused by Type: message)`).

[System\.Exception\.ToString](https://learn.microsoft.com/en-us/dotnet/api/system.exception.tostring 'System\.Exception\.ToString'), [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)'), [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)') and
            [ToUserFriendlyMessage\(\)](CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage().md 'CurlDotNet\.Exceptions\.CurlException\.ToUserFriendlyMessage\(\)') all use this normalised text.

### See Also
- [WhatHappened](CurlDotNet.Exceptions.CurlException.WhatHappened.md 'CurlDotNet\.Exceptions\.CurlException\.WhatHappened')
- [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions')
- [DocumentationUrl](CurlDotNet.Exceptions.CurlException.DocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlException\.DocumentationUrl')