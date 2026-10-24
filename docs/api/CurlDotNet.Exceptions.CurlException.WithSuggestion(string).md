#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.WithSuggestion\(string\) Method

Adds your own advice on how to fix the problem to [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(ignored if the same text is already
there\) and returns the same exception for chaining\.

```csharp
public CurlDotNet.Exceptions.CurlException WithSuggestion(string suggestion);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.WithSuggestion(string).suggestion'></a>

`suggestion` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

One sentence telling the reader what to do, such as `"Renew the API token."`\.

#### Returns
[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
This same exception instance, for chaining\.

### Remarks

Use it in middleware or wrappers that know something CurlDotNet does not, such as which configuration key
            holds the API token. The text is shown by [ToUserFriendlyMessage\(\)](CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage().md 'CurlDotNet\.Exceptions\.CurlException\.ToUserFriendlyMessage\(\)') and [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)');
            [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message') is not changed.

Example: `catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode == 401) { throw ex.WithSuggestion("Renew the token in appsettings.json:Api:Token."); }`

### See Also
- [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions')
- [ToUserFriendlyMessage\(\)](CurlDotNet.Exceptions.CurlException.ToUserFriendlyMessage().md 'CurlDotNet\.Exceptions\.CurlException\.ToUserFriendlyMessage\(\)')