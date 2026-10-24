/***************************************************************************
 * InvokeWebRequestTranslator - PowerShell's Invoke-WebRequest syntax as curl
 *
 * Reads Invoke-WebRequest / Invoke-RestMethod commands (and the curl, wget,
 * iwr and irm aliases) and produces the equivalent curl argument list, which
 * the CommandParser then turns into CurlOptions like any curl command. Used
 * for CurlFlavor.InvokeWebRequest.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Translates a PowerShell <c>Invoke-WebRequest</c> / <c>Invoke-RestMethod</c> command into the curl arguments
    /// that send the same request (see <see cref="CurlFlavor.InvokeWebRequest"/> for the parameter mapping).
    /// </summary>
    /// <remarks>
    /// <para>The output is an argument vector, not a command string, so no re-quoting is involved: each translated
    /// value is passed to the curl option parser exactly as PowerShell would have built it. For example
    /// <c>iwr https://x.example -Method Post -Headers @{ 'X-Id' = '7' } -Body 'a=1'</c> becomes
    /// <c>-g -L --max-redirs 5 -X POST -H "X-Id: 7" --data-raw a=1 --url https://x.example</c>.</para>
    /// </remarks>
    internal static class InvokeWebRequestTranslator
    {
        private enum ValueKind
        {
            Text,
            Hashtable,
            Expression,
            FileReference,
            FileContent
        }

        private sealed class PsValue
        {
            public ValueKind Kind;
            public string Text = string.Empty;
            public bool Bare;
            public List<KeyValuePair<string, PsValue>> Entries = new List<KeyValuePair<string, PsValue>>();

            public static PsValue OfText(string text, bool bare = false) => new PsValue { Kind = ValueKind.Text, Text = text, Bare = bare };
        }

        private sealed class PsToken
        {
            public string? Parameter;
            public PsValue? Attached;
            public PsValue? Value;
        }

        private enum ParameterKind
        {
            Value,
            Switch,
            IgnoredValue,
            IgnoredSwitch,
            Refused
        }

        /// <summary>Invoke-WebRequest / Invoke-RestMethod parameters and how each is handled.</summary>
        private static readonly Dictionary<string, ParameterKind> Parameters = new Dictionary<string, ParameterKind>(StringComparer.OrdinalIgnoreCase)
        {
            ["Uri"] = ParameterKind.Value,
            ["Method"] = ParameterKind.Value,
            ["CustomMethod"] = ParameterKind.Value,
            ["Headers"] = ParameterKind.Value,
            ["Body"] = ParameterKind.Value,
            ["ContentType"] = ParameterKind.Value,
            ["InFile"] = ParameterKind.Value,
            ["Form"] = ParameterKind.Value,
            ["OutFile"] = ParameterKind.Value,
            ["UserAgent"] = ParameterKind.Value,
            ["Proxy"] = ParameterKind.Value,
            ["NoProxy"] = ParameterKind.Switch,
            ["TimeoutSec"] = ParameterKind.Value,
            ["ConnectionTimeoutSeconds"] = ParameterKind.Value,
            ["MaximumRedirection"] = ParameterKind.Value,
            ["MaximumRetryCount"] = ParameterKind.Value,
            ["RetryIntervalSec"] = ParameterKind.Value,
            ["SkipCertificateCheck"] = ParameterKind.Switch,
            ["Authentication"] = ParameterKind.Value,
            ["Token"] = ParameterKind.Value,
            ["HttpVersion"] = ParameterKind.Value,
            ["SslProtocol"] = ParameterKind.Value,
            ["PreserveAuthorizationOnRedirect"] = ParameterKind.Switch,
            ["PreserveHttpMethodOnRedirect"] = ParameterKind.Switch,
            ["Resume"] = ParameterKind.Switch,
            ["TransferEncoding"] = ParameterKind.Value,
            ["UseBasicParsing"] = ParameterKind.IgnoredSwitch,
            ["DisableKeepAlive"] = ParameterKind.IgnoredSwitch,
            ["PassThru"] = ParameterKind.IgnoredSwitch,
            ["SkipHeaderValidation"] = ParameterKind.IgnoredSwitch,
            ["SkipHttpErrorCheck"] = ParameterKind.IgnoredSwitch,
            ["AllowUnencryptedAuthentication"] = ParameterKind.IgnoredSwitch,
            ["AllowInsecureRedirect"] = ParameterKind.IgnoredSwitch,
            ["StatusCodeVariable"] = ParameterKind.IgnoredValue,
            ["ResponseHeadersVariable"] = ParameterKind.IgnoredValue,
            ["OperationTimeoutSeconds"] = ParameterKind.IgnoredValue,
            ["Verbose"] = ParameterKind.IgnoredSwitch,
            ["Debug"] = ParameterKind.IgnoredSwitch,
            ["ErrorAction"] = ParameterKind.IgnoredValue,
            ["WarningAction"] = ParameterKind.IgnoredValue,
            ["InformationAction"] = ParameterKind.IgnoredValue,
            ["ProgressAction"] = ParameterKind.IgnoredValue,
            ["ErrorVariable"] = ParameterKind.IgnoredValue,
            ["WarningVariable"] = ParameterKind.IgnoredValue,
            ["InformationVariable"] = ParameterKind.IgnoredValue,
            ["OutVariable"] = ParameterKind.IgnoredValue,
            ["OutBuffer"] = ParameterKind.IgnoredValue,
            ["PipelineVariable"] = ParameterKind.IgnoredValue,
            ["Credential"] = ParameterKind.Refused,
            ["ProxyCredential"] = ParameterKind.Refused,
            ["UseDefaultCredentials"] = ParameterKind.Refused,
            ["ProxyUseDefaultCredentials"] = ParameterKind.Refused,
            ["SessionVariable"] = ParameterKind.Refused,
            ["WebSession"] = ParameterKind.Refused,
            ["Certificate"] = ParameterKind.Refused,
            ["CertificateThumbprint"] = ParameterKind.Refused,
            ["FollowRelLink"] = ParameterKind.Refused,
            ["MaximumFollowRelLink"] = ParameterKind.Refused,
        };

        /// <summary>PowerShell's own short aliases for parameters.</summary>
        private static readonly Dictionary<string, string> Aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Url"] = "Uri",
            ["vb"] = "Verbose",
            ["db"] = "Debug",
            ["ea"] = "ErrorAction",
            ["wa"] = "WarningAction",
            ["infa"] = "InformationAction",
            ["proga"] = "ProgressAction",
            ["ev"] = "ErrorVariable",
            ["wv"] = "WarningVariable",
            ["iv"] = "InformationVariable",
            ["ov"] = "OutVariable",
            ["ob"] = "OutBuffer",
            ["pv"] = "PipelineVariable",
            ["SessionVar"] = "SessionVariable",
            ["SV"] = "SessionVariable",
            ["SSL"] = "SslProtocol",
        };

        /// <summary>Parameters whose presence in a <c>curl</c> command means it is really Invoke-WebRequest.</summary>
        private static readonly HashSet<string> Telltales = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Uri", "Method", "Headers", "Body", "ContentType", "OutFile", "UseBasicParsing", "InFile", "Form",
            "TimeoutSec", "MaximumRedirection", "SkipCertificateCheck", "UserAgent", "Credential", "WebSession",
            "SessionVariable", "Authentication", "Token", "DisableKeepAlive", "PassThru", "CustomMethod",
            "ProxyCredential", "UseDefaultCredentials", "SkipHttpErrorCheck", "MaximumRetryCount", "RetryIntervalSec",
            "SkipHeaderValidation", "PreserveAuthorizationOnRedirect", "ConnectionTimeoutSeconds",
        };

        /// <summary>Cmdlet names and aliases that start an Invoke-WebRequest command.</summary>
        private static readonly HashSet<string> CmdletNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Invoke-WebRequest", "Invoke-RestMethod", "iwr", "irm", "wget", "curl"
        };

        private static readonly HashSet<string> FileCmdlets = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Get-Item", "gi", "Get-ChildItem", "gci", "dir", "ls", "Resolve-Path", "rvpa"
        };

        private static readonly HashSet<string> ContentCmdlets = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Get-Content", "gc", "cat", "type"
        };

        private static readonly string[] Methods = { "Default", "Get", "Head", "Post", "Put", "Delete", "Trace", "Options", "Merge", "Patch" };

        /// <summary>
        /// True when the text is an Invoke-WebRequest command: it starts with <c>Invoke-WebRequest</c>,
        /// <c>Invoke-RestMethod</c>, <c>iwr</c>, <c>irm</c> or <c>wget</c>, or it starts with <c>curl</c> (not
        /// <c>curl.exe</c> or a path) or an option and uses a cmdlet parameter such as <c>-Uri</c> or <c>-Headers</c>.
        /// </summary>
        internal static bool LooksLikeInvokeWebRequest(string command)
        {
            var text = CommandParser.StripPowerShellAssignment(command).TrimStart();
            if (text.StartsWith("&", StringComparison.Ordinal))
            {
                text = text.Substring(1).TrimStart();
            }

            var end = 0;
            while (end < text.Length && !char.IsWhiteSpace(text[end])) end++;
            var first = text.Substring(0, end).Trim('\'', '"');
            var name = CmdletName(first);
            if (name != null && !name.Equals("curl", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (name == null && (first.IndexOf('/') >= 0 || first.IndexOf('\\') >= 0 ||
                                 first.Equals("curl.exe", StringComparison.OrdinalIgnoreCase)))
            {
                return false; // a real curl program
            }

            // only unquoted words count: -d 'text -Body x' is an ordinary curl command
            foreach (Match match in Regex.Matches(BlankOutQuotedText(text), @"(?<=^|\s)-(?<name>[A-Za-z]+)(?=[\s:]|$)"))
            {
                if (Telltales.Contains(match.Groups["name"].Value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Replaces the inside of every single- or double-quoted part with spaces (a backslash or backtick before a
        /// double quote does not end it), so word-level telltales are only found in unquoted text.
        /// </summary>
        private static string BlankOutQuotedText(string text)
        {
            var sb = new StringBuilder(text.Length);
            var quote = '\0';
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (quote == '\0')
                {
                    if (c == '\'' || c == '"')
                    {
                        quote = c;
                    }

                    sb.Append(c);
                    continue;
                }

                if (quote == '"' && (c == '\\' || c == '`') && i + 1 < text.Length)
                {
                    sb.Append("  ");
                    i++;
                    continue;
                }

                if (c == quote)
                {
                    quote = '\0';
                    sb.Append(c);
                    continue;
                }

                sb.Append(char.IsWhiteSpace(c) ? c : ' ');
            }

            return sb.ToString();
        }

        /// <summary>The cmdlet name a first word stands for (module-qualified names allowed), or null.</summary>
        private static string? CmdletName(string word)
        {
            var slash = word.LastIndexOf('\\');
            var name = slash >= 0 ? word.Substring(slash + 1) : word;
            return CmdletNames.Contains(name) ? name : null;
        }

        /// <summary>
        /// Translates an Invoke-WebRequest command into curl arguments (without the program name).
        /// </summary>
        /// <exception cref="CurlInvalidCommandException">A parameter is unknown, ambiguous, refused or has a value
        /// CurlDotNet cannot evaluate; the message names the parameter and the fix.</exception>
        internal static List<string> Translate(string command)
        {
            var text = CommandParser.StripPowerShellAssignment(command).Trim();
            var tokens = Tokenize(text, command, out var end); // a "| ConvertFrom-Json" tail does not change the request
            if (end < text.Length && text[end] == '|')
            {
                var rest = text.Substring(end + 1).TrimStart();
                var restWord = rest.Split(new[] { ' ', '\t', '\r', '\n' }, 2)[0];
                if (CmdletName(restWord) != null)
                {
                    throw Error($"The request is the second element of a pipeline (... | {restWord}), so its input comes from the command before the |",
                        $"Pass that value with a parameter instead, for example {restWord} -Uri https://... -Method Post -Body '...'.",
                        "|", command);
                }
            }

            var start = 0;
            var cmdlet = "Invoke-WebRequest";
            if (start < tokens.Count && tokens[start].Value != null && tokens[start].Value!.Bare && tokens[start].Value!.Text == "&")
            {
                start++;
            }

            if (start < tokens.Count && tokens[start].Value != null && tokens[start].Value!.Kind == ValueKind.Text)
            {
                var name = CmdletName(tokens[start].Value!.Text);
                if (name != null)
                {
                    cmdlet = name;
                    start++;
                }
            }

            var values = new Dictionary<string, PsValue>(StringComparer.OrdinalIgnoreCase);
            var switches = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            var positional = new List<PsValue>();
            for (var i = start; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token.Parameter == null)
                {
                    positional.Add(token.Value!);
                    continue;
                }

                var name = ResolveParameter(token.Parameter, cmdlet, command);
                var kind = Parameters[name];
                if (values.ContainsKey(name) || switches.ContainsKey(name))
                {
                    throw Error($"The parameter -{name} is given twice", $"Keep only one -{name}; PowerShell itself rejects a repeated parameter.", "-" + token.Parameter, command);
                }

                if (kind == ParameterKind.Switch || kind == ParameterKind.IgnoredSwitch ||
                    (kind == ParameterKind.Refused && IsSwitchName(name)))
                {
                    var on = token.Attached == null || !(token.Attached.Text.Equals("False", StringComparison.OrdinalIgnoreCase) || token.Attached.Text == "0");
                    switches[name] = on;
                    continue;
                }

                PsValue value;
                if (token.Attached != null)
                {
                    value = token.Attached;
                }
                else if (i + 1 < tokens.Count && tokens[i + 1].Parameter == null)
                {
                    value = tokens[++i].Value!;
                }
                else
                {
                    throw Error($"The parameter -{name} needs a value, but nothing follows it",
                        $"Write it as -{name} <value>, quoting the value if it contains spaces.", "-" + token.Parameter, command);
                }

                values[name] = value;
            }

            RefuseUnsupported(values, switches, command);

            if (!values.ContainsKey("Uri") && positional.Count > 0)
            {
                values["Uri"] = positional[0];
                positional.RemoveAt(0);
            }

            if (positional.Count > 0)
            {
                throw Error($"\"{Describe(positional[0])}\" is an extra positional argument",
                    "Invoke-WebRequest takes only one positional argument, the URI; name every other value with its parameter, for example -Body '...'.",
                    Describe(positional[0]), command);
            }

            if (!values.TryGetValue("Uri", out var uriValue))
            {
                throw Error($"The {cmdlet} command has no URI",
                    "Add -Uri https://example.com/path, or put the URL right after the cmdlet name.", cmdlet, command);
            }

            return BuildCurlArguments(values, switches, RequireText(uriValue, "Uri", command), command);
        }

        private static bool IsSwitchName(string name) =>
            name == "UseDefaultCredentials" || name == "ProxyUseDefaultCredentials" || name == "FollowRelLink";

        /// <summary>Resolves a typed parameter name (case-insensitive, alias or unique prefix) to its canonical name.</summary>
        private static string ResolveParameter(string typed, string cmdlet, string command)
        {
            if (Aliases.TryGetValue(typed, out var aliased))
            {
                return aliased;
            }

            if (Parameters.ContainsKey(typed))
            {
                return Parameters.Keys.First(k => k.Equals(typed, StringComparison.OrdinalIgnoreCase));
            }

            var candidates = Parameters.Keys.Where(k => k.StartsWith(typed, StringComparison.OrdinalIgnoreCase)).ToList();
            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            var hint = cmdlet.Equals("wget", StringComparison.OrdinalIgnoreCase)
                ? " If this is a GNU wget command (not PowerShell's wget alias), rewrite it as a curl command."
                : cmdlet.Equals("curl", StringComparison.OrdinalIgnoreCase)
                    ? " If this is a real curl command, pass CurlFlavor.Posix, WindowsCmd or PowerShell instead (or type curl.exe)."
                    : string.Empty;

            if (candidates.Count > 1)
            {
                throw Error($"The parameter name -{typed} is ambiguous: it could be " + string.Join(", ", candidates.Select(c => "-" + c)),
                    "Type more of the name so only one parameter matches." + hint, "-" + typed, command);
            }
            throw Error($"{cmdlet} has no parameter -{typed} that CurlDotNet can translate",
                "Check the spelling; the translated parameters are -Uri, -Method, -CustomMethod, -Headers, -Body, -ContentType, -InFile, " +
                "-Form, -OutFile, -UserAgent, -Proxy, -NoProxy, -TimeoutSec, -ConnectionTimeoutSeconds, -MaximumRedirection, " +
                "-MaximumRetryCount, -RetryIntervalSec, -SkipCertificateCheck, -Authentication, -Token, -HttpVersion, -SslProtocol, " +
                "-PreserveAuthorizationOnRedirect, -PreserveHttpMethodOnRedirect, -Resume and -TransferEncoding." + hint,
                "-" + typed, command);
        }

        /// <summary>Throws the explanation for parameters that only work inside a PowerShell session.</summary>
        private static void RefuseUnsupported(Dictionary<string, PsValue> values, Dictionary<string, bool> switches, string command)
        {
            if (values.ContainsKey("Credential"))
            {
                throw Error("-Credential takes a PSCredential object (from Get-Credential), which only exists inside a PowerShell session and cannot be read from a command string",
                    "Use a curl command with -u user:pass (for example curl -u alice:secret https://...), " +
                    "or send the header yourself with -Headers @{ Authorization = 'Basic <base64 of user:pass>' }.", "-Credential", command);
            }

            if (values.ContainsKey("ProxyCredential"))
            {
                throw Error("-ProxyCredential takes a PSCredential object, which only exists inside a PowerShell session",
                    "Use a curl command with -x http://proxy:8080 -U user:pass for proxy authentication.", "-ProxyCredential", command);
            }

            if ((switches.TryGetValue("UseDefaultCredentials", out var useDefault) && useDefault) ||
                (switches.TryGetValue("ProxyUseDefaultCredentials", out var proxyDefault) && proxyDefault))
            {
                throw Error("-UseDefaultCredentials / -ProxyUseDefaultCredentials sign in as the current Windows user, which a translated command cannot express",
                    "Use a curl command with --negotiate -u : (or --ntlm -u user:pass), or pass a CurlSettings with your own HttpMessageHandler that has UseDefaultCredentials = true.",
                    "-UseDefaultCredentials", command);
            }

            if (values.ContainsKey("SessionVariable") || values.ContainsKey("WebSession"))
            {
                throw Error("-SessionVariable / -WebSession keep cookies in a PowerShell WebRequestSession variable, which does not exist outside PowerShell",
                    "Keep cookies across requests with curl's cookie jar instead: -c cookies.txt on the first request and -b cookies.txt on the next ones.",
                    values.ContainsKey("WebSession") ? "-WebSession" : "-SessionVariable", command);
            }

            if (values.ContainsKey("Certificate") || values.ContainsKey("CertificateThumbprint"))
            {
                throw Error("-Certificate / -CertificateThumbprint pick a client certificate from a PowerShell object or the Windows certificate store",
                    "Use a curl command with --cert client.pem --key client.key (or --cert client.p12:password --cert-type P12).",
                    "-Certificate", command);
            }

            if ((switches.TryGetValue("FollowRelLink", out var follow) && follow) || values.ContainsKey("MaximumFollowRelLink"))
            {
                throw Error("-FollowRelLink makes Invoke-RestMethod fetch every page named in Link headers, which is a loop over several requests",
                    "Run the first request, read result.Headers[\"Link\"] and request the next URL yourself.", "-FollowRelLink", command);
            }
        }

        /// <summary>Builds curl's argument vector for the translated request.</summary>
        private static List<string> BuildCurlArguments(Dictionary<string, PsValue> values, Dictionary<string, bool> switches, string uri, string command)
        {
            var args = new List<string> { "-g" }; // Invoke-WebRequest never expands [] or {} in URLs

            var redirects = 5;
            if (values.TryGetValue("MaximumRedirection", out var maxRedirects))
            {
                redirects = RequireInteger(maxRedirects, "MaximumRedirection", command);
            }

            if (redirects > 0)
            {
                args.Add("-L");
                args.Add("--max-redirs");
                args.Add(redirects.ToString(CultureInfo.InvariantCulture));
            }

            string? method = null;
            if (values.TryGetValue("CustomMethod", out var customMethod))
            {
                method = RequireText(customMethod, "CustomMethod", command).ToUpperInvariant();
            }
            else if (values.TryGetValue("Method", out var methodValue))
            {
                var typed = RequireText(methodValue, "Method", command);
                var known = Methods.FirstOrDefault(m => m.Equals(typed, StringComparison.OrdinalIgnoreCase));
                if (known == null)
                {
                    throw Error($"-Method {typed} is not one of PowerShell's WebRequestMethod values (Get, Head, Post, Put, Delete, Trace, Options, Merge, Patch)",
                        $"Use -CustomMethod {typed} for any other HTTP method.", "-Method", command);
                }

                method = known == "Default" ? null : known.ToUpperInvariant();
            }

            var hasBody = values.ContainsKey("Body") || values.ContainsKey("InFile");
            if (values.ContainsKey("Body") && values.ContainsKey("Form"))
            {
                throw Error("-Body and -Form are both given", "Send either a raw body (-Body) or a multipart form (-Form), as PowerShell does.", "-Form", command);
            }

            if (values.ContainsKey("Body") && values.ContainsKey("InFile"))
            {
                throw Error("-Body and -InFile are both given", "Send the body either from a string (-Body) or from a file (-InFile), not both.", "-InFile", command);
            }

            // GET with a hashtable body puts the fields in the query string
            if (values.TryGetValue("Body", out var bodyValue) && bodyValue.Kind == ValueKind.Hashtable &&
                (method == null || method == "GET"))
            {
                var query = EncodeHashtable(bodyValue, "Body", command);
                if (query.Length > 0)
                {
                    uri += (uri.IndexOf('?') >= 0 ? "&" : "?") + query;
                }

                values.Remove("Body");
                hasBody = values.ContainsKey("InFile");
            }

            if (method == "HEAD")
            {
                args.Add("-I");
            }
            else if (method != null)
            {
                args.Add("-X");
                args.Add(method);
            }
            else if (hasBody)
            {
                // Invoke-WebRequest keeps GET when only -Body is given; curl would switch to POST
                args.Add("-X");
                args.Add("GET");
            }

            if (values.TryGetValue("Headers", out var headers))
            {
                if (headers.Kind != ValueKind.Hashtable)
                {
                    throw Error("-Headers must be a hashtable such as @{ 'Accept' = 'application/json'; 'X-Id' = '7' }, but it is " + Describe(headers),
                        "Write the headers inline as a hashtable; a PowerShell variable holding one cannot be read from a command string.", "-Headers", command);
                }

                foreach (var entry in headers.Entries)
                {
                    var headerValue = RequireText(entry.Value, "Headers['" + entry.Key + "']", command);
                    args.Add("-H");
                    args.Add(headerValue.Length == 0 ? entry.Key + ";" : entry.Key + ": " + headerValue);
                }
            }

            if (values.TryGetValue("ContentType", out var contentType))
            {
                args.Add("-H");
                args.Add("Content-Type: " + RequireText(contentType, "ContentType", command));
            }

            AddAuthentication(values, args, command);

            if (values.TryGetValue("UserAgent", out var userAgent))
            {
                args.Add("-A");
                args.Add(RequireText(userAgent, "UserAgent", command));
            }

            if (values.TryGetValue("Body", out bodyValue))
            {
                switch (bodyValue.Kind)
                {
                    case ValueKind.Hashtable:
                        args.Add("--data-raw");
                        args.Add(EncodeHashtable(bodyValue, "Body", command));
                        break;
                    case ValueKind.FileContent:
                        args.Add("--data-binary");
                        args.Add("@" + bodyValue.Text);
                        break;
                    default:
                        args.Add("--data-raw");
                        args.Add(RequireText(bodyValue, "Body", command));
                        break;
                }
            }

            if (values.TryGetValue("InFile", out var inFile))
            {
                args.Add("--data-binary");
                args.Add("@" + RequireText(inFile, "InFile", command));
            }

            if (values.TryGetValue("Form", out var form))
            {
                if (form.Kind != ValueKind.Hashtable)
                {
                    throw Error("-Form must be a hashtable such as @{ name = 'Ada'; avatar = Get-Item ./ada.png }, but it is " + Describe(form),
                        "Write the form fields inline as a hashtable.", "-Form", command);
                }

                foreach (var entry in form.Entries)
                {
                    if (entry.Value.Kind == ValueKind.FileReference)
                    {
                        args.Add("-F");
                        args.Add(entry.Key + "=@" + entry.Value.Text);
                    }
                    else
                    {
                        args.Add("--form-string");
                        args.Add(entry.Key + "=" + RequireText(entry.Value, "Form['" + entry.Key + "']", command));
                    }
                }
            }

            if (values.TryGetValue("OutFile", out var outFile))
            {
                args.Add("-o");
                args.Add(RequireText(outFile, "OutFile", command));
            }

            if (values.TryGetValue("Proxy", out var proxy))
            {
                args.Add("-x");
                args.Add(RequireText(proxy, "Proxy", command));
            }

            if (switches.TryGetValue("NoProxy", out var noProxy) && noProxy)
            {
                args.Add("--noproxy");
                args.Add("*");
            }

            if (values.TryGetValue("TimeoutSec", out var timeout))
            {
                var seconds = RequireInteger(timeout, "TimeoutSec", command);
                if (seconds > 0)
                {
                    args.Add("-m");
                    args.Add(seconds.ToString(CultureInfo.InvariantCulture));
                }
            }

            if (values.TryGetValue("ConnectionTimeoutSeconds", out var connectTimeout))
            {
                var seconds = RequireInteger(connectTimeout, "ConnectionTimeoutSeconds", command);
                if (seconds > 0)
                {
                    args.Add("--connect-timeout");
                    args.Add(seconds.ToString(CultureInfo.InvariantCulture));
                }
            }

            if (values.TryGetValue("MaximumRetryCount", out var retries))
            {
                args.Add("--retry");
                args.Add(RequireInteger(retries, "MaximumRetryCount", command).ToString(CultureInfo.InvariantCulture));
            }

            if (values.TryGetValue("RetryIntervalSec", out var retryDelay))
            {
                args.Add("--retry-delay");
                args.Add(RequireInteger(retryDelay, "RetryIntervalSec", command).ToString(CultureInfo.InvariantCulture));
            }

            if (switches.TryGetValue("SkipCertificateCheck", out var insecure) && insecure)
            {
                args.Add("-k");
            }

            if (switches.TryGetValue("PreserveAuthorizationOnRedirect", out var trusted) && trusted)
            {
                args.Add("--location-trusted");
            }

            if (switches.TryGetValue("PreserveHttpMethodOnRedirect", out var keepMethod) && keepMethod)
            {
                args.Add("--post301");
                args.Add("--post302");
                args.Add("--post303");
            }

            if (switches.TryGetValue("Resume", out var resume) && resume)
            {
                args.Add("-C");
                args.Add("-");
            }

            if (values.TryGetValue("TransferEncoding", out var transferEncoding))
            {
                args.Add("-H");
                args.Add("Transfer-Encoding: " + RequireText(transferEncoding, "TransferEncoding", command));
            }

            if (values.TryGetValue("HttpVersion", out var httpVersion))
            {
                args.Add(HttpVersionOption(RequireText(httpVersion, "HttpVersion", command), command));
            }

            if (values.TryGetValue("SslProtocol", out var sslProtocol))
            {
                var option = SslProtocolOption(RequireText(sslProtocol, "SslProtocol", command), command);
                if (option != null)
                {
                    args.Add(option);
                }
            }

            args.Add("--url");
            args.Add(uri);
            return args;
        }

        private static void AddAuthentication(Dictionary<string, PsValue> values, List<string> args, string command)
        {
            values.TryGetValue("Token", out var token);
            var scheme = "Bearer";
            if (values.TryGetValue("Authentication", out var authentication))
            {
                scheme = RequireText(authentication, "Authentication", command);
                if (scheme.Equals("None", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                {
                    throw Error("-Authentication Basic needs -Credential, a PSCredential object that only exists inside PowerShell",
                        "Use a curl command with -u user:pass, which sends the same Basic Authorization header.", "-Authentication", command);
                }

                if (!scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) && !scheme.Equals("OAuth", StringComparison.OrdinalIgnoreCase))
                {
                    throw Error($"-Authentication {scheme} is not one of None, Basic, Bearer or OAuth",
                        "Use -Authentication Bearer -Token <token>, or send the header yourself with -Headers @{ Authorization = '...' }.", "-Authentication", command);
                }

                if (token == null)
                {
                    throw Error($"-Authentication {scheme} needs -Token",
                        $"Add -Token <token> (a plain string, or (ConvertTo-SecureString '<token>' -AsPlainText -Force)).", "-Authentication", command);
                }
            }

            if (token != null)
            {
                args.Add("-H");
                args.Add("Authorization: Bearer " + RequireText(token, "Token", command));
            }
        }

        private static string HttpVersionOption(string version, string command)
        {
            switch (version.Trim())
            {
                case "1.0": return "--http1.0";
                case "1.1": return "--http1.1";
                case "2":
                case "2.0": return "--http2";
                case "3":
                case "3.0": return "--http3";
                default:
                    throw Error($"-HttpVersion {version} is not an HTTP version", "Use 1.0, 1.1, 2.0 or 3.0.", "-HttpVersion", command);
            }
        }

        private static string? SslProtocolOption(string protocols, string command)
        {
            string? lowest = null;
            var rank = int.MaxValue;
            foreach (var part in protocols.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string option;
                int order;
                switch (part.Trim().ToLowerInvariant())
                {
                    case "default":
                    case "none": continue;
                    case "tls": option = "--tlsv1.0"; order = 0; break;
                    case "tls11": option = "--tlsv1.1"; order = 1; break;
                    case "tls12": option = "--tlsv1.2"; order = 2; break;
                    case "tls13": option = "--tlsv1.3"; order = 3; break;
                    default:
                        throw Error($"-SslProtocol {part} is not one of Default, Tls, Tls11, Tls12 or Tls13",
                            "Use those names, separated by commas for several (the lowest becomes the minimum version).", "-SslProtocol", command);
                }

                if (order < rank)
                {
                    rank = order;
                    lowest = option;
                }
            }

            return lowest;
        }

        /// <summary>Encodes a hashtable as <c>name=value&amp;...</c> with <see cref="WebUtility.UrlEncode"/>, like Invoke-WebRequest.</summary>
        private static string EncodeHashtable(PsValue table, string parameter, string command)
        {
            var parts = new List<string>();
            foreach (var entry in table.Entries)
            {
                parts.Add(WebUtility.UrlEncode(entry.Key) + "=" + WebUtility.UrlEncode(RequireText(entry.Value, parameter + "['" + entry.Key + "']", command)));
            }

            return string.Join("&", parts);
        }

        private static string RequireText(PsValue value, string parameter, string command)
        {
            switch (value.Kind)
            {
                case ValueKind.Text:
                case ValueKind.FileReference:
                    return value.Text;
                case ValueKind.Hashtable:
                    throw Error($"-{parameter} expects a single value but is given a hashtable",
                        $"Pass a string, for example -{parameter} 'value'.", "-" + parameter, command);
                case ValueKind.FileContent:
                    throw Error($"-{parameter} is given the contents of the file {value.Text}, which only -Body accepts",
                        $"Pass the value as a string, or use -InFile {value.Text} to send a file as the body.", "-" + parameter, command);
                default:
                    throw Error($"-{parameter} is given the PowerShell expression {value.Text}, which CurlDotNet cannot evaluate outside PowerShell",
                        $"Paste the literal value instead (for example -{parameter} 'value'); for JSON bodies write the JSON text, not (... | ConvertTo-Json).",
                        value.Text, command);
            }
        }

        private static int RequireInteger(PsValue value, string parameter, string command)
        {
            var text = RequireText(value, parameter, command);
            if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number) || number < 0)
            {
                throw Error($"-{parameter} must be a whole number of 0 or more, but is \"{text}\"",
                    $"Write it as -{parameter} 30.", "-" + parameter, command);
            }

            return number;
        }

        private static string Describe(PsValue value)
        {
            switch (value.Kind)
            {
                case ValueKind.Hashtable: return "a hashtable";
                case ValueKind.Expression: return "the PowerShell expression " + value.Text;
                case ValueKind.FileReference: return "a file " + value.Text;
                case ValueKind.FileContent: return "the contents of " + value.Text;
                default: return value.Text;
            }
        }

        private static CurlInvalidCommandException Error(string problem, string fix, string part, string command)
        {
            var near = string.IsNullOrEmpty(part) || problem.IndexOf(part, StringComparison.Ordinal) >= 0
                ? string.Empty
                : $", near \"{ErrorText.Preview(part, 40)}\"";
            return new CurlInvalidCommandException(
                CurlException.Compose($"{problem} (reading the command as Invoke-WebRequest syntax{near}).", fix, DocLinks.HowToFlavors),
                part, command);
        }

        #region PowerShell syntax reader

        /// <summary>
        /// Reads cmdlet-call syntax: <c>-Name</c> / <c>-Name:value</c> parameters, quoted and bare values,
        /// <c>@{ ... }</c> hashtables, <c>( ... )</c> expressions, here-strings, backtick continuations and
        /// <c>#</c> comments, up to an unquoted <c>|</c> or <c>;</c>.
        /// </summary>
        private static List<PsToken> Tokenize(string text, string command, out int end)
        {
            var tokens = new List<PsToken>();
            var i = 0;
            while (true)
            {
                i = SkipBlank(text, i, true);
                if (i >= text.Length || text[i] == '|' || text[i] == ';')
                {
                    end = i;
                    break;
                }

                if (text[i] == '-' && i + 1 < text.Length && char.IsLetter(text[i + 1]))
                {
                    var start = ++i;
                    while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_')) i++;
                    var token = new PsToken { Parameter = text.Substring(start, i - start) };
                    if (i < text.Length && text[i] == ':')
                    {
                        var valueStart = SkipBlank(text, i + 1, false);
                        if (valueStart >= text.Length || text[valueStart] == '|' || text[valueStart] == ';' || text[valueStart] == '\r' || text[valueStart] == '\n')
                        {
                            throw Error($"The parameter -{token.Parameter}: ends with a colon but no value follows it",
                                $"Write it as -{token.Parameter}:value or -{token.Parameter} value.", "-" + token.Parameter + ":", command);
                        }

                        i++;
                        token.Attached = ParseValue(text, ref i, false, command);
                    }

                    tokens.Add(token);
                    continue;
                }

                var before = i;
                var value = ParseValue(text, ref i, false, command);
                if (i == before)
                {
                    throw Error($"The character '{text[i]}' at position {i + 1} does not start a value",
                        "Remove it, or quote the value it belongs to (a URL containing ( or ) is safest in single quotes).",
                        text.Substring(i), command);
                }

                tokens.Add(new PsToken { Value = value });
            }

            return tokens;
        }

        /// <summary>Skips spaces, tabs, backtick line continuations, comments and (when allowed) line breaks.</summary>
        private static int SkipBlank(string text, int i, bool newlines)
        {
            while (i < text.Length)
            {
                var c = text[i];
                if (c == ' ' || c == '\t' || (newlines && (c == '\r' || c == '\n')))
                {
                    i++;
                }
                else if (c == '`' && i + 1 < text.Length && (text[i + 1] == '\r' || text[i + 1] == '\n'))
                {
                    i += text[i + 1] == '\r' && i + 2 < text.Length && text[i + 2] == '\n' ? 3 : 2;
                }
                else if (c == '#')
                {
                    while (i < text.Length && text[i] != '\n' && text[i] != '\r') i++;
                }
                else
                {
                    break;
                }
            }

            return i;
        }

        private static PsValue ParseValue(string text, ref int i, bool inHashtable, string command)
        {
            i = SkipBlank(text, i, false);
            if (i >= text.Length)
            {
                return PsValue.OfText(string.Empty);
            }

            var c = text[i];
            if (c == '[')
            {
                // a type cast such as [ordered] or [string] does not change what is sent
                var close = text.IndexOf(']', i);
                if (close > i)
                {
                    i = close + 1;
                    return ParseValue(text, ref i, inHashtable, command);
                }
            }

            if (c == '@' && i + 1 < text.Length && text[i + 1] == '{')
            {
                return ParseHashtable(text, ref i, command);
            }

            if ((c == '@' || c == '$') && i + 1 < text.Length && text[i + 1] == '(')
            {
                // @( ... ) array and $( ... ) sub-expressions are code, kept for the error message
                var end = MatchingParen(text, i + 1, command);
                var raw = text.Substring(i, end - i);
                i = end;
                return new PsValue { Kind = ValueKind.Expression, Text = raw };
            }

            if (c == '(')
            {
                var end = MatchingParen(text, i, command);
                var inner = text.Substring(i + 1, Math.Max(0, end - i - 2));
                var raw = text.Substring(i, end - i);
                i = end;
                return InterpretExpression(inner, raw, command);
            }

            if (c == '$')
            {
                var name = Regex.Match(text.Substring(i), @"^\$(?<name>[A-Za-z_][A-Za-z0-9_]*)(?![A-Za-z0-9_:])");
                if (name.Success)
                {
                    var variable = name.Groups["name"].Value;
                    if (variable.Equals("true", StringComparison.OrdinalIgnoreCase) || variable.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        i += name.Length;
                        return PsValue.OfText(variable.Equals("true", StringComparison.OrdinalIgnoreCase) ? "True" : "False", true);
                    }

                    if (variable.Equals("null", StringComparison.OrdinalIgnoreCase))
                    {
                        i += name.Length;
                        return PsValue.OfText(string.Empty, true);
                    }

                    if (Environment.GetEnvironmentVariable(variable) == null)
                    {
                        var after = i + name.Length;
                        if (after >= text.Length || char.IsWhiteSpace(text[after]) || text[after] == ';' || text[after] == '|' || text[after] == '}' || text[after] == ')')
                        {
                            i = after;
                            return new PsValue { Kind = ValueKind.Expression, Text = name.Value };
                        }
                    }
                }
            }

            if (inHashtable && char.IsLetter(c))
            {
                var wordEnd = i;
                while (wordEnd < text.Length && (char.IsLetterOrDigit(text[wordEnd]) || text[wordEnd] == '-')) wordEnd++;
                var word = text.Substring(i, wordEnd - i);
                if (FileCmdlets.Contains(word) || ContentCmdlets.Contains(word))
                {
                    var j = wordEnd;
                    var path = ParseCmdletPath(text, ref j, command);
                    if (path != null)
                    {
                        i = j;
                        return new PsValue { Kind = ContentCmdlets.Contains(word) ? ValueKind.FileContent : ValueKind.FileReference, Text = path };
                    }
                }
            }

            return ParseWord(text, ref i, inHashtable);
        }

        /// <summary>Reads the path argument of <c>Get-Item path</c> / <c>Get-Content -Raw -Path path</c>.</summary>
        private static string? ParseCmdletPath(string text, ref int i, string command)
        {
            string? path = null;
            while (true)
            {
                var j = SkipBlank(text, i, false);
                if (j >= text.Length || text[j] == ';' || text[j] == '}' || text[j] == '|' || text[j] == ')' || text[j] == '\r' || text[j] == '\n')
                {
                    i = j;
                    return path;
                }

                if (text[j] == '-' && j + 1 < text.Length && char.IsLetter(text[j + 1]))
                {
                    var start = j + 1;
                    while (j < text.Length && !char.IsWhiteSpace(text[j])) j++;
                    var parameter = text.Substring(start, j - start);
                    i = j;
                    if (parameter.Equals("Path", StringComparison.OrdinalIgnoreCase) || parameter.Equals("LiteralPath", StringComparison.OrdinalIgnoreCase))
                    {
                        path = ParseWord(text, ref i, true).Text;
                    }

                    continue;
                }

                i = j;
                var value = ParseWord(text, ref i, true);
                path ??= value.Text;
            }
        }

        /// <summary>
        /// Gives meaning to the few <c>( ... )</c> expressions a command string can carry: a single literal,
        /// <c>Get-Item path</c> (a file), <c>Get-Content path</c> (a file's text) and
        /// <c>ConvertTo-SecureString 'text' -AsPlainText</c> (the text). Anything else stays an expression.
        /// </summary>
        private static PsValue InterpretExpression(string inner, string raw, string command)
        {
            try
            {
                var trimmed = inner.Trim();
                var tokens = Tokenize(trimmed, command, out var end);
                if (end < trimmed.Length)
                {
                    // a pipeline such as (@{ a = 1 } | ConvertTo-Json) is code, not a value
                    return new PsValue { Kind = ValueKind.Expression, Text = raw };
                }

                if (tokens.Count == 1 && tokens[0].Value != null)
                {
                    return tokens[0].Value!;
                }

                var first = tokens.Count > 0 ? tokens[0].Value : null;
                if (first != null && first.Bare)
                {
                    if (FileCmdlets.Contains(first.Text) || ContentCmdlets.Contains(first.Text))
                    {
                        var j = first.Text.Length;
                        var path = ParseCmdletPath(trimmed, ref j, command);
                        if (path != null)
                        {
                            return new PsValue { Kind = ContentCmdlets.Contains(first.Text) ? ValueKind.FileContent : ValueKind.FileReference, Text = path };
                        }
                    }

                    if (first.Text.Equals("ConvertTo-SecureString", StringComparison.OrdinalIgnoreCase))
                    {
                        for (var k = 1; k < tokens.Count; k++)
                        {
                            if (tokens[k].Parameter != null && tokens[k].Parameter!.Equals("String", StringComparison.OrdinalIgnoreCase) && k + 1 < tokens.Count && tokens[k + 1].Value != null)
                            {
                                return tokens[k + 1].Value!;
                            }

                            if (tokens[k].Value != null && tokens[k].Value!.Kind == ValueKind.Text)
                            {
                                return tokens[k].Value!;
                            }
                        }
                    }
                }
            }
            catch (CurlInvalidCommandException)
            {
                // not something a command string can express: report it as an expression below
            }

            return new PsValue { Kind = ValueKind.Expression, Text = raw };
        }

        /// <summary>Index after the parenthesis matching the one at <paramref name="open"/>, honouring quotes.</summary>
        /// <exception cref="CurlInvalidCommandException">The parenthesis is never closed.</exception>
        private static int MatchingParen(string text, int open, string command)
        {
            var depth = 0;
            var i = open;
            while (i < text.Length)
            {
                var c = text[i];
                if (CommandParser.IsPowerShellSingleQuote(c))
                {
                    i = SkipQuoted(text, i + 1, CommandParser.IsPowerShellSingleQuote);
                    continue;
                }

                if (CommandParser.IsPowerShellDoubleQuote(c))
                {
                    i = SkipQuoted(text, i + 1, CommandParser.IsPowerShellDoubleQuote);
                    continue;
                }

                if (c == '(') depth++;
                else if (c == ')' && --depth == 0) return i + 1;
                i++;
            }

            throw Error("A parenthesis ( is never closed", "Add the closing ), or quote the value if the ( belongs to it.", text.Substring(open), command);
        }

        private static int SkipQuoted(string text, int i, Func<char, bool> isQuote)
        {
            while (i < text.Length)
            {
                if (text[i] == '`' && i + 1 < text.Length)
                {
                    i += 2;
                    continue;
                }

                if (isQuote(text[i]))
                {
                    if (i + 1 < text.Length && isQuote(text[i + 1]))
                    {
                        i += 2;
                        continue;
                    }

                    return i + 1;
                }

                i++;
            }

            return i;
        }

        /// <summary>
        /// Reads one word: concatenated quoted strings, backtick escapes, <c>$env:NAME</c> expansions and plain
        /// characters, up to whitespace, <c>;</c> or <c>|</c> (and <c>}</c> inside a hashtable).
        /// </summary>
        private static PsValue ParseWord(string text, ref int i, bool inHashtable)
        {
            var sb = new StringBuilder();
            var bare = true;
            var depth = 0;
            while (i < text.Length)
            {
                var c = text[i];
                if (char.IsWhiteSpace(c) || c == ';' || c == '|' || (inHashtable && c == '}') || (c == ')' && depth == 0))
                {
                    break;
                }

                if (sb.Length > 0 && (c == '(' || c == ')'))
                {
                    // parentheses inside a word, as in https://en.wikipedia.org/wiki/Foo_(bar), belong to it
                    depth += c == '(' ? 1 : -1;
                    sb.Append(c);
                    i++;
                    continue;
                }

                if (c == '`' && i + 1 < text.Length)
                {
                    if (text[i + 1] == '\r' || text[i + 1] == '\n')
                    {
                        break;
                    }

                    i = CommandParser.AppendPowerShellEscape(text, i + 1, sb);
                    continue;
                }

                if (c == '@' && sb.Length == 0 && i + 1 < text.Length &&
                    (CommandParser.IsPowerShellSingleQuote(text[i + 1]) || CommandParser.IsPowerShellDoubleQuote(text[i + 1])))
                {
                    i = CommandParser.ReadPowerShellHereString(text, i, sb, CurlFlavor.InvokeWebRequest);
                    bare = false;
                    continue;
                }

                if (CommandParser.IsPowerShellSingleQuote(c))
                {
                    i = CommandParser.ReadPowerShellSingleQuoted(text, i + 1, sb, CurlFlavor.InvokeWebRequest);
                    bare = false;
                    continue;
                }

                if (CommandParser.IsPowerShellDoubleQuote(c))
                {
                    i = CommandParser.ReadPowerShellDoubleQuoted(text, i + 1, sb, CurlFlavor.InvokeWebRequest);
                    bare = false;
                    continue;
                }

                if (c == '$' && CommandParser.TryExpandPowerShellVariable(text, i, out var value, out var consumed))
                {
                    sb.Append(value);
                    i += consumed;
                    continue;
                }

                sb.Append(c);
                i++;
            }

            return PsValue.OfText(sb.ToString(), bare);
        }

        /// <summary>Reads <c>@{ key = value; ... }</c> starting at the <c>@</c>.</summary>
        private static PsValue ParseHashtable(string text, ref int i, string command)
        {
            var table = new PsValue { Kind = ValueKind.Hashtable };
            var start = i;
            i += 2;
            while (true)
            {
                while (true)
                {
                    i = SkipBlank(text, i, true);
                    if (i < text.Length && text[i] == ';')
                    {
                        i++;
                        continue;
                    }

                    break;
                }

                if (i >= text.Length)
                {
                    throw Error("A hashtable @{ ... } is never closed", "Add the closing }.", text.Substring(start), command);
                }

                if (text[i] == '}')
                {
                    i++;
                    return table;
                }

                string key;
                if (CommandParser.IsPowerShellSingleQuote(text[i]) || CommandParser.IsPowerShellDoubleQuote(text[i]))
                {
                    key = ParseWord(text, ref i, true).Text;
                }
                else
                {
                    var keyStart = i;
                    while (i < text.Length && text[i] != '=' && !char.IsWhiteSpace(text[i]) && text[i] != '}' && text[i] != ';') i++;
                    key = text.Substring(keyStart, i - keyStart);
                }

                i = SkipBlank(text, i, false);
                if (i >= text.Length || text[i] != '=')
                {
                    throw Error($"The hashtable entry \"{key}\" has no = sign",
                        "Write each entry as Name = 'value', separated by ; or new lines, for example @{ 'Accept' = 'application/json'; 'X-Id' = '7' }.",
                        key, command);
                }

                i++;
                var value = ParseValue(text, ref i, true, command);
                table.Entries.Add(new KeyValuePair<string, PsValue>(key, value));
            }
        }

        #endregion
    }
}
