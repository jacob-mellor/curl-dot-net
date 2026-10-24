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

| Fields | |
| :--- | :--- |
| [TransportOptionsRequiringValue](CurlDotNet.Core.CommandParser.TransportOptionsRequiringValue.md 'CurlDotNet\.Core\.CommandParser\.TransportOptionsRequiringValue') | Long option names handled by [ProcessTransportOption\(string, string, CurlOptions, bool\)](CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).md 'CurlDotNet\.Core\.CommandParser\.ProcessTransportOption\(string, string, CurlDotNet\.Core\.CurlOptions, bool\)') that take a value, so the tokenizer consumes the following argument instead of mistaking it for the URL\. |

| Methods | |
| :--- | :--- |
| [AppendJsonData\(CurlOptions, string\)](CurlDotNet.Core.CommandParser.AppendJsonData(CurlDotNet.Core.CurlOptions,string).md 'CurlDotNet\.Core\.CommandParser\.AppendJsonData\(CurlDotNet\.Core\.CurlOptions, string\)') | Appends JSON data to the options\. Unlike [CurlDotNet\.Core\.CommandParser\.AppendData\(CurlDotNet\.Core\.CurlOptions,System\.String\)](https://learn.microsoft.com/en-us/dotnet/api/curldotnet.core.commandparser.appenddata#curldotnet-core-commandparser-appenddata(curldotnet-core-curloptions-system-string) 'CurlDotNet\.Core\.CommandParser\.AppendData\(CurlDotNet\.Core\.CurlOptions,System\.String\)'), multiple \-\-json values are concatenated without any separator, matching curl's behavior\. Curl 7\.82\+ introduced \-\-json as shorthand for \-\-data with JSON headers\. |
| [CollapseUnquotedWhitespace\(string\)](CurlDotNet.Core.CommandParser.CollapseUnquotedWhitespace(string).md 'CurlDotNet\.Core\.CommandParser\.CollapseUnquotedWhitespace\(string\)') | Collapses runs of whitespace to a single space outside quoted regions while leaving the inside of single\- and double\-quoted strings untouched, so paths like `"C:\Temp\file with  double  spaces.txt"` and JSON payloads keep their exact spacing\. |
| [ExpandEnvironmentVariables\(string\)](CurlDotNet.Core.CommandParser.ExpandEnvironmentVariables(string).md 'CurlDotNet\.Core\.CommandParser\.ExpandEnvironmentVariables\(string\)') | Expands environment variables across different shell formats\. \- Windows CMD: %VAR% \- PowerShell: $env:VAR or $VAR \- Bash/ZSH: $VAR or $\{VAR\} |
| [NormalizeLineContinuations\(string\)](CurlDotNet.Core.CommandParser.NormalizeLineContinuations(string).md 'CurlDotNet\.Core\.CommandParser\.NormalizeLineContinuations\(string\)') | Normalizes line continuations across different shells\. \- Windows CMD: ^ at end of line \- PowerShell: backtick \` at end of line \- Bash/ZSH: \\ at end of line |
| [NormalizeUrl\(string\)](CurlDotNet.Core.CommandParser.NormalizeUrl(string).md 'CurlDotNet\.Core\.CommandParser\.NormalizeUrl\(string\)') | Adds the scheme curl would guess for a URL given without one: `ftp.example.com` becomes `ftp://ftp.example.com`, anything else becomes `http://...`\. URLs that already carry a scheme are returned unchanged\. |
| [ParseArguments\(string\)](CurlDotNet.Core.CommandParser.ParseArguments(string).md 'CurlDotNet\.Core\.CommandParser\.ParseArguments\(string\)') | Parses arguments from command string, handling quotes from all shells\. Supports: \- Single quotes: 'text' \- Double quotes: "text" \- Escaped quotes: \\" and \\' and "" \(Windows\-style\) |
| [PercentEncode\(string\)](CurlDotNet.Core.CommandParser.PercentEncode(string).md 'CurlDotNet\.Core\.CommandParser\.PercentEncode\(string\)') | Percent\-encodes a string the way curl's `curl_easy_escape` does: UTF\-8 bytes, with only ASCII letters, digits and `- . _ ~` left as\-is\. Spaces become `%20`, never `+`\. |
| [ProcessTransportOption\(string, string, CurlOptions, bool\)](CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).md 'CurlDotNet\.Core\.CommandParser\.ProcessTransportOption\(string, string, CurlDotNet\.Core\.CurlOptions, bool\)') | Handles transport\-side options\. Returns true when [option](CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).md#CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).option 'CurlDotNet\.Core\.CommandParser\.ProcessTransportOption\(string, string, CurlDotNet\.Core\.CurlOptions, bool\)\.option') was recognised\. |
| [ShellQuote\(string\)](CurlDotNet.Core.CommandParser.ShellQuote(string).md 'CurlDotNet\.Core\.CommandParser\.ShellQuote\(string\)') | Wraps a value in single quotes so it survives a round\-trip through [CurlDotNet\.Core\.CommandParser\.Parse\(System\.String\)](https://learn.microsoft.com/en-us/dotnet/api/curldotnet.core.commandparser.parse#curldotnet-core-commandparser-parse(system-string) 'CurlDotNet\.Core\.CommandParser\.Parse\(System\.String\)') \(and through a real POSIX shell\) unchanged, even when it contains spaces, double quotes, backslashes or single quotes\. Embedded single quotes use the standard `'\''` idiom\. |
| [ShellQuoteIfNeeded\(string\)](CurlDotNet.Core.CommandParser.ShellQuoteIfNeeded(string).md 'CurlDotNet\.Core\.CommandParser\.ShellQuoteIfNeeded\(string\)') | Like [ShellQuote\(string\)](CurlDotNet.Core.CommandParser.ShellQuote(string).md 'CurlDotNet\.Core\.CommandParser\.ShellQuote\(string\)') but leaves values that contain only shell\-safe characters \(letters, digits and `- _ . / : = @ % ? & + ~ , # ;`\) unquoted, which keeps generated commands readable for URLs and file names\. |
| [UnescapeWindowsStyleQuotes\(StringBuilder, int\)](CurlDotNet.Core.CommandParser.UnescapeWindowsStyleQuotes(System.Text.StringBuilder,int).md 'CurlDotNet\.Core\.CommandParser\.UnescapeWindowsStyleQuotes\(System\.Text\.StringBuilder, int\)') | Rewrites a just\-closed single\-quoted segment that was written for Windows\. |
| [UrlEncodeDataArgument\(string\)](CurlDotNet.Core.CommandParser.UrlEncodeDataArgument(string).md 'CurlDotNet\.Core\.CommandParser\.UrlEncodeDataArgument\(string\)') | Applies curl's `--data-urlencode` rules to one argument: `content`, `=content`, `name=content`, `@file` and `name@file`\. Only the content part is percent\-encoded; the name and the `=` separator are kept\. |
