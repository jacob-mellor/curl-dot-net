#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

// Generate documentation using DocFX for proper HTML generation
// This ensures links work correctly on GitHub Pages
// Pass --no-build to skip the build step (useful when called from test-all-locally.csx)

var skipBuild = Args.Any(a => a == "--no-build");

AnsiConsole.Write(new FigletText("CurlDotNet Docs")
    .Color(Color.Blue));

AnsiConsole.MarkupLine("[yellow]Generating documentation with DocFX...[/]");
AnsiConsole.Write(new Rule("[blue]Documentation Generation[/]").LeftJustified());

// 1. Build the project to generate XML docs (skip if already built)
if (!skipBuild)
{
    AnsiConsole.MarkupLine("\n[yellow]Building project to generate XML documentation...[/]");
    var buildResult = await RunCommand("dotnet", "build src/CurlDotNet/CurlDotNet.csproj -c Release -p:GenerateDocumentationFile=true -v quiet");
    if (buildResult != 0)
    {
        AnsiConsole.MarkupLine("[red]Build failed[/]");
        Environment.Exit(1);
    }
    AnsiConsole.MarkupLine("[green]Build succeeded[/]");
}
else
{
    AnsiConsole.MarkupLine("\n[dim]Skipping build (--no-build flag set)[/]");
}

// 2. Check if DocFX is installed, if not install it
AnsiConsole.MarkupLine("\n[yellow]Checking DocFX installation...[/]");
var (checkExitCode, toolListOutput) = await RunCommandWithOutput("dotnet", "tool list -g");
var docfxInstalled = toolListOutput.Contains("docfx", StringComparison.OrdinalIgnoreCase);

if (!docfxInstalled)
{
    AnsiConsole.MarkupLine("[yellow]Installing DocFX...[/]");
    var installResult = await RunCommand("dotnet", "tool install -g docfx");
    if (installResult != 0)
    {
        AnsiConsole.MarkupLine("[red]Failed to install DocFX[/]");
        Environment.Exit(1);
    }
}
AnsiConsole.MarkupLine("[green]DocFX is ready[/]");

// 3. Generate API documentation with DocFX
AnsiConsole.MarkupLine("\n[yellow]Generating API documentation with DocFX...[/]");

// Clean previous DocFX output only - do NOT delete root obj/ as it contains NuGet restore cache
if (Directory.Exists("_site"))
{
    Directory.Delete("_site", recursive: true);
}
// Only clean DocFX-specific artifacts, not the entire obj/ tree
var docfxApiDir = Path.Combine("obj", "api");
if (Directory.Exists(docfxApiDir))
{
    Directory.Delete(docfxApiDir, recursive: true);
}

// Run DocFX metadata and build using docfx.json in project root
var metadataResult = await RunCommand("docfx", "metadata docfx.json");
if (metadataResult != 0)
{
    AnsiConsole.MarkupLine("[red]DocFX metadata generation failed[/]");
    Environment.Exit(1);
}

var docfxResult = await RunCommand("docfx", "build docfx.json");
if (docfxResult != 0)
{
    AnsiConsole.MarkupLine("[red]DocFX build failed[/]");
    Environment.Exit(1);
}

// Count generated files
if (Directory.Exists("_site"))
{
    var htmlFileCount = Directory.GetFiles("_site", "*.html", SearchOption.AllDirectories).Length;
    var totalSize = GetDirectorySize("_site") / (1024 * 1024); // Convert to MB

    AnsiConsole.MarkupLine($"[green]Generated {htmlFileCount} HTML files ({totalSize:F1} MB)[/]");
}

// 4. Create summary
AnsiConsole.Write(new Rule("[green]Generation Complete[/]").LeftJustified());
AnsiConsole.MarkupLine("\n[green]Documentation generated successfully![/]");
AnsiConsole.MarkupLine("[cyan]Output location: _site/[/]");
AnsiConsole.MarkupLine("[cyan]View locally: docfx serve _site[/]");
AnsiConsole.MarkupLine("[cyan]GitHub Pages: https://jacob-mellor.github.io/curl-dot-net/[/]");

// Helper function to run shell commands - reads streams concurrently to prevent deadlocks
async Task<int> RunCommand(string command, string arguments = "", bool suppressErrors = false)
{
    var startInfo = new ProcessStartInfo
    {
        FileName = command,
        Arguments = arguments,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using var process = Process.Start(startInfo);
    if (process == null)
    {
        return -1;
    }

    // CRITICAL: Read stdout and stderr concurrently BEFORE waiting for exit.
    // If we call WaitForExit first, the process can fill the OS pipe buffer
    // and block on write, while we block on WaitForExit = deadlock.
    var stdoutTask = process.StandardOutput.ReadToEndAsync();
    var stderrTask = process.StandardError.ReadToEndAsync();
    await process.WaitForExitAsync();
    var stderr = await stderrTask;
    await stdoutTask;

    if (!suppressErrors && process.ExitCode != 0 && !string.IsNullOrWhiteSpace(stderr))
    {
        AnsiConsole.MarkupLine($"[red]{Markup.Escape(stderr)}[/]");
    }

    return process.ExitCode;
}

// Helper function to run a command and capture stdout
async Task<(int exitCode, string output)> RunCommandWithOutput(string command, string arguments = "")
{
    var startInfo = new ProcessStartInfo
    {
        FileName = command,
        Arguments = arguments,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using var process = Process.Start(startInfo);
    if (process == null)
    {
        return (-1, string.Empty);
    }

    var stdoutTask = process.StandardOutput.ReadToEndAsync();
    var stderrTask = process.StandardError.ReadToEndAsync();
    await process.WaitForExitAsync();
    var stdout = await stdoutTask;
    await stderrTask;

    return (process.ExitCode, stdout);
}

// Helper function to get directory size
long GetDirectorySize(string path)
{
    var directory = new DirectoryInfo(path);
    return directory.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
}
