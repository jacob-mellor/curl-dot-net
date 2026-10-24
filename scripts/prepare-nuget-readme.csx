#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"

using System;
using System.IO;
using System.Text.RegularExpressions;
using Spectre.Console;

// Validates nuget-readme.md, the short README packed into the NuGet package.
//
// nuget.org renders a sanitised Markdown subset: raw HTML is stripped and relative links or
// images do not resolve. nuget-readme.md is therefore hand-maintained (it is NOT a copy of
// README.md) and this script only rewrites any relative link it finds to an absolute GitHub
// URL and fails if HTML or a relative image slipped in.

AnsiConsole.Write(new FigletText("NuGet README").Color(Color.Magenta1));

const string REPO_BLOB = "https://github.com/jacob-mellor/curl-dot-net/blob/master/";
const string NUGET_README = "nuget-readme.md";

if (!File.Exists(NUGET_README))
{
    AnsiConsole.MarkupLine($"[red]❌ {NUGET_README} not found. It is packed as README.md by src/CurlDotNet/CurlDotNet.csproj.[/]");
    Environment.Exit(1);
}

var content = File.ReadAllText(NUGET_README);
var converted = 0;

// [text](relative/path) -> absolute GitHub URL (anchors and absolute URLs are left alone)
content = Regex.Replace(content, @"\[([^\]]+)\]\((?!https?://|#|mailto:)([^)\s]+)\)", m =>
{
    converted++;
    return $"[{m.Groups[1].Value}]({REPO_BLOB}{m.Groups[2].Value.TrimStart('.', '/')})";
});

var problems = 0;
if (Regex.IsMatch(content, @"<(div|img|br|p|span|table|a)\b", RegexOptions.IgnoreCase))
{
    AnsiConsole.MarkupLine("[red]❌ Raw HTML found: nuget.org strips it. Use Markdown only.[/]");
    problems++;
}
if (Regex.IsMatch(content, @"!\[[^\]]*\]\((?!https?://)"))
{
    AnsiConsole.MarkupLine("[red]❌ Relative image found: images must use absolute https:// URLs on nuget.org.[/]");
    problems++;
}
var lines = content.Split('\n').Length;
if (lines > 200)
{
    AnsiConsole.MarkupLine($"[yellow]⚠️  {lines} lines: keep the NuGet README short and link to the docs for detail.[/]");
}

File.WriteAllText(NUGET_README, content);

if (problems > 0)
{
    Environment.Exit(1);
}

AnsiConsole.MarkupLine($"[green]✅ {NUGET_README} is ready ({lines} lines, {converted} relative link(s) made absolute).[/]");
