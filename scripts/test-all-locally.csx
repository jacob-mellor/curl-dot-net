#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

// Comprehensive Local Testing Script
// Runs build, docs, tests, and NuGet pack in a single pass with no redundant rebuilds.
// Pass --include-integration to also run network-dependent integration tests.

var includeIntegration = Args.Any(a => a == "--include-integration");

AnsiConsole.Write(new FigletText("CurlDotNet Tests")
    .Color(Color.Blue));

if (!includeIntegration)
{
    AnsiConsole.MarkupLine("[dim]Running Unit + Synthetic tests only. Use --include-integration for all.[/]");
}

var stopwatch = Stopwatch.StartNew();
var hasErrors = false;

// On macOS, tests only target net10.0 (net48 requires Mono which isn't installed).
// On Windows, omit the flag so dotnet test runs all configured frameworks.
var frameworkArg = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    ? ""
    : "--framework net10.0";

// Step 1: Build once (reused by docs, tests, and pack via --no-build)
AnsiConsole.MarkupLine("\n[yellow]Step 1: Building...[/]");
AnsiConsole.Write(new Rule("[blue]Build[/]").LeftJustified());

if (await RunCommand("dotnet", "build -c Release") != 0)
{
    AnsiConsole.MarkupLine("[red]Build failed![/]");
    Environment.Exit(1);
}
AnsiConsole.MarkupLine("[green]Build succeeded[/]");

// Step 2: Generate API documentation (uses existing build output - no rebuild)
AnsiConsole.MarkupLine("\n[yellow]Step 2: Generating API documentation...[/]");
AnsiConsole.Write(new Rule("[blue]Documentation Generation[/]").LeftJustified());

await RunCommand("dotnet", "script scripts/generate-docs.csx -- --no-build");

// Step 3: Run tests
AnsiConsole.MarkupLine("\n[yellow]Step 3: Running tests...[/]");
AnsiConsole.Write(new Rule("[blue]Test Execution[/]").LeftJustified());

// Use positive filter (Category=Synthetic|Category=Unit) to run only properly-categorized
// fast tests. Tests without a Category trait may start local HTTP servers that never terminate.
// --blame-hang kills any test that hangs beyond 30s (prevents infinite waits from orphaned servers).
// --blame-hang-dump-type none avoids permission issues with crash dump creation on macOS.
var filter = includeIntegration
    ? ""
    : "--filter \"Category=Synthetic|Category=Unit\"";

var testArgs = $"test -c Release --no-build {frameworkArg} {filter} --blame-hang --blame-hang-timeout 30s --blame-hang-dump-type none --logger:console;verbosity=minimal";

var testResult = await RunCommand("dotnet", testArgs);
if (testResult != 0)
{
    AnsiConsole.MarkupLine("[red]Tests had failures![/]");
    hasErrors = true;
}
else
{
    AnsiConsole.MarkupLine("[green]All tests passed[/]");
}

// Step 4: Create and validate NuGet package (uses existing build - no rebuild)
AnsiConsole.MarkupLine("\n[yellow]Step 4: Creating NuGet package...[/]");
AnsiConsole.Write(new Rule("[blue]NuGet Package[/]").LeftJustified());

var packagesDir = Path.Combine(Path.GetTempPath(), $"curl-packages-{DateTime.Now:yyyyMMdd-HHmmss}");
Directory.CreateDirectory(packagesDir);

if (await RunCommand("dotnet", $"pack -c Release --no-build -o {packagesDir}") != 0)
{
    AnsiConsole.MarkupLine("[red]Package creation failed![/]");
    hasErrors = true;
}
else
{
    var packageFiles = Directory.GetFiles(packagesDir, "*.nupkg");
    if (packageFiles.Any())
    {
        AnsiConsole.MarkupLine($"[green]Package created: {Path.GetFileName(packageFiles.First())}[/]");
    }
    else
    {
        AnsiConsole.MarkupLine("[red]No package found![/]");
        hasErrors = true;
    }
}

// Step 5: Check for common issues
AnsiConsole.MarkupLine("\n[yellow]Step 5: Checking for common issues...[/]");
AnsiConsole.Write(new Rule("[blue]Issue Detection[/]").LeftJustified());

var gitStatus = await RunCommandWithOutput("git", "status --porcelain");
if (!string.IsNullOrWhiteSpace(gitStatus))
{
    AnsiConsole.MarkupLine("[yellow]You have uncommitted changes[/]");
}

// Final summary
AnsiConsole.Write(new Rule().DoubleBorder());
stopwatch.Stop();

if (hasErrors)
{
    AnsiConsole.MarkupLine($"[red]SOME STEPS HAD FAILURES - review output above[/]");
    Environment.Exit(1);
}
else
{
    AnsiConsole.MarkupLine($"[green]ALL STEPS PASSED[/]");
    AnsiConsole.MarkupLine($"[dim]Total time: {stopwatch.Elapsed.TotalSeconds:F1} seconds[/]");
}

// Helper: run a command with 5-minute timeout to prevent infinite hangs
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
    if (process == null) return -1;

    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
    try
    {
        await process.WaitForExitAsync(cts.Token);
    }
    catch (OperationCanceledException)
    {
        AnsiConsole.MarkupLine($"[red]Command timed out after 5 minutes: {Markup.Escape(command + " " + args)}[/]");
        process.Kill(true);
        return -1;
    }

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
    if (process == null) return string.Empty;

    var outputTask = process.StandardOutput.ReadToEndAsync();
    var errorTask = process.StandardError.ReadToEndAsync();

    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
    try
    {
        await process.WaitForExitAsync(cts.Token);
    }
    catch (OperationCanceledException)
    {
        process.Kill(true);
        return string.Empty;
    }

    var output = await outputTask;
    var error = await errorTask;
    return string.IsNullOrEmpty(error) ? output : $"{output}\n{error}";
}
