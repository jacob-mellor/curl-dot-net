/***************************************************************************
 * TransportErrors - turns transport failures into the curl exit code curl would use
 *
 * HttpClient reports most problems as HttpRequestException/IOException with a generic
 * message. curl distinguishes:
 *   5  could not resolve proxy        6  could not resolve host     7  could not connect
 *   1  unsupported protocol (HTTP/0.9, HTTP/1.2, bad status line)
 *   8  weird server reply (malformed header, bad Content-Length, duplicate Location)
 *  18  partial file (Content-Length or chunked body cut short)
 *  28  timeout                      52  empty reply              56  failure receiving data
 * 100  response headers too large
 * The decision uses the connect callback's failure kind and the recorded wire bytes.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    internal static class TransportErrors
    {
        private static readonly Regex StatusLine = new Regex(@"^HTTP/1\.[01] \d{3}( |\r?$)", RegexOptions.Compiled);

        /// <summary>
        /// Applies curl's response-header checks that HttpClient is more lenient about. Throws the
        /// curl exception for the first violation.
        /// </summary>
        public static void ValidateHeads(WireExchange exchange, HttpTransfer transfer)
        {
            foreach (var head in exchange.Heads)
            {
                var problem = Validate(head);
                if (problem != null)
                {
                    throw problem.Value.Code == 1
                        ? (CurlException)new CurlUnsupportedProtocolException(StatusLineOf(head.Text), problem.Value.Message, transfer.Options.OriginalCommand)
                        : new CurlWeirdServerReplyException(WeirdMessage(problem.Value.Message, transfer), transfer.Options.OriginalCommand);
                }
            }
        }

        private static string StatusLineOf(string text)
        {
            var nl = text.IndexOf('\n');
            return (nl < 0 ? text : text.Substring(0, nl)).TrimEnd('\r');
        }

        private static string WeirdMessage(string what, HttpTransfer transfer)
        {
            var host = transfer.CurrentRequest?.RequestUri?.Host ?? transfer.FirstUri.Host;
            return $"Weird server reply from {host}: {what}. curl rejects responses that break HTTP/1.1 framing rules because " +
                   "they can be used to smuggle responses; fix the server, there is no client option to accept it." +
                   "\nFor HTTP error handling, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md";
        }

        /// <summary>Returns the first rule <paramref name="head"/> breaks, or null.</summary>
        internal static (int Code, string Message)? Validate(RawResponseHead head)
        {
            var lines = head.Text.Split('\n');
            var status = lines[0].TrimEnd('\r');
            if (!StatusLine.IsMatch(status))
            {
                if (status.StartsWith("HTTP/1.", StringComparison.Ordinal) && !Regex.IsMatch(status, @"^HTTP/1\.\d "))
                {
                    return (1, $"Unsupported HTTP version in response: \"{Shorten(status)}\". curl speaks HTTP/1.0 and HTTP/1.1 here; the server sent something else");
                }
                if (status.StartsWith("HTTP/", StringComparison.Ordinal))
                {
                    return (1, $"Unsupported HTTP version or invalid status line in response: \"{Shorten(status)}\". A status line must look like \"HTTP/1.1 200 OK\" with a three-digit code");
                }
                return (1, $"Received HTTP/0.9 when not allowed: the reply does not start with an HTTP status line (\"{Shorten(status)}\"). Add --http0.9 if this server really speaks HTTP/0.9");
            }

            var locations = 0;
            string? contentLength = null;
            for (var i = 1; i < lines.Length; i++)
            {
                var raw = lines[i];
                var line = raw.EndsWith("\r", StringComparison.Ordinal) ? raw.Substring(0, raw.Length - 1) : raw;
                if (line.Length == 0)
                {
                    continue;
                }
                if (line.IndexOf('\0') >= 0)
                {
                    return (8, $"a header contains a NUL byte (\"{Shorten(line.Replace("\0", "\\0"))}\")");
                }
                if (line.IndexOf('\r') >= 0)
                {
                    return (8, $"a header contains a stray carriage return (\"{Shorten(line.Replace("\r", "\\r"))}\")");
                }
                if (line[0] == ' ' || line[0] == '\t')
                {
                    continue; // folded continuation line
                }
                var colon = line.IndexOf(':');
                if (colon <= 0)
                {
                    return (8, $"header line without a colon (\"{Shorten(line)}\")");
                }

                var name = line.Substring(0, colon).Trim();
                var value = line.Substring(colon + 1).Trim();
                if (name.Equals("Location", StringComparison.OrdinalIgnoreCase))
                {
                    locations++;
                    if (locations > 1)
                    {
                        return (8, "more than one Location header");
                    }
                }
                else if (name.Equals("Content-Length", StringComparison.OrdinalIgnoreCase) && !head.IsInformational)
                {
                    if (value.Length == 0 || !value.All(char.IsDigit))
                    {
                        return (8, $"invalid Content-Length value \"{Shorten(value)}\"");
                    }
                    if (contentLength != null && contentLength.TrimStart('0') != value.TrimStart('0'))
                    {
                        return (8, $"conflicting Content-Length values {contentLength} and {value}");
                    }
                    contentLength = value;
                }
            }

            return null;
        }

        private static string Shorten(string s) => s.Length > 80 ? s.Substring(0, 77) + "..." : s;

        /// <summary>
        /// Maps an exception from sending a request or reading its body to the curl exception
        /// curl would report. Returns null when the exception is not a transport failure.
        /// </summary>
        public static CurlException? Translate(Exception ex, HttpTransfer transfer, bool readingBody)
        {
            var command = transfer.Options.OriginalCommand;
            var uri = transfer.CurrentRequest?.RequestUri ?? transfer.FirstUri;
            var viaProxy = transfer.CurrentViaProxy;

            for (var e = ex; e != null; e = e.InnerException)
            {
                if (e is CurlException curl)
                {
                    return curl;
                }

                if (e is CurlConnectFailure failure)
                {
                    switch (failure.Kind)
                    {
                        case ConnectFailureKind.Resolve:
                            return viaProxy
                                ? (CurlException)new CurlCouldntResolveProxyException(failure.Host, failure.Message, command)
                                : new CurlCouldntResolveHostException(failure.Host, failure.Message, command);
                        case ConnectFailureKind.Connect:
                            return new CurlCouldntConnectException(failure.Host, failure.Port, failure.Message, viaProxy, command);
                        case ConnectFailureKind.Timeout:
                            var seconds = transfer.HopOptions.ConnectTimeout ?? 0;
                            return new CurlOperationTimeoutException(seconds,
                                $"Connection timed out after {(long)(seconds * 1000)} milliseconds while connecting to {failure.Host} port {failure.Port}" +
                                $"{(viaProxy ? " (the proxy)" : string.Empty)}. Raise --connect-timeout, or check that the host is reachable from this machine.",
                                command);
                        case ConnectFailureKind.Tunnel:
                            return new CurlCouldntConnectException(failure.Host, failure.Port, failure.Message +
                                (failure.TunnelStatus == 407 ? " (proxy authentication required: pass -U user:password or --proxy-anyauth)" : string.Empty),
                                viaProxy: true, command);
                    }
                }

                if (e is SocketException socket && transfer.LastExchange?.Segments.Count == 0)
                {
                    switch (socket.SocketErrorCode)
                    {
                        case SocketError.HostNotFound:
                        case SocketError.TryAgain:
                        case SocketError.NoData:
                            return viaProxy
                                ? (CurlException)new CurlCouldntResolveProxyException(ProxyHost(transfer, uri), socket.Message, command)
                                : new CurlCouldntResolveHostException(uri.Host, socket.Message, command);
                        default:
                            return new CurlCouldntConnectException(viaProxy ? ProxyHost(transfer, uri) : uri.Host,
                                viaProxy ? ProxyPort(transfer, uri) : uri.Port, socket.Message, viaProxy, command);
                    }
                }
            }

            var exchange = transfer.LastExchange;
            var recorded = exchange != null && exchange.Segments.Count > 0;

#if NET8_0_OR_GREATER
            var error = (ex as HttpRequestException)?.HttpRequestError
                        ?? (ex as HttpIOException)?.HttpRequestError
                        ?? (ex.InnerException as HttpIOException)?.HttpRequestError;
            if (!recorded)
            {
                switch (error)
                {
                    case HttpRequestError.NameResolutionError:
                        return viaProxy
                            ? (CurlException)new CurlCouldntResolveProxyException(ProxyHost(transfer, uri), ex.Message, command)
                            : new CurlCouldntResolveHostException(uri.Host, ex.Message, command);
                    case HttpRequestError.ConnectionError:
                        return new CurlCouldntConnectException(viaProxy ? ProxyHost(transfer, uri) : uri.Host,
                            viaProxy ? ProxyPort(transfer, uri) : uri.Port, InnermostMessage(ex), viaProxy, command);
                    case HttpRequestError.ResponseEnded:
                        return new CurlGotNothingException(uri.Host, command, string.Empty);
                    case HttpRequestError.ConfigurationLimitExceeded:
                        return new CurlTooLargeException(InnermostMessage(ex), command);
                    case HttpRequestError.InvalidResponse:
                        return new CurlWeirdServerReplyException(WeirdMessage(InnermostMessage(ex), transfer), command);
                }
            }
            else if (error == HttpRequestError.ConfigurationLimitExceeded)
            {
                return new CurlTooLargeException("the response header block exceeds the limit (" + InnermostMessage(ex) + ")", command);
            }
#endif

            if (recorded)
            {
                return FromRecording(ex, exchange!, transfer, readingBody, uri);
            }

            if (ex is HttpRequestException)
            {
                // No recording (netstandard2.0/.NET Framework or a custom HttpClient): curl's closest match
                // for "could not complete the request" is a connection failure.
                return new CurlCouldntConnectException(viaProxy ? ProxyHost(transfer, uri) : uri.Host,
                    viaProxy ? ProxyPort(transfer, uri) : uri.Port, InnermostMessage(ex), viaProxy, command);
            }

            if (ex is IOException)
            {
                return new CurlReceiveErrorException(ReceiveMessage(InnermostMessage(ex), uri), command);
            }

            return null;
        }

        private static CurlException FromRecording(Exception ex, WireExchange exchange, HttpTransfer transfer, bool readingBody, Uri uri)
        {
            var command = transfer.Options.OriginalCommand;
            var segments = exchange.Segments;
            var segment = segments.LastOrDefault(s => s.Parser.TotalBytes > 0) ?? segments.Last();
            var parser = segment.Parser;

            if (!exchange.ReceivedAnything)
            {
                return new CurlGotNothingException(transfer.CurrentViaProxy ? ProxyHost(transfer, uri) : uri.Host, command, string.Empty);
            }

            var final = parser.Heads.LastOrDefault();
            var pending = parser.PendingHeadBytes > 0;

            if (final == null || (pending && final.IsInformational) || (pending && final.BodyComplete))
            {
                // the header block never completed
                if (final == null && pending)
                {
                    var text = parser.PendingHeadText;
                    if (!text.StartsWith("HTTP/", StringComparison.Ordinal))
                    {
                        return new CurlUnsupportedProtocolException("HTTP/0.9",
                            $"Received HTTP/0.9 when not allowed: the reply from {uri.Host} does not start with an HTTP status line. " +
                            "Add --http0.9 if this server really speaks HTTP/0.9.", command);
                    }

                    var probe = RawResponseHead.Parse(text + "\r\n", 0);
                    var problem = Validate(probe);
                    if (problem != null)
                    {
                        return problem.Value.Code == 1
                            ? (CurlException)new CurlUnsupportedProtocolException(StatusLineOf(text), problem.Value.Message, command)
                            : new CurlWeirdServerReplyException(WeirdMessage(problem.Value.Message, transfer), command);
                    }

                    if (parser.Closed)
                    {
                        return new CurlPartialFileException(uri.AbsoluteUri, 0, null, command);
                    }
                }

                if (final != null && final.IsInformational && !pending)
                {
                    return new CurlWeirdServerReplyException(WeirdMessage($"the server sent \"{StatusLineOf(final.Text)}\" and then no final response", transfer), command);
                }

                return new CurlWeirdServerReplyException(WeirdMessage("the response header block is malformed (" + InnermostMessage(ex) + ")", transfer), command);
            }

            if (final.IsInformational)
            {
                return new CurlWeirdServerReplyException(WeirdMessage($"the server sent \"{StatusLineOf(final.Text)}\" and then data that is not an HTTP response", transfer), command);
            }

            var problemFinal = Validate(final);
            if (problemFinal != null)
            {
                return problemFinal.Value.Code == 1
                    ? (CurlException)new CurlUnsupportedProtocolException(StatusLineOf(final.Text), problemFinal.Value.Message, command)
                    : new CurlWeirdServerReplyException(WeirdMessage(problemFinal.Value.Message, transfer), command);
            }

            if (!final.BodyComplete)
            {
                if (parser.Closed || IsResponseEnded(ex))
                {
                    if (final.DeclaredLength.HasValue || final.Chunked)
                    {
                        return new CurlPartialFileException(uri.AbsoluteUri, final.BodyBytes, final.DeclaredLength, command);
                    }
                }

                if (final.Chunked)
                {
                    return new CurlReceiveErrorException(ReceiveMessage("the chunked body is malformed (" + InnermostMessage(ex) + ")", uri), command);
                }
            }

            return new CurlReceiveErrorException(ReceiveMessage(InnermostMessage(ex), uri), command);
        }

        private static bool IsResponseEnded(Exception ex)
        {
#if NET8_0_OR_GREATER
            for (var e = ex; e != null; e = e.InnerException)
            {
                if (e is HttpIOException io && io.HttpRequestError == HttpRequestError.ResponseEnded) return true;
                if (e is HttpRequestException hre && hre.HttpRequestError == HttpRequestError.ResponseEnded) return true;
            }
#endif
            return ex.Message.IndexOf("ended prematurely", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string ReceiveMessage(string detail, Uri uri)
        {
            return $"Failure when receiving data from {uri.Host}: {detail}. The connection broke or the server sent data that " +
                   "is not valid HTTP; retry with --retry, or inspect the exchange with -v or --trace-ascii -." +
                   "\nFor HTTP error handling, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md";
        }

        private static string InnermostMessage(Exception ex)
        {
            var e = ex;
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }
            return e.Message.TrimEnd('.');
        }

        private static string ProxyHost(HttpTransfer transfer, Uri uri) => transfer.Proxy?.ProxyFor(uri)?.Host ?? uri.Host;

        private static int ProxyPort(HttpTransfer transfer, Uri uri) => transfer.Proxy?.ProxyFor(uri)?.Port ?? uri.Port;
    }
}
