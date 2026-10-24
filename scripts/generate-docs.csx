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

// 2. Regenerate the Markdown API reference (docs/api) with DefaultDocumentation.
//    The GitHub Pages site is built by Jekyll from docs/, so these .md files ARE the public API docs.
AnsiConsole.MarkupLine("\n[yellow]Regenerating docs/api with DefaultDocumentation...[/]");
var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
var toolCandidates = new[]
{
    "defaultdocumentation",
    Path.Combine(home, ".dotnet", "tools", "defaultdocumentation"),
    Path.Combine(home, ".dotnet", "tools", "defaultdocumentation.exe"),
};
var toolPath = toolCandidates.FirstOrDefault(c => c == "defaultdocumentation" ? false : File.Exists(c)) ?? "defaultdocumentation";
var toolAvailable = false;
try
{
    var (toolCheck, _) = await RunCommandWithOutput(toolPath, "--help");
    toolAvailable = toolCheck == 0;
}
catch (System.ComponentModel.Win32Exception)
{
    toolAvailable = false; // not on PATH and not in ~/.dotnet/tools
}
if (!toolAvailable)
{
    AnsiConsole.MarkupLine("[yellow]Installing DefaultDocumentation.Console...[/]");
    var installResult = await RunCommand("dotnet", "tool install -g DefaultDocumentation.Console");
    if (installResult != 0)
    {
        AnsiConsole.MarkupLine("[red]Failed to install DefaultDocumentation.Console (dotnet tool install -g DefaultDocumentation.Console)[/]");
        Environment.Exit(1);
    }
    toolPath = Path.Combine(home, ".dotnet", "tools", "defaultdocumentation");
}

var assemblyCandidates = new[] { "net10.0", "net9.0", "net8.0", "netstandard2.0" }
    .Select(tfm => Path.Combine("src", "CurlDotNet", "bin", "Release", tfm, "CurlDotNet.dll"))
    .Where(File.Exists)
    .ToList();
if (assemblyCandidates.Count == 0)
{
    AnsiConsole.MarkupLine("[red]No built CurlDotNet.dll found under src/CurlDotNet/bin/Release. Build first (or drop --no-build).[/]");
    Environment.Exit(1);
}
var assembly = assemblyCandidates[0];

var apiDir = Path.Combine("docs", "api");
if (Directory.Exists(apiDir))
{
    // Replace the whole set: members that no longer exist must not keep a page
    foreach (var stale in Directory.GetFiles(apiDir, "*.md"))
    {
        File.Delete(stale);
    }
}
Directory.CreateDirectory(apiDir);

// Public and protected surface only: internals (e.g. the TLS helpers) are not part of the API and some
// have signatures long enough to exceed the file-name limit when a page is generated for them.
var apiResult = await RunCommand(toolPath, $"-a \"{assembly}\" -o \"{apiDir}\" --GeneratedAccessModifiers Public,Protected");
if (apiResult != 0)
{
    AnsiConsole.MarkupLine("[red]DefaultDocumentation failed[/]");
    Environment.Exit(1);
}
var apiPages = Directory.GetFiles(apiDir, "*.md").Length;
AnsiConsole.MarkupLine($"[green]docs/api: {apiPages} pages generated from {assembly}[/]");

// 3. Optional local HTML preview with DocFX (pass --docfx). Not deployed: GitHub Pages renders docs/ with Jekyll.
if (Args.Any(a => a == "--docfx"))
{
    AnsiConsole.MarkupLine("\n[yellow]Building DocFX preview site (_site)...[/]");
    if (Directory.Exists("_site"))
    {
        Directory.Delete("_site", recursive: true);
    }
    var docfxApiDir = Path.Combine("obj", "api");
    if (Directory.Exists(docfxApiDir))
    {
        Directory.Delete(docfxApiDir, recursive: true);
    }
    var metadataResult = await RunCommand("docfx", "metadata docfx.json");
    var docfxResult = metadataResult == 0 ? await RunCommand("docfx", "build docfx.json") : metadataResult;
    if (docfxResult != 0)
    {
        AnsiConsole.MarkupLine("[yellow]DocFX preview failed (this does not affect the published site)[/]");
    }
    else if (Directory.Exists("_site"))
    {
        var htmlFileCount = Directory.GetFiles("_site", "*.html", SearchOption.AllDirectories).Length;
        AnsiConsole.MarkupLine($"[green]DocFX preview: {htmlFileCount} HTML files in _site/ (docfx serve _site)[/]");
    }
}

// 4. Summary
AnsiConsole.Write(new Rule("[green]Generation Complete[/]").LeftJustified());
AnsiConsole.MarkupLine("\n[green]Documentation generated successfully![/]");
AnsiConsole.MarkupLine("[cyan]API reference: docs/api/ (commit it; the site is built from docs/ by GitHub Pages)[/]");
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
