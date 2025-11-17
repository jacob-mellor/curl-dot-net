#### [CurlDotNet](index.md 'index')

## CurlDotNet\.Core Namespace

| Classes | |
| :--- | :--- |
| [CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser') | Parses curl command strings into CurlOptions, following curl's command\-line syntax exactly\. This parser is inspired by curl's tool\_getparam\.c which handles all parameter parsing\.    Supports all curl options including short forms (-X POST), long forms (--request POST), combined short options (-sS), option arguments, quote handling, and line continuations. The parser normalizes different shell syntaxes (bash, PowerShell, cmd, zsh) into a consistent format.  <b>Cross-Shell Compatibility:</b> Paste curl commands from any shell and they work:\.\.\.  For curl option reference: https://curl.se/docs/manpage.html#OPTIONS |
| [CurlEngine](CurlDotNet.Core.CurlEngine.md 'CurlDotNet\.Core\.CurlEngine') | Core engine that processes and executes curl commands\. |
| [CurlFtpException](CurlDotNet.Core.CurlFtpException.md 'CurlDotNet\.Core\.CurlFtpException') | FTP\-specific exception\. |
| [CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException') |   <b>Exception for HTTP errors (4xx, 5xx status codes).</b>  Thrown by EnsureSuccess() when request fails:\`\.\.\.\` |
| [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') | Represents parsed curl command options\. |
| [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') |   <b>🎨 Fluent Builder API - Build curl requests programmatically!</b>  For developers who prefer a fluent API over curl command strings.              This builder lets you construct HTTP requests using method chaining,              perfect for IntelliSense and compile-time checking.  <b>When to use Builder vs Curl String:</b>\.\.\.  <b>Quick Example:</b>\`\.\.\.\` |
| [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') |   <b>🎯 The response from your curl command - everything you need is here!</b>  After running any curl command, you get this object back. It has the status code,              response body, headers, and helpful methods to work with the data.  <b>The API is designed to be intuitive - just type what you want to do:</b>\.\.\.  <b>Quick Example:</b>\`\.\.\.\` |
| [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') | Fluent builder for \.NET\-specific curl settings\. |
| [CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings') |   <b>Detailed timing breakdown of the curl operation.</b>  See where time was spent (like curl -w):\`\.\.\.\` |
| [FileHandler](CurlDotNet.Core.FileHandler.md 'CurlDotNet\.Core\.FileHandler') | Handler for file:// protocol\. |
| [FtpHandler](CurlDotNet.Core.FtpHandler.md 'CurlDotNet\.Core\.FtpHandler') | Handler for FTP and FTPS protocols\. |
| [HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler') | Handler for HTTP and HTTPS protocols\. |
| [ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult') | Result of command validation\. |

| Interfaces | |
| :--- | :--- |
| [ICommandParser](CurlDotNet.Core.ICommandParser.md 'CurlDotNet\.Core\.ICommandParser') | Interface for parsing curl command strings into options\. |
| [IProtocolHandler](CurlDotNet.Core.IProtocolHandler.md 'CurlDotNet\.Core\.IProtocolHandler') | Interface for protocol\-specific handlers \(HTTP, FTP, FILE, etc\.\)\. |
