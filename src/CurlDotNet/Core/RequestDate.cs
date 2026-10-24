/***************************************************************************
 * RequestDate - curl_getdate() for -z / --time-cond
 *
 * A port of the behaviour of curl's lib/parsedate.c: dates are a free-form mix of week day,
 * month name, day, year, hh:mm[:ss] and a time zone, in any order, as in
 * "Sun, 06 Nov 1994 08:49:37 GMT", "Sunday, 06-Nov-94 08:49:37 GMT", "Sun Nov  6 08:49:37 1994",
 * "dec 12 12:00:00 1999 GMT", "06 Nov 1994 08:49 +0100" or "19941106".
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;

namespace CurlDotNet.Core
{
    /// <summary>Parses the date formats curl accepts.</summary>
    internal static class RequestDate
    {
        private static readonly string[] WeekDays = { "mon", "tue", "wed", "thu", "fri", "sat", "sun" };

        private static readonly string[] Months = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };

        /// <summary>Time zone names curl knows, with their offset from UTC in minutes.</summary>
        private static readonly Dictionary<string, int> TimeZones = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["GMT"] = 0, ["UT"] = 0, ["UTC"] = 0, ["WET"] = 0, ["BST"] = 60,
            ["WAT"] = -60, ["AST"] = -240, ["ADT"] = -180, ["EST"] = -300, ["EDT"] = -240,
            ["CST"] = -360, ["CDT"] = -300, ["MST"] = -420, ["MDT"] = -360, ["PST"] = -480,
            ["PDT"] = -420, ["YST"] = -540, ["YDT"] = -480, ["HST"] = -600, ["HDT"] = -540,
            ["CAT"] = -600, ["AHST"] = -600, ["NT"] = -660, ["IDLW"] = -720, ["CET"] = 60,
            ["MET"] = 60, ["MEWT"] = 60, ["MEST"] = 120, ["CEST"] = 120, ["MESZ"] = 120,
            ["FWT"] = 60, ["FST"] = 120, ["EET"] = 120, ["WAST"] = 420, ["WADT"] = 480,
            ["CCT"] = 480, ["JST"] = 540, ["EAST"] = 600, ["EADT"] = 660, ["GST"] = 600,
            ["NZT"] = 720, ["NZST"] = 720, ["NZDT"] = 780, ["IDLE"] = 720,
        };

        /// <summary>
        /// Parses <paramref name="text"/> like <c>curl_getdate()</c>. Returns false for anything that is not
        /// a date (curl then tries the text as a file name).
        /// </summary>
        internal static bool TryParse(string text, out DateTimeOffset date)
        {
            date = default;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            int? weekDay = null, month = null, day = null, year = null, hour = null, minute = null, second = null, offsetMinutes = null;
            var i = 0;
            while (i < text.Length)
            {
                var c = text[i];
                if (char.IsLetter(c))
                {
                    var start = i;
                    while (i < text.Length && char.IsLetter(text[i]))
                    {
                        i++;
                    }

                    var word = text.Substring(start, i - start).ToLowerInvariant();
                    var found = false;
                    if (weekDay == null && word.Length >= 3)
                    {
                        var index = Array.FindIndex(WeekDays, d => word.StartsWith(d, StringComparison.Ordinal) && (word.Length == 3 || IsFullDayName(word)));
                        if (index >= 0)
                        {
                            weekDay = index;
                            found = true;
                        }
                    }

                    if (!found && month == null && word.Length >= 3)
                    {
                        var index = Array.FindIndex(Months, m => word.StartsWith(m, StringComparison.Ordinal) && (word.Length == 3 || IsFullMonthName(word)));
                        if (index >= 0)
                        {
                            month = index + 1;
                            found = true;
                        }
                    }

                    if (!found && offsetMinutes == null && TimeZones.TryGetValue(word, out var zone))
                    {
                        offsetMinutes = zone;
                        found = true;
                    }

                    if (!found && offsetMinutes == null && word.Length == 1 && word[0] != 'j')
                    {
                        offsetMinutes = MilitaryZone(word[0]);
                        found = true;
                    }

                    if (!found)
                    {
                        return false;
                    }

                    continue;
                }

                if (char.IsDigit(c))
                {
                    var start = i;
                    while (i < text.Length && char.IsDigit(text[i]))
                    {
                        i++;
                    }

                    var digits = text.Substring(start, i - start);

                    // hh:mm or hh:mm:ss
                    if (hour == null && i < text.Length && text[i] == ':' && digits.Length <= 2)
                    {
                        var parts = ReadTime(text, start, out var end);
                        if (parts == null)
                        {
                            return false;
                        }

                        hour = parts[0];
                        minute = parts[1];
                        second = parts[2];
                        i = end;
                        continue;
                    }

                    // +hhmm / -hhmm time zone offset
                    if (offsetMinutes == null && start > 0 && (text[start - 1] == '+' || text[start - 1] == '-') && digits.Length == 4 &&
                        (day != null || year != null || hour != null))
                    {
                        var value = int.Parse(digits, CultureInfo.InvariantCulture);
                        var minutes = value / 100 * 60 + value % 100;
                        offsetMinutes = text[start - 1] == '+' ? minutes : -minutes;
                        continue;
                    }

                    // YYYYMMDD
                    if (digits.Length == 8 && year == null && month == null && day == null)
                    {
                        year = int.Parse(digits.Substring(0, 4), CultureInfo.InvariantCulture);
                        month = int.Parse(digits.Substring(4, 2), CultureInfo.InvariantCulture);
                        day = int.Parse(digits.Substring(6, 2), CultureInfo.InvariantCulture);
                        continue;
                    }

                    if (digits.Length > 9)
                    {
                        return false;
                    }

                    var number = int.Parse(digits, CultureInfo.InvariantCulture);
                    if (day == null && digits.Length <= 2 && number >= 1 && number <= 31)
                    {
                        day = number;
                    }
                    else if (year == null)
                    {
                        year = digits.Length <= 2 ? (number >= 70 ? 1900 + number : 2000 + number) : number;
                    }
                    else
                    {
                        return false;
                    }

                    continue;
                }

                i++; // separators: space, comma, dash, slash, dot
            }

            if (day == null || month == null || year == null || year < 1583)
            {
                return false;
            }

            try
            {
                var utc = new DateTime(year.Value, month.Value, day.Value, hour ?? 0, minute ?? 0, second ?? 0, DateTimeKind.Utc);
                date = new DateTimeOffset(utc, TimeSpan.Zero).AddMinutes(-(offsetMinutes ?? 0));
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        private static bool IsFullDayName(string word)
        {
            switch (word)
            {
                case "monday":
                case "tuesday":
                case "wednesday":
                case "thursday":
                case "friday":
                case "saturday":
                case "sunday":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsFullMonthName(string word)
        {
            foreach (var name in CultureInfo.InvariantCulture.DateTimeFormat.MonthNames)
            {
                if (string.Equals(name, word, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Military single-letter zones (A..Z without J); curl, like RFC 822, treats them as offsets.</summary>
        private static int MilitaryZone(char letter)
        {
            if (letter == 'z')
            {
                return 0;
            }

            if (letter >= 'a' && letter <= 'i')
            {
                return -(letter - 'a' + 1) * 60;
            }

            if (letter >= 'k' && letter <= 'm')
            {
                return -(letter - 'k' + 10) * 60;
            }

            if (letter >= 'n' && letter <= 'y')
            {
                return (letter - 'n' + 1) * 60;
            }

            return 0;
        }

        private static int[]? ReadTime(string text, int start, out int end)
        {
            var values = new List<int>();
            end = start;
            while (values.Count < 3)
            {
                var digitStart = end;
                while (end < text.Length && char.IsDigit(text[end]))
                {
                    end++;
                }

                if (end == digitStart || end - digitStart > 2)
                {
                    return null;
                }

                values.Add(int.Parse(text.Substring(digitStart, end - digitStart), CultureInfo.InvariantCulture));
                if (end < text.Length && text[end] == ':' && values.Count < 3)
                {
                    end++;
                    continue;
                }

                break;
            }

            if (values.Count < 2 || values[0] > 23 || values[1] > 59 || (values.Count == 3 && values[2] > 60))
            {
                return null;
            }

            return new[] { values[0], values[1], values.Count == 3 ? Math.Min(values[2], 59) : 0 };
        }
    }
}
