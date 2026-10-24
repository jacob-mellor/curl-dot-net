/***************************************************************************
 * TransferSession - what the transfers of one command line share
 *
 * Mirrors curl's src/tool_operate.c: every URL, glob match and --next group is its own transfer,
 * run one after the other; they share one cookie store (curl's CURLSH share handle), their
 * standard output is concatenated, and the exit code is the last failure.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>State shared by every transfer of one command line.</summary>
    internal sealed class TransferSession
    {
        /// <summary>The cookie store, created by the first transfer that enables the cookie engine.</summary>
        public CookieEngine? Cookies { get; set; }
    }

    /// <summary>Reads and writes the standard-output text that travels on a failed transfer's exception.</summary>
    internal static class TransferStdout
    {
        /// <summary>The standard output stored on <paramref name="ex"/>, or an empty string.</summary>
        public static string Get(Exception ex)
        {
            return ex.Data[CurlException.StandardOutputDataKey] as string ?? string.Empty;
        }

        /// <summary>Replaces the standard output stored on <paramref name="ex"/> (nothing is stored for an empty text).</summary>
        public static void Set(Exception ex, string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                ex.Data.Remove(CurlException.StandardOutputDataKey);
                return;
            }
            ex.Data[CurlException.StandardOutputDataKey] = text;
        }

        /// <summary>Adds <paramref name="text"/> in front of what <paramref name="ex"/> already holds.</summary>
        public static void Prepend(Exception ex, string? text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Set(ex, text + Get(ex));
            }
        }

        /// <summary>Adds <paramref name="text"/> after what <paramref name="ex"/> already holds.</summary>
        public static void Append(Exception ex, string? text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Set(ex, Get(ex) + text);
            }
        }
    }
}
