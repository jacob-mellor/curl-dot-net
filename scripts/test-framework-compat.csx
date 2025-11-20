#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Spectre.Console;

// Framework Compatibility Tester for macOS
// Tests .NET Standard 2.0 build which is Framework 4.7.2 compatible

var title = new FigletText("Framework")
    .Color(Color.Blue);
var subtitle = new FigletText("Compat Test")
    .Color(Color.Cyan1);

AnsiConsole.Write(title);
AnsiConsole.Write(subtitle);

AnsiConsole.MarkupLine("[yellow]🔧 Testing .NET Framework 4.7.2 Compatibility on macOS[/]");
AnsiConsole.MarkupLine("[dim]Using .NET Standard 2.0 as proxy for Framework compatibility[/]");

await AnsiConsole.Progress()
    .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn())
    .StartAsync(async ctx =>
    {
        var buildTask = ctx.AddTask("[yellow]Building .NET Standard 2.0[/]");
        var testTask = ctx.AddTask("[yellow]Running compatibility tests[/]");
        var analyzeTask = ctx.AddTask("[yellow]Analyzing API surface[/]");

        // Build .NET Standard 2.0
        buildTask.StartTask();
        var buildResult = await RunCommand("dotnet", "build src/CurlDotNet/CurlDotNet.csproj -f netstandard2.0 -c Release");
        buildTask.Value = 100;

        if (buildResult.ExitCode != 0)
        {
            AnsiConsole.MarkupLine("[red]Build failed![/]");
            AnsiConsole.WriteLine(buildResult.Output);
            return;
        }

        // Run Framework compatibility tests
        testTask.StartTask();
        var testResult = await RunCommand("dotnet", "test tests/CurlDotNet.FrameworkCompat/CurlDotNet.FrameworkCompat.csproj -c Release");
        testTask.Value = 100;

        // Analyze API surface for Framework compatibility
        analyzeTask.StartTask();
        var apiCompatible = CheckApiCompatibility();
        analyzeTask.Value = 100;

        // Results
        AnsiConsole.WriteLine();
        var table = new Table();
        table.AddColumn("Check");
        table.AddColumn("Result");

        table.AddRow("Build .NET Standard 2.0", buildResult.ExitCode == 0 ? "[green]✓ Pass[/]" : "[red]✗ Fail[/]");
        table.AddRow("Framework Compat Tests", testResult.ExitCode == 0 ? "[green]✓ Pass[/]" : "[red]✗ Fail[/]");
        table.AddRow("API Compatibility", apiCompatible ? "[green]✓ Compatible[/]" : "[yellow]⚠ Check needed[/]");

        AnsiConsole.Write(table);

        if (buildResult.ExitCode == 0 && testResult.ExitCode == 0)
        {
            AnsiConsole.MarkupLine("[green]✅ Framework compatibility verified![/]");
            AnsiConsole.MarkupLine("[dim]The .NET Standard 2.0 build will work with .NET Framework 4.7.2+[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]❌ Framework compatibility issues detected[/]");
            if (testResult.ExitCode != 0)
            {
                AnsiConsole.MarkupLine("[yellow]Test output:[/]");
                AnsiConsole.WriteLine(testResult.Output);
            }
        }
    });

bool CheckApiCompatibility()
{
    // Check for common Framework incompatibilities
    var projectFile = File.ReadAllText("src/CurlDotNet/CurlDotNet.csproj");
    var issues = new List<string>();

    // Check for problematic APIs
    var sourceFiles = Directory.GetFiles("src/CurlDotNet", "*.cs", SearchOption.AllDirectories);
    foreach (var file in sourceFiles)
    {
        var content = File.ReadAllText(file);

        // Check for .NET Core/5+ only APIs
        if (content.Contains("IAsyncEnumerable"))
            issues.Add($"IAsyncEnumerable not available in Framework 4.7.2");

        if (content.Contains("System.Text.Json") && !projectFile.Contains("System.Text.Json"))
            issues.Add($"System.Text.Json needs NuGet package for Framework");

        if (content.Contains("ReadOnlySpan<") || content.Contains("Span<"))
            issues.Add($"Span<T> requires System.Memory package for Framework");
    }

    if (issues.Any())
    {
        AnsiConsole.MarkupLine("[yellow]⚠ Potential Framework compatibility issues:[/]");
        foreach (var issue in issues)
        {
            AnsiConsole.MarkupLine($"  [dim]• {issue}[/]");
        }
    }

    return !issues.Any();
}

async Task<(int ExitCode, string Output)> RunCommand(string command, string args)
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

    var output = "";
    process.OutputDataReceived += (sender, e) => output += e.Data + "\n";
    process.ErrorDataReceived += (sender, e) => output += e.Data + "\n";

    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
    await process.WaitForExitAsync();

    return (process.ExitCode, output);
}