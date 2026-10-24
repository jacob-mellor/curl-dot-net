/***************************************************************************
 * CurlFlavor - which shell a pasted curl command was written for
 *
 * The same text means different bytes in bash, cmd.exe, PowerShell and in
 * PowerShell's Invoke-WebRequest "curl" alias. A flavor names the rules
 * CurlDotNet uses to turn the text into curl's argument list.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

namespace CurlDotNet
{
    /// <summary>
    /// Names the shell a curl command string was written for, so CurlDotNet applies that shell's
    /// quoting, escaping, line-continuation and variable rules before reading the curl options.
    /// </summary>
    /// <remarks>
    /// <para>A command copied from a terminal is shell text, not curl syntax. bash, Windows <c>cmd.exe</c>
    /// and PowerShell each turn the same characters into different arguments, so
    /// <c>-d '{\"a\":1}'</c> sends <c>{\"a\":1}</c> (backslashes included) from bash but <c>{"a":1}</c>
    /// from PowerShell running <c>curl.exe</c>. Pick the flavor of the place the command came from, or
    /// leave it at <see cref="Auto"/> to let CurlDotNet recognise it.</para>
    /// <para>The flavor in force is, in order: the one passed to
    /// <see cref="Curl.ExecuteAsync(string, CurlFlavor)"/> / <see cref="Curl.Execute(string, CurlFlavor)"/> /
    /// <see cref="Curl.Validate(string, CurlFlavor)"/>, the one set with
    /// <see cref="Core.CurlSettings.WithFlavor(CurlFlavor)"/>, and finally <see cref="Curl.DefaultFlavor"/>
    /// (which is <see cref="Auto"/> unless you change it). After parsing,
    /// <see cref="Core.CurlOptions.Flavor"/> holds the flavor that was actually used, and
    /// <see cref="Curl.DetectFlavor(string)"/> tells you what <see cref="Auto"/> would choose.</para>
    /// <para>Environment variables that are not set are left in the command exactly as typed in every
    /// flavor (real shells would substitute an empty string), so a missing variable shows up in the
    /// request and in <see cref="Curl.Validate(string)"/> instead of silently disappearing.</para>
    /// <para>Example: <c>Curl.Execute("curl -d '{\"a\":1}' https://api.example.com", CurlFlavor.PowerShell)</c>
    /// sends the body <c>{"a":1}</c>; the same text with <see cref="Posix"/> sends <c>{\"a\":1}</c>.</para>
    /// <para>Full guide with a table of every difference:
    /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</para>
    /// </remarks>
    /// <seealso cref="Curl.DefaultFlavor"/>
    /// <seealso cref="Curl.DetectFlavor(string)"/>
    /// <seealso cref="Core.CurlOptions.Flavor"/>
    /// <seealso cref="Core.CurlSettings.WithFlavor(CurlFlavor)"/>
    public enum CurlFlavor
    {
        /// <summary>
        /// Recognise the flavor from telltales in the text, and read the command permissively (the default).
        /// </summary>
        /// <remarks>
        /// <para><b>Detection, first match wins:</b></para>
        /// <list type="number">
        /// <item><see cref="InvokeWebRequest"/> when the first word is <c>Invoke-WebRequest</c>,
        /// <c>Invoke-RestMethod</c>, <c>iwr</c>, <c>irm</c> or <c>wget</c>, or when the first word is
        /// <c>curl</c> (not <c>curl.exe</c>) or missing and the command contains a cmdlet parameter such as
        /// <c>-Uri</c>, <c>-Method</c>, <c>-Headers</c>, <c>-Body</c>, <c>-ContentType</c>, <c>-OutFile</c> or
        /// <c>-UseBasicParsing</c>, or a hashtable <c>@{</c>.</item>
        /// <item><see cref="PowerShell"/> when a line ends with a backtick, or the text contains
        /// <c>$env:</c>, a backtick-escaped quote <c>`"</c>, or the stop-parsing token <c>--%</c>.</item>
        /// <item><see cref="WindowsCmd"/> when a line ends with <c>^</c>, the text contains a caret-escaped
        /// quote <c>^"</c>, or it contains <c>%NAME%</c> where <c>NAME</c> is a set environment variable.</item>
        /// <item><see cref="Posix"/> otherwise.</item>
        /// </list>
        /// <para><b>Reading:</b> when the detected flavor is <see cref="InvokeWebRequest"/>, the cmdlet syntax is
        /// translated. When the text contains a construct only one shell's exact rules can read
        /// (<c>^"</c> for cmd.exe; <c>`"</c> or <c>--%</c> for PowerShell) that flavor's exact rules are used.
        /// Otherwise CurlDotNet uses its permissive reader, which accepts all three shells at once:
        /// single and double quotes both group; <c>\"</c>, <c>\'</c>, <c>\\</c>, <c>\$</c> and a backslash before
        /// a space are escapes, any other backslash is literal (so Windows paths survive); <c>""</c> inside double
        /// quotes is a quote; <c>\</c>, <c>^</c> and a backtick at the end of a line all continue it;
        /// <c>$VAR</c>, <c>${VAR}</c> and <c>$env:VAR</c> expand outside single quotes and <c>%VAR%</c> expands
        /// outside single quotes when the variable is set; and a single-quoted value containing <c>\"</c> but no
        /// bare <c>"</c> has its <c>\"</c> turned into <c>"</c>, because that is only ever written for Windows.
        /// A leading <c>curl</c>, <c>curl.exe</c> or a path to either is removed.</para>
        /// <para><see cref="Core.CurlOptions.Flavor"/> then reports the detected flavor, never <c>Auto</c>.</para>
        /// <para>Example: <c>curl -X POST https://api.example.com -H 'Content-Type: application/json' -d '{"a":1}'</c>
        /// is detected as <see cref="Posix"/> and sends <c>POST / HTTP/1.1</c> with
        /// <c>Content-Type: application/json</c> and the 7-byte body <c>{"a":1}</c>.</para>
        /// </remarks>
        /// <seealso cref="Curl.DetectFlavor(string)"/>
        /// <seealso cref="Curl.DefaultFlavor"/>
        /// <seealso href="https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md">curl flavors guide</seealso>
        Auto = 0,

        /// <summary>
        /// bash, zsh, sh and other POSIX shells (macOS and Linux terminals, Git Bash, WSL, CI scripts,
        /// "Copy as cURL (bash)" in browsers).
        /// </summary>
        /// <remarks>
        /// <para><b>Single quotes</b> are completely literal: nothing inside <c>'...'</c> is special, a backslash
        /// included, so <c>'{\"a\":1}'</c> stays the eight characters <c>{\"a\":1}</c>. A single quote cannot
        /// appear inside single quotes; write <c>'it'\''s'</c>.</para>
        /// <para><b>Double quotes</b> group text; inside them <c>\"</c>, <c>\\</c>, <c>\$</c> and <c>\`</c> give
        /// the second character, a backslash before a newline removes both, and any other backslash is kept
        /// (<c>"a\nb"</c> is the four characters <c>a\nb</c>). <c>$VAR</c> and <c>${VAR}</c> expand inside
        /// double quotes.</para>
        /// <para><b>ANSI-C quotes</b> <c>$'...'</c> (bash, zsh) understand <c>\n</c>, <c>\t</c>, <c>\r</c>,
        /// <c>\'</c>, <c>\"</c>, <c>\\</c>, <c>\xHH</c>, <c>\uHHHH</c> and octal <c>\NNN</c>; browsers use them when
        /// a copied request body contains a quote or a control character.</para>
        /// <para><b>Escape character:</b> backslash. Outside quotes a backslash makes the next character literal,
        /// whatever it is (<c>a\ b</c> is one argument with a space, <c>C:\temp</c> becomes <c>C:temp</c>).</para>
        /// <para><b>Line continuation:</b> a backslash as the last character of a line.</para>
        /// <para><b>Variables:</b> <c>$VAR</c> and <c>${VAR}</c> expand outside single quotes from the process
        /// environment; outside double quotes the value is split into separate arguments at spaces, tabs and
        /// newlines (so <c>$CURL_OPTS</c> holding <c>-s -L</c> is two options). <c>%VAR%</c> and
        /// <c>$env:VAR</c> are ordinary text.</para>
        /// <para><b>End of the command:</b> <c>#</c> at the start of a word begins a comment that runs to the end
        /// of the line, and an unquoted <c>|</c>, <c>;</c>, <c>&amp;</c>, <c>&gt;</c> or <c>&lt;</c> ends the curl
        /// command, so a pasted <c>| jq .</c>, <c>&amp;&amp; echo ok</c> or <c>2&gt;/dev/null</c> tail is ignored.</para>
        /// <para>Example: <c>curl -H "Authorization: Bearer $TOKEN" -d '{"a":1}' https://api.example.com</c> with
        /// <c>TOKEN=abc</c> sends <c>Authorization: Bearer abc</c> and the body <c>{"a":1}</c>.</para>
        /// </remarks>
        /// <seealso cref="WindowsCmd"/>
        /// <seealso cref="PowerShell"/>
        /// <seealso cref="Curl.ExecuteAsync(string, CurlFlavor)"/>
        Posix = 1,

        /// <summary>
        /// <c>curl.exe</c> run from the Windows Command Prompt (<c>cmd.exe</c>), batch files, and
        /// "Copy as cURL (cmd)" in browsers.
        /// </summary>
        /// <remarks>
        /// <para>The text goes through two stages, exactly as on Windows: cmd.exe first, then the C runtime that
        /// splits <c>curl.exe</c>'s command line into arguments.</para>
        /// <para><b>Single quotes</b> are ordinary characters: <c>'{"a":1}'</c> does not group anything and
        /// sends the quote characters (and loses the double quotes) just as real cmd.exe does.</para>
        /// <para><b>Double quotes</b> group text containing spaces. Inside a quoted part, <c>\"</c> is a literal
        /// quote and <c>""</c> is also a literal quote (C runtime rules); a run of backslashes is halved only
        /// when it is followed by a quote.</para>
        /// <para><b>Escape character:</b> caret <c>^</c>, outside double quotes: <c>^&amp;</c>, <c>^|</c>,
        /// <c>^&lt;</c>, <c>^&gt;</c> and <c>^^</c> give the second character, and <c>^"</c> gives a quote that
        /// cmd.exe does not treat as quoting but the C runtime still does. Inside double quotes a caret is literal.
        /// An unescaped <c>&amp;</c>, <c>|</c>, <c>&lt;</c> or <c>&gt;</c> outside double quotes ends the curl
        /// command (so <c>| findstr x</c> or <c>2&gt;nul</c> is ignored), exactly as cmd.exe would run it.</para>
        /// <para><b>Line continuation:</b> <c>^</c> as the last character of a line.</para>
        /// <para><b>Variables:</b> <c>%VAR%</c> expands everywhere, inside double quotes too, when the variable
        /// is set (an unset one stays <c>%VAR%</c>, as in an interactive cmd.exe). Expansion happens before
        /// carets are read, so a caret cannot stop it: <c>^%VAR%</c> still expands. <c>$VAR</c> is ordinary
        /// text.</para>
        /// <para><b>Backslash</b> is a path separator, never an escape on its own: <c>C:\temp\a.txt</c> stays as
        /// typed. It only matters directly before a double quote.</para>
        /// <para>Example: <c>curl -H "Content-Type: application/json" -d "{\"a\":1}" https://api.example.com</c>
        /// sends <c>Content-Type: application/json</c> and the body <c>{"a":1}</c>.</para>
        /// </remarks>
        /// <seealso cref="PowerShell"/>
        /// <seealso cref="Posix"/>
        /// <seealso cref="Curl.ExecuteAsync(string, CurlFlavor)"/>
        WindowsCmd = 2,

        /// <summary>
        /// <c>curl.exe</c> run from PowerShell 7 (<c>pwsh</c>) or Windows PowerShell 5.1 (always type
        /// <c>curl.exe</c> there; plain <c>curl</c> is the <see cref="InvokeWebRequest"/> alias in 5.1).
        /// </summary>
        /// <remarks>
        /// <para><b>Single quotes</b> are literal except that <c>''</c> inside them is one <c>'</c>
        /// (<c>'it''s'</c> is <c>it's</c>).</para>
        /// <para><b>Double quotes</b> expand <c>$env:NAME</c>, <c>${env:NAME}</c> and <c>$NAME</c> (looked up as
        /// an environment variable, because PowerShell's own session variables are not visible to .NET), and use
        /// backtick escapes: <c>`"</c> is a quote, <c>``</c> a backtick, <c>`$</c> a dollar sign, <c>`n</c>,
        /// <c>`r</c>, <c>`t</c>, <c>`0</c>, <c>`a</c>, <c>`b</c>, <c>`e</c>, <c>`f</c> and <c>`v</c> control
        /// characters, and <c>""</c> is one quote. <c>$(...)</c> sub-expressions are left as typed.</para>
        /// <para><b>Escape character:</b> backtick, inside double quotes and in bare words. A backslash is not a
        /// PowerShell escape.</para>
        /// <para><b>Line continuation:</b> a backtick as the last character of a line.</para>
        /// <para><b>Windows argument rule:</b> how an argument reaches curl.exe depends on the PowerShell version.
        /// Windows PowerShell 5.1 (and 7.2 and older) hands curl.exe a command line that its C runtime reads again,
        /// so commands written for it escape quotes with a backslash: <c>-d '{\"key\":\"value\"}'</c>, where
        /// <c>\"</c> arrives as <c>"</c> (and <c>\\\"</c> as <c>\"</c>). PowerShell 7.3+ passes arguments intact,
        /// so commands written for it use bare quotes: <c>-d '{"key":"value"}'</c>. CurlDotNet reads each argument
        /// the way it was evidently written: an argument with no bare double quote has every backslash-escaped
        /// quote turned into a quote (the 5.1 reading), and an argument that contains a bare double quote is sent
        /// as typed, <c>\"</c> included (the 7.3+ reading, which keeps JSON escapes such as
        /// <c>{"say":"\"hi\""}</c> intact). Either way the author's intended bytes are sent.</para>
        /// <para><b>Stop-parsing:</b> after the token <c>--%</c>, the rest of the line (up to an unquoted
        /// <c>|</c>) is not PowerShell syntax: it is handed to curl.exe as typed, with only <c>%NAME%</c>
        /// environment references expanded, and split with the <see cref="WindowsCmd"/> C runtime rules (double
        /// quotes group, <c>\"</c> is a quote).</para>
        /// <para><b>Variables:</b> <c>$env:NAME</c> as above; <c>%VAR%</c> does not expand (outside <c>--%</c>).</para>
        /// <para><b>End of the command:</b> <c>#</c> at the start of a word begins a comment. An unquoted
        /// <c>|</c>, <c>;</c>, <c>&gt;</c>, <c>&amp;&amp;</c> or a later <c>&amp;</c> ends the command, so a pasted
        /// <c>| ConvertFrom-Json</c> or <c>&amp;&amp; echo ok</c> tail is ignored. A leading <c>&amp;</c> call
        /// operator and a <c>$result =</c> assignment are removed.</para>
        /// <para>Example: <c>curl.exe -H 'Content-Type: application/json' -d '{\"a\":1}' https://api.example.com</c>
        /// sends <c>Content-Type: application/json</c> and the body <c>{"a":1}</c>.</para>
        /// </remarks>
        /// <seealso cref="InvokeWebRequest"/>
        /// <seealso cref="WindowsCmd"/>
        /// <seealso cref="Curl.ExecuteAsync(string, CurlFlavor)"/>
        PowerShell = 3,

        /// <summary>
        /// PowerShell's <c>Invoke-WebRequest</c> / <c>Invoke-RestMethod</c> cmdlets, including the <c>curl</c>,
        /// <c>wget</c>, <c>iwr</c> and <c>irm</c> aliases: cmdlet parameters are translated into the equivalent
        /// curl request.
        /// </summary>
        /// <remarks>
        /// <para>Strings follow the <see cref="PowerShell"/> rules (single quotes literal with <c>''</c>, double
        /// quotes with <c>$env:NAME</c> and backtick escapes, backtick line continuation) without the Windows
        /// <c>\"</c> step, because cmdlets receive strings directly. The first positional argument is the URI.</para>
        /// <para><b>Translated parameters</b> (names are case-insensitive and may be abbreviated when the prefix
        /// is unique): <c>-Uri</c>, <c>-Method</c> (<c>Head</c> becomes <c>-I</c>), <c>-CustomMethod</c>,
        /// <c>-Headers @{ 'Name' = 'Value'; ... }</c> (one <c>-H</c> per entry), <c>-Body</c> (a string is sent as
        /// is; a hashtable is URL-encoded as <c>name=value&amp;...</c>, or appended to the query string for GET),
        /// <c>-ContentType</c>, <c>-InFile</c>, <c>-Form @{ name = 'value'; file = Get-Item ./a.png }</c>
        /// (multipart), <c>-OutFile</c>, <c>-UserAgent</c>, <c>-Proxy</c>, <c>-NoProxy</c>, <c>-TimeoutSec</c>,
        /// <c>-ConnectionTimeoutSeconds</c>, <c>-MaximumRedirection</c>, <c>-MaximumRetryCount</c>,
        /// <c>-RetryIntervalSec</c>, <c>-SkipCertificateCheck</c>, <c>-Authentication Bearer -Token ...</c>
        /// (also <c>OAuth</c>, and <c>(ConvertTo-SecureString 'x' -AsPlainText -Force)</c> tokens),
        /// <c>-HttpVersion</c>, <c>-SslProtocol</c>, <c>-PreserveAuthorizationOnRedirect</c> and <c>-Resume</c>.</para>
        /// <para><b>Ignored</b> (no effect on the request): <c>-UseBasicParsing</c>, <c>-DisableKeepAlive</c>,
        /// <c>-PassThru</c>, <c>-SkipHeaderValidation</c>, <c>-SkipHttpErrorCheck</c>,
        /// <c>-AllowUnencryptedAuthentication</c>, <c>-StatusCodeVariable</c>,
        /// <c>-ResponseHeadersVariable</c> and the common parameters such as <c>-Verbose</c>.</para>
        /// <para><b>Refused with an explanation</b>: <c>-Credential</c>, <c>-ProxyCredential</c>,
        /// <c>-UseDefaultCredentials</c> and <c>-ProxyUseDefaultCredentials</c> (credential objects only exist
        /// inside PowerShell; use <c>curl -u user:pass</c>), <c>-SessionVariable</c> / <c>-WebSession</c> (use
        /// <c>-c</c> / <c>-b</c> cookie files), <c>-Certificate</c> / <c>-CertificateThumbprint</c>, and values
        /// held in PowerShell variables such as <c>-Headers $h</c>.</para>
        /// <para><b>Cmdlet defaults kept:</b> redirects are followed up to 5 times (<c>-MaximumRedirection 0</c>
        /// turns that off), URLs are never globbed, the method stays GET when only <c>-Body</c> is given, and
        /// curl's default <c>User-Agent</c> is sent unless <c>-UserAgent</c> or a <c>User-Agent</c> header is
        /// given.</para>
        /// <para>Example: <c>Invoke-RestMethod -Uri https://api.example.com -Method Post -ContentType 'application/json' -Body '{"a":1}'</c>
        /// sends <c>POST / HTTP/1.1</c> with <c>Content-Type: application/json</c> and the body <c>{"a":1}</c>,
        /// the same bytes as the <see cref="Posix"/> command <c>curl -L --max-redirs 5 -X POST -H 'Content-Type: application/json' -d '{"a":1}' https://api.example.com</c>.</para>
        /// </remarks>
        /// <seealso cref="PowerShell"/>
        /// <seealso cref="Curl.ToPowershellCode(string)">The opposite direction: curl to Invoke-RestMethod</seealso>
        /// <seealso cref="Curl.ExecuteAsync(string, CurlFlavor)"/>
        InvokeWebRequest = 4
    }
}
