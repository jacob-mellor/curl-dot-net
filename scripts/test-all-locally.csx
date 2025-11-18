#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;

// Comprehensive Local Testing Script
// This script runs ALL tests locally to catch issues before pushing

AnsiConsole.Write(new FigletText("CurlDotNet Tests")
    .Color(Color.Blue));

var stopwatch = Stopwatch.StartNew();
var hasErrors = false;

// Step 0: Generate API documentation
AnsiConsole.MarkupLine("[yellow]Step 0: Generating API documentation...[/]");
AnsiConsole.Write(new Rule("[blue]Documentation Generation[/]").LeftJustified());

if (await RunCommand("dotnet", "build -c Release") != 0)
{
    AnsiConsole.MarkupLine("[red]❌ Build failed![/]");
    hasErrors = true;
}
else
{
    await RunCommand("dotnet", "script scripts/generate-docs.csx");
}

// Step 1: Clean and build
AnsiConsole.MarkupLine("\n[yellow]Step 1: Clean build...[/]");
AnsiConsole.Write(new Rule("[blue]Build[/]").LeftJustified());

await RunCommand("dotnet", "clean");
if (await RunCommand("dotnet", "build -c Release") != 0)
{
    AnsiConsole.MarkupLine("[red]❌ Build failed![/]");
    hasErrors = true;
}
else
{
    AnsiConsole.MarkupLine("[green]✓ Build succeeded[/]");
}

// Step 2: Run ALL tests
AnsiConsole.MarkupLine("\n[yellow]Step 2: Running ALL tests...[/]");
AnsiConsole.Write(new Rule("[blue]Test Execution[/]").LeftJustified());

var testResult = await RunCommand("dotnet", "test -c Release --logger:console;verbosity=minimal");
if (testResult != 0)
{
    AnsiConsole.MarkupLine("[red]❌ Tests failed![/]");
    hasErrors = true;
}
else
{
    AnsiConsole.MarkupLine("[green]✓ All tests passed[/]");
}

// Step 3: Check test coverage
AnsiConsole.MarkupLine("\n[yellow]Step 3: Checking test coverage...[/]");
AnsiConsole.Write(new Rule("[blue]Coverage Analysis[/]").LeftJustified());

// Check if coverlet is installed
var coverletCheck = await RunCommandWithOutput("dotnet", "tool list -g");
if (!coverletCheck.Contains("coverlet.console"))
{
    AnsiConsole.MarkupLine("[dim]ℹ️  Coverage tool not installed (install with: dotnet tool install -g coverlet.console)[/]");
}
else
{
    await RunCommand("dotnet", "test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover");
}

// Step 4: Build documentation
AnsiConsole.MarkupLine("\n[yellow]Step 4: Building documentation...[/]");
AnsiConsole.Write(new Rule("[blue]Documentation[/]").LeftJustified());

var docsPath = Path.Combine(Directory.GetCurrentDirectory(), "gh-pages");
if (Directory.Exists(docsPath))
{
    AnsiConsole.MarkupLine($"[green]✓ Documentation found at {docsPath}[/]");
}
else
{
    AnsiConsole.MarkupLine("[dim]ℹ️  Documentation not configured[/]");
}

// Step 5: Create NuGet package
AnsiConsole.MarkupLine("\n[yellow]Step 5: Creating NuGet package...[/]");
AnsiConsole.Write(new Rule("[blue]NuGet Package[/]").LeftJustified());

var packagesDir = Path.Combine(Directory.GetCurrentDirectory(), "test-packages");
if (!Directory.Exists(packagesDir))
{
    Directory.CreateDirectory(packagesDir);
}

if (await RunCommand("dotnet", $"pack -c Release -o {packagesDir}") != 0)
{
    AnsiConsole.MarkupLine("[red]❌ Package creation failed![/]");
    hasErrors = true;
}
else
{
    AnsiConsole.MarkupLine("[green]✓ Package created successfully[/]");
}

// Step 6: Validate package
AnsiConsole.MarkupLine("\n[yellow]Step 6: Validating package...[/]");
AnsiConsole.Write(new Rule("[blue]Package Validation[/]").LeftJustified());

var packageFiles = Directory.GetFiles(packagesDir, "*.nupkg");
if (packageFiles.Any())
{
    AnsiConsole.MarkupLine($"[green]✓ Package created: {Path.GetFileName(packageFiles.First())}[/]");
}
else
{
    AnsiConsole.MarkupLine("[red]❌ No package found![/]");
    hasErrors = true;
}

// Step 7: Check for common issues
AnsiConsole.MarkupLine("\n[yellow]Step 7: Checking for common issues...[/]");
AnsiConsole.Write(new Rule("[blue]Issue Detection[/]").LeftJustified());

// Check for uncommitted changes
var gitStatus = await RunCommandWithOutput("git", "status --porcelain");
if (!string.IsNullOrWhiteSpace(gitStatus))
{
    AnsiConsole.MarkupLine("[yellow]⚠️  You have uncommitted changes[/]");
}

// Final summary
AnsiConsole.Write(new Rule().DoubleBorder());
stopwatch.Stop();

if (hasErrors)
{
    AnsiConsole.MarkupLine($"[red]❌ SOME TESTS FAILED[/]");
    AnsiConsole.MarkupLine("[yellow]Fix issues above before pushing![/]");
    Environment.Exit(1);
}
else
{
    AnsiConsole.MarkupLine($"[green]✅ ALL TESTS PASSED[/]");
    AnsiConsole.MarkupLine($"[dim]Total time: {stopwatch.Elapsed.TotalSeconds:F1} seconds[/]");
}

// Helper functions
async Task<int> RunCommand(string command, string args, bool silent = false)
{
    var startInfo = new ProcessStartInfo
    {
        FileName = command,
        Arguments = args,
        UseShellExecute = false,
        RedirectStandardOutput = silent,
        RedirectStandardError = silent
    };

    using var process = Process.Start(startInfo);
    await process.WaitForExitAsync();
    return process.ExitCode;
}

async Task<string> RunCommandWithOutput(string command, string args)
{
    var startInfo = new ProcessStartInfo
    {
        FileName = command,
        Arguments = args,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };

    using var process = Process.Start(startInfo);
    if (process == null)
    {
        return string.Empty;
    }

    var outputTask = process.StandardOutput.ReadToEndAsync();
    var errorTask = process.StandardError.ReadToEndAsync();
    await process.WaitForExitAsync();

    var output = await outputTask;
    var error = await errorTask;

    return string.IsNullOrEmpty(error) ? output : $"{output}\n{error}";
}