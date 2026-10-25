#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.ToDetailedString\(\) Method

Returns a multi\-line report for logs: type, what happened, documentation link, command, exit code and name,
context, how\-to\-fix list, diagnostics, inner exception and stack trace\.

```csharp
public virtual string ToDetailedString();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A string that starts with `=== CURL EXCEPTION DETAILS ===` and has one labelled line per fact\.

### Remarks

Use it where a human will read the log later; use [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)') for structured logging and
            [ToUserFriendlyMessage\(\)](CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage().md 'CurlDotNet\.Exceptions\.CurlException\.ToUserFriendlyMessage\(\)') for end users. Credentials in the command (`-u`/`-U` values, bearer
            tokens, `Authorization`/`Cookie` header values, URL passwords) are printed as `***`; the
            [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') property itself keeps the original text. [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') adds the status,
            headers and body.

Example: `catch (CurlException ex) { logger.LogError(ex.ToDetailedString()); }`

### See Also
- [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)')
- [ToUserFriendlyMessage\(\)](CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage().md 'CurlDotNet\.Exceptions\.CurlException\.ToUserFriendlyMessage\(\)')
- [Log\(Action&lt;string,Dictionary&lt;string,object&gt;&gt;\)](CurlDotNet.Exceptions.CurlException.Log(System.Action_string,System.Collections.Generic.Dictionary_string,object__).md 'CurlDotNet\.Exceptions\.CurlException\.Log\(System\.Action\<string,System\.Collections\.Generic\.Dictionary\<string,object\>\>\)')