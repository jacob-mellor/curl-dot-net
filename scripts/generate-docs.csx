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

AnsiConsole.Write(new FigletText("CurlDotNet Docs")
    .Color(Color.Blue));

AnsiConsole.MarkupLine("[yellow]📚 Generating documentation with DocFX...[/]");
AnsiConsole.Write(new Rule("[blue]Documentation Generation[/]").LeftJustified());

// 1. Build the project to generate XML docs
AnsiConsole.MarkupLine("\n[yellow]🔨 Building project to generate XML documentation...[/]");
var buildResult = await RunCommand("dotnet", "build src/CurlDotNet/CurlDotNet.csproj -c Release -p:GenerateDocumentationFile=true -v quiet");
if (buildResult != 0)
{
    AnsiConsole.MarkupLine("[red]❌ Build failed[/]");
    Environment.Exit(1);
}
AnsiConsole.MarkupLine("[green]✓ Build succeeded[/]");

// 2. Check if DocFX is installed, if not install it
AnsiConsole.MarkupLine("\n[yellow]🔍 Checking DocFX installation...[/]");
var checkDocFx = await RunCommand("dotnet", "tool list -g | grep docfx", suppressErrors: true);
if (checkDocFx != 0)
{
    AnsiConsole.MarkupLine("[yellow]📦 Installing DocFX...[/]");
    var installResult = await RunCommand("dotnet", "tool install -g docfx");
    if (installResult != 0)
    {
        AnsiConsole.MarkupLine("[red]❌ Failed to install DocFX[/]");
        Environment.Exit(1);
    }
}
AnsiConsole.MarkupLine("[green]✓ DocFX is ready[/]");

// 3. Generate API documentation with DocFX
AnsiConsole.MarkupLine("\n[yellow]📝 Generating API documentation with DocFX...[/]");

// Clean previous builds
if (Directory.Exists("_site"))
{
    Directory.Delete("_site", recursive: true);
}
if (Directory.Exists("obj"))
{
    Directory.Delete("obj", recursive: true);
}

// Run DocFX metadata and build using docfx.json in project root
var metadataResult = await RunCommand("docfx", "metadata docfx.json");
if (metadataResult != 0)
{
    AnsiConsole.MarkupLine("[red]❌ DocFX metadata generation failed[/]");
    Environment.Exit(1);
}

var docfxResult = await RunCommand("docfx", "build docfx.json");
if (docfxResult != 0)
{
    AnsiConsole.MarkupLine("[red]❌ DocFX build failed[/]");
    Environment.Exit(1);
}

// Count generated files
if (Directory.Exists("_site"))
{
    var htmlFileCount = Directory.GetFiles("_site", "*.html", SearchOption.AllDirectories).Length;
    var totalSize = GetDirectorySize("_site") / (1024 * 1024); // Convert to MB

    AnsiConsole.MarkupLine($"[green]✅ Generated {htmlFileCount} HTML files ({totalSize:F1} MB)[/]");
}

// 4. Create summary
AnsiConsole.Write(new Rule("[green]Generation Complete[/]").LeftJustified());
AnsiConsole.MarkupLine("\n[green]✅ Documentation generated successfully![/]");
AnsiConsole.MarkupLine("[cyan]📁 Output location: _site/[/]");
AnsiConsole.MarkupLine("[cyan]🌐 View locally: docfx serve _site[/]");
AnsiConsole.MarkupLine("[cyan]📚 GitHub Pages: https://jacob-mellor.github.io/curl-dot-net/[/]");

// Helper function to run shell commands
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

    await process.WaitForExitAsync();

    if (!suppressErrors && process.ExitCode != 0)
    {
        var error = await process.StandardError.ReadToEndAsync();
        if (!string.IsNullOrWhiteSpace(error))
        {
            AnsiConsole.MarkupLine($"[red]{error}[/]");
        }
    }

    return process.ExitCode;
}

// Helper function to get directory size
long GetDirectorySize(string path)
{
    var directory = new DirectoryInfo(path);
    return directory.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
}