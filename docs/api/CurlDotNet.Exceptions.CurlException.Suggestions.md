#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.Suggestions Property

How to fix this error, one sentence per entry, taken from the message \(every line between the
first line and the `See https://...` link\) or, when the message has none, from curl's exit code\.

```csharp
public System.Collections.Generic.List<string> Suggestions { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
Never null\. Every exception CurlDotNet throws has at least one entry; an exception you
            construct yourself with a one\-line message and no exit code has none until you call
            [WithSuggestion\(string\)](CurlDotNet.Exceptions.CurlException.WithSuggestion(string).md 'CurlDotNet\.Exceptions\.CurlException\.WithSuggestion\(string\)')\.

### Remarks

The same text already appears in [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message'); this list exists so that a UI or a log
            can show "what happened" ([WhatHappened](CurlDotNet.Exceptions.CurlException.WhatHappened.md 'CurlDotNet\.Exceptions\.CurlException\.WhatHappened')) and "how to fix it" separately, for example as
            bullet points under an error banner.

Example: `catch (CurlException ex) { foreach (var fix in ex.Suggestions) Console.WriteLine("- " + fix); }`
            prints, for `curl https://no-such-host.invalid`, a line telling you to check the spelling of the
            host and your DNS settings.

HTTP errors ([CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')) add status-specific advice, such as checking your
            token for a 401 or backing off for a 429.

### See Also
- [WithSuggestion\(string\)](CurlDotNet.Exceptions.CurlException.WithSuggestion(string).md 'CurlDotNet\.Exceptions\.CurlException\.WithSuggestion\(string\)')
- [WhatHappened](CurlDotNet.Exceptions.CurlException.WhatHappened.md 'CurlDotNet\.Exceptions\.CurlException\.WhatHappened')
- [DocumentationUrl](CurlDotNet.Exceptions.CurlException.DocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlException\.DocumentationUrl')
- [ToUserFriendlyMessage\(\)](CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage().md 'CurlDotNet\.Exceptions\.CurlException\.ToUserFriendlyMessage\(\)')