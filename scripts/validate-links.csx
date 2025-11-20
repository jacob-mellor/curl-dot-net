#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.Console;

// ANSI color codes for output
const string Reset = "\u001b[0m";
const string Red = "\u001b[31m";
const string Green = "\u001b[32m";
const string Yellow = "\u001b[33m";
const string Blue = "\u001b[34m";
const string Cyan = "\u001b[36m";

class LinkValidator
{
    private readonly List<string> brokenLinks = new List<string>();
    private readonly List<string> warnings = new List<string>();
    private int totalLinks = 0;
    private int validLinks = 0;

    public async Task<int> ValidateAllLinks(string rootPath)
    {
        AnsiConsole.Write(new FigletText("Link Validator").Color(Color.Cyan1));
        AnsiConsole.WriteLine();

        var markdownFiles = Directory.GetFiles(rootPath, "*.md", SearchOption.AllDirectories)
            .Where(f => !f.Contains("node_modules") && !f.Contains("_site") && !f.Contains(".git"))
            .ToList();

        AnsiConsole.MarkupLine($"[cyan]Found {markdownFiles.Count} markdown files to check[/]");
        AnsiConsole.WriteLine();

        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("[green]Validating links...[/]", maxValue: markdownFiles.Count);

                foreach (var file in markdownFiles)
                {
                    ValidateFileLinks(file, rootPath);
                    task.Increment(1);
                }
            });

        // Print results
        PrintResults();

        return brokenLinks.Count > 0 ? 1 : 0;
    }

    private void ValidateFileLinks(string filePath, string rootPath)
    {
        var content = File.ReadAllText(filePath);
        var relativePath = Path.GetRelativePath(rootPath, filePath);
        var fileDir = Path.GetDirectoryName(filePath);

        // Find all markdown links: [text](url)
        var linkPattern = @"\[([^\]]*)\]\(([^)]+)\)";
        var matches = Regex.Matches(content, linkPattern);

        foreach (Match match in matches)
        {
            var linkText = match.Groups[1].Value;
            var linkUrl = match.Groups[2].Value;
            totalLinks++;

            // Skip external links and anchors
            if (linkUrl.StartsWith("http://") || linkUrl.StartsWith("https://") || linkUrl.StartsWith("#"))
            {
                validLinks++;
                continue;
            }

            // Remove anchor from link for file checking
            var cleanUrl = linkUrl.Split('#')[0];
            if (string.IsNullOrEmpty(cleanUrl))
            {
                validLinks++;
                continue;
            }

            // Resolve relative paths
            string targetPath;
            if (cleanUrl.StartsWith("/"))
            {
                // Absolute path from root
                targetPath = Path.Combine(rootPath, cleanUrl.TrimStart('/'));
            }
            else
            {
                // Relative path from current file
                targetPath = Path.GetFullPath(Path.Combine(fileDir, cleanUrl));
            }

            // Check if file exists
            bool exists = false;

            // Check exact path
            if (File.Exists(targetPath))
            {
                exists = true;
            }
            // Check if it's a directory with index.md or README.md
            else if (Directory.Exists(targetPath))
            {
                var indexPath = Path.Combine(targetPath, "index.md");
                var readmePath = Path.Combine(targetPath, "README.md");

                if (File.Exists(indexPath) || File.Exists(readmePath))
                {
                    exists = true;
                }
                else
                {
                    warnings.Add($"Directory link without index: {relativePath} -> {linkUrl}");
                }
            }
            // Check with .md extension if not specified
            else if (!cleanUrl.EndsWith(".md") && !cleanUrl.EndsWith(".html"))
            {
                var mdPath = targetPath + ".md";
                if (File.Exists(mdPath))
                {
                    exists = true;
                    warnings.Add($"Missing .md extension: {relativePath} -> {linkUrl}");
                }
            }

            if (exists)
            {
                validLinks++;
            }
            else
            {
                brokenLinks.Add($"{relativePath}:{GetLineNumber(content, match.Index)} -> [{linkText}]({linkUrl})");
            }
        }
    }

    private int GetLineNumber(string content, int charIndex)
    {
        var lineNumber = 1;
        for (int i = 0; i < charIndex && i < content.Length; i++)
        {
            if (content[i] == '\n')
                lineNumber++;
        }
        return lineNumber;
    }

    private void PrintResults()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Rule("[yellow]Validation Results[/]");

        // Summary
        var table = new Table();
        table.AddColumn("Metric");
        table.AddColumn("Count");

        table.AddRow("Total Links", totalLinks.ToString());
        table.AddRow("[green]Valid Links[/]", validLinks.ToString());
        table.AddRow("[yellow]Warnings[/]", warnings.Count.ToString());
        table.AddRow("[red]Broken Links[/]", brokenLinks.Count.ToString());

        AnsiConsole.Write(table);

        // Show warnings
        if (warnings.Count > 0)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]⚠ Warnings:[/]");
            foreach (var warning in warnings)
            {
                AnsiConsole.MarkupLine($"  [yellow]• {warning}[/]");
            }
        }

        // Show broken links
        if (brokenLinks.Count > 0)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[red]✗ Broken Links Found:[/]");
            foreach (var link in brokenLinks)
            {
                AnsiConsole.MarkupLine($"  [red]• {link}[/]");
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[red bold]VALIDATION FAILED[/] - Please fix the broken links above!");
        }
        else
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green bold]✓ ALL LINKS VALID[/] - No broken links found!");
        }
    }
}

// Main execution
var validator = new LinkValidator();
var rootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "docs"));

if (!Directory.Exists(rootPath))
{
    AnsiConsole.MarkupLine($"[red]Error: Documentation directory not found at {rootPath}[/]");
    AnsiConsole.MarkupLine("[yellow]Please run this script from the repository root.[/]");
    return 1;
}

AnsiConsole.MarkupLine($"[cyan]Validating links in: {rootPath}[/]");
var exitCode = await validator.ValidateAllLinks(rootPath);

// Also check main README
var mainReadme = Path.Combine(Directory.GetCurrentDirectory(), "README.md");
if (File.Exists(mainReadme))
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[cyan]Also checking main README.md...[/]");
    var readmeValidator = new LinkValidator();
    var readmeExitCode = await readmeValidator.ValidateAllLinks(Path.GetDirectoryName(mainReadme));
    exitCode = Math.Max(exitCode, readmeExitCode);
}

// Check NuGet README
var nugetReadme = Path.Combine(Directory.GetCurrentDirectory(), "src/CurlDotNet/README.md");
if (File.Exists(nugetReadme))
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[cyan]Also checking NuGet README.md...[/]");

    // Special validation for NuGet README - only check internal links
    var content = File.ReadAllText(nugetReadme);
    var linkPattern = @"\[([^\]]*)\]\(([^)]+)\)";
    var matches = Regex.Matches(content, linkPattern);
    var brokenNugetLinks = new List<string>();

    foreach (Match match in matches)
    {
        var linkUrl = match.Groups[2].Value;

        // For NuGet README, we're primarily concerned with documentation site links
        if (linkUrl.StartsWith("https://jacob-mellor.github.io/curl-dot-net/"))
        {
            var path = linkUrl.Replace("https://jacob-mellor.github.io/curl-dot-net/", "");

            // Check known valid paths (these will be generated when docs are published)
            var validPaths = new[] {
                "new-to-curl",
                "api/",
                "cookbook/",
                "tutorials/",
                "migration/",
                "reference/curl-cli-compatibility"
            };

            if (!validPaths.Any(p => path.StartsWith(p) || path == p))
            {
                brokenNugetLinks.Add($"Line {GetLineNumber(content, match.Index)}: {linkUrl}");
            }
        }
    }

    if (brokenNugetLinks.Count > 0)
    {
        AnsiConsole.MarkupLine("[red]✗ Potentially broken documentation links in NuGet README:[/]");
        foreach (var link in brokenNugetLinks)
        {
            AnsiConsole.MarkupLine($"  [red]• {link}[/]");
        }
        exitCode = 1;
    }
    else
    {
        AnsiConsole.MarkupLine("[green]✓ NuGet README links appear valid[/]");
    }
}

int GetLineNumber(string content, int charIndex)
{
    var lineNumber = 1;
    for (int i = 0; i < charIndex && i < content.Length; i++)
    {
        if (content[i] == '\n')
            lineNumber++;
    }
    return lineNumber;
}

AnsiConsole.WriteLine();
if (exitCode == 0)
{
    AnsiConsole.MarkupLine("[green bold]🎉 SUCCESS - All documentation links are valid![/]");
}
else
{
    AnsiConsole.MarkupLine("[red bold]❌ FAILURE - Some links are broken. Please fix them before committing.[/]");
}

return exitCode;