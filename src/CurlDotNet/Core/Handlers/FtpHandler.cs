/***************************************************************************
 * FtpHandler - FTP/FTPS protocol handler
 *
 * Based on curl's lib/ftp.c by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Handler for FTP and FTPS protocols.
    /// </summary>
    /// <remarks>
    /// <para>Implements FTP operations using .NET's FtpWebRequest.</para>
    /// <para>AI-Usage: This handler supports FTP file operations matching curl's FTP functionality.</para>
    /// </remarks>
    internal class FtpHandler : IProtocolHandler
    {
        public async Task<CurlResult> ExecuteAsync(CurlOptions options, CancellationToken cancellationToken)
        {
            try
            {
                FtpWebRequest request;
                try
                {
                    request = (FtpWebRequest)WebRequest.Create(options.Url);
                }
                catch (NotSupportedException)
                {
                    throw new CurlException(NotSupportedMessage(options), 1, options.OriginalCommand);
                }

                // Set method
                request.Method = DetermineFtpMethod(options);

                // Set credentials
                if (options.Credentials != null)
                {
                    request.Credentials = options.Credentials;
                }
                else
                {
                    request.Credentials = new NetworkCredential("anonymous", "anonymous@");
                }

                // Set options
                request.UseBinary = true;
                request.UsePassive = options.FtpPassive;
                request.EnableSsl = options.FtpSsl || options.Url.StartsWith("ftps://");
                request.KeepAlive = options.KeepAliveTime.HasValue;

                if (options.ConnectTimeout.HasValue && options.ConnectTimeout.Value > 0)
                {
                    request.Timeout = (int)(options.ConnectTimeout.Value * 1000);
                }

                // Set proxy if specified
                if (!string.IsNullOrEmpty(options.Proxy))
                {
                    request.Proxy = new WebProxy(options.Proxy);
                    if (options.ProxyCredentials != null)
                    {
                        request.Proxy.Credentials = options.ProxyCredentials;
                    }
                }

                // Handle upload: -T FILE puts the file's exact bytes (options.BinaryData); a hand-built
                // request may carry text in options.Data instead. An empty file is uploaded as an empty file.
                if (request.Method == WebRequestMethods.Ftp.UploadFile)
                {
                    var fileContents = UploadPayload(options);
                    request.ContentLength = fileContents.Length;

                    using (Stream requestStream = await Task.Factory.FromAsync(
                        request.BeginGetRequestStream,
                        request.EndGetRequestStream,
                        null).ConfigureAwait(false))
                    {
                        await requestStream.WriteAsync(fileContents, 0, fileContents.Length, cancellationToken).ConfigureAwait(false);
                    }
                }

                // Get response
                using var response = (FtpWebResponse)await Task.Factory.FromAsync(
                    request.BeginGetResponse,
                    request.EndGetResponse,
                    null).ConfigureAwait(false);

                var result = new CurlResult
                {
                    StatusCode = (int)response.StatusCode,
                    Command = options.OriginalCommand
                };

                // Add FTP-specific headers
                result.Headers["Status-Description"] = response.StatusDescription;
                result.Headers["Banner-Message"] = response.BannerMessage ?? "";
                result.Headers["Welcome-Message"] = response.WelcomeMessage ?? "";
                result.Headers["Exit-Message"] = response.ExitMessage ?? "";

                // Download content if applicable
                if (request.Method == WebRequestMethods.Ftp.DownloadFile ||
                    request.Method == WebRequestMethods.Ftp.ListDirectory ||
                    request.Method == WebRequestMethods.Ftp.ListDirectoryDetails)
                {
                    // Read raw bytes: FTP downloads are frequently binary, and decoding
                    // through a StreamReader corrupts them. Directory listings are text.
                    byte[] contentBytes;
                    using (var stream = response.GetResponseStream())
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms).ConfigureAwait(false);
                        contentBytes = ms.ToArray();
                    }

                    var isListing = request.Method != WebRequestMethods.Ftp.DownloadFile;
                    if (isListing || !HttpHandler.LooksBinary(contentBytes))
                    {
                        result.Body = Encoding.UTF8.GetString(contentBytes);
                    }
                    else
                    {
                        result.BinaryData = contentBytes;
                    }

                    // Save to output file if specified - always write the wire bytes
                    // verbatim, like curl does (I/O failures become exit code 23 naming the file).
                    if (!string.IsNullOrEmpty(options.OutputFile) && options.OutputFile != "-")
                    {
                        TransferOutput.EnsureDirectory(options.OutputFile!, options, "-o/--output");
                        TransferOutput.WriteFile(options.OutputFile!, contentBytes, append: false, options, "-o/--output");
                        result.OutputFiles.Add(options.OutputFile!);
                    }
                }

                return result;
            }
            catch (NotSupportedException)
            {
                throw new CurlException(NotSupportedMessage(options), 1, options.OriginalCommand); // CURLE_UNSUPPORTED_PROTOCOL
            }
            catch (WebException ex)
            {
                if (ex.Response is FtpWebResponse ftpResponse)
                {
                    throw new CurlFtpException(FtpReplyMessage((int)ftpResponse.StatusCode, ftpResponse.StatusDescription, options),
                        (int)ftpResponse.StatusCode, options.OriginalCommand);
                }

                var host = Uri.TryCreate(options.Url, UriKind.Absolute, out var uri) ? uri.Host : ErrorText.SafeUrl(options.Url);
                var port = uri != null && uri.Port > 0 ? uri.Port : 21;
                // the subtype stays "could not connect" (exit 7); the detail says what WebRequest reported
                throw new CurlCouldntConnectException(host, port, $"{ex.Status}: {ex.Message.TrimEnd('.')}", viaProxy: !string.IsNullOrEmpty(options.Proxy),
                    options.OriginalCommand);
            }
            catch (IOException ex)
            {
                // the control or data connection broke while sending or receiving
                throw new CurlReceiveErrorException(CurlException.Compose(
                    $"Failure when receiving data from the FTP server of {ErrorText.SafeUrl(options.Url)}: {ex.GetType().Name}: {ex.Message}",
                    "The FTP connection broke during the transfer; retry, and check that a firewall or NAT between you and the server allows the passive-mode data connection CurlDotNet uses.",
                    DocLinks.ErrorReference), options.OriginalCommand, ex);
            }
            catch (UriFormatException ex)
            {
                throw new CurlMalformedUrlException(options.Url, CurlException.Compose(
                    $"'{ErrorText.SafeUrl(options.Url)}' is not a valid FTP URL ({ex.Message})",
                    "Write it as ftp://host[:port]/path/file (ftps:// for TLS), percent-encode spaces as %20, and pass credentials with -u user:password.",
                    DocLinks.Url), options.OriginalCommand);
            }
        }

        private static string NotSupportedMessage(CurlOptions options) => CurlException.Compose(
            $"FTP is not supported by the .NET runtime on this platform, so {ErrorText.SafeUrl(options.Url)} cannot be fetched (FtpWebRequest is unavailable)",
            "Use an http:// or https:// URL, or run the FTP transfer on a desktop or server runtime (.NET Framework or .NET on Windows, Linux or macOS); " +
            "browser WebAssembly and some trimmed or mobile runtimes do not include FtpWebRequest.",
            DocLinks.SupportedProtocols);

        /// <summary>A different message for each class of FTP reply: login refused, file unavailable, server busy, other.</summary>
        private static string FtpReplyMessage(int code, string? description, CurlOptions options)
        {
            var url = ErrorText.SafeUrl(options.Url);
            var what = $"The FTP server answered {code} ({description?.Trim()}) for {url}";
            string fix;
            if (code == 530 || code == 331 || code == 332)
            {
                fix = options.Credentials == null
                    ? "The server refused the anonymous login; pass a user name and password with -u user:password."
                    : "The server refused the user name or password given with -u; check them (and the account's FTP access).";
            }
            else if (code == 550 || code == 553)
            {
                fix = "The file or folder is missing or this user may not access it; check the path in the URL (it is relative to the login folder; use %2F for an absolute path) and the user's permissions.";
            }
            else if (code == 421 || code == 425 || code == 426)
            {
                fix = "The server closed or could not open the data connection; retry later, and check that a firewall or NAT allows the passive-mode data connection (CurlDotNet always uses passive mode).";
            }
            else if (code == 552)
            {
                fix = "The server's storage limit for this user is exceeded; free space on the server or upload a smaller file.";
            }
            else
            {
                fix = "Check the path, the user name and password (-u user:password; anonymous is used without -u) and that the server allows this operation.";
            }
            return CurlException.Compose(what, fix, DocLinks.ErrorReference);
        }

        /// <summary>The bytes an FTP upload sends: the <c>-T</c> file, else <see cref="CurlOptions.Data"/> as UTF-8, else nothing.</summary>
        internal static byte[] UploadPayload(CurlOptions options)
        {
            if (options.BinaryData != null)
            {
                return options.BinaryData;
            }
            return options.Data != null ? Encoding.UTF8.GetBytes(options.Data) : Array.Empty<byte>();
        }

        public bool SupportsProtocol(string protocol)
        {
            return protocol.Equals("ftp", StringComparison.OrdinalIgnoreCase) ||
                   protocol.Equals("ftps", StringComparison.OrdinalIgnoreCase);
        }

        private string DetermineFtpMethod(CurlOptions options)
        {
            // Determine FTP method based on options and URL
            if (!string.IsNullOrEmpty(options.CustomMethod))
            {
                return MapToFtpMethod(options.CustomMethod);
            }

            // If uploading data
            if (!string.IsNullOrEmpty(options.Data) || options.BinaryData != null)
            {
                return WebRequestMethods.Ftp.UploadFile;
            }

            // If URL ends with / assume directory listing
            if (options.Url.EndsWith("/"))
            {
                return options.Verbose
                    ? WebRequestMethods.Ftp.ListDirectoryDetails
                    : WebRequestMethods.Ftp.ListDirectory;
            }

            // Default to download
            return WebRequestMethods.Ftp.DownloadFile;
        }

        private string MapToFtpMethod(string method)
        {
            return method.ToUpper() switch
            {
                "LIST" => WebRequestMethods.Ftp.ListDirectory,
                "NLST" => WebRequestMethods.Ftp.ListDirectory,
                "RETR" => WebRequestMethods.Ftp.DownloadFile,
                "STOR" => WebRequestMethods.Ftp.UploadFile,
                "DELE" => WebRequestMethods.Ftp.DeleteFile,
                "MKD" => WebRequestMethods.Ftp.MakeDirectory,
                "RMD" => WebRequestMethods.Ftp.RemoveDirectory,
                "PWD" => WebRequestMethods.Ftp.PrintWorkingDirectory,
                "SIZE" => WebRequestMethods.Ftp.GetFileSize,
                "MDTM" => WebRequestMethods.Ftp.GetDateTimestamp,
                _ => WebRequestMethods.Ftp.DownloadFile
            };
        }
    }

    /// <summary>
    /// Thrown when an FTP server answers an <c>ftp://</c> or <c>ftps://</c> transfer with an error reply
    /// (4xx/5xx), such as 530 "Not logged in" or 550 "File unavailable"; <see cref="FtpStatusCode"/> holds the reply code.
    /// </summary>
    /// <remarks>
    /// <para>The message names the reply code, the server's text and the URL (without credentials), and
    /// says how to fix that class of reply: 530/331/332 mean the login was refused (pass or correct
    /// <c>-u user:password</c>; without <c>-u</c> CurlDotNet logs in as <c>anonymous</c>), 550/553 mean the
    /// path is missing or not permitted (FTP paths are relative to the login folder; write <c>%2F</c> for an
    /// absolute path), 421/425/426 mean the data connection failed (CurlDotNet uses passive mode; check that firewalls allow the
    /// passive data ports), 552 means the server's storage limit was hit.</para>
    /// <para><see cref="CurlException.CurlErrorCode"/> is 9 (<c>CURLE_REMOTE_ACCESS_DENIED</c>) for every reply code.
    /// A server that cannot be reached at all is a <see cref="CurlCouldntConnectException"/> (exit 7) instead.</para>
    /// <para>FTP uses <c>FtpWebRequest</c>, which .NET Framework and .NET on Windows, Linux and macOS include;
    /// where it is missing (browser WebAssembly, some trimmed runtimes) the transfer fails with exit code 1
    /// before any reply.</para>
    /// <para>Example: <c>try { await Curl.ExecuteAsync("curl -u alice:secret ftp://files.example.com/reports/q3.csv -o q3.csv"); }
    /// catch (CurlFtpException ex) when (ex.FtpStatusCode == 550) { Console.WriteLine("No such report yet"); }</c></para>
    /// <para>Reply codes and fixes: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</para>
    /// </remarks>
    /// <seealso cref="CurlException"/>
    /// <seealso cref="CurlCouldntConnectException"/>
    /// <seealso cref="CurlDotNet.Exceptions.CurlLoginDeniedException"/>
    public class CurlFtpException : CurlException
    {
        /// <summary>
        /// The FTP reply code the server sent, for example 530 (not logged in), 550 (file unavailable)
        /// or 552 (storage allocation exceeded).
        /// </summary>
        /// <value>A three-digit FTP reply code from RFC 959; 4xx codes are temporary, 5xx permanent.</value>
        /// <remarks>
        /// <para>Use it to decide what to do: retry later on 4xx, fix credentials on 530, fix the path on 550.</para>
        /// <para>Example: <c>catch (CurlFtpException ex) when (ex.FtpStatusCode / 100 == 4) { await Task.Delay(5000); /* retry */ }</c></para>
        /// </remarks>
        /// <seealso cref="CurlException.CurlErrorCode"/>
        public int FtpStatusCode { get; }

        /// <summary>
        /// Creates the exception for an FTP error reply; the message should say what failed and how to fix it.
        /// </summary>
        /// <remarks>
        /// <para>The exit code (<see cref="CurlException.CurlErrorCode"/>) is always 9. When
        /// <paramref name="message"/> has no fix line or link, the generic advice and documentation page for
        /// exit code 9 are added, so <see cref="CurlException.Suggestions"/> is never empty.</para>
        /// <para>Example: <c>throw new CurlFtpException("The FTP server answered 550 for ftp://example.com/x.csv\nCheck the path.", 550);</c></para>
        /// </remarks>
        /// <param name="message">What happened, then (optionally, on new lines) how to fix it and a
        /// <c>See https://...</c> link.</param>
        /// <param name="ftpStatusCode">The FTP reply code from the server, such as 530 or 550.</param>
        /// <seealso cref="CurlFtpException(string, int, string)"/>
        public CurlFtpException(string message, int ftpStatusCode)
            : base(message, 9) // CURLE_REMOTE_ACCESS_DENIED
        {
            FtpStatusCode = ftpStatusCode;
        }

        /// <summary>
        /// Creates the exception for an FTP error reply and records the curl command that was running,
        /// so <see cref="CurlException.Command"/> shows what to retry.
        /// </summary>
        /// <remarks>
        /// <para>Same as <see cref="CurlFtpException(string, int)"/> plus the command. The command is kept as
        /// given (it may contain <c>-u user:password</c>), so log <see cref="CurlException.Message"/> rather than
        /// <see cref="CurlException.Command"/> when credentials must not reach the logs.</para>
        /// <para>Example: <c>throw new CurlFtpException(message, 530, "curl -u bob:x ftp://example.com/");</c></para>
        /// </remarks>
        /// <param name="message">What happened, then how to fix it and a <c>See https://...</c> link, on separate lines.</param>
        /// <param name="ftpStatusCode">The FTP reply code from the server, such as 530 or 550.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlFtpException(string, int)"/>
        public CurlFtpException(string message, int ftpStatusCode, string? command)
            : base(message, 9, command)
        {
            FtpStatusCode = ftpStatusCode;
        }
    }
}