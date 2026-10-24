/***************************************************************************
 * Netrc - credentials from a .netrc file (curl -n, --netrc-optional, --netrc-file)
 *
 * Transpiled from curl's lib/netrc.c and the override_login() rules of lib/url.c:
 * tokens are separated by white space, "quoted strings" may hold \" \\ \n \r \t escapes,
 * "machine NAME" starts an entry for one host, "default" matches every host, "macdef" skips a
 * macro until the next empty line. A login named in the URL picks the entry for that login.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>How a command uses the .netrc file.</summary>
    internal enum NetrcMode
    {
        /// <summary>No <c>-n</c>, <c>--netrc-optional</c> or <c>--netrc-file</c>.</summary>
        None = 0,

        /// <summary><c>--netrc-optional</c>: credentials from the URL win; the file fills in what is missing.</summary>
        Optional = 1,

        /// <summary><c>-n</c>/<c>--netrc</c> (or <c>--netrc-file</c> alone): the file's password is used even when the URL has one.</summary>
        Required = 2,
    }

    /// <summary>Reads curl's .netrc format and picks the credentials for a host.</summary>
    internal static class Netrc
    {
        private const string DocsUrl = "https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/index.md";

        /// <summary>
        /// The credentials a request to <paramref name="uri"/> uses when <c>-n</c>/<c>--netrc-optional</c>
        /// is on and no <c>-u</c> was given, following curl's override_login(): <paramref name="fromUrl"/>
        /// (user-info of the URL) names the login to look for.
        /// </summary>
        /// <returns>The credentials, or <paramref name="fromUrl"/> when the file has nothing for the host.</returns>
        public static NetworkCredential? Resolve(CurlOptions options, Uri uri, NetworkCredential? fromUrl)
        {
            if (options.NetrcMode == NetrcMode.None)
            {
                return fromUrl;
            }

            if (options.NetrcMode == NetrcMode.Optional && fromUrl != null && !string.IsNullOrEmpty(fromUrl.Password))
            {
                return fromUrl; // the URL already has a password
            }

            var login = fromUrl != null && fromUrl.UserName.Length > 0 ? fromUrl.UserName : null;
            var path = FilePath(options);
            if (path == null || !File.Exists(path))
            {
                return Fallback(options, fromUrl);
            }

            string text;
            try
            {
                text = File.ReadAllText(path, Encoding.UTF8);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                return Fallback(options, fromUrl);
            }

            var found = Find(text, uri.Host.Trim('[', ']'), login, path, options.OriginalCommand);
            if (found == null)
            {
                return Fallback(options, fromUrl);
            }

            return new NetworkCredential(login ?? found.Value.Login ?? string.Empty, found.Value.Password ?? string.Empty);
        }

        /// <summary>
        /// What is left when the file has no entry: with <c>-n</c> curl drops the URL's password but keeps
        /// its user; with <c>--netrc-optional</c> the URL credentials stay as they were.
        /// </summary>
        private static NetworkCredential? Fallback(CurlOptions options, NetworkCredential? fromUrl)
        {
            if (fromUrl == null || options.NetrcMode != NetrcMode.Required)
            {
                return fromUrl;
            }
            return new NetworkCredential(fromUrl.UserName, string.Empty);
        }

        /// <summary><c>--netrc-file</c>, else <c>$NETRC</c>, else <c>~/.netrc</c> (<c>_netrc</c> on Windows when there is no <c>.netrc</c>).</summary>
        private static string? FilePath(CurlOptions options)
        {
            if (!string.IsNullOrEmpty(options.NetrcFile))
            {
                return options.NetrcFile;
            }

            var env = Environment.GetEnvironmentVariable("NETRC");
            if (!string.IsNullOrEmpty(env))
            {
                return env;
            }

            var home = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(home))
            {
                home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            if (string.IsNullOrEmpty(home))
            {
                return null;
            }

            var dotted = Path.Combine(home, ".netrc");
            var underscored = Path.Combine(home, "_netrc");
            return !File.Exists(dotted) && Path.DirectorySeparatorChar == '\\' && File.Exists(underscored) ? underscored : dotted;
        }

        /// <summary>One <c>machine</c> or <c>default</c> entry of the file.</summary>
        private struct Entry
        {
            public string? Login;
            public string? Password;
        }

        /// <summary>
        /// Finds the entry for <paramref name="host"/>. With a <paramref name="login"/>, entries with another
        /// login, or without a password, are skipped; without one the first entry that has a login or a
        /// password wins. Entries for other hosts and <c>macdef</c> macros are ignored.
        /// </summary>
        internal static (string? Login, string? Password)? Find(string text, string host, string? login, string path, string? command)
        {
            var tokens = Tokenize(text, path, command);
            var inEntry = false;
            var entry = new Entry();

            (string?, string?)? Finish()
            {
                if (!inEntry)
                {
                    return null;
                }
                inEntry = false;
                if (login != null)
                {
                    if (entry.Login != null && !string.Equals(entry.Login, login, StringComparison.Ordinal))
                    {
                        return null;
                    }
                    return entry.Password != null ? (login, entry.Password) : ((string?, string?)?)null;
                }
                if (entry.Login == null && entry.Password == null)
                {
                    return null;
                }
                return (entry.Login, entry.Password);
            }

            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                switch (token.ToLowerInvariant()) // curl matches the keywords case-insensitively
                {
                    case "machine":
                    {
                        var done = Finish();
                        if (done != null)
                        {
                            return done;
                        }
                        var name = i + 1 < tokens.Count ? tokens[++i] : string.Empty;
                        inEntry = string.Equals(name, host, StringComparison.OrdinalIgnoreCase);
                        entry = new Entry();
                        break;
                    }

                    case "default":
                    {
                        var done = Finish();
                        if (done != null)
                        {
                            return done;
                        }
                        inEntry = true;
                        entry = new Entry();
                        break;
                    }

                    case "login":
                        if (i + 1 < tokens.Count)
                        {
                            var value = tokens[++i];
                            if (inEntry)
                            {
                                entry.Login = value;
                            }
                        }
                        break;

                    case "password":
                        if (i + 1 < tokens.Count)
                        {
                            var value = tokens[++i];
                            if (inEntry)
                            {
                                entry.Password = value; // the last password of an entry counts
                            }
                        }
                        break;

                    case "account":
                        i++; // value ignored, like curl
                        break;
                }
            }

            return Finish();
        }

        /// <summary>Splits the file into tokens, dropping comments and <c>macdef</c> bodies.</summary>
        private static List<string> Tokenize(string text, string path, string? command)
        {
            var tokens = new List<string>();
            var lines = text.Replace("\r\n", "\n").Split('\n');
            var inMacro = false;
            foreach (var line in lines)
            {
                if (inMacro)
                {
                    if (line.Trim().Length == 0)
                    {
                        inMacro = false; // a macro ends at the first empty line
                    }
                    continue;
                }

                var pos = 0;
                while (pos < line.Length)
                {
                    while (pos < line.Length && char.IsWhiteSpace(line[pos])) pos++;
                    if (pos >= line.Length)
                    {
                        break;
                    }
                    if (line[pos] == '#')
                    {
                        break; // comment to the end of the line
                    }

                    string token;
                    if (line[pos] == '"')
                    {
                        var sb = new StringBuilder();
                        pos++;
                        var closed = false;
                        while (pos < line.Length)
                        {
                            var c = line[pos++];
                            if (c == '"')
                            {
                                closed = true;
                                break;
                            }
                            if (c == '\\' && pos < line.Length)
                            {
                                var e = line[pos++];
                                sb.Append(e == 'n' ? '\n' : e == 'r' ? '\r' : e == 't' ? '\t' : e);
                                continue;
                            }
                            sb.Append(c);
                        }
                        if (!closed)
                        {
                            throw new CurlReadErrorException(path,
                                $"The .netrc file \"{path}\" has a quoted value without its closing quote (\"). " +
                                "Close the quote, or remove the quotes around values that contain no spaces. See " + DocsUrl,
                                command);
                        }
                        token = sb.ToString();
                    }
                    else
                    {
                        var start = pos;
                        while (pos < line.Length && !char.IsWhiteSpace(line[pos])) pos++;
                        token = line.Substring(start, pos - start);
                    }

                    if (string.Equals(token, "macdef", StringComparison.OrdinalIgnoreCase))
                    {
                        inMacro = true; // the rest of this line names the macro; its body follows
                        break;
                    }
                    tokens.Add(token);
                }
            }
            return tokens;
        }
    }
}
