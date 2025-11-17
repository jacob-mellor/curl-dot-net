#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CommandParser Class

Parses curl command strings into CurlOptions, following curl's command\-line syntax exactly\.
This parser is inspired by curl's tool\_getparam\.c which handles all parameter parsing\.


Supports all curl options including short forms (-X POST), long forms (--request POST),
combined short options (-sS), option arguments, quote handling, and line continuations.
The parser normalizes different shell syntaxes (bash, PowerShell, cmd, zsh) into a consistent format.

<b>Cross-Shell Compatibility:</b> Paste curl commands from any shell and they work:
- <b>Windows CMD:</b>`curl -H "Content-Type: application/json" -d "{\"key\":\"value\"}"`
- <b>PowerShell:</b>`curl -H 'Content-Type: application/json' -d '{\"key\":\"value\"}'`
- <b>Bash/ZSH:</b>`curl -H 'Content-Type: application/json' -d '{"key":"value"}'`

For curl option reference: https://curl.se/docs/manpage.html#OPTIONS

```csharp
internal class CommandParser
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CommandParser

### Remarks

This parser handles all curl command-line syntax including short and long options.

Processes commands in the same order as curl for compatibility.

Thread-safe and can be reused for multiple parse operations.\<ai\-semantic\-usage\>
             Core parser that translates curl command strings to structured options\.
             Handles shell quoting, escaping, line continuations, and all curl option formats\.
             Use this when you need to parse curl commands programmatically\.
             \</ai\-semantic\-usage\>\<ai\-patterns\>
             \- Always preserves curl's option precedence rules
             \- Later options override earlier ones \(like curl\)
             \- Multiple \-d options are concatenated
             \- File references \(@file\) are expanded
             \- Handles Windows, PowerShell, Bash, ZSH syntax differences
             \</ai\-patterns\>

| Methods | |
| :--- | :--- |
| [IsValid\(string\)](CurlDotNet.Core.CommandParser.IsValid(string).md 'CurlDotNet\.Core\.CommandParser\.IsValid\(string\)') | |
| [Parse\(string\)](CurlDotNet.Core.CommandParser.Parse(string).md 'CurlDotNet\.Core\.CommandParser\.Parse\(string\)') | |
