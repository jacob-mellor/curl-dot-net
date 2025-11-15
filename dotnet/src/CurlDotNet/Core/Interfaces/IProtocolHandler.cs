/***************************************************************************
 * IProtocolHandler - Interface for protocol-specific handlers
 *
 * Based on curl's protocol handler architecture by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System.Threading;
using System.Threading.Tasks;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Interface for protocol-specific handlers (HTTP, FTP, FILE, etc.).
    /// </summary>
    internal interface IProtocolHandler
    {
        /// <summary>
        /// Execute a request with the given options.
        /// </summary>
        /// <param name="options">Parsed curl options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        Task<CurlResult> ExecuteAsync(CurlOptions options, CancellationToken cancellationToken);

        /// <summary>
        /// Check if this handler supports the given protocol.
        /// </summary>
        /// <param name="protocol">Protocol scheme (http, ftp, file, etc.)</param>
        /// <returns>True if supported</returns>
        bool SupportsProtocol(string protocol);
    }
}