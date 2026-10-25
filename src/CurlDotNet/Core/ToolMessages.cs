/***************************************************************************
 * ToolMessages - the warnings and notes curl's tool prints on standard error
 *
 * Mirrors curl's src/tool_msgs.c: warnf() ("Warning: ", not under -s) and notef() ("Note: ", only
 * with -v/--trace/--trace-ascii and not -s), each message wrapped at the terminal width (COLUMNS).
 * The command parser records the messages it finds while reading the command line; the engine
 * writes them when the command runs and adds the ones that belong to a transfer. A library does not
 * print warnings to the console by itself: they go to a --stderr FILE only, and always onto
 * CurlResult.Warnings / CurlException.Warnings. Notes follow the note convention of the default
 * config file ("Read config file from"): --stderr FILE, or standard error.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CurlDotNet.Core
{
    /// <summary>The <c>Warning: </c>/<c>Note: </c> messages of one command line, in the order curl prints them.</summary>
    internal sealed class ToolMessages
    {
        /// <summary>curl's <c>WARN_PREFIX</c>.</summary>
        internal const string WarningPrefix = "Warning: ";

        /// <summary>curl's <c>NOTE_PREFIX</c>.</summary>
        internal const string NotePrefix = "Note: ";

        private readonly object _lock = new object();
        private readonly List<Entry> _entries = new List<Entry>();
        private int _written;

        private readonly struct Entry
        {
            public Entry(string prefix, string text)
            {
                Prefix = prefix;
                Text = text;
            }

            public string Prefix { get; }

            public string Text { get; }

            public bool IsNote => ReferenceEquals(Prefix, NotePrefix);
        }

        /// <summary>True when the whole command line has <c>-s</c> (curl's <c>global-&gt;silent</c> once parsed).</summary>
        public bool Silent { get; set; }

        /// <summary>True when the command asks for <c>-v</c>, <c>--trace</c> or <c>--trace-ascii</c> (curl's <c>global-&gt;tracetype</c>).</summary>
        public bool Tracing { get; set; }

        /// <summary>The <c>--stderr</c> file, or null for standard error (also for <c>--stderr -</c>).</summary>
        public string? StderrFile { get; set; }

        /// <summary>The messages so far, each with its prefix; a copy.</summary>
        public IReadOnlyList<string> Snapshot()
        {
            lock (_lock)
            {
                var list = new string[_entries.Count];
                for (var i = 0; i < list.Length; i++)
                {
                    list[i] = _entries[i].Prefix + _entries[i].Text;
                }

                return list;
            }
        }

        /// <summary>Records curl's <c>warnf()</c> unless <paramref name="silent"/> (the <c>-s</c> state at that point).</summary>
        public void AddWarning(string text, bool silent)
        {
            if (!silent)
            {
                Add(new Entry(WarningPrefix, text));
            }
        }

        /// <summary>Records curl's <c>notef()</c>: only when tracing and not silent.</summary>
        public void AddNote(string text, bool tracing, bool silent)
        {
            if (tracing && !silent)
            {
                Add(new Entry(NotePrefix, text));
            }
        }

        /// <summary>A warning found while a transfer is set up or run (the whole command is parsed): recorded and written.</summary>
        public void Warn(string text)
        {
            AddWarning(text, Silent);
            Flush();
        }

        /// <summary>A note found while a transfer is set up or run: recorded and written.</summary>
        public void Note(string text)
        {
            AddNote(text, Tracing, Silent);
            Flush();
        }

        /// <summary>
        /// Writes every message not written yet: to the <c>--stderr</c> file when there is one, otherwise notes to
        /// <see cref="Console.Error"/> and warnings nowhere (they stay on the result).
        /// </summary>
        public void Flush()
        {
            List<Entry> pending;
            lock (_lock)
            {
                if (_written >= _entries.Count)
                {
                    return;
                }

                pending = _entries.GetRange(_written, _entries.Count - _written);
                _written = _entries.Count;
            }

            foreach (var entry in pending)
            {
                var text = Format(entry.Prefix, entry.Text, TerminalColumns());
                if (StderrFile != null)
                {
                    try
                    {
                        File.AppendAllText(StderrFile, text);
                        continue;
                    }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException || ex is NotSupportedException)
                    {
                        // curl falls back to the terminal when --stderr cannot be opened
                    }
                }

                if (entry.IsNote)
                {
                    Console.Error.Write(text);
                }
            }
        }

        private void Add(Entry entry)
        {
            lock (_lock)
            {
                _entries.Add(entry);
            }
        }

        /// <summary>
        /// curl's <c>get_terminal_columns()</c> as far as a library can see it: <c>COLUMNS</c> when it is a number above 20,
        /// otherwise 79 (curl's value when it cannot ask the terminal).
        /// </summary>
        internal static int TerminalColumns()
        {
            var text = Environment.GetEnvironmentVariable("COLUMNS");
            if (!string.IsNullOrEmpty(text) && int.TryParse(text, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var columns)
                && columns > 20 && columns <= 10000)
            {
                return columns;
            }

            return 79;
        }

        /// <summary>
        /// curl's <c>voutf()</c>: the message in lines of at most <c>columns - prefix</c> bytes (UTF-8), each starting with
        /// <paramref name="prefix"/>; a line is cut after the last blank that fits (the blank stays at the end of the line),
        /// or at the width when there is none.
        /// </summary>
        internal static string Format(string prefix, string text, int columns)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var prefixBytes = Encoding.UTF8.GetBytes(prefix);
            var width = columns > prefixBytes.Length ? columns - prefixBytes.Length : int.MaxValue;
            var output = new MemoryStream();
            var pos = 0;
            var len = bytes.Length;
            while (len > 0)
            {
                output.Write(prefixBytes, 0, prefixBytes.Length);
                if (len > width)
                {
                    var cut = width - 1;
                    while (cut > 0 && bytes[pos + cut] != ' ' && bytes[pos + cut] != '\t')
                    {
                        cut--;
                    }

                    if (cut == 0)
                    {
                        cut = width - 1;
                    }

                    output.Write(bytes, pos, cut + 1);
                    output.WriteByte((byte)'\n');
                    pos += cut + 1;
                    len -= cut + 1;
                }
                else
                {
                    output.Write(bytes, pos, len);
                    output.WriteByte((byte)'\n');
                    len = 0;
                }
            }

            return Encoding.UTF8.GetString(output.ToArray());
        }
    }
}
