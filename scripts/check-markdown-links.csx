#!/usr/bin/env dotnet-script

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Markdown Link Checker for CurlDotNet Documentation
/// Recursively scans directories for markdown files and checks all links
/// </summary>

// Configuration
var rootPath = Args.FirstOrDefault() ?? ".";
var verbose = Args.Contains("--verbose") || Args.Contains("-v");
var fix = Args.Contains("--fix");
var reportFile = Args.FirstOrDefault(a => a.StartsWith("--report="))?.Substring(9) ?? "link-check-report.md";

// Statistics
var totalFiles = 0;
var totalLinks = 0;
var brokenLinks = new List<BrokenLink>();
var workingLinks = 0;

// Regex patterns for markdown links
var markdownLinkPattern = @"\[([^\]]+)\]\(([^\)]+)\)";
var referenceLinkPattern = @"\[([^\]]+)\]\[([^\]]*)\]";
var referenceDefPattern = @"^\[([^\]]+)\]:\s*(.+)$";

// Colors for console output
void WriteSuccess(string message) => Console.WriteLine($"\u001b[32m✓ {message}\u001b[0m");
void WriteError(string message) => Console.WriteLine($"\u001b[31m✗ {message}\u001b[0m");
void WriteWarning(string message) => Console.WriteLine($"\u001b[33m⚠ {message}\u001b[0m");
void WriteInfo(string message) => Console.WriteLine($"\u001b[34mℹ {message}\u001b[0m");

// Data structures
public class BrokenLink
{
    public string SourceFile { get; set; }
    public int LineNumber { get; set; }
    public string LinkText { get; set; }
    public string Target { get; set; }
    public string Issue { get; set; }
    public string SuggestedFix { get; set; }
}

// Helper functions
string NormalizePath(string path)
{
    return path.Replace('\\', '/').TrimEnd('/');
}

string ResolvePath(string basePath, string relativePath)
{
    if (relativePath.StartsWith("http://") || relativePath.StartsWith("https://"))
        return null; // External link, skip

    if (relativePath.StartsWith("#"))
        return null; // Anchor link, skip for now

    if (relativePath.StartsWith("/"))
    {
        // Absolute path from root
        return Path.Combine(rootPath, relativePath.TrimStart('/'));
    }

    // Remove any fragment identifier
    var fragmentIndex = relativePath.IndexOf('#');
    if (fragmentIndex > 0)
        relativePath = relativePath.Substring(0, fragmentIndex);

    // Resolve relative path
    var dir = Path.GetDirectoryName(basePath);
    return Path.GetFullPath(Path.Combine(dir, relativePath));
}

bool FileOrDirectoryExists(string path)
{
    if (string.IsNullOrEmpty(path))
        return true; // Skip external links and anchors

    // Check if it's a file
    if (File.Exists(path))
        return true;

    // Check if it's a directory
    if (Directory.Exists(path))
    {
        // Check for index files
        var indexFiles = new[] { "index.md", "README.md", "index.html" };
        foreach (var indexFile in indexFiles)
        {
            if (File.Exists(Path.Combine(path, indexFile)))
                return true;
        }
    }

    // Check with .md extension if not specified
    if (!path.EndsWith(".md") && File.Exists(path + ".md"))
        return true;

    // Check with .html extension (for Jekyll generated files)
    if (path.EndsWith(".html"))
    {
        var mdPath = path.Replace(".html", ".md");
        if (File.Exists(mdPath))
            return true;
    }

    return false;
}

string SuggestFix(string path)
{
    if (string.IsNullOrEmpty(path))
        return null;

    // Try to find similar files
    var dir = Path.GetDirectoryName(path);
    var filename = Path.GetFileName(path);

    if (!Directory.Exists(dir))
    {
        // Suggest creating directory structure
        return $"Create directory: {dir}";
    }

    // Look for similar files in the directory
    var files = Directory.GetFiles(dir, "*.md");
    var similarFiles = files.Where(f =>
        Path.GetFileNameWithoutExtension(f).Contains(Path.GetFileNameWithoutExtension(filename)) ||
        Path.GetFileNameWithoutExtension(filename).Contains(Path.GetFileNameWithoutExtension(f))
    ).ToList();

    if (similarFiles.Any())
    {
        return $"Did you mean: {Path.GetFileName(similarFiles.First())}?";
    }

    return $"Create file: {path}";
}

void CheckFile(string filePath)
{
    totalFiles++;
    if (verbose) WriteInfo($"Checking: {filePath}");

    var lines = File.ReadAllLines(filePath);
    var referenceDefinitions = new Dictionary<string, string>();

    // First pass: collect reference definitions
    for (int i = 0; i < lines.Length; i++)
    {
        var refDefMatch = Regex.Match(lines[i], referenceDefPattern, RegexOptions.Multiline);
        if (refDefMatch.Success)
        {
            referenceDefinitions[refDefMatch.Groups[1].Value.ToLower()] = refDefMatch.Groups[2].Value;
        }
    }

    // Second pass: check all links
    for (int i = 0; i < lines.Length; i++)
    {
        var line = lines[i];
        var lineNumber = i + 1;

        // Check inline links [text](url)
        var matches = Regex.Matches(line, markdownLinkPattern);
        foreach (Match match in matches)
        {
            totalLinks++;
            var linkText = match.Groups[1].Value;
            var target = match.Groups[2].Value;

            var resolvedPath = ResolvePath(filePath, target);
            if (resolvedPath != null && !FileOrDirectoryExists(resolvedPath))
            {
                brokenLinks.Add(new BrokenLink
                {
                    SourceFile = filePath,
                    LineNumber = lineNumber,
                    LinkText = linkText,
                    Target = target,
                    Issue = $"Target does not exist: {resolvedPath}",
                    SuggestedFix = SuggestFix(resolvedPath)
                });
            }
            else
            {
                workingLinks++;
            }
        }

        // Check reference-style links [text][ref]
        var refMatches = Regex.Matches(line, referenceLinkPattern);
        foreach (Match match in refMatches)
        {
            totalLinks++;
            var linkText = match.Groups[1].Value;
            var reference = match.Groups[2].Value;

            // If reference is empty, use linkText as reference
            if (string.IsNullOrEmpty(reference))
                reference = linkText;

            reference = reference.ToLower();

            if (referenceDefinitions.ContainsKey(reference))
            {
                var target = referenceDefinitions[reference];
                var resolvedPath = ResolvePath(filePath, target);

                if (resolvedPath != null && !FileOrDirectoryExists(resolvedPath))
                {
                    brokenLinks.Add(new BrokenLink
                    {
                        SourceFile = filePath,
                        LineNumber = lineNumber,
                        LinkText = linkText,
                        Target = target,
                        Issue = $"Reference target does not exist: {resolvedPath}",
                        SuggestedFix = SuggestFix(resolvedPath)
                    });
                }
                else
                {
                    workingLinks++;
                }
            }
            else
            {
                brokenLinks.Add(new BrokenLink
                {
                    SourceFile = filePath,
                    LineNumber = lineNumber,
                    LinkText = linkText,
                    Target = $"[{reference}]",
                    Issue = $"Reference not defined: {reference}",
                    SuggestedFix = $"Add reference definition: [{reference}]: <url>"
                });
            }
        }
    }
}

void ScanDirectory(string directory)
{
    // Skip certain directories
    var skipDirs = new[] { ".git", "node_modules", "_site", "bin", "obj", ".vs" };
    var dirName = Path.GetFileName(directory);
    if (skipDirs.Contains(dirName))
    {
        if (verbose) WriteWarning($"Skipping directory: {directory}");
        return;
    }

    // Process all markdown files in this directory
    var mdFiles = Directory.GetFiles(directory, "*.md");
    foreach (var file in mdFiles)
    {
        CheckFile(file);
    }

    // Recursively process subdirectories
    var subdirs = Directory.GetDirectories(directory);
    foreach (var subdir in subdirs)
    {
        ScanDirectory(subdir);
    }
}

void GenerateReport()
{
    var report = new System.Text.StringBuilder();

    report.AppendLine("# Markdown Link Check Report");
    report.AppendLine($"\n**Generated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    report.AppendLine($"**Root Path:** {Path.GetFullPath(rootPath)}");
    report.AppendLine();

    report.AppendLine("## Summary");
    report.AppendLine($"- **Total Files Scanned:** {totalFiles}");
    report.AppendLine($"- **Total Links Found:** {totalLinks}");
    report.AppendLine($"- **Working Links:** {workingLinks}");
    report.AppendLine($"- **Broken Links:** {brokenLinks.Count}");

    var successRate = totalLinks > 0 ? (workingLinks * 100.0 / totalLinks) : 100;
    report.AppendLine($"- **Success Rate:** {successRate:F1}%");
    report.AppendLine();

    if (brokenLinks.Any())
    {
        report.AppendLine("## Broken Links by File");

        var groupedByFile = brokenLinks.GroupBy(l => l.SourceFile).OrderBy(g => g.Key);
        foreach (var fileGroup in groupedByFile)
        {
            var relativePath = Path.GetRelativePath(rootPath, fileGroup.Key);
            report.AppendLine($"\n### {relativePath}");
            report.AppendLine($"**{fileGroup.Count()} broken link(s)**\n");

            foreach (var link in fileGroup.OrderBy(l => l.LineNumber))
            {
                report.AppendLine($"- **Line {link.LineNumber}:** `[{link.LinkText}]({link.Target})`");
                report.AppendLine($"  - Issue: {link.Issue}");
                if (!string.IsNullOrEmpty(link.SuggestedFix))
                {
                    report.AppendLine($"  - Suggestion: {link.SuggestedFix}");
                }
            }
        }

        report.AppendLine("\n## Broken Links by Target");
        var groupedByTarget = brokenLinks.GroupBy(l => l.Target).OrderByDescending(g => g.Count());
        report.AppendLine("\n| Target | Count | Source Files |");
        report.AppendLine("|--------|-------|--------------|");

        foreach (var targetGroup in groupedByTarget)
        {
            var sources = string.Join(", ", targetGroup.Select(l =>
                Path.GetFileName(l.SourceFile)).Distinct());
            report.AppendLine($"| `{targetGroup.Key}` | {targetGroup.Count()} | {sources} |");
        }

        if (fix)
        {
            report.AppendLine("\n## Auto-Fix Actions");
            report.AppendLine("\nThe following actions would be taken with --fix:");

            var missingDirs = brokenLinks
                .Select(l => l.SuggestedFix)
                .Where(s => s != null && s.StartsWith("Create directory:"))
                .Distinct();

            var missingFiles = brokenLinks
                .Select(l => l.SuggestedFix)
                .Where(s => s != null && s.StartsWith("Create file:"))
                .Distinct();

            if (missingDirs.Any())
            {
                report.AppendLine("\n### Directories to Create:");
                foreach (var dir in missingDirs)
                {
                    report.AppendLine($"- {dir}");
                }
            }

            if (missingFiles.Any())
            {
                report.AppendLine("\n### Files to Create:");
                foreach (var file in missingFiles)
                {
                    report.AppendLine($"- {file}");
                }
            }
        }
    }
    else
    {
        report.AppendLine("## ✅ No Broken Links Found!");
        report.AppendLine("\nAll markdown links are valid and working correctly.");
    }

    File.WriteAllText(reportFile, report.ToString());
}

// Main execution
Console.WriteLine("═══════════════════════════════════════════════════════");
Console.WriteLine("  CurlDotNet Markdown Link Checker");
Console.WriteLine("═══════════════════════════════════════════════════════");
Console.WriteLine();

if (!Directory.Exists(rootPath))
{
    WriteError($"Directory not found: {rootPath}");
    Environment.Exit(1);
}

WriteInfo($"Scanning directory: {Path.GetFullPath(rootPath)}");
if (verbose) WriteInfo("Verbose mode enabled");
if (fix) WriteWarning("Fix mode enabled - will suggest fixes");
Console.WriteLine();

// Start scanning
var stopwatch = System.Diagnostics.Stopwatch.StartNew();
ScanDirectory(rootPath);
stopwatch.Stop();

// Display results
Console.WriteLine();
Console.WriteLine("═══════════════════════════════════════════════════════");
Console.WriteLine("  Results");
Console.WriteLine("═══════════════════════════════════════════════════════");
Console.WriteLine();

Console.WriteLine($"Scanned {totalFiles} files in {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"Found {totalLinks} total links");

if (brokenLinks.Any())
{
    WriteError($"Found {brokenLinks.Count} broken links!");
    Console.WriteLine();

    // Show top 10 broken links
    Console.WriteLine("Top broken links:");
    foreach (var link in brokenLinks.Take(10))
    {
        var relPath = Path.GetRelativePath(rootPath, link.SourceFile);
        WriteError($"  {relPath}:{link.LineNumber} → {link.Target}");
    }

    if (brokenLinks.Count > 10)
    {
        Console.WriteLine($"  ... and {brokenLinks.Count - 10} more");
    }
}
else
{
    WriteSuccess($"All {workingLinks} links are valid!");
}

// Generate report
Console.WriteLine();
GenerateReport();
WriteInfo($"Full report written to: {reportFile}");

// Create missing files if --fix is specified
if (fix && brokenLinks.Any())
{
    Console.WriteLine();
    WriteWarning("Fix mode: Creating missing files...");

    var filesToCreate = brokenLinks
        .Select(l => ResolvePath(l.SourceFile, l.Target))
        .Where(p => p != null && !FileOrDirectoryExists(p))
        .Distinct()
        .ToList();

    foreach (var file in filesToCreate)
    {
        if (file.EndsWith(".md") || !Path.HasExtension(file))
        {
            var mdFile = file.EndsWith(".md") ? file : file + ".md";
            var dir = Path.GetDirectoryName(mdFile);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                WriteSuccess($"Created directory: {dir}");
            }

            if (!File.Exists(mdFile))
            {
                var title = Path.GetFileNameWithoutExtension(mdFile)
                    .Replace("-", " ")
                    .Replace("_", " ");

                var content = $@"# {System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title)}

> 📝 **Note:** This page was auto-generated to fix broken links. Please update with actual content.

## Overview

This documentation page needs to be completed.

## Topics to Cover

- [ ] Main concept explanation
- [ ] Usage examples
- [ ] Common scenarios
- [ ] Best practices
- [ ] Troubleshooting

## Related Pages

- [Back to Documentation](../README.md)

---
*This page was auto-generated by the link checker script. Please update with actual content.*
";
                File.WriteAllText(mdFile, content);
                WriteSuccess($"Created file: {mdFile}");
            }
        }
    }

    Console.WriteLine();
    WriteSuccess("Fix completed! Please review and update the generated files.");
}

// Exit with appropriate code
Environment.Exit(brokenLinks.Any() ? 1 : 0);