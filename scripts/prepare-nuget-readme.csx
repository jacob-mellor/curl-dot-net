#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.Console;

// Script to prepare README for NuGet package with absolute links
// .NET-friendly NuGet README converter for CurlDotNet

AnsiConsole.Write(new FigletText("NuGet README")
    .Color(Color.Magenta1));

AnsiConsole.MarkupLine("[magenta]📄 Preparing README for NuGet package...[/]");
AnsiConsole.Write(new Rule("[blue]README Conversion[/]").LeftJustified());

const string REPO_BASE = "https://github.com/jacob-mellor/curl-dot-net";
const string DOCS_BASE = "https://jacob-mellor.github.io/curl-dot-net";
const string SOURCE_README = "README.md";
const string NUGET_README = "nuget-readme.md";

if (!File.Exists(SOURCE_README))
{
    AnsiConsole.MarkupLine($"[red]❌ Source README not found: {SOURCE_README}[/]");
    Environment.Exit(1);
}

// Copy the main README
AnsiConsole.MarkupLine("[yellow]→ Copying source README...[/]");
File.Copy(SOURCE_README, NUGET_README, overwrite: true);

// Read the content
var content = await File.ReadAllTextAsync(NUGET_README);
var originalLength = content.Length;

AnsiConsole.MarkupLine("[yellow]🔗 Converting relative links to absolute...[/]");

// Convert relative GitHub links to absolute
// [text](relative/path.md) -> [text](https://github.com/jacob-mellor/curl-dot-net/blob/master/relative/path.md)
var mdLinkPattern = @"\[([^\]]+)\]\(([^)]+\.md)\)";
content = Regex.Replace(content, mdLinkPattern, m =>
{
    var text = m.Groups[1].Value;
    var path = m.Groups[2].Value;

    // Skip if already absolute
    if (path.StartsWith("http://") || path.StartsWith("https://"))
        return m.Value;

    return $"[{text}]({REPO_BASE}/blob/master/{path})";
});

// Convert documentation links to absolute
var docPatterns = new[]
{
    ("tutorials", $"{DOCS_BASE}/tutorials/"),
    ("cookbook", $"{DOCS_BASE}/cookbook/"),
    ("api-guide", $"{DOCS_BASE}/api-guide/"),
    ("guides", $"{DOCS_BASE}/guides/"),
    ("troubleshooting", $"{DOCS_BASE}/troubleshooting/"),
    ("getting-started", $"{DOCS_BASE}/getting-started/"),
    ("api", $"{DOCS_BASE}/api/"),
    ("reference", $"{DOCS_BASE}/reference/"),
    ("exceptions", $"{DOCS_BASE}/exceptions/")
};

foreach (var (folder, baseUrl) in docPatterns)
{
    var pattern = $@"\[([^\]]+)\]\({folder}/([^)]+)\)";
    content = Regex.Replace(content, pattern, m =>
    {
        var text = m.Groups[1].Value;
        var path = m.Groups[2].Value;
        return $"[{text}]({baseUrl}{path})";
    });
}

// Write the updated content
await File.WriteAllTextAsync(NUGET_README, content);

// Count the changes
var linkPattern = @"\[([^\]]+)\]\(([^)]+)\)";
var originalLinks = Regex.Matches(File.ReadAllText(SOURCE_README), linkPattern).Count;
var convertedLinks = Regex.Matches(content, linkPattern)
    .Count(m => m.Groups[2].Value.StartsWith("http"));

AnsiConsole.MarkupLine($"[green]✅ Converted {convertedLinks} links to absolute URLs[/]");

// Copy to the src/CurlDotNet directory for packaging
var targetPath = Path.Combine("src", "CurlDotNet", "README.md");
if (File.Exists(targetPath))
{
    AnsiConsole.MarkupLine("[yellow]→ Copying to package directory...[/]");
    File.Copy(NUGET_README, targetPath, overwrite: true);
    AnsiConsole.MarkupLine($"[green]📦 README copied to {targetPath} for NuGet packaging[/]");
}

// Summary
AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule().DoubleBorder());

var panel = new Panel(new Markup("[green]✅ NuGet README prepared successfully![/]"))
    .Header("[green]Complete[/]")
    .Border(BoxBorder.Rounded)
    .BorderColor(Color.Green);

AnsiConsole.Write(panel);

var table = new Table();
table.AddColumn("File");
table.AddColumn("Size");
table.Border(TableBorder.Rounded);

var sourceInfo = new FileInfo(SOURCE_README);
var nugetInfo = new FileInfo(NUGET_README);

table.AddRow("Source README", FormatBytes(sourceInfo.Length));
table.AddRow("NuGet README", FormatBytes(nugetInfo.Length));
table.AddRow("Links Converted", convertedLinks.ToString());

AnsiConsole.Write(table);

AnsiConsole.MarkupLine("\n[dim]The NuGet README is ready with all absolute links[/]");

// Helper function
string FormatBytes(long bytes)
{
    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
    int order = 0;
    double size = bytes;

    while (size >= 1024 && order < sizes.Length - 1)
    {
        order++;
        size = size / 1024;
    }

    return $"{size:0.##} {sizes[order]}";
}