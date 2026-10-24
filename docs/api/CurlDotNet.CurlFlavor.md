#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet')

## CurlFlavor Enum

Names the shell a curl command string was written for, so CurlDotNet applies that shell's
quoting, escaping, line\-continuation and variable rules before reading the curl options\.

```csharp
public enum CurlFlavor
```
### Fields

<a name='CurlDotNet.CurlFlavor.Auto'></a>

`Auto` 0

Recognise the flavor from telltales in the text, and read the command permissively \(the default\)\.

### Remarks

<b>Detection, first match wins:</b>
1. [InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest') when the first word is `Invoke-WebRequest`,
              `Invoke-RestMethod`, `iwr`, `irm` or `wget`, or when the first word is
              `curl` (not `curl.exe`) or missing and the command contains a cmdlet parameter such as
              `-Uri`, `-Method`, `-Headers`, `-Body`, `-ContentType`, `-OutFile` or
              `-UseBasicParsing`, or a hashtable `@{`.
2. [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell') when a line ends with a backtick, or the text contains
              `$env:`, a backtick-escaped quote ``"`, or the stop-parsing token `--%`.
3. [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd') when a line ends with `^`, the text contains a caret-escaped
              quote `^"`, or it contains `%NAME%` where `NAME` is a set environment variable.
4. [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix') otherwise.

<b>Reading:</b> when the detected flavor is [InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest'), the cmdlet syntax is
            translated. When the text contains a construct only one shell's exact rules can read
            (`^"` for cmd.exe; ``"` or `--%` for PowerShell) that flavor's exact rules are used.
            Otherwise CurlDotNet uses its permissive reader, which accepts all three shells at once:
            single and double quotes both group; `\"`, `\'`, `\\`, `\$` and a backslash before
            a space are escapes, any other backslash is literal (so Windows paths survive); `""` inside double
            quotes is a quote; `\`, `^` and a backtick at the end of a line all continue it;
            `$VAR`, `${VAR}` and `$env:VAR` expand outside single quotes and `%VAR%` expands
            outside single quotes when the variable is set; and a single-quoted value containing `\"` but no
            bare `"` has its `\"` turned into `"`, because that is only ever written for Windows.
            A leading `curl`, `curl.exe` or a path to either is removed.

[Flavor](CurlDotNet.Core.CurlOptions.Flavor.md 'CurlDotNet\.Core\.CurlOptions\.Flavor') then reports the detected flavor, never `Auto`.

Example: `curl -X POST https://api.example.com -H 'Content-Type: application/json' -d '{"a":1}'`
            is detected as [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix') and sends `POST / HTTP/1.1` with
            `Content-Type: application/json` and the 7-byte body `{"a":1}`.

### See Also
- [DetectFlavor\(string\)](CurlDotNet.Curl.DetectFlavor(string).md 'CurlDotNet\.Curl\.DetectFlavor\(string\)')
- [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')
- [curl flavors guide](https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md 'https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-flavors\-bash\-cmd\-powershell/README\.md')

<a name='CurlDotNet.CurlFlavor.Posix'></a>

`Posix` 1

bash, zsh, sh and other POSIX shells \(macOS and Linux terminals, Git Bash, WSL, CI scripts,
"Copy as cURL \(bash\)" in browsers\)\.

### Remarks

<b>Single quotes</b> are completely literal: nothing inside `'...'` is special, a backslash
            included, so `'{\"a\":1}'` stays the eight characters `{\"a\":1}`. A single quote cannot
            appear inside single quotes; write `'it'\''s'`.

<b>Double quotes</b> group text; inside them `\"`, `\\`, `\$` and `\`` give
            the second character, a backslash before a newline removes both, and any other backslash is kept
            (`"a\nb"` is the four characters `a\nb`). `$VAR` and `${VAR}` expand inside
            double quotes.

<b>ANSI-C quotes</b>`$'...'` (bash, zsh) understand `\n`, `\t`, `\r`,
            `\'`, `\"`, `\\`, `\xHH`, `\uHHHH` and octal `\NNN`; browsers use them when
            a copied request body contains a quote or a control character.

<b>Escape character:</b> backslash. Outside quotes a backslash makes the next character literal,
            whatever it is (`a\ b` is one argument with a space, `C:\temp` becomes `C:temp`).

<b>Line continuation:</b> a backslash as the last character of a line.

<b>Variables:</b>`$VAR` and `${VAR}` expand outside single quotes from the process
            environment; outside double quotes the value is split into separate arguments at spaces, tabs and
            newlines (so `$CURL_OPTS` holding `-s -L` is two options). `%VAR%` and
            `$env:VAR` are ordinary text.

<b>End of the command:</b>`#` at the start of a word begins a comment that runs to the end
            of the line, and an unquoted `|`, `;`, `&`, `>` or `<` ends the curl
            command, so a pasted `| jq .`, `&& echo ok` or `2>/dev/null` tail is ignored.

Example: `curl -H "Authorization: Bearer $TOKEN" -d '{"a":1}' https://api.example.com` with
            `TOKEN=abc` sends `Authorization: Bearer abc` and the body `{"a":1}`.

### See Also
- [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd')
- [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell')
- [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)')

<a name='CurlDotNet.CurlFlavor.WindowsCmd'></a>

`WindowsCmd` 2

`curl.exe` run from the Windows Command Prompt \(`cmd.exe`\), batch files, and
            "Copy as cURL \(cmd\)" in browsers\.

### Remarks

The text goes through two stages, exactly as on Windows: cmd.exe first, then the C runtime that
            splits `curl.exe`'s command line into arguments.

<b>Single quotes</b> are ordinary characters: `'{"a":1}'` does not group anything and
            sends the quote characters (and loses the double quotes) just as real cmd.exe does.

<b>Double quotes</b> group text containing spaces. Inside a quoted part, `\"` is a literal
            quote and `""` is also a literal quote (C runtime rules); a run of backslashes is halved only
            when it is followed by a quote.

<b>Escape character:</b> caret `^`, outside double quotes: `^&`, `^|`,
            `^<`, `^>` and `^^` give the second character, and `^"` gives a quote that
            cmd.exe does not treat as quoting but the C runtime still does. Inside double quotes a caret is literal.
            An unescaped `&`, `|`, `<` or `>` outside double quotes ends the curl
            command (so `| findstr x` or `2>nul` is ignored), exactly as cmd.exe would run it.

<b>Line continuation:</b>`^` as the last character of a line.

<b>Variables:</b>`%VAR%` expands everywhere, inside double quotes too, when the variable
            is set (an unset one stays `%VAR%`, as in an interactive cmd.exe). Expansion happens before
            carets are read, so a caret cannot stop it: `^%VAR%` still expands. `$VAR` is ordinary
            text.

<b>Backslash</b> is a path separator, never an escape on its own: `C:\temp\a.txt` stays as
            typed. It only matters directly before a double quote.

Example: `curl -H "Content-Type: application/json" -d "{\"a\":1}" https://api.example.com`
            sends `Content-Type: application/json` and the body `{"a":1}`.

### See Also
- [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell')
- [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix')
- [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)')

<a name='CurlDotNet.CurlFlavor.PowerShell'></a>

`PowerShell` 3

`curl.exe` run from PowerShell 7 \(`pwsh`\) or Windows PowerShell 5\.1 \(always type
            `curl.exe` there; plain `curl` is the [InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest') alias in 5\.1\)\.

### Remarks

<b>Single quotes</b> are literal except that `''` inside them is one `'`
            (`'it''s'` is `it's`).

<b>Double quotes</b> expand `$env:NAME`, `${env:NAME}` and `$NAME` (looked up as
            an environment variable, because PowerShell's own session variables are not visible to .NET), and use
            backtick escapes: ``"` is a quote, ```` a backtick, ``$` a dollar sign, ``n`,
            ``r`, ``t`, ``0`, ``a`, ``b`, ``e`, ``f` and ``v` control
            characters, and `""` is one quote. `$(...)` sub-expressions are left as typed.

<b>Escape character:</b> backtick, inside double quotes and in bare words. A backslash is not a
            PowerShell escape.

<b>Line continuation:</b> a backtick as the last character of a line.

<b>Windows argument rule:</b> how an argument reaches curl.exe depends on the PowerShell version.
            Windows PowerShell 5.1 (and 7.2 and older) hands curl.exe a command line that its C runtime reads again,
            so commands written for it escape quotes with a backslash: `-d '{\"key\":\"value\"}'`, where
            `\"` arrives as `"` (and `\\\"` as `\"`). PowerShell 7.3+ passes arguments intact,
            so commands written for it use bare quotes: `-d '{"key":"value"}'`. CurlDotNet reads each argument
            the way it was evidently written: an argument with no bare double quote has every backslash-escaped
            quote turned into a quote (the 5.1 reading), and an argument that contains a bare double quote is sent
            as typed, `\"` included (the 7.3+ reading, which keeps JSON escapes such as
            `{"say":"\"hi\""}` intact). Either way the author's intended bytes are sent.

<b>Stop-parsing:</b> after the token `--%`, the rest of the line (up to an unquoted
            `|`) is not PowerShell syntax: it is handed to curl.exe as typed, with only `%NAME%`
            environment references expanded, and split with the [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd') C runtime rules (double
            quotes group, `\"` is a quote).

<b>Variables:</b>`$env:NAME` as above; `%VAR%` does not expand (outside `--%`).

<b>End of the command:</b>`#` at the start of a word begins a comment. An unquoted
            `|`, `;`, `>`, `&&` or a later `&` ends the command, so a pasted
            `| ConvertFrom-Json` or `&& echo ok` tail is ignored. A leading `&` call
            operator and a `$result =` assignment are removed.

Example: `curl.exe -H 'Content-Type: application/json' -d '{\"a\":1}' https://api.example.com`
            sends `Content-Type: application/json` and the body `{"a":1}`.

### See Also
- [InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest')
- [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd')
- [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)')

<a name='CurlDotNet.CurlFlavor.InvokeWebRequest'></a>

`InvokeWebRequest` 4

PowerShell's `Invoke-WebRequest` / `Invoke-RestMethod` cmdlets, including the `curl`,
`wget`, `iwr` and `irm` aliases: cmdlet parameters are translated into the equivalent
curl request\.

### Remarks

Strings follow the [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell') rules (single quotes literal with `''`, double
            quotes with `$env:NAME` and backtick escapes, backtick line continuation) without the Windows
            `\"` step, because cmdlets receive strings directly. The first positional argument is the URI.

<b>Translated parameters</b> (names are case-insensitive and may be abbreviated when the prefix
            is unique): `-Uri`, `-Method` (`Head` becomes `-I`), `-CustomMethod`,
            `-Headers @{ 'Name' = 'Value'; ... }` (one `-H` per entry), `-Body` (a string is sent as
            is; a hashtable is URL-encoded as `name=value&...`, or appended to the query string for GET),
            `-ContentType`, `-InFile`, `-Form @{ name = 'value'; file = Get-Item ./a.png }`
            (multipart), `-OutFile`, `-UserAgent`, `-Proxy`, `-NoProxy`, `-TimeoutSec`,
            `-ConnectionTimeoutSeconds`, `-MaximumRedirection`, `-MaximumRetryCount`,
            `-RetryIntervalSec`, `-SkipCertificateCheck`, `-Authentication Bearer -Token ...`
            (also `OAuth`, and `(ConvertTo-SecureString 'x' -AsPlainText -Force)` tokens),
            `-HttpVersion`, `-SslProtocol`, `-PreserveAuthorizationOnRedirect` and `-Resume`.

<b>Ignored</b> (no effect on the request): `-UseBasicParsing`, `-DisableKeepAlive`,
            `-PassThru`, `-SkipHeaderValidation`, `-SkipHttpErrorCheck`,
            `-AllowUnencryptedAuthentication`, `-StatusCodeVariable`,
            `-ResponseHeadersVariable` and the common parameters such as `-Verbose`.

<b>Refused with an explanation</b>: `-Credential`, `-ProxyCredential`,
            `-UseDefaultCredentials` and `-ProxyUseDefaultCredentials` (credential objects only exist
            inside PowerShell; use `curl -u user:pass`), `-SessionVariable` / `-WebSession` (use
            `-c` / `-b` cookie files), `-Certificate` / `-CertificateThumbprint`, and values
            held in PowerShell variables such as `-Headers $h`.

<b>Cmdlet defaults kept:</b> redirects are followed up to 5 times (`-MaximumRedirection 0`
            turns that off), URLs are never globbed, the method stays GET when only `-Body` is given, and
            curl's default `User-Agent` is sent unless `-UserAgent` or a `User-Agent` header is
            given.

Example: `Invoke-RestMethod -Uri https://api.example.com -Method Post -ContentType 'application/json' -Body '{"a":1}'`
            sends `POST / HTTP/1.1` with `Content-Type: application/json` and the body `{"a":1}`,
            the same bytes as the [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix') command `curl -L --max-redirs 5 -X POST -H 'Content-Type: application/json' -d '{"a":1}' https://api.example.com`.

### See Also
- [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell')
- [The opposite direction: curl to Invoke\-RestMethod](CurlDotNet.Curl.ToPowershellCode(string).md 'CurlDotNet\.Curl\.ToPowershellCode\(string\)')
- [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)')

### Remarks

A command copied from a terminal is shell text, not curl syntax. bash, Windows `cmd.exe`
            and PowerShell each turn the same characters into different arguments, so
            `-d '{\"a\":1}'` sends `{\"a\":1}` (backslashes included) from bash but `{"a":1}`
            from PowerShell running `curl.exe`. Pick the flavor of the place the command came from, or
            leave it at [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') to let CurlDotNet recognise it.

The flavor in force is, in order: the one passed to
            [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)') / [Execute\(string, CurlFlavor\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.CurlFlavor\)') /
            [Validate\(string, CurlFlavor\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Validate\(string, CurlDotNet\.CurlFlavor\)'), the one set with
            [WithFlavor\(CurlFlavor\)](CurlDotNet.Core.CurlSettings.WithFlavor(CurlDotNet.CurlFlavor).md 'CurlDotNet\.Core\.CurlSettings\.WithFlavor\(CurlDotNet\.CurlFlavor\)'), and finally [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')
            (which is [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') unless you change it). After parsing,
            [Flavor](CurlDotNet.Core.CurlOptions.Flavor.md 'CurlDotNet\.Core\.CurlOptions\.Flavor') holds the flavor that was actually used, and
            [DetectFlavor\(string\)](CurlDotNet.Curl.DetectFlavor(string).md 'CurlDotNet\.Curl\.DetectFlavor\(string\)') tells you what [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') would choose.

Environment variables that are not set are left in the command exactly as typed in every
            flavor (real shells would substitute an empty string), so a missing variable shows up in the
            request and in [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)') instead of silently disappearing.

Example: `Curl.Execute("curl -d '{\"a\":1}' https://api.example.com", CurlFlavor.PowerShell)`
            sends the body `{"a":1}`; the same text with [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix') sends `{\"a\":1}`.

Full guide with a table of every difference:
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md

### See Also
- [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')
- [DetectFlavor\(string\)](CurlDotNet.Curl.DetectFlavor(string).md 'CurlDotNet\.Curl\.DetectFlavor\(string\)')
- [Flavor](CurlDotNet.Core.CurlOptions.Flavor.md 'CurlDotNet\.Core\.CurlOptions\.Flavor')
- [WithFlavor\(CurlFlavor\)](CurlDotNet.Core.CurlSettings.WithFlavor(CurlDotNet.CurlFlavor).md 'CurlDotNet\.Core\.CurlSettings\.WithFlavor\(CurlDotNet\.CurlFlavor\)')