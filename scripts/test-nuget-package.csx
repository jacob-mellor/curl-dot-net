#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.47.0"
#r "nuget: System.IO.Compression, 4.3.0"

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;

// Test NuGet Package - Validates the NuGet package before deployment
// .NET-friendly NuGet package validator for CurlDotNet

AnsiConsole.Write(new FigletText("NuGet Validator")
    .Color(Color.Cyan1));

AnsiConsole.MarkupLine("[cyan]🧪 NuGet Package Validation Test[/]");
AnsiConsole.Write(new Rule("[blue]Package Testing[/]").LeftJustified());

// Find the latest .nupkg file
var projectRoot = Directory.GetCurrentDirectory();
if (projectRoot.EndsWith("scripts"))
{
    projectRoot = Path.GetFullPath("..");
}

var packageSearchPath = Path.Combine(projectRoot, "src", "CurlDotNet", "bin", "Release");
var packages = Directory.Exists(packageSearchPath)
    ? Directory.GetFiles(packageSearchPath, "*.nupkg", SearchOption.TopDirectoryOnly)
        .OrderBy(f => new FileInfo(f).LastWriteTime)
        .ToArray()
    : Array.Empty<string>();

if (packages.Length == 0)
{
    AnsiConsole.MarkupLine("[red]❌ No .nupkg file found. Run 'dotnet pack' first.[/]");
    Environment.Exit(1);
}

var packagePath = packages.Last();
AnsiConsole.MarkupLine($"[yellow]📦 Testing package:[/] {Path.GetFileName(packagePath)}");

// Create temp directory for testing
var tempDir = Path.Combine(Path.GetTempPath(), $"nuget-test-{Guid.NewGuid()}");
Directory.CreateDirectory(tempDir);
AnsiConsole.MarkupLine($"[dim]📁 Working directory: {tempDir}[/]");

try
{
    // 1. Unpack the NuGet package
    AnsiConsole.MarkupLine("\n[yellow]1️⃣  Unpacking NuGet package...[/]");
    var unpackedPath = Path.Combine(tempDir, "unpacked");
    ZipFile.ExtractToDirectory(packagePath, unpackedPath);
    AnsiConsole.MarkupLine("[green]   ✓ Package unpacked[/]");

    // 2. Verify required files exist
    AnsiConsole.MarkupLine("\n[yellow]2️⃣  Verifying package contents...[/]");

    var libPath = Path.Combine(unpackedPath, "lib");
    var hasNet10 = Directory.Exists(Path.Combine(libPath, "net10.0"));

    var requiredFiles = new List<string>
    {
        Path.Combine("lib", "netstandard2.0", "CurlDotNet.dll"),
        Path.Combine("lib", "net8.0", "CurlDotNet.dll"),
        "CurlDotNet.nuspec"
    };

    if (hasNet10)
    {
        requiredFiles.Add(Path.Combine("lib", "net9.0", "CurlDotNet.dll"));
        requiredFiles.Add(Path.Combine("lib", "net10.0", "CurlDotNet.dll"));
    }

    // Check for Windows-specific framework
    if (Directory.Exists(Path.Combine(libPath, "net472")))
    {
        requiredFiles.Add(Path.Combine("lib", "net472", "CurlDotNet.dll"));
    }
    if (Directory.Exists(Path.Combine(libPath, "net48")))
    {
        requiredFiles.Add(Path.Combine("lib", "net48", "CurlDotNet.dll"));
    }

    bool allFilesPresent = true;
    foreach (var file in requiredFiles)
    {
        var fullPath = Path.Combine(unpackedPath, file);
        if (File.Exists(fullPath))
        {
            AnsiConsole.MarkupLine($"   [green]✅ Found:[/] {file}");
        }
        else
        {
            AnsiConsole.MarkupLine($"   [red]❌ Missing:[/] {file}");
            allFilesPresent = false;
        }
    }

    if (!allFilesPresent)
    {
        Environment.Exit(1);
    }

    // Extract version from package name
    var packageName = Path.GetFileNameWithoutExtension(packagePath);
    var version = packageName.Replace("CurlDotNet.", "");
    AnsiConsole.MarkupLine($"\n[yellow]📌 Package Version:[/] {version}");

    // 3. Create a test project that consumes the package
    AnsiConsole.MarkupLine("\n[yellow]3️⃣  Creating test consumer project...[/]");

    var testProjectPath = Path.Combine(tempDir, "NuGetTest");
    Directory.CreateDirectory(testProjectPath);

    // Create project file
    var csprojContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>";
    await File.WriteAllTextAsync(Path.Combine(testProjectPath, "NuGetTest.csproj"), csprojContent);

    // Create a local NuGet source
    var localPackagesPath = Path.Combine(testProjectPath, "local-packages");
    Directory.CreateDirectory(localPackagesPath);
    File.Copy(packagePath, Path.Combine(localPackagesPath, Path.GetFileName(packagePath)));

    // Create a nuget.config to use local source
    var nugetConfigContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <clear />
    <add key=""local"" value=""./local-packages"" />
    <add key=""nuget.org"" value=""https://api.nuget.org/v3/index.json"" />
  </packageSources>
</configuration>";
    await File.WriteAllTextAsync(Path.Combine(testProjectPath, "nuget.config"), nugetConfigContent);

    // 4. Add the package reference
    AnsiConsole.MarkupLine("\n[yellow]4️⃣  Adding package reference...[/]");
    var addPackageResult = await RunCommand("dotnet", $"add package CurlDotNet --version {version}", testProjectPath);
    if (addPackageResult != 0)
    {
        AnsiConsole.MarkupLine("[red]   ❌ Failed to add package reference[/]");
        Environment.Exit(1);
    }
    AnsiConsole.MarkupLine("[green]   ✓ Package reference added[/]");

    // 5. Create a simple test program
    AnsiConsole.MarkupLine("\n[yellow]5️⃣  Creating test program...[/]");
    var testProgramContent = @"using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            Console.WriteLine(""🧪 Testing CurlDotNet package..."");

            // Test 1: Verify main Curl class exists
            var curlType = typeof(CurlDotNet.Curl);
            if (curlType == null)
            {
                Console.WriteLine(""❌ Test 1 Failed: Curl class not found"");
                return 1;
            }
            Console.WriteLine(""✅ Test 1 Passed: Curl class exists"");

            // Test 2: Verify LibCurl class exists
            var libcurl = new CurlDotNet.Lib.LibCurl();
            if (libcurl == null)
            {
                Console.WriteLine(""❌ Test 2 Failed: LibCurl instantiation"");
                return 1;
            }
            Console.WriteLine(""✅ Test 2 Passed: LibCurl instantiation"");

            // Test 3: Check that we can set default properties
            CurlDotNet.Curl.DefaultMaxTimeSeconds = 30;
            if (CurlDotNet.Curl.DefaultMaxTimeSeconds != 30)
            {
                Console.WriteLine(""❌ Test 3 Failed: Setting default timeout"");
                return 1;
            }
            Console.WriteLine(""✅ Test 3 Passed: Setting default timeout"");

            // Test 4: Exception types exist
            var exceptionType = typeof(CurlDotNet.Exceptions.CurlException);
            if (exceptionType == null)
            {
                Console.WriteLine(""❌ Test 4 Failed: Exception types"");
                return 1;
            }
            Console.WriteLine(""✅ Test 4 Passed: Exception types"");

            Console.WriteLine("""");
            Console.WriteLine(""✨ All NuGet package tests passed!"");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($""❌ Test failed with exception: {ex.Message}"");
            return 1;
        }
    }
}";
    await File.WriteAllTextAsync(Path.Combine(testProjectPath, "Program.cs"), testProgramContent);
    AnsiConsole.MarkupLine("[green]   ✓ Test program created[/]");

    // 6. Build the test project
    AnsiConsole.MarkupLine("\n[yellow]6️⃣  Building test project...[/]");
    var buildResult = await RunCommand("dotnet", "build --configuration Release", testProjectPath);
    if (buildResult != 0)
    {
        AnsiConsole.MarkupLine("[red]   ❌ Build failed - See error above[/]");
        Environment.Exit(1);
    }
    AnsiConsole.MarkupLine("[green]   ✓ Build successful[/]");

    // 7. Run smoke tests
    AnsiConsole.MarkupLine("\n[yellow]7️⃣  Running smoke tests...[/]");
    var runResult = await RunCommand("dotnet", "run --configuration Release --no-build", testProjectPath);
    if (runResult != 0)
    {
        AnsiConsole.MarkupLine("\n[red]❌ NuGet package validation FAILED![/]");
        Environment.Exit(1);
    }

    // Success!
    AnsiConsole.WriteLine();
    AnsiConsole.Write(new Rule().DoubleBorder());

    var panel = new Panel(new Markup("[green]✅ NuGet package validation PASSED![/]"))
        .Header("[green]Success[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Green);
    AnsiConsole.Write(panel);

    // Show package info
    var table = new Table();
    table.AddColumn("Property");
    table.AddColumn("Value");
    table.Border(TableBorder.Rounded);

    table.AddRow("Name", "CurlDotNet");
    table.AddRow("Version", version);

    var fileInfo = new FileInfo(packagePath);
    table.AddRow("Size", FormatBytes(fileInfo.Length));

    // List actual frameworks in the package
    var frameworks = Directory.GetDirectories(libPath)
        .Select(d => Path.GetFileName(d))
        .Select(f => f switch
        {
            "netstandard2.0" => ".NET Standard 2.0",
            "net472" => ".NET Framework 4.7.2",
            "net48" => ".NET Framework 4.8",
            "net8.0" => ".NET 8.0",
            "net9.0" => ".NET 9.0",
            "net10.0" => ".NET 10.0",
            _ => f
        });
    table.AddRow("Frameworks", string.Join(", ", frameworks));

    AnsiConsole.Write(table);

    AnsiConsole.MarkupLine($"\n[dim]🎉 NuGet package is ready for deployment![/]");
    AnsiConsole.MarkupLine($"[dim]   Use: dotnet nuget push {packagePath}[/]");
}
finally
{
    // Cleanup
    if (Directory.Exists(tempDir))
    {
        try
        {
            Directory.Delete(tempDir, recursive: true);
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}

// Helper functions
async Task<int> RunCommand(string command, string args, string workingDir = null)
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

    if (!string.IsNullOrEmpty(workingDir))
    {
        startInfo.WorkingDirectory = workingDir;
    }

    using var process = Process.Start(startInfo);
    await process.WaitForExitAsync();
    return process.ExitCode;
}

string FormatBytes(long bytes)
{
    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
    int order = 0;
    double size = bytes;

    while (size >= 1024 && order < sizes.Length - 1)
    {
        order++;
        size = size / 1024;
    }

    return $"{size:0.##} {sizes[order]}";
}