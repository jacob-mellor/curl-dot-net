#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Spectre.Console;

/// <summary>
/// CurlDotNet Code Coverage Tool
///
/// This script:
/// 1. Runs all unit tests with code coverage collection
/// 2. Generates a detailed HTML coverage report
/// 3. Updates README files with coverage badges and statistics
/// 4. Updates NuGet package release notes with coverage information
/// 5. Ensures 100% test passing before release
///
/// Run: dotnet script scripts/code-coverage.csx
/// </summary>

// Configuration
var rootDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));
var testsProject = Path.Combine(rootDir, "tests", "CurlDotNet.Tests", "CurlDotNet.Tests.csproj");
// Store coverage reports in temp directory, NOT in project root
var coverageDir = Path.Combine(Path.GetTempPath(), $"curl-coverage-{DateTime.Now:yyyyMMdd-HHmmss}");
var mainReadme = Path.Combine(rootDir, "README.md");
var nugetReadme = Path.Combine(rootDir, "src", "CurlDotNet", "README.md");
var projectFile = Path.Combine(rootDir, "src", "CurlDotNet", "CurlDotNet.csproj");

AnsiConsole.Write(new FigletText("Code Coverage")
    .Centered()
    .Color(Color.Cyan1));

AnsiConsole.WriteLine();

// Step 1: Clean previous coverage reports
await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .SpinnerStyle(Style.Parse("cyan"))
    .StartAsync("🧹 Cleaning previous coverage reports...", async ctx =>
    {
        if (Directory.Exists(coverageDir))
        {
            Directory.Delete(coverageDir, true);
        }
        Directory.CreateDirectory(coverageDir);
        await Task.Delay(500);
    });

// Step 2: Run tests with coverage
var testResults = await AnsiConsole.Progress()
    .AutoRefresh(true)
    .HideCompleted(false)
    .Columns(
        new TaskDescriptionColumn(),
        new ProgressBarColumn(),
        new PercentageColumn(),
        new RemainingTimeColumn(),
        new SpinnerColumn())
    .StartAsync(async ctx =>
    {
        var task = ctx.AddTask("🧪 Running tests with coverage", maxValue: 100);
        task.StartTask();

        var processInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"test \"{testsProject}\" " +
                       $"-c Debug " +
                       $"/p:CollectCoverage=true " +
                       $"/p:CoverletOutputFormat=\"cobertura,json,opencover\" " +
                       $"/p:CoverletOutput=\"{coverageDir}/\" " +
                       $"/p:Include=\"[CurlDotNet]*\" " +
                       $"/p:Exclude=\"[CurlDotNet.Tests]*\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = rootDir
        };

        using var process = Process.Start(processInfo);
        var output = "";
        var errorOutput = "";

        // Simulate progress while tests run
        var progressTask = Task.Run(async () =>
        {
            for (int i = 0; i <= 100; i += 5)
            {
                task.Increment(5);
                await Task.Delay(500);
                if (process.HasExited) break;
            }
        });

        output = await process.StandardOutput.ReadToEndAsync();
        errorOutput = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();
        await progressTask;

        task.Value = 100;
        task.StopTask();

        return new { ExitCode = process.ExitCode, Output = output, Error = errorOutput };
    });

// Parse test results
var testsPassed = 0;
var testsFailed = 0;
var testsSkipped = 0;
var coverageLine = 0.0;
var coverageBranch = 0.0;
var coverageMethod = 0.0;

// Parse test counts from output
var passedMatch = Regex.Match(testResults.Output, @"Passed:\s+(\d+)");
var failedMatch = Regex.Match(testResults.Output, @"Failed:\s+(\d+)");
var skippedMatch = Regex.Match(testResults.Output, @"Skipped:\s+(\d+)");

if (passedMatch.Success) testsPassed = int.Parse(passedMatch.Groups[1].Value);
if (failedMatch.Success) testsFailed = int.Parse(failedMatch.Groups[1].Value);
if (skippedMatch.Success) testsSkipped = int.Parse(skippedMatch.Groups[1].Value);

// Parse coverage from output - look for the table format
var coverageMatch = Regex.Match(testResults.Output, @"\|\s+Total\s+\|\s+([\d.]+)%\s+\|\s+([\d.]+)%\s+\|\s+([\d.]+)%\s+\|");
if (coverageMatch.Success)
{
    coverageLine = double.Parse(coverageMatch.Groups[1].Value);
    coverageBranch = double.Parse(coverageMatch.Groups[2].Value);
    coverageMethod = double.Parse(coverageMatch.Groups[3].Value);
}

// Display test results
AnsiConsole.WriteLine();
var testTable = new Table()
    .Border(TableBorder.Rounded)
    .Title("📊 Test Results")
    .AddColumn("Metric")
    .AddColumn("Value");

testTable.AddRow("✅ Tests Passed", $"[green]{testsPassed}[/]");
testTable.AddRow("❌ Tests Failed", testsFailed > 0 ? $"[red]{testsFailed}[/]" : $"{testsFailed}");
testTable.AddRow("⏭️ Tests Skipped", $"{testsSkipped}");
testTable.AddRow("📈 Line Coverage", GetCoverageColor(coverageLine));
testTable.AddRow("🌿 Branch Coverage", GetCoverageColor(coverageBranch));
testTable.AddRow("🎯 Method Coverage", GetCoverageColor(coverageMethod));

AnsiConsole.Write(testTable);

// Check for test failures
if (testsFailed > 0)
{
    AnsiConsole.MarkupLine("[red]❌ TESTS FAILED! Cannot proceed with release.[/]");
    AnsiConsole.MarkupLine("[yellow]Fix all failing tests before running code coverage again.[/]");
    Environment.Exit(1);
}

// Step 3: Generate HTML report if reportgenerator is available
var hasReportGenerator = await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .SpinnerStyle(Style.Parse("cyan"))
    .StartAsync("🔍 Checking for ReportGenerator...", async ctx =>
    {
        var checkProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "tool list -g",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        var output = await checkProcess.StandardOutput.ReadToEndAsync();
        await checkProcess.WaitForExitAsync();

        return output.Contains("dotnet-reportgenerator-globaltool");
    });

if (!hasReportGenerator)
{
    AnsiConsole.MarkupLine("[yellow]⚠️ ReportGenerator not installed. Installing...[/]");
    var installProcess = Process.Start(new ProcessStartInfo
    {
        FileName = "dotnet",
        Arguments = "tool install -g dotnet-reportgenerator-globaltool",
        UseShellExecute = false,
        CreateNoWindow = true
    });
    await installProcess.WaitForExitAsync();
}

// Generate HTML report
await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .SpinnerStyle(Style.Parse("cyan"))
    .StartAsync("📊 Generating HTML coverage report...", async ctx =>
    {
        var coberturaFile = Directory.GetFiles(coverageDir, "*.cobertura.xml").FirstOrDefault();
        if (coberturaFile != null)
        {
            var reportProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "reportgenerator",
                Arguments = $"-reports:\"{coberturaFile}\" " +
                           $"-targetdir:\"{coverageDir}\" " +
                           $"-reporttypes:Html_Dark;Badges;TextSummary",
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = rootDir
            });

            await reportProcess.WaitForExitAsync();
        }
    });

// Step 4: Update README files with coverage badge
await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .SpinnerStyle(Style.Parse("cyan"))
    .StartAsync("📝 Updating README files with coverage information...", async ctx =>
    {
        // Create coverage badge URL
        var badgeColor = coverageLine >= 80 ? "brightgreen" :
                        coverageLine >= 60 ? "green" :
                        coverageLine >= 40 ? "yellow" :
                        coverageLine >= 20 ? "orange" : "red";

        var coverageBadge = $"![Coverage](https://img.shields.io/badge/coverage-{coverageLine:F1}%25-{badgeColor})";

        // Update main README
        await UpdateReadmeWithCoverage(mainReadme, coverageBadge, coverageLine, testsPassed);

        // Update NuGet README
        await UpdateReadmeWithCoverage(nugetReadme, coverageBadge, coverageLine, testsPassed);

        // Update project file release notes
        await UpdateProjectReleaseNotes(projectFile, coverageLine, testsPassed);
    });

// Step 5: Display final summary
AnsiConsole.WriteLine();
var summaryPanel = new Panel(
    new Markup($"""
    [bold cyan]Code Coverage Analysis Complete![/]

    📊 Coverage: [bold]{GetCoverageColor(coverageLine)}[/]
    ✅ All {testsPassed} tests passing
    📁 Report saved to: {coverageDir}/index.html

    [dim]To view the HTML report:[/]
    [yellow]open {coverageDir}/index.html[/]

    {(coverageLine < 100 ? "[yellow]⚠️ Coverage is below 100%. Consider adding more tests.[/]" : "[green]🎉 Excellent! 100% code coverage achieved![/]")}
    """))
    .Header("✨ Summary")
    .Border(BoxBorder.Rounded)
    .BorderColor(Color.Cyan1);

AnsiConsole.Write(summaryPanel);

// Helper functions
string GetCoverageColor(double percentage)
{
    var color = percentage >= 80 ? "green" :
                percentage >= 60 ? "yellow" :
                percentage >= 40 ? "orange" : "red";
    return $"[{color}]{percentage:F1}%[/]";
}

async Task UpdateReadmeWithCoverage(string readmePath, string badge, double coverage, int tests)
{
    if (!File.Exists(readmePath)) return;

    var content = await File.ReadAllTextAsync(readmePath);
    var lines = content.Split('\n').ToList();

    // Find the badges section (usually near the top)
    var badgeLineIndex = lines.FindIndex(l => l.Contains("![") && l.Contains("shields.io"));
    if (badgeLineIndex >= 0)
    {
        // Check if coverage badge already exists
        var coverageBadgeIndex = lines.FindIndex(l => l.Contains("coverage") && l.Contains("shields.io"));
        if (coverageBadgeIndex >= 0)
        {
            // Update existing badge
            lines[coverageBadgeIndex] = badge;
        }
        else
        {
            // Add new badge after other badges
            lines.Insert(badgeLineIndex + 1, badge);
        }
    }

    // Update or add coverage section
    var coverageSectionIndex = lines.FindIndex(l => l.Contains("## 📊 Code Coverage") || l.Contains("## Code Coverage"));
    if (coverageSectionIndex >= 0)
    {
        // Update existing section
        var endIndex = coverageSectionIndex + 1;
        while (endIndex < lines.Count && !lines[endIndex].StartsWith("## "))
        {
            endIndex++;
        }

        // Replace content
        var newContent = new List<string>
        {
            "## 📊 Code Coverage",
            "",
            $"- **Line Coverage:** {coverage:F1}%",
            $"- **Tests:** {tests} passing, 0 failing",
            $"- **Last Updated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            ""
        };

        lines.RemoveRange(coverageSectionIndex, endIndex - coverageSectionIndex);
        lines.InsertRange(coverageSectionIndex, newContent);
    }
    else
    {
        // Add new section before installation or at end
        var installIndex = lines.FindIndex(l => l.Contains("## Installation") || l.Contains("## 📦 Installation"));
        if (installIndex < 0) installIndex = lines.Count;

        var newContent = new List<string>
        {
            "",
            "## 📊 Code Coverage",
            "",
            $"- **Line Coverage:** {coverage:F1}%",
            $"- **Tests:** {tests} passing, 0 failing",
            $"- **Last Updated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            ""
        };

        lines.InsertRange(installIndex, newContent);
    }

    await File.WriteAllTextAsync(readmePath, string.Join('\n', lines));
}

async Task UpdateProjectReleaseNotes(string projectPath, double coverage, int tests)
{
    if (!File.Exists(projectPath)) return;

    var content = await File.ReadAllTextAsync(projectPath);

    // Find PackageReleaseNotes section
    var releaseNotesMatch = Regex.Match(content, @"<PackageReleaseNotes>(.*?)</PackageReleaseNotes>", RegexOptions.Singleline);
    if (releaseNotesMatch.Success)
    {
        var currentNotes = releaseNotesMatch.Groups[1].Value;

        // Add coverage info to release notes if not already present
        if (!currentNotes.Contains($"📊 Code Coverage: {coverage:F1}%"))
        {
            var lines = currentNotes.Split('\n').ToList();

            // Find where to insert (after version line)
            var versionLineIndex = lines.FindIndex(l => l.Contains("Version"));
            if (versionLineIndex >= 0)
            {
                lines.Insert(versionLineIndex + 1, $"📊 Code Coverage: {coverage:F1}% with {tests} tests passing");
            }

            var newNotes = string.Join('\n', lines);
            content = content.Replace(releaseNotesMatch.Value, $"<PackageReleaseNotes>{newNotes}</PackageReleaseNotes>");

            await File.WriteAllTextAsync(projectPath, content);
        }
    }
}

// Open the HTML report if on a desktop environment
if (Environment.GetEnvironmentVariable("DISPLAY") != null || Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    var htmlReport = Path.Combine(coverageDir, "index.html");
    if (File.Exists(htmlReport))
    {
        AnsiConsole.WriteLine();
        if (AnsiConsole.Confirm("📊 Would you like to open the HTML coverage report?"))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = htmlReport,
                UseShellExecute = true
            });
        }
    }
}

// Return exit code based on coverage threshold
if (coverageLine < 80)
{
    AnsiConsole.MarkupLine($"[yellow]⚠️ Warning: Coverage ({coverageLine:F1}%) is below 80% threshold[/]");
    Environment.Exit(2); // Warning exit code
}

AnsiConsole.MarkupLine("[green]✅ Code coverage analysis completed successfully![/]");
Environment.Exit(0);