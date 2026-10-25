#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.CurlErrorMessage Property

The short error text the curl command\-line tool prints after `curl: (N) ` for this failure, in curl's own
wording where CurlDotNet knows it, such as `The requested URL returned error: 404` for `-f`\.

```csharp
public virtual string CurlErrorMessage { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Text without a trailing newline and without the `curl: (N) ` prefix\. Never null\. Where CurlDotNet knows
            curl's wording it is exactly that text, for example `The requested URL returned error: 404` \(`-f`, no reason
            phrase, no URL\), `HTTP upload cannot be resumed` \(`-C` with `-d`, `-F` or `-T`\),
            `Unsolicited Transfer-Encoding (identityx) found` and `Reject response exceeding limit of 5 transfer
            encodings`\. A few of curl's messages span lines, and so does this text then: an output name that uses a glob that
            does not exist is `no glob exists with this name in position 16:`, the name, and a line with a `^` under the
            position\. For a usage error \([IsUsageError](CurlDotNet.Exceptions.CurlException.IsUsageError.md 'CurlDotNet\.Exceptions\.CurlException\.IsUsageError')\) every line is one line curl prints after `curl: `\. For
            any other failure it is [WhatHappened](CurlDotNet.Exceptions.CurlException.WhatHappened.md 'CurlDotNet\.Exceptions\.CurlException\.WhatHappened'), CurlDotNet's own first message line, which states the same failure
            with the values involved but not always in curl's exact words\.

### Remarks

The curl tool ends a failed run with one line on standard error, `curl: (22) The requested URL returned
            error: 404`, unless `-s`/`--silent` is given without `-S`/`--show-error`. Use this property
            to print that line yourself, for example when a .NET tool wraps CurlDotNet and must keep scripts that parse curl's
            error output working: `Console.Error.WriteLine($"curl: ({ex.CurlErrorCode}) {ex.CurlErrorMessage}");`.
            `-w '%{errormsg}'` expands to this text as well.

Prefer [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message') or [ToUserFriendlyMessage\(\)](CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage().md 'CurlDotNet\.Exceptions\.CurlException\.ToUserFriendlyMessage\(\)') for people: they add how to fix the
            problem and a documentation link.

### See Also
- [WhatHappened](CurlDotNet.Exceptions.CurlException.WhatHappened.md 'CurlDotNet\.Exceptions\.CurlException\.WhatHappened')
- [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')
- [IsUsageError](CurlDotNet.Exceptions.CurlException.IsUsageError.md 'CurlDotNet\.Exceptions\.CurlException\.IsUsageError')