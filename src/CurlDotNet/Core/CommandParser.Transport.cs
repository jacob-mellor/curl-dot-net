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
        };

        /// <summary>
        /// Handles transport-side options. Returns true when <paramref name="option"/> was recognised.
        /// </summary>
        private bool ProcessTransportOption(string option, string value, CurlOptions options, ref bool methodSpecified)
        {
            return false;
        }
    }
}
