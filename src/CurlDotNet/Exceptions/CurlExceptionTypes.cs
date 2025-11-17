/***************************************************************************
 * Comprehensive curl exception hierarchy
 *
 * Every curl error code gets its own exception type for precise catching
 * Based on curl's libcurl/include/curl/curl.h error codes
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * By Jacob Mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Runtime.Serialization;

namespace CurlDotNet.Exceptions
{
    /// <summary>
    /// CURLE_UNSUPPORTED_PROTOCOL (1) - Unsupported protocol
    /// </summary>
    [Serializable]
    public class CurlUnsupportedProtocolException : CurlException
    {
        /// <summary>
        /// Gets the unsupported protocol that was attempted
        /// </summary>
        public string Protocol { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlUnsupportedProtocolException"/> class.
        /// </summary>
        /// <param name="protocol">The protocol that is not supported (e.g., "gopher", "telnet").</param>
        /// <param name="command">The curl command that was executing when the error occurred.</param>
        public CurlUnsupportedProtocolException(string protocol, string command = null)
            : base($"Unsupported protocol: {protocol}\nFor supported protocols, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/supported-protocols.md", 1, command)
        {
            Protocol = protocol;
        }
    }

    /// <summary>
    /// CURLE_FAILED_INIT (2) - Failed to initialize
    /// </summary>
    [Serializable]
    public class CurlFailedInitException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlFailedInitException"/> class.
        /// </summary>
        /// <param name="message">The error message describing the initialization failure.</param>
        /// <param name="command">The curl command that was executing when the error occurred.</param>
        public CurlFailedInitException(string message, string command = null)
            : base($"{message}\nFor initialization help, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/init-errors.md", 2, command) { }
    }

    /// <summary>
    /// CURLE_URL_MALFORMAT (3) - Malformed URL
    /// </summary>
    [Serializable]
    public class CurlMalformedUrlException : CurlException
    {
        /// <summary>
        /// Gets the malformed URL that caused the error
        /// </summary>
        public string MalformedUrl { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlMalformedUrlException"/> class.
        /// </summary>
        /// <param name="url">The malformed URL that caused the error.</param>
        /// <param name="command">The curl command that was executing when the error occurred.</param>
        public CurlMalformedUrlException(string url, string command = null)
            : base($"Malformed URL: {url}\nFor URL syntax help, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md", 3, command)
        {
            MalformedUrl = url;
        }
    }

    /// <summary>
    /// CURLE_NOT_BUILT_IN (4) - Feature not built in
    /// </summary>
    [Serializable]
    public class CurlNotBuiltInException : CurlException
    {
        /// <summary>
        /// Gets the feature that is not available
        /// </summary>
        public string Feature { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlNotBuiltInException"/> class.
        /// </summary>
        /// <param name="feature">The feature that is not available in this build.</param>
        /// <param name="command">The curl command that was executing when the error occurred.</param>
        public CurlNotBuiltInException(string feature, string command = null)
            : base($"Feature not available: {feature}", 4, command)
        {
            Feature = feature;
        }
    }

    /// <summary>
    /// CURLE_COULDNT_RESOLVE_PROXY (5) - Couldn't resolve proxy
    /// </summary>
    [Serializable]
    public class CurlCouldntResolveProxyException : CurlException
    {
        /// <summary>
        /// Gets the proxy host that could not be resolved
        /// </summary>
        public string ProxyHost { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlCouldntResolveProxyException"/> class.
        /// </summary>
        /// <param name="proxyHost">The proxy hostname that could not be resolved.</param>
        /// <param name="command">The curl command that was executing when the error occurred.</param>
        public CurlCouldntResolveProxyException(string proxyHost, string command = null)
            : base($"Could not resolve proxy: {proxyHost}", 5, command)
        {
            ProxyHost = proxyHost;
        }
    }

    /// <summary>
    /// CURLE_COULDNT_RESOLVE_HOST (6) - Couldn't resolve host
    /// </summary>
    [Serializable]
    public class CurlCouldntResolveHostException : CurlException
    {
        /// <summary>
        /// Gets the hostname that could not be resolved
        /// </summary>
        public string Hostname { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlCouldntResolveHostException"/> class.
        /// </summary>
        /// <param name="hostname">The hostname that could not be resolved.</param>
        /// <param name="command">The curl command that was executing when the error occurred.</param>
        public CurlCouldntResolveHostException(string hostname, string command = null)
            : base($"Could not resolve host: {hostname}", 6, command)
        {
            Hostname = hostname;
        }
    }

    /// <summary>
    /// CURLE_COULDNT_CONNECT (7) - Failed to connect to host
    /// </summary>
    [Serializable]
    public class CurlCouldntConnectException : CurlException
    {
        /// <summary>
        /// Gets the host that could not be connected to
        /// </summary>
        public string Host { get; }
        /// <summary>
        /// Gets the port number that could not be connected to
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlCouldntConnectException"/> class.
        /// </summary>
        /// <param name="host">The host that could not be connected to.</param>
        /// <param name="port">The port number that was attempted.</param>
        /// <param name="command">The curl command that was executing when the error occurred.</param>
        public CurlCouldntConnectException(string host, int port, string command = null)
            : base($"Failed to connect to {host}:{port}", 7, command)
        {
            Host = host;
            Port = port;
        }
    }

    /// <summary>
    /// CURLE_WEIRD_SERVER_REPLY (8) - Weird server reply
    /// </summary>
    [Serializable]
    public class CurlWeirdServerReplyException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlWeirdServerReplyException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlWeirdServerReplyException(string message, string command = null)
            : base(message, 8, command) { }
    }

    /// <summary>
    /// CURLE_REMOTE_ACCESS_DENIED (9) - Access denied to remote resource
    /// </summary>
    [Serializable]
    public class CurlRemoteAccessDeniedException : CurlException
    {
        /// <summary>
        /// Gets the resource that was denied access
        /// </summary>
        public string Resource { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlRemoteAccessDeniedException"/> class.
        /// </summary>
        /// <param name="resource">The resource that was denied access.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlRemoteAccessDeniedException(string resource, string command = null)
            : base($"Access denied to: {resource}", 9, command)
        {
            Resource = resource;
        }
    }

    /// <summary>
    /// CURLE_FTP_ACCEPT_FAILED (10) - FTP accept failed
    /// </summary>
    [Serializable]
    public class CurlFtpAcceptFailedException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of the CurlFtpAcceptFailedException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlFtpAcceptFailedException(string message, string command = null)
            : base(message, 10, command) { }
    }

    /// <summary>
    /// CURLE_FTP_WEIRD_PASS_REPLY (11) - FTP weird PASS reply
    /// </summary>
    [Serializable]
    public class CurlFtpWeirdPassReplyException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlFtpWeirdPassReplyException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlFtpWeirdPassReplyException(string message, string command = null)
            : base(message, 11, command) { }
    }

    /// <summary>
    /// CURLE_FTP_ACCEPT_TIMEOUT (12) - FTP accept timeout
    /// </summary>
    [Serializable]
    public class CurlFtpAcceptTimeoutException : CurlTimeoutException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlFtpAcceptTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlFtpAcceptTimeoutException(string message, string command = null)
            : base(message, command, null)
        {
        }
    }

    /// <summary>
    /// CURLE_HTTP_RETURNED_ERROR (22) - HTTP returned error
    /// </summary>
    [Serializable]
    public class CurlHttpReturnedErrorException : CurlHttpException
    {
        /// <summary>
        /// Initializes a new instance of CurlHttpReturnedErrorException.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="statusText">The HTTP status text.</param>
        /// <param name="body">The response body.</param>
        /// <param name="command">The curl command that failed.</param>
        public CurlHttpReturnedErrorException(int statusCode, string statusText, string body, string? command = null)
            : base($"HTTP error {statusCode}: {statusText}", statusCode, statusText, body, command)
        {
        }
    }

    /// <summary>
    /// CURLE_WRITE_ERROR (23) - Write error
    /// </summary>
    [Serializable]
    public class CurlWriteErrorException : CurlException
    {
        /// <summary>
        /// The file path where the write error occurred.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Initializes a new instance of CurlWriteErrorException.
        /// </summary>
        /// <param name="filePath">The file path where the error occurred.</param>
        /// <param name="message">The error message.</param>
        /// <param name="command">The curl command that failed.</param>
        public CurlWriteErrorException(string filePath, string message, string? command = null)
            : base($"Write error for {filePath}: {message}", 23, command)
        {
            FilePath = filePath;
        }
    }

    /// <summary>
    /// CURLE_UPLOAD_FAILED (25) - Upload failed
    /// </summary>
    [Serializable]
    public class CurlUploadFailedException : CurlException
    {
        /// <summary>
        /// Gets the file name that failed to upload
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlUploadFailedException"/> class.
        /// </summary>
        /// <param name="fileName">The name of the file that failed to upload.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlUploadFailedException(string fileName, string message, string command = null)
            : base($"Upload failed for {fileName}: {message}", 25, command)
        {
            FileName = fileName;
        }
    }

    /// <summary>
    /// CURLE_READ_ERROR (26) - Read error
    /// </summary>
    [Serializable]
    public class CurlReadErrorException : CurlException
    {
        /// <summary>
        /// The file path where the read error occurred.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Initializes a new instance of CurlReadErrorException.
        /// </summary>
        /// <param name="filePath">The file path where the error occurred.</param>
        /// <param name="message">The error message.</param>
        /// <param name="command">The curl command that failed.</param>
        public CurlReadErrorException(string filePath, string message, string? command = null)
            : base($"Read error for {filePath}: {message}", 26, command)
        {
            FilePath = filePath;
        }
    }

    /// <summary>
    /// CURLE_OUT_OF_MEMORY (27) - Out of memory
    /// </summary>
    [Serializable]
    public class CurlOutOfMemoryException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlOutOfMemoryException"/> class.
        /// </summary>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlOutOfMemoryException(string command = null)
            : base("Out of memory", 27, command) { }
    }

    /// <summary>
    /// CURLE_OPERATION_TIMEDOUT (28) - Operation timeout
    /// </summary>
    [Serializable]
    public class CurlOperationTimeoutException : CurlTimeoutException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlOperationTimeoutException"/> class.
        /// </summary>
        /// <param name="timeoutSeconds">The timeout value in seconds that was exceeded.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlOperationTimeoutException(double timeoutSeconds, string command = null)
            : base($"Operation timed out after {timeoutSeconds} seconds", command, TimeSpan.FromSeconds(timeoutSeconds))
        {
        }
    }

    /// <summary>
    /// CURLE_HTTP_POST_ERROR (34) - HTTP POST error
    /// </summary>
    [Serializable]
    public class CurlHttpPostErrorException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of CurlHttpPostErrorException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="command">The curl command that failed.</param>
        public CurlHttpPostErrorException(string message, string? command = null)
            : base($"HTTP POST error: {message}", 34, command) { }
    }

    /// <summary>
    /// CURLE_SSL_CONNECT_ERROR (35) - SSL connect error
    /// </summary>
    [Serializable]
    public class CurlSslConnectErrorException : CurlSslException
    {
        /// <summary>
        /// Initializes a new instance of CurlSslConnectErrorException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="command">The curl command that failed.</param>
        public CurlSslConnectErrorException(string message, string? command = null)
            : base($"SSL connect error: {message}", null, command)
        {
        }
    }

    /// <summary>
    /// CURLE_BAD_DOWNLOAD_RESUME (36) - Bad download resume
    /// </summary>
    [Serializable]
    public class CurlBadDownloadResumeException : CurlException
    {
        /// <summary>
        /// Gets the offset where the bad download resume occurred
        /// </summary>
        public long ResumeOffset { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlBadDownloadResumeException"/> class.
        /// </summary>
        /// <param name="offset">The offset where the bad download resume occurred.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlBadDownloadResumeException(long offset, string command = null)
            : base($"Bad download resume at offset {offset}", 36, command)
        {
            ResumeOffset = offset;
        }
    }

    /// <summary>
    /// CURLE_FILE_COULDNT_READ_FILE (37) - Couldn't read file
    /// </summary>
    [Serializable]
    public class CurlFileCouldntReadException : CurlException
    {
        /// <summary>
        /// Gets the file path that could not be read
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlFileCouldntReadException"/> class.
        /// </summary>
        /// <param name="filePath">The file path that could not be read.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlFileCouldntReadException(string filePath, string command = null)
            : base($"Could not read file: {filePath}", 37, command)
        {
            FilePath = filePath;
        }
    }

    /// <summary>
    /// CURLE_FUNCTION_NOT_FOUND (41) - Function not found
    /// </summary>
    [Serializable]
    public class CurlFunctionNotFoundException : CurlException
    {
        /// <summary>
        /// Gets the function name that was not found
        /// </summary>
        public string FunctionName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlFunctionNotFoundException"/> class.
        /// </summary>
        /// <param name="functionName">The function name that was not found.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlFunctionNotFoundException(string functionName, string command = null)
            : base($"Function not found: {functionName}", 41, command)
        {
            FunctionName = functionName;
        }
    }

    /// <summary>
    /// CURLE_ABORTED_BY_CALLBACK (42) - Aborted by callback
    /// </summary>
    [Serializable]
    public class CurlAbortedByCallbackException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlAbortedByCallbackException"/> class.
        /// </summary>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlAbortedByCallbackException(string command = null)
            : base("Operation aborted by callback", 42, command) { }
    }

    /// <summary>
    /// CURLE_BAD_FUNCTION_ARGUMENT (43) - Bad function argument
    /// </summary>
    [Serializable]
    public class CurlBadFunctionArgumentException : CurlException
    {
        /// <summary>
        /// Gets the name of the argument that was invalid
        /// </summary>
        public string ArgumentName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlBadFunctionArgumentException"/> class.
        /// </summary>
        /// <param name="argumentName">The name of the argument that was invalid.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlBadFunctionArgumentException(string argumentName, string command = null)
            : base($"Bad function argument: {argumentName}", 43, command)
        {
            ArgumentName = argumentName;
        }
    }

    /// <summary>
    /// CURLE_INTERFACE_FAILED (45) - Interface failed
    /// </summary>
    [Serializable]
    public class CurlInterfaceFailedException : CurlException
    {
        /// <summary>
        /// Gets the name of the network interface that failed
        /// </summary>
        public string InterfaceName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlInterfaceFailedException"/> class.
        /// </summary>
        /// <param name="interfaceName">The name of the network interface that failed.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlInterfaceFailedException(string interfaceName, string command = null)
            : base($"Interface failed: {interfaceName}", 45, command)
        {
            InterfaceName = interfaceName;
        }
    }

    /// <summary>
    /// CURLE_TOO_MANY_REDIRECTS (47) - Too many redirects
    /// </summary>
    [Serializable]
    public class CurlTooManyRedirectsException : CurlException
    {
        /// <summary>
        /// Gets the number of redirects that were attempted before failing
        /// </summary>
        public int RedirectCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlTooManyRedirectsException"/> class.
        /// </summary>
        /// <param name="count">The number of redirects that were attempted before failing.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlTooManyRedirectsException(int count, string command = null)
            : base($"Too many redirects: {count}", 47, command)
        {
            RedirectCount = count;
        }
    }

    /// <summary>
    /// CURLE_UNKNOWN_OPTION (48) - Unknown option
    /// </summary>
    [Serializable]
    public class CurlUnknownOptionException : CurlInvalidCommandException
    {
        /// <summary>
        /// Gets the name of the unknown option that was specified
        /// </summary>
        public string OptionName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlUnknownOptionException"/> class.
        /// </summary>
        /// <param name="optionName">The name of the unknown option that was specified.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlUnknownOptionException(string optionName, string command = null)
            : base($"Unknown option: {optionName}", optionName, command)
        {
            OptionName = optionName;
        }
    }

    /// <summary>
    /// CURLE_SETOPT_OPTION_SYNTAX (49) - Option syntax error
    /// </summary>
    [Serializable]
    public class CurlOptionSyntaxException : CurlInvalidCommandException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlOptionSyntaxException"/> class.
        /// </summary>
        /// <param name="option">The option that caused the syntax error.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlOptionSyntaxException(string option, string command = null)
            : base($"Option syntax error: {option}", option, command)
        {
        }
    }

    /// <summary>
    /// CURLE_GOT_NOTHING (52) - Got nothing (empty reply)
    /// </summary>
    [Serializable]
    public class CurlGotNothingException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlGotNothingException"/> class.
        /// </summary>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlGotNothingException(string command = null)
            : base("Got nothing (empty reply from server)", 52, command) { }
    }

    /// <summary>
    /// CURLE_SSL_ENGINE_NOTFOUND (53) - SSL engine not found
    /// </summary>
    [Serializable]
    public class CurlSslEngineNotFoundException : CurlSslException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlSslEngineNotFoundException"/> class.
        /// </summary>
        /// <param name="engine">The SSL engine that was not found.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlSslEngineNotFoundException(string engine, string command = null)
            : base($"SSL engine not found: {engine}", null, command)
        {
        }
    }

    /// <summary>
    /// CURLE_SSL_ENGINE_SETFAILED (54) - Failed setting SSL engine
    /// </summary>
    [Serializable]
    public class CurlSslEngineSetFailedException : CurlSslException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlSslEngineSetFailedException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlSslEngineSetFailedException(string message, string command = null)
            : base($"Failed to set SSL engine: {message}", null, command)
        {
        }
    }

    /// <summary>
    /// CURLE_SEND_ERROR (55) - Send error
    /// </summary>
    [Serializable]
    public class CurlSendErrorException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of CurlSendErrorException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="command">The curl command that failed.</param>
        public CurlSendErrorException(string message, string? command = null)
            : base($"Send error: {message}", 55, command) { }
    }

    /// <summary>
    /// CURLE_RECV_ERROR (56) - Receive error
    /// </summary>
    [Serializable]
    public class CurlReceiveErrorException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of CurlReceiveErrorException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="command">The curl command that failed.</param>
        public CurlReceiveErrorException(string message, string? command = null)
            : base($"Receive error: {message}", 56, command) { }
    }

    /// <summary>
    /// CURLE_SSL_CERTPROBLEM (58) - Problem with local certificate
    /// </summary>
    [Serializable]
    public class CurlSslCertificateProblemException : CurlSslException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlSslCertificateProblemException"/> class.
        /// </summary>
        /// <param name="certError">Details about the certificate error.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlSslCertificateProblemException(string certError, string command = null)
            : base($"Problem with local certificate: {certError}", certError, command)
        {
        }
    }

    /// <summary>
    /// CURLE_SSL_CIPHER (59) - Couldn't use SSL cipher
    /// </summary>
    [Serializable]
    public class CurlSslCipherException : CurlSslException
    {
        /// <summary>
        /// Gets the name of the SSL cipher that could not be used
        /// </summary>
        public string CipherName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlSslCipherException"/> class.
        /// </summary>
        /// <param name="cipher">The name of the SSL cipher that could not be used.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlSslCipherException(string cipher, string command = null)
            : base($"Could not use SSL cipher: {cipher}", null, command)
        {
            CipherName = cipher;
        }
    }

    /// <summary>
    /// CURLE_PEER_FAILED_VERIFICATION (60) - Peer certificate verification failed
    /// </summary>
    [Serializable]
    public class CurlPeerFailedVerificationException : CurlSslException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlPeerFailedVerificationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlPeerFailedVerificationException(string message, string command = null)
            : base($"Peer certificate verification failed: {message}", null, command)
        {
        }
    }

    /// <summary>
    /// CURLE_BAD_CONTENT_ENCODING (61) - Unrecognized content encoding
    /// </summary>
    [Serializable]
    public class CurlBadContentEncodingException : CurlException
    {
        /// <summary>
        /// Gets the content encoding that was not recognized or could not be decoded
        /// </summary>
        public string Encoding { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlBadContentEncodingException"/> class.
        /// </summary>
        /// <param name="encoding">The content encoding that was not recognized or could not be decoded.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlBadContentEncodingException(string encoding, string command = null)
            : base($"Bad content encoding: {encoding}", 61, command)
        {
            Encoding = encoding;
        }
    }

    /// <summary>
    /// CURLE_FILESIZE_EXCEEDED (63) - File size exceeded
    /// </summary>
    [Serializable]
    public class CurlFileSizeExceededException : CurlException
    {
        /// <summary>
        /// Gets the maximum allowed file size in bytes
        /// </summary>
        public long MaxSize { get; }
        /// <summary>
        /// Gets the actual file size in bytes that exceeded the limit
        /// </summary>
        public long ActualSize { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlFileSizeExceededException"/> class.
        /// </summary>
        /// <param name="maxSize">The maximum allowed file size in bytes.</param>
        /// <param name="actualSize">The actual file size in bytes that exceeded the limit.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlFileSizeExceededException(long maxSize, long actualSize, string command = null)
            : base($"File size {actualSize} exceeded maximum {maxSize}", 63, command)
        {
            MaxSize = maxSize;
            ActualSize = actualSize;
        }
    }

    /// <summary>
    /// CURLE_USE_SSL_FAILED (64) - Required SSL level failed
    /// </summary>
    [Serializable]
    public class CurlUseSslFailedException : CurlSslException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlUseSslFailedException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlUseSslFailedException(string message, string command = null)
            : base($"Required SSL level failed: {message}", null, command)
        {
        }
    }

    /// <summary>
    /// CURLE_LOGIN_DENIED (67) - Login denied
    /// </summary>
    [Serializable]
    public class CurlLoginDeniedException : CurlAuthenticationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlLoginDeniedException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="command">The curl command that caused the exception.</param>
        public CurlLoginDeniedException(string message, string command = null)
            : base($"Login denied: {message}", null, command)
        {
        }
    }
}