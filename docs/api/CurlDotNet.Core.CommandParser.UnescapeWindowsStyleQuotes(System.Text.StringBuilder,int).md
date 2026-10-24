#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.UnescapeWindowsStyleQuotes\(StringBuilder, int\) Method

Rewrites a just\-closed single\-quoted segment that was written for Windows\.

```csharp
private static void UnescapeWindowsStyleQuotes(System.Text.StringBuilder current, int start);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.UnescapeWindowsStyleQuotes(System.Text.StringBuilder,int).current'></a>

`current` [System\.Text\.StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder 'System\.Text\.StringBuilder')

<a name='CurlDotNet.Core.CommandParser.UnescapeWindowsStyleQuotes(System.Text.StringBuilder,int).start'></a>

`start` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

### Remarks
On Windows the C runtime that curl\.exe uses to split its command line treats
`\"` as a literal double quote even when PowerShell passed the text through
single quotes, so Windows documentation and Stack Overflow answers are full of
`-d '{\"key\":\"value\"}'`\. A POSIX shell would send the backslashes verbatim,
which is never what the author meant\. If a single\-quoted segment contains
`\"` and no bare `"` at all, every `\"` is unescaped\. A segment with
any bare double quote is treated as genuine JSON \(where `\"` is a real escape
inside a string literal\) and left untouched\.