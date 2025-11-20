#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"

using System;
using System.Diagnostics;
using System.Linq;
using Spectre.Console;

// Build configuration script for local development
// Allows selecting which frameworks to build

AnsiConsole.Write(new FigletText("CurlDotNet")
    .Centered()
    .Color(Color.Cyan1));

AnsiConsole.WriteLine();
AnsiConsole.MarkupLine("[cyan]Local Build Configuration[/]");
AnsiConsole.WriteLine();

// Check available SDKs
var availableFrameworks = new List<string>();

// Check for .NET SDKs
var sdkProcess = Process.Start(new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = "--list-sdks",
    RedirectStandardOutput = true,
    UseShellExecute = false
});

var sdkOutput = sdkProcess.StandardOutput.ReadToEnd();
sdkProcess.WaitForExit();

var has8 = sdkOutput.Contains("8.");
var has9 = sdkOutput.Contains("9.");
var has10 = sdkOutput.Contains("10.");

AnsiConsole.MarkupLine("[yellow]Detected SDKs:[/]");
if (has8) AnsiConsole.MarkupLine("  [green]✓[/] .NET 8.0");
if (has9) AnsiConsole.MarkupLine("  [green]✓[/] .NET 9.0");
if (has10) AnsiConsole.MarkupLine("  [green]✓[/] .NET 10.0");

// Check if on Windows
var isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
if (isWindows)
{
    AnsiConsole.MarkupLine("  [green]✓[/] .NET Framework 4.7.2");
    AnsiConsole.MarkupLine("  [green]✓[/] .NET Framework 4.8");
}

AnsiConsole.WriteLine();

// Build options
var buildChoice = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Select [cyan]build configuration[/]:")
        .AddChoices(new[]
        {
            "Full - All available frameworks (includes .NET 10 if available)",
            "Standard - Production frameworks only (no .NET 10)",
            "Quick - Minimal for fast iteration (net8.0 only)",
            "Custom - Select specific frameworks"
        }));

string frameworks = "";

switch (buildChoice)
{
    case "Full - All available frameworks (includes .NET 10 if available)":
        // Build with .NET 10 if available
        if (isWindows)
            frameworks = has10 ? "netstandard2.0;net472;net48;net8.0;net9.0;net10.0" : "netstandard2.0;net472;net48;net8.0;net9.0";
        else
            frameworks = has10 ? "netstandard2.0;net8.0;net9.0;net10.0" : "netstandard2.0;net8.0;net9.0";
        break;

    case "Standard - Production frameworks only (no .NET 10)":
        // Never include .NET 10
        frameworks = isWindows ? "netstandard2.0;net472;net48;net8.0;net9.0" : "netstandard2.0;net8.0;net9.0";
        break;

    case "Quick - Minimal for fast iteration (net8.0 only)":
        frameworks = "net8.0";
        break;

    case "Custom - Select specific frameworks":
        var choices = new List<string> { "netstandard2.0", "net8.0" };
        if (has9) choices.Add("net9.0");
        if (has10) choices.Add("net10.0");
        if (isWindows)
        {
            choices.Add("net472");
            choices.Add("net48");
        }

        var selected = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select frameworks to build:")
                .AddChoices(choices));

        frameworks = string.Join(";", selected);
        break;
}

AnsiConsole.WriteLine();
AnsiConsole.MarkupLine($"[cyan]Building frameworks:[/] {frameworks}");
AnsiConsole.WriteLine();

// Configuration choice
var config = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Select [cyan]configuration[/]:")
        .AddChoices(new[] { "Debug", "Release" }));

// Run tests?
var runTests = AnsiConsole.Confirm("Run tests after build?");

// Execute build
AnsiConsole.Status()
    .Start("Building...", ctx =>
    {
        ctx.Spinner(Spinner.Known.Star);
        ctx.SpinnerStyle(Style.Parse("cyan"));

        var buildArgs = $"build --configuration {config} -p:TargetFrameworks=\"{frameworks}\"";

        var buildProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = buildArgs,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        });

        var output = buildProcess.StandardOutput.ReadToEnd();
        var error = buildProcess.StandardError.ReadToEnd();
        buildProcess.WaitForExit();

        if (buildProcess.ExitCode == 0)
        {
            AnsiConsole.MarkupLine("[green]✓ Build succeeded![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]✗ Build failed![/]");
            AnsiConsole.WriteLine(error);
        }
    });

if (runTests)
{
    AnsiConsole.WriteLine();
    AnsiConsole.Status()
        .Start("Running tests...", ctx =>
        {
            ctx.Spinner(Spinner.Known.Star);
            ctx.SpinnerStyle(Style.Parse("cyan"));

            var testProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"test --configuration {config} --no-build",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            var output = testProcess.StandardOutput.ReadToEnd();
            testProcess.WaitForExit();

            if (testProcess.ExitCode == 0)
            {
                AnsiConsole.MarkupLine("[green]✓ Tests passed![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]✗ Some tests failed[/]");
            }

            // Parse and display test results
            if (output.Contains("Passed:"))
            {
                AnsiConsole.WriteLine(output.Substring(output.LastIndexOf("Passed:")));
            }
        });
}

AnsiConsole.WriteLine();
AnsiConsole.MarkupLine("[green]Build complete![/]");

// Show package info if Release build
if (config == "Release")
{
    var packagePath = "src/CurlDotNet/bin/Release";
    if (Directory.Exists(packagePath))
    {
        var packages = Directory.GetFiles(packagePath, "*.nupkg")
            .Where(f => !f.Contains(".symbols.") && !f.EndsWith(".snupkg"))
            .ToList();

        if (packages.Any())
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]NuGet packages created:[/]");
            foreach (var package in packages)
            {
                var info = new FileInfo(package);
                AnsiConsole.MarkupLine($"  [cyan]📦[/] {Path.GetFileName(package)} ({info.Length / 1024.0 / 1024.0:F2} MB)");
            }
        }
    }
}