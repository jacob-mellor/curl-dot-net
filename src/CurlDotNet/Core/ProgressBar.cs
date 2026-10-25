/***************************************************************************
 * ProgressBar - curl's -#/--progress-bar, transpiled from src/tool_cb_prg.c
 *
 * The bar goes to curl's tool stderr (--stderr FILE, else standard error): "\r" followed by
 * width-7 columns of '#' (left aligned) and the percentage as %5.1f%%, redrawn at most ten
 * times a second and always at 100%; a transfer of unknown size draws the "flying" -=O=-
 * animation instead. A newline ends the bar once the transfer is over (tool_operate.c).
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace CurlDotNet.Core
{
    /// <summary>One transfer's <c>-#</c> progress bar.</summary>
    internal sealed class ProgressBar
    {
        private const int MaxBarLength = 400;
        private const int MinBarLength = 20;

        // tool_cb_prg.c's sine table for the flying animation (200 steps, 0..1000000)
        private static readonly int[] Sinus =
        {
            515704, 531394, 547052, 562664, 578214, 593687, 609068, 624341, 639491,
            654504, 669364, 684057, 698568, 712883, 726989, 740870, 754513, 767906,
            781034, 793885, 806445, 818704, 830647, 842265, 853545, 864476, 875047,
            885248, 895069, 904500, 913532, 922156, 930363, 938145, 945495, 952406,
            958870, 964881, 970434, 975522, 980141, 984286, 987954, 991139, 993840,
            996054, 997778, 999011, 999752, 999999, 999754, 999014, 997783, 996060,
            993848, 991148, 987964, 984298, 980154, 975536, 970449, 964898, 958888,
            952426, 945516, 938168, 930386, 922180, 913558, 904527, 895097, 885277,
            875077, 864507, 853577, 842299, 830682, 818739, 806482, 793922, 781072,
            767945, 754553, 740910, 727030, 712925, 698610, 684100, 669407, 654548,
            639536, 624386, 609113, 593733, 578260, 562710, 547098, 531440, 515751,
            500046, 484341, 468651, 452993, 437381, 421830, 406357, 390976, 375703,
            360552, 345539, 330679, 315985, 301474, 287158, 273052, 259170, 245525,
            232132, 219003, 206152, 193590, 181331, 169386, 157768, 146487, 135555,
            124983, 114781, 104959, 95526, 86493, 77868, 69660, 61876, 54525,
            47613, 41147, 35135, 29581, 24491, 19871, 15724, 12056, 8868,
            6166, 3951, 2225, 990, 248, 0, 244, 982, 2212,
            3933, 6144, 8842, 12025, 15690, 19832, 24448, 29534, 35084,
            41092, 47554, 54462, 61809, 69589, 77794, 86415, 95445, 104873,
            114692, 124891, 135460, 146389, 157667, 169282, 181224, 193480, 206039,
            218888, 232015, 245406, 259048, 272928, 287032, 301346, 315856, 330548,
            345407, 360419, 375568, 390841, 406221, 421693, 437243, 452854, 468513,
            484202, 499907,
        };

        private readonly CurlOptions _options;
        private readonly long _initialSize;
        private readonly Stopwatch _clock = Stopwatch.StartNew();
        private long _calls;
        private long _prev;
        private TimeSpan _prevTime;
        private int _width;
        private int _tick = 150;
        private int _bar;
        private int _barMove = 1;

        private ProgressBar(CurlOptions options)
        {
            _options = options;
            // the resume offset counts towards the whole file (progressbarinit)
            _initialSize = options.ResumeFrom ?? 0;
        }

        /// <summary>
        /// The bar for a transfer with <c>-#</c>, or null when none is drawn: no <c>-#</c>, or <c>-s</c>
        /// (config2setopts.c installs the bar callback only when not silent).
        /// </summary>
        internal static ProgressBar? For(CurlOptions options) =>
            options.ProgressBar && !options.Silent ? new ProgressBar(options) : null;

        /// <summary>
        /// curl's tool_progress_cb: called with the download and upload totals (0 when unknown) and the bytes moved
        /// so far; draws when the position changed and 100 ms passed, or the transfer reached 100%.
        /// </summary>
        internal void Update(long downloadTotal, long downloadNow, long uploadTotal, long uploadNow)
        {
            var now = _clock.Elapsed;
            var totalAll = long.MaxValue - downloadTotal < uploadTotal ? long.MaxValue : downloadTotal + uploadTotal;
            var nowAll = long.MaxValue - downloadNow < uploadNow ? long.MaxValue : downloadNow + uploadNow;

            if (_calls == 0)
            {
                UpdateWidth();
            }

            long total;
            long point;
            if (_initialSize < 0)
            {
                total = downloadTotal != 0 || uploadTotal != 0 ? totalAll : long.MaxValue;
                point = downloadTotal != 0 || uploadTotal != 0 ? nowAll : long.MaxValue;
            }
            else
            {
                total = long.MaxValue - _initialSize < totalAll ? long.MaxValue : totalAll + _initialSize;
                point = long.MaxValue - _initialSize < nowAll ? long.MaxValue : nowAll + _initialSize;
            }

            if (_calls > 0)
            {
                if (total != 0)
                {
                    if (_prev == point)
                    {
                        return; // no progress since the last call
                    }
                    if ((now - _prevTime).TotalMilliseconds < 100 && point < total)
                    {
                        return; // at most 10 updates a second, except at 100%
                    }
                }
                else
                {
                    if ((now - _prevTime).TotalMilliseconds < 100)
                    {
                        return;
                    }
                    UpdateWidth();
                    Fly(point != _prev);
                }
            }

            _calls++;
            UpdateWidth();
            if (total > 0 && point != _prev)
            {
                if (point > total)
                {
                    total = point; // more than announced
                }
                var frac = (double)point / total;
                var percent = frac * 100.0;
                var barWidth = _width - 7;
                var num = (int)Math.Min(MaxBarLength, barWidth * frac);
                var line = new string('#', Math.Max(0, num));
                Write("\r" + line.PadRight(barWidth) + " " +
                      percent.ToString("0.0", CultureInfo.InvariantCulture).PadLeft(5) + "%");
            }
            _prev = point;
            _prevTime = now;
        }

        /// <summary>Ends the bar with a newline when it was drawn at all (tool_operate.c post_per_transfer).</summary>
        internal void Finish()
        {
            if (_calls > 0)
            {
                Write("\n");
                _calls = 0;
            }
        }

        /// <summary>The "-=O=-" animation for a transfer of unknown size.</summary>
        private void Fly(bool moved)
        {
            var buf = new StringBuilder(_width + 1);
            buf.Append('\r').Append(' ', _width);
            var check = _width - 2;
            void Put(int index, string text)
            {
                for (var i = 0; i < text.Length && index + i < buf.Length; i++)
                {
                    buf[index + i] = text[i];
                }
            }
            Put(_bar + 1, "-=O=-");
            for (var k = 0; k <= 15; k += 5)
            {
                Put(Sinus[(_tick + k) % 200] / (1000000 / check) + 1, "#");
            }
            Write(buf.ToString());
            _tick += 2;
            if (_tick >= 200)
            {
                _tick -= 200;
            }
            _bar += moved ? _barMove : 0;
            if (_bar >= _width - 6)
            {
                _barMove = -1;
                _bar = _width - 6;
            }
            else if (_bar < 0)
            {
                _barMove = 1;
                _bar = 0;
            }
        }

        private void UpdateWidth()
        {
            var cols = TerminalColumns();
            _width = cols > MaxBarLength ? MaxBarLength : cols > MinBarLength ? cols : MinBarLength;
        }

        /// <summary>
        /// terminal.c's get_terminal_columns: <c>COLUMNS</c> when above 20, else the console width when one is
        /// attached, else 79.
        /// </summary>
        private static int TerminalColumns()
        {
            var env = Environment.GetEnvironmentVariable("COLUMNS");
            if (env != null && int.TryParse(env.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var fromEnv) && fromEnv > 20 && fromEnv <= 10000)
            {
                return fromEnv;
            }
            try
            {
                if (!(Console.IsInputRedirected && Console.IsOutputRedirected && Console.IsErrorRedirected))
                {
                    var cols = Console.WindowWidth;
                    if (cols > 0 && cols < 10000)
                    {
                        return cols;
                    }
                }
            }
            catch (Exception ex) when (ex is System.IO.IOException || ex is PlatformNotSupportedException || ex is InvalidOperationException)
            {
                // no console attached
            }
            return 79;
        }

        private void Write(string text) => TransferOutput.WriteStderr(_options, text);
    }
}
