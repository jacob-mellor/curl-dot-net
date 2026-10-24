/***************************************************************************
 * CommandParser.Transport - transport-side curl options
 *
 * Options that configure how a request travels (proxies, cookies, redirects, output files,
 * --write-out, retries, TLS...) are parsed here, in a separate partial-class file, so that
 * request-shaping options in CommandParser.cs can evolve independently.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace CurlDotNet.Core
{
    internal partial class CommandParser
    {
        /// <summary>
        /// Long option names handled by <see cref="ProcessTransportOption"/> that take a value, so the
        /// tokenizer consumes the following argument instead of mistaking it for the URL.
        /// </summary>
        private static readonly HashSet<string> TransportOptionsRequiringValue = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "--noproxy",
            "--proxy1.0",
            "-U",
            "-D",
            "--dump-header",
            "--output-dir",
            "--stderr",
            "--form-string",
            "--expect100-timeout",
            "--max-filesize",
            "--etag-save",
            "--etag-compare",
        };

        /// <summary>
        /// Handles transport-side options. Returns true when <paramref name="option"/> was recognised.
        /// </summary>
        private bool ProcessTransportOption(string option, string value, CurlOptions options, ref bool methodSpecified)
        {
            switch (option)
            {
                // --- proxy -------------------------------------------------------------------
                case "--noproxy":
                    options.NoProxy = value;
                    return true;

                case "--proxy-basic":
                    options.ProxyAuthScheme = "basic";
                    return true;

                case "--proxy-digest":
                    options.ProxyAuthScheme = "digest";
                    return true;

                case "--proxy-ntlm":
                    options.ProxyAuthScheme = "ntlm";
                    return true;

                case "--proxy-negotiate":
                    options.ProxyAuthScheme = "negotiate";
                    return true;

                case "--proxy-anyauth":
                    options.ProxyAuthScheme = "anyauth";
                    return true;

                case "-p":
                case "--proxytunnel":
                    options.ProxyTunnel = true;
                    return true;

                case "--proxy1.0":
                    options.Proxy = value;
                    options.ProxyHttp10 = true;
                    return true;

                case "-U":
                    // user[:password]; only the first colon separates them, like curl
                    var colon = value.IndexOf(':');
                    options.ProxyCredentials = colon < 0
                        ? new NetworkCredential(value, string.Empty)
                        : new NetworkCredential(value.Substring(0, colon), value.Substring(colon + 1));
                    return true;

                // --- redirects ---------------------------------------------------------------
                case "--post301":
                    options.Post301 = true;
                    return true;

                case "--post302":
                    options.Post302 = true;
                    return true;

                case "--post303":
                    options.Post303 = true;
                    return true;

                case "--follow":
                    options.FollowLocation = true;
                    options.FollowObeysMethod = true;
                    return true;

                // --- failure handling ----------------------------------------------------------
                case "--fail-with-body":
                    options.FailWithBody = true;
                    return true;

                case "--fail-early":
                    options.FailEarly = true;
                    return true;

                // --- output --------------------------------------------------------------------
                case "-D":
                case "--dump-header":
                    options.DumpHeaderFile = value;
                    return true;

                case "-J":
                case "--remote-header-name":
                    options.RemoteHeaderName = true;
                    return true;

                case "--output-dir":
                    options.OutputDir = value;
                    return true;

                case "-R":
                case "--remote-time":
                    options.RemoteTime = true;
                    return true;

                case "--no-clobber":
                    options.NoClobber = true;
                    return true;

                case "--remove-on-error":
                    options.RemoveOnError = true;
                    return true;

                case "--remote-name-all":
                    options.UseRemoteFileName = true;
                    return true;

                case "--stderr":
                    options.StderrFile = value;
                    return true;

                case "--etag-save":
                    options.EtagSave = value;
                    return true;

                case "--etag-compare":
                    options.EtagCompare = value;
                    return true;

                case "--max-filesize":
                    var size = ParseByteSize(value);
                    if (size.HasValue)
                    {
                        options.MaxFileSize = size.Value;
                    }
                    return true;

                // --- cookies -------------------------------------------------------------------
                case "-j":
                case "--junk-session-cookies":
                    options.JunkSessionCookies = true;
                    return true;

                // --- forms ---------------------------------------------------------------------
                case "--form-string":
                    AddFormPart(value, literal: true, options);
                    if (string.IsNullOrEmpty(options.Method) || options.Method == "GET")
                    {
                        options.Method = "POST";
                        methodSpecified = true;
                    }
                    return true;

                // --- connection ----------------------------------------------------------------
                case "--expect100-timeout":
                    if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var expectTimeout))
                    {
                        options.Expect100TimeoutSeconds = expectTimeout;
                    }
                    return true;

                // Accepted for compatibility; they only tune buffering or the progress meter.
                case "-N":
                case "--no-buffer":
                case "--buffer":
                case "--no-progress-meter":
                case "--tcp-nodelay":
                case "--no-keepalive":
                case "--tcp-fastopen":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a <c>-F</c> or <c>--form-string</c> part (with its <see cref="CurlOptions.FormData"/>/<see cref="CurlOptions.Files"/> mirror).
        /// </summary>
        private static void AddFormPart(string field, bool literal, CurlOptions options)
        {
            var part = MultipartFormBuilder.Parse(field, literal);
            if (part == null)
            {
                return; // curl ignores (and warns about) a form argument without '='
            }

            options.FormParts.Add(part);
            if (part.IsFileUpload)
            {
                options.Files[part.Name] = part.FilePath ?? string.Empty;
            }
            else
            {
                options.FormData[part.Name] = part.IsFileContent ? "<" + part.FilePath : part.Value ?? string.Empty;
            }
        }

        /// <summary>curl's size syntax: a number with an optional k, M, G or T suffix (powers of 1024).</summary>
        private static long? ParseByteSize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var text = value.Trim();
            long multiplier = 1;
            switch (char.ToUpperInvariant(text[text.Length - 1]))
            {
                case 'K': multiplier = 1024L; break;
                case 'M': multiplier = 1024L * 1024; break;
                case 'G': multiplier = 1024L * 1024 * 1024; break;
                case 'T': multiplier = 1024L * 1024 * 1024 * 1024; break;
            }
            if (multiplier != 1)
            {
                text = text.Substring(0, text.Length - 1);
            }

            return long.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var number) ? number * multiplier : (long?)null;
        }
    }
}
