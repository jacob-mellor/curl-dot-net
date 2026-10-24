#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.ToUserFriendlyMessage\(\) Method

Returns short text for end users: `Error:` what happened, a bullet list of possible solutions, a note when a
retry may help, and the documentation link, without stack traces or the command\.

```csharp
public virtual string ToUserFriendlyMessage();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Several lines: `Error: ...`, `Possible solutions:` with `•` bullets, optionally
            `This error might be temporary`, and `More help: https://...`\.

### Remarks

Suitable for a dialog or a CLI tool's error output. Example: `catch (CurlException ex) { Console.Error.WriteLine(ex.ToUserFriendlyMessage()); return ex.CurlErrorCode ?? 1; }`

### See Also
- [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions')
- [WhatHappened](CurlDotNet.Exceptions.CurlException.WhatHappened.md 'CurlDotNet\.Exceptions\.CurlException\.WhatHappened')
- [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)')