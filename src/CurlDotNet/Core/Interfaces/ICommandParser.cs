/***************************************************************************
 * ICommandParser - Interface for parsing curl commands
 *
 * Based on curl's src/tool_getparam.c by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

namespace CurlDotNet.Core
{
    /// <summary>
    /// Interface for parsing curl command strings into options.
    /// </summary>
    internal interface ICommandParser
    {
        /// <summary>
        /// Parse a curl command string into options.
        /// </summary>
        /// <param name="command">The curl command string</param>
        /// <returns>Parsed options</returns>
        CurlOptions Parse(string command);

        /// <summary>
        /// Parse a command written for a specific shell flavor (see <see cref="CurlFlavor"/>).
        /// </summary>
        /// <param name="command">The command string</param>
        /// <param name="flavor">The shell the text was written for; <see cref="CurlFlavor.Auto"/> detects it</param>
        /// <returns>Parsed options, with <see cref="CurlOptions.Flavor"/> set to the flavor used</returns>
        CurlOptions Parse(string command, CurlFlavor flavor);

        /// <summary>
        /// Validate a curl command without fully parsing.
        /// </summary>
        /// <param name="command">The curl command string</param>
        /// <returns>True if command syntax is valid</returns>
        bool IsValid(string command);
    }
}