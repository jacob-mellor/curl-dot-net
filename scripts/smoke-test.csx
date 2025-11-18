#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Spectre.Console;

// Smoke test - Quick validation that everything works
// Run this before pushing to catch obvious issues FAST
// .NET-friendly smoke test for CurlDotNet

AnsiConsole.Write(new FigletText("Smoke Test")
    .Color(Color.Yellow));

AnsiConsole.MarkupLine("[yellow]🚬 Quick Validation[/]");
AnsiConsole.Write(new Rule("[blue]Fast Smoke Test[/]").LeftJustified());

var stopwatch = Stopwatch.StartNew();
var hasErrors = false;

// 1. Quick build check
await AnsiConsole.Status()
    .Spinner(Spinner.Known.Star)
    .SpinnerStyle(Style.Parse("yellow"))
    .StartAsync("Building project...", async ctx =>
    {
        var buildResult = await RunCommand("dotnet", "build --verbosity quiet");
        if (buildResult != 0)
        {
            AnsiConsole.MarkupLine("[red]❌ BUILD BROKEN - Fix immediately![/]");
            hasErrors = true;
        }
        else
        {
            AnsiConsole.MarkupLine("[green]✓ Build passed[/]");
        }
    });

if (hasErrors)
{
    Environment.Exit(1);
}

// 2. Run just the fast unit tests (not integration)
await AnsiConsole.Status()
    .Spinner(Spinner.Known.Star)
    .SpinnerStyle(Style.Parse("yellow"))
    .StartAsync("Running fast tests only...", async ctx =>
    {
        var testResult = await RunCommand("dotnet",
            "test --no-build --verbosity quiet --filter \"Category!=Integration\"");
        if (testResult != 0)
        {
            AnsiConsole.MarkupLine("[red]❌ UNIT TESTS BROKEN - Fix before pushing![/]");
            hasErrors = true;
        }
        else
        {
            AnsiConsole.MarkupLine("[green]✓ Unit tests passed[/]");
        }
    });

if (hasErrors)
{
    Environment.Exit(1);
}

// 3. Check if NuGet package can be created
var tempPackageDir = Path.Combine(Path.GetTempPath(), "smoke-test-pkg-" + Guid.NewGuid().ToString());

await AnsiConsole.Status()
    .Spinner(Spinner.Known.Star)
    .SpinnerStyle(Style.Parse("yellow"))
    .StartAsync("Checking package creation...", async ctx =>
    {
        Directory.CreateDirectory(tempPackageDir);

        var packResult = await RunCommand("dotnet",
            $"pack --no-build --verbosity quiet -o \"{tempPackageDir}\"");

        if (packResult != 0)
        {
            AnsiConsole.MarkupLine("[red]❌ PACKAGE BROKEN - Can't create NuGet package![/]");
            hasErrors = true;
        }
        else
        {
            AnsiConsole.MarkupLine("[green]✓ Package creation succeeded[/]");
        }

        // Clean up temp directory
        if (Directory.Exists(tempPackageDir))
        {
            try
            {
                Directory.Delete(tempPackageDir, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    });

if (hasErrors)
{
    Environment.Exit(1);
}

stopwatch.Stop();

// Success summary
AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule().DoubleBorder());

var panel = new Panel(new Markup("[green]✅ Smoke test PASSED![/]\n[dim]Safe for quick push[/]"))
    .Header("[green]Success[/]")
    .Border(BoxBorder.Rounded)
    .BorderColor(Color.Green);

AnsiConsole.Write(panel);

AnsiConsole.MarkupLine($"[dim]Time: {stopwatch.Elapsed.TotalSeconds:F1} seconds[/]");
AnsiConsole.MarkupLine("[dim]Run test-all-locally.csx for full validation[/]");

// Helper function
async Task<int> RunCommand(string command, string args)
{
    var startInfo = new ProcessStartInfo
    {
        FileName = command,
        Arguments = args,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    };

    using var process = Process.Start(startInfo);
    await process.WaitForExitAsync();
    return process.ExitCode;
}