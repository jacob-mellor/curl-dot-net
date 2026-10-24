#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.ShellQuoteIfNeeded\(string\) Method

Like [ShellQuote\(string\)](CurlDotNet.Core.CommandParser.ShellQuote(string).md 'CurlDotNet\.Core\.CommandParser\.ShellQuote\(string\)') but leaves values that contain only
shell\-safe characters \(letters, digits and `- _ . / : = @ % ? & + ~ , # ;`\)
unquoted, which keeps generated commands readable for URLs and file names\.

```csharp
public static string ShellQuoteIfNeeded(string? value);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.ShellQuoteIfNeeded(string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Raw value such as a URL or output path\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The value, quoted only when it needs to be\.