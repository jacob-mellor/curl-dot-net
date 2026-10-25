/***************************************************************************
 * RequestTransfers - turning a parsed command line into curl's list of transfers
 *
 * Part of CommandParser. Mirrors curl's src/tool_operate.c: every URL of a --next group gets
 * the group's options, the nth -o / -O / -T belongs to the nth URL, globs expand into one
 * transfer per match, and per-transfer request shaping (-G, -T file names, --url-query,
 * URL credentials, -d @file bodies) happens once all options are known.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    internal partial class CommandParser
    {
        /// <summary>The output slot <c>--out-null</c> adds: the paired URL's response is discarded.</summary>
        private static readonly string OutNullSlot = new string('\0', 1);

        /// <summary>State shared by one <see cref="Parse(string)"/> call.</summary>
        private sealed class ParseContext
        {
            public ParseContext(string? originalCommand)
            {
                OriginalCommand = originalCommand;
                StartNewGroup();
            }

            public string? OriginalCommand { get; }

            public List<TransferGroup> Groups { get; } = new List<TransferGroup>();

            public TransferGroup Current { get; private set; } = null!;

            /// <summary><c>--variable</c> values, shared by every group like in curl.</summary>
            public Dictionary<string, string> Variables { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

            /// <summary>Nesting depth of <c>-K</c> files, to stop a config file that includes itself.</summary>
            public int ConfigDepth { get; set; }

            /// <summary>Set when a transfer after the first could not be set up; no further transfers are made.</summary>
            public bool SetupFailed { get; set; }

            /// <summary>curl's warnings and notes found while reading the command line.</summary>
            public ToolMessages Messages { get; } = new ToolMessages();

            /// <summary>The command-line word being processed (curl's <c>orig_opt</c>, such as <c>-C</c> or <c>--no-clobber</c>).</summary>
            public string CurrentWord { get; set; } = string.Empty;

            /// <summary>
            /// curl's <c>global-&gt;silent</c> at this point of the command line: the last <c>-s</c>/<c>--no-silent</c>, whatever
            /// <c>--next</c> group it is in.
            /// </summary>
            public bool Silent { get; set; }

            /// <summary>curl's <c>warnf()</c> at this point of the command line (nothing under <c>-s</c>).</summary>
            public void Warn(string text) => Messages.AddWarning(text, Silent);

            public void StartNewGroup()
            {
                Current = new TransferGroup(new CurlOptions { OriginalCommand = OriginalCommand });
                Groups.Add(Current);
            }
        }

        /// <summary>The options and URLs between two <c>--next</c> separators.</summary>
        private sealed class TransferGroup
        {
            public TransferGroup(CurlOptions options)
            {
                Options = options;
            }

            /// <summary>Options for the group's first transfer (applied as they were parsed).</summary>
            public CurlOptions Options { get; }

            /// <summary>Set when an option chose the method; a plain GET is then reported as null.</summary>
            public bool MethodSpecified;

            /// <summary>Every option of the group in order, replayed for the group's other transfers.</summary>
            public List<KeyValuePair<string, string>> Recorded { get; } = new List<KeyValuePair<string, string>>();

            public List<string> Urls { get; } = new List<string>();

            /// <summary>One entry per <c>-o</c> (the file name) or <c>-O</c> (null), paired with URLs in order.</summary>
            public List<string?> OutputSlots { get; } = new List<string?>();

            /// <summary>One entry per <c>-T</c>, paired with URLs in order.</summary>
            public List<string> UploadSlots { get; } = new List<string>();

            public bool RemoteNameAll { get; set; }

            /// <summary>How many of curl's URL nodes the output options (<c>-o</c>, <c>-O</c>, <c>--out-null</c>) took.</summary>
            public int OutputNodes { get; set; }

            /// <summary>curl's <c>config-&gt;fail</c>: 0 none, 1 <c>--fail</c>, 2 <c>--fail-with-body</c>.</summary>
            public int FailMode { get; set; }

            /// <summary>curl's <c>config-&gt;followlocation</c>: 0 none, 1 <c>--location</c> (ALL), 2 <c>--follow</c> (OBEYCODE).</summary>
            public int FollowMode { get; set; }

            /// <summary>curl's <c>config-&gt;use_resume</c>: <c>-C</c> was given.</summary>
            public bool Resume { get; set; }

            /// <summary><c>-r</c>/<c>--range</c> was given.</summary>
            public bool RangeGiven { get; set; }

            /// <summary>curl's <c>config-&gt;rm_partial</c>: <c>--remove-on-error</c>.</summary>
            public bool RemoveOnError { get; set; }

            /// <summary>curl's <c>CLOBBER_NEVER</c>: <c>--no-clobber</c>.</summary>
            public bool NoClobber { get; set; }
        }

        /// <summary>Processes the argument vector and returns the first transfer, with the rest attached.</summary>
        private CurlOptions ParseArgumentList(IList<string> args, string? originalCommand)
        {
            var context = new ParseContext(originalCommand);
            try
            {
                return BuildTransfers(args, originalCommand, context);
            }
            catch (CurlException ex)
            {
                // what curl would have printed before it stopped
                ex.Warnings = context.Messages.Snapshot();
                throw;
            }
        }

        private CurlOptions BuildTransfers(IList<string> args, string? originalCommand, ParseContext context)
        {
            ProcessArguments(args, context);

            var transfers = new List<CurlOptions>();
            for (var g = 0; g < context.Groups.Count; g++)
            {
                var before = transfers.Count;
                BuildGroupTransfers(context.Groups[g], context, transfers);
                for (var i = before; i < transfers.Count; i++)
                {
                    transfers[i].GroupNumber = g;
                }
            }

            if (transfers.Count == 0)
            {
                throw new CurlInvalidCommandException(
                    CurlException.Compose(
                        $"No URL specified in command \"{ErrorText.Preview(MaskCredentials(originalCommand), 120)}\": every word was read as an option or an option value.",
                        "Add the URL to fetch, for example \"curl https://example.com\". If the command has one, an option that takes a value may have swallowed it " +
                        "(-H, -d, -o and -u need a value right after them), a quote may never be closed, or the command was written for another shell " +
                        "(pass the matching CurlFlavor, such as CurlFlavor.WindowsCmd or CurlFlavor.PowerShell).",
                        DocLinks.HowToFlavors),
                    2, "URL", originalCommand);
            }

            // the state curl's messages see once the whole command line is read
            var messages = context.Messages;
            messages.Silent = context.Silent;
            foreach (var group in context.Groups)
            {
                var o = group.Options;
                messages.Tracing |= o.Verbose || !string.IsNullOrEmpty(o.TraceFile) || !string.IsNullOrEmpty(o.TraceAsciiFile);
                if (!string.IsNullOrEmpty(o.StderrFile))
                {
                    messages.StderrFile = o.StderrFile == "-" ? null : o.StderrFile;
                }
            }

            // tool_operate.c create_single(): a -o/-O/-T slot left without a URL
            foreach (var group in context.Groups)
            {
                if (group.Urls.Count > 0 && Math.Max(group.OutputNodes, group.UploadSlots.Count) > group.Urls.Count)
                {
                    messages.AddWarning("Got more output options than URLs", messages.Silent);
                }
            }

            var first = transfers[0];
            first.Messages = messages;
            for (var i = 1; i < transfers.Count; i++)
            {
                transfers[i].TransferNumber = i;
                transfers[i].Messages = messages;
                first.AdditionalTransfers.Add(transfers[i]);
            }

            return first;
        }

        /// <summary>
        /// The messages curl prints while it sets a transfer up (<c>config2setopts.c</c>'s <c>customrequest_helper()</c>):
        /// <c>-X</c> naming the method the other options already select, or <c>-X HEAD</c> without <c>-I</c>. Called by the
        /// engine just before each transfer runs.
        /// </summary>
        internal static void AddTransferSetupMessages(CurlOptions options)
        {
            var messages = options.Messages;
            var method = options.CustomMethod;
            if (messages == null || string.IsNullOrEmpty(method))
            {
                return;
            }

            // curl's dflt[] by config->httpreq: unspecified and -G are GET, -I HEAD, -F and -d POST, -T PUT
            var inferred = "GET";
            foreach (var kind in options.RequestKinds)
            {
                if (options.UseGet && kind.Key == RequestKind.Post)
                {
                    continue;
                }

                inferred = kind.Key == RequestKind.Head ? "HEAD"
                    : kind.Key == RequestKind.Put ? "PUT"
                    : kind.Key == RequestKind.Post || kind.Key == RequestKind.MimePost ? "POST"
                    : inferred;
                break;
            }

            if (string.Equals(method, inferred, StringComparison.OrdinalIgnoreCase))
            {
                messages.Note($"Unnecessary use of -X or --request, {inferred} is already inferred.");
            }
            else if (string.Equals(method, "head", StringComparison.OrdinalIgnoreCase))
            {
                messages.Warn("Setting custom HTTP method to HEAD with -X/--request may not work the way you want. Consider using -I/--head instead.");
            }
        }

        private static void AddUrl(ParseContext context, string url)
        {
            context.Current.Urls.Add(url);
        }

        /// <summary>
        /// <c>--url URL</c> adds a URL; <c>--url @file</c> adds every non-empty line of the file
        /// (lines starting with <c>#</c> are comments), as in curl 8.13 and later.
        /// </summary>
        private static void AddUrlArgument(ParseContext context, string value)
        {
            if (!value.StartsWith("@", StringComparison.Ordinal))
            {
                AddUrl(context, value);
                return;
            }

            foreach (var line in ReadAllLinesForOption(value.Substring(1), "--url @file", context.OriginalCommand))
            {
                var url = line.Trim();
                if (url.Length > 0 && url[0] != '#')
                {
                    AddUrl(context, url);
                }
            }
        }

        /// <summary>The command with every credential replaced by *** (see <see cref="ErrorText.RedactCommand"/>).</summary>
        internal static string MaskCredentials(string? command) => ErrorText.RedactCommand(command);

        /// <summary>Reads a text file named by an option, one line per entry; failures become exit code 26.</summary>
        private static List<string> ReadAllLinesForOption(string path, string option, string? command, bool keepCarriageReturns = false)
        {
            var text = Encoding.UTF8.GetString(RequestShaping.ReadFileForOption(path, option, command));
            if (text.Length > 0 && text[0] == '﻿')
            {
                text = text.Substring(1);
            }

            var lines = new List<string>(text.Split('\n'));
            for (var i = 0; i < lines.Count && !keepCarriageReturns; i++)
            {
                lines[i] = lines[i].TrimEnd('\r');
            }

            if (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            return lines;
        }

        /// <summary>Creates one transfer per URL, upload file and glob match of a group, in curl's order.</summary>
        private void BuildGroupTransfers(TransferGroup group, ParseContext context, List<CurlOptions> transfers)
        {
            var firstUsed = false;
            for (var urlIndex = 0; urlIndex < group.Urls.Count; urlIndex++)
            {
                var url = group.Urls[urlIndex];
                var hasOutputSlot = urlIndex < group.OutputSlots.Count;
                var outputSlot = hasOutputSlot ? group.OutputSlots[urlIndex] : null;
                var upload = urlIndex < group.UploadSlots.Count ? group.UploadSlots[urlIndex] : null;

                var uploads = new List<RequestGlobResult?>();
                if (upload != null && !group.Options.GlobOff && RequestGlob.HasGlob(upload))
                {
                    uploads.AddRange(RequestGlob.Expand(upload, context.OriginalCommand));
                }
                else
                {
                    uploads.Add(upload == null ? null : new RequestGlobResult(upload, new List<string>(), new Dictionary<string, string>()));
                }

                // without a glob the escapes still apply: "\{" in a URL is a literal "{" (curl test 214)
                var urls = group.Options.GlobOff
                    ? new List<RequestGlobResult> { new RequestGlobResult(url, new List<string>(), new Dictionary<string, string>()) }
                    : RequestGlob.Expand(url, context.OriginalCommand);

                foreach (var uploadMatch in uploads)
                {
                    foreach (var urlMatch in urls)
                    {
                        CurlOptions options;
                        bool methodSpecified;
                        if (!firstUsed)
                        {
                            options = group.Options;
                            methodSpecified = group.MethodSpecified;
                            firstUsed = true;
                        }
                        else
                        {
                            options = new CurlOptions { OriginalCommand = context.OriginalCommand };
                            methodSpecified = false;
                            foreach (var recorded in group.Recorded)
                            {
                                ApplyOption(options, recorded.Key, recorded.Value, ref methodSpecified, context);
                            }
                        }

                        var matches = new List<string>(urlMatch.Matches);
                        var named = new Dictionary<string, string>(urlMatch.Named, StringComparer.Ordinal);
                        if (uploadMatch != null)
                        {
                            matches.InsertRange(0, uploadMatch.Matches);
                            foreach (var pair in uploadMatch.Named)
                            {
                                named[pair.Key] = pair.Value;
                            }
                        }

                        if (context.SetupFailed)
                        {
                            return;
                        }

                        try
                        {
                            FinishTransfer(options, urlMatch.Url, matches, named, hasOutputSlot, outputSlot, uploadMatch?.Url, group, ref methodSpecified);
                        }
                        catch (CurlException ex) when (transfers.Count > 0)
                        {
                            // curl sets up each transfer just before it runs: the earlier ones still happen,
                            // then this failure ends the command (for example the 2nd -T file is missing)
                            options.SetupError = ex;
                            context.SetupFailed = true;
                        }
                        transfers.Add(options);
                    }
                }
            }
        }

        /// <summary>
        /// Completes one transfer once every option is known: pairs it with its <c>-o</c>/<c>-O</c> and
        /// <c>-T</c>, validates option combinations like curl, and shapes the URL and body.
        /// </summary>
        private static void FinishTransfer(CurlOptions options, string url, List<string> matches, Dictionary<string, string> named,
            bool hasOutputSlot, string? outputSlot, string? upload, TransferGroup group, ref bool methodSpecified)
        {
            options.Url = NormalizeUrl(url, options.DefaultProtocol);
            options.GlobMatches.Clear();
            options.GlobMatches.AddRange(matches);

            // The nth -o / -O belongs to the nth URL, exactly like curl (extra ones are ignored)
            options.DiscardOutput = false;
            if (hasOutputSlot)
            {
                if (outputSlot == null)
                {
                    options.UseRemoteFileName = true;
                    options.OutputFile = null;
                }
                else if (ReferenceEquals(outputSlot, OutNullSlot))
                {
                    // --out-null: this URL's response is read and thrown away
                    options.DiscardOutput = true;
                    options.UseRemoteFileName = false;
                    options.OutputFile = null;
                }
                else
                {
                    options.OutputFile = RequestGlob.SubstituteOutputName(outputSlot, matches, named, options.OriginalCommand);
                    options.UseRemoteFileName = false;
                }
            }
            else if (group.OutputSlots.Count > 0)
            {
                options.OutputFile = null;
                options.UseRemoteFileName = group.RemoteNameAll;
            }
            else if (group.RemoteNameAll)
            {
                options.UseRemoteFileName = true;
            }

            if (upload != null)
            {
                ApplyUpload(options, upload, ref methodSpecified);
            }

            ValidateRequestKinds(options);
            ValidateResume(options);
            ApplyAutoResume(options);
            RequestShaping.BuildRequestBody(options);
            ShapeUrl(options);

            if (!methodSpecified && string.Equals(options.Method, "GET", StringComparison.OrdinalIgnoreCase))
            {
                options.Method = null;
            }
        }

        /// <summary>
        /// <c>-T FILE</c>: the file becomes the PUT body (<see cref="CurlOptions.BinaryData"/>), unless
        /// <c>-X</c> chose another method.
        /// </summary>
        private static void ApplyUpload(CurlOptions options, string file, ref bool methodSpecified)
        {
            if (file == "-" || file == ".")
            {
                throw new CurlReadErrorException(
                    file,
                    CurlException.Compose(
                        $"-T {file} means \"upload standard input\" in curl, and CurlDotNet runs inside your process, which has no standard input to read.",
                        "Save the content to a file and use -T path/to/file (for example -T backup.tar.gz), or send it inline with --data-binary 'text'.",
                        DocLinks.HowToUpload),
                    options.OriginalCommand);
            }

            SetRequestKind(options, RequestKind.Put, "PUT (-T, --upload-file)");
            options.BinaryData = RequestShaping.ReadFileForOption(file, "-T/--upload-file", options.OriginalCommand);
            options.UploadFile = file;
            if (string.IsNullOrEmpty(options.CustomMethod))
            {
                options.Method = "PUT";
            }

            methodSpecified = true;
        }

        /// <summary>
        /// curl cannot resume an HTTP upload: <c>-C</c> with <c>-d</c>, <c>-F</c> or <c>-T</c> fails with
        /// exit code 43 ("HTTP upload cannot be resumed"). An offset of 0 means "no resume".
        /// </summary>
        private static void ValidateResume(CurlOptions options)
        {
            if (options.ResumeFrom == null || options.ResumeFrom == 0)
            {
                return;
            }

            var upload = false;
            foreach (var kind in options.RequestKinds)
            {
                upload |= (kind.Key == RequestKind.Post && !options.UseGet) || kind.Key == RequestKind.MimePost || kind.Key == RequestKind.Put;
            }
            var scheme = options.Url.Length > 0 ? options.Url.Substring(0, Math.Max(0, options.Url.IndexOf(':'))) : string.Empty;
            var http = scheme.Equals("http", StringComparison.OrdinalIgnoreCase) || scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
            if (upload && http)
            {
                // curl: "HTTP upload cannot be resumed", from config2setopts.c (-d, -F) or lib/http.c (-T)
                throw new CurlDotNet.Exceptions.CurlBadFunctionArgumentException(
                    "--continue-at",
                    CurlException.Compose(
                        $"HTTP upload cannot be resumed: -C/--continue-at {(options.ResumeFrom.Value < 0 ? "-" : options.ResumeFrom.Value.ToString(System.Globalization.CultureInfo.InvariantCulture))} resumes downloads, " +
                        $"but this command also sends a body to {ErrorText.SafeUrl(options.Url)} (-d, -F or -T).",
                        "Remove -C to send the whole body again, or, to fetch only part of a download, drop the body and use -r/--range (for example -r 1000-).",
                        DocLinks.HowToDownload),
                    options.OriginalCommand).WithCurlErrorMessage("HTTP upload cannot be resumed");
            }
        }

        /// <summary>
        /// <c>-C -</c>: resume from the size of the output file that already exists, by asking for
        /// <c>Range: bytes=SIZE-</c>. Nothing is resumed when the file is missing or empty.
        /// </summary>
        private static void ApplyAutoResume(CurlOptions options)
        {
            if (options.ResumeFrom != -1 || string.IsNullOrEmpty(options.OutputFile) || options.OutputFile == "-")
            {
                return;
            }

            try
            {
                var info = new FileInfo(options.OutputFile);
                if (info.Exists && info.Length > 0)
                {
                    options.ResumeFrom = info.Length;
                    options.Range = info.Length + "-";
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException || ex is NotSupportedException)
            {
                // An unusable output path is reported when the response is written
            }
        }

        /// <summary>
        /// Final URL shaping: the <c>-T</c> file name for URLs that end in <c>/</c>, <c>--url-query</c>
        /// items, <c>-G</c> data as a query string, and <c>user:password@</c> credentials.
        /// </summary>
        private static void ShapeUrl(CurlOptions options)
        {
            var url = options.Url;

            if (options.UploadFile != null)
            {
                url = AppendUploadFileName(url, options.UploadFile);
            }

            if (options.UrlQueryParts.Count > 0)
            {
                var query = new StringBuilder();
                foreach (var part in options.UrlQueryParts)
                {
                    var encoded = EncodeUrlQueryArgument(part, options.OriginalCommand);
                    if (encoded.Length == 0)
                    {
                        continue;
                    }

                    if (query.Length > 0)
                    {
                        query.Append('&');
                    }

                    query.Append(encoded);
                }

                url = AppendQuery(url, query.ToString());
            }

            if (options.UseGet && options.Data != null)
            {
                var body = options.GetParsedBody();
                var query = body != null ? Encoding.UTF8.GetString(body) : options.Data;
                url = AppendQuery(url, query);
                options.Data = null;
                options.ParsedDataBytes = null;
                options.ParsedDataText = null;
                options.DataParts.Clear();
                if (string.Equals(options.Method, "POST", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(options.CustomMethod))
                {
                    options.Method = null; // a plain GET, reported as "no explicit method"
                }
            }

            options.Url = url;

            if (options.Credentials == null)
            {
                try
                {
                    var credentials = RequestShaping.GetUrlCredentials(new Uri(url));
                    if (credentials != null)
                    {
                        options.Credentials = credentials;
                        options.CredentialsFromUrl = true;
                    }
                }
                catch (UriFormatException)
                {
                    // Malformed URLs are reported when the request is made
                }
            }
        }

        /// <summary>Appends <paramref name="query"/> after <c>?</c>, or after <c>&amp;</c> when there already is a query.</summary>
        private static string AppendQuery(string url, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return url;
            }

            var hash = url.IndexOf('#');
            var fragment = hash >= 0 ? url.Substring(hash) : string.Empty;
            var baseUrl = hash >= 0 ? url.Substring(0, hash) : url;
            var separator = baseUrl.IndexOf('?') >= 0 ? (baseUrl.EndsWith("?", StringComparison.Ordinal) || baseUrl.EndsWith("&", StringComparison.Ordinal) ? string.Empty : "&") : "?";
            return baseUrl + separator + query + fragment;
        }

        /// <summary>
        /// curl's rule for <c>-T file URL</c>: when nothing follows the URL's last slash (or the URL has no
        /// path at all) the local file name, percent-encoded, is appended.
        /// </summary>
        private static string AppendUploadFileName(string url, string file)
        {
            var schemeEnd = url.IndexOf("://", StringComparison.Ordinal);
            var rest = schemeEnd >= 0 ? url.Substring(schemeEnd + 3) : url;
            var slash = rest.LastIndexOf('/');
            if (slash >= 0 && slash < rest.Length - 1)
            {
                return url;
            }

            var name = file;
            var lastSeparator = Math.Max(name.LastIndexOf('/'), name.LastIndexOf('\\'));
            if (lastSeparator >= 0)
            {
                name = name.Substring(lastSeparator + 1);
            }

            var encoded = PercentEncode(name);
            return slash < 0 ? url + "/" + encoded : url + encoded;
        }

        /// <summary>
        /// Encodes one <c>--url-query</c> argument: like <c>--data-urlencode</c> (<c>content</c>,
        /// <c>name=content</c>, <c>@file</c>, <c>name@file</c>) with spaces as <c>+</c>, while a leading
        /// <c>+</c> adds the rest verbatim.
        /// </summary>
        private static string EncodeUrlQueryArgument(string value, string? command)
        {
            if (value.StartsWith("+", StringComparison.Ordinal))
            {
                // Added as-is, except that curl's URL parser normalises percent-escapes to upper case (%3d -> %3D)
                return Regex.Replace(value.Substring(1), "%[0-9a-fA-F]{2}", m => m.Value.ToUpperInvariant());
            }

            static string Encode(string text) => FormEncode(text);

            // Like --data-urlencode, curl looks for '=' first; only without one does '@' mean "read a file"
            var eq = value.IndexOf('=');
            if (eq >= 0)
            {
                var name = value.Substring(0, eq);
                var content = value.Substring(eq + 1);
                return name.Length > 0 ? name + "=" + Encode(content) : Encode(content);
            }

            var at = value.IndexOf('@');
            if (at >= 0)
            {
                var name = value.Substring(0, at);
                var content = Encoding.UTF8.GetString(RequestShaping.ReadFileForOption(value.Substring(at + 1), "--url-query @file", command));
                return name.Length > 0 ? name + "=" + Encode(content) : Encode(content);
            }

            return Encode(value);
        }
    }
}
