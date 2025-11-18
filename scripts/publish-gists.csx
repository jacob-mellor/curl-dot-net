#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.48.0"
#r "nuget: System.Text.Json, 8.0.0"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;

// Gist Publishing Script for CurlDotNet Examples
// Publishes example files to GitHub Gists for easy sharing

var client = new HttpClient();
client.DefaultRequestHeaders.Add("User-Agent", "CurlDotNet-GistPublisher");

// Get GitHub token from environment
var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (!string.IsNullOrEmpty(token))
{
    client.DefaultRequestHeaders.Add("Authorization", $"token {token}");
}

// Define examples to publish
var examples = new Dictionary<string, string>
{
    ["Simple GET Request"] = "examples/BasicExamples/01-SimpleGetRequest.cs",
    ["POST JSON Data"] = "examples/BasicExamples/02-PostJsonData.cs",
    ["Error Handling"] = "examples/BasicExamples/03-ErrorHandling.cs",
    ["Bearer Token Auth"] = "examples/Authentication/01-BearerToken.cs",
    ["File Download"] = "examples/FileOperations/01-DownloadFile.cs",
    ["Proxy Usage"] = "examples/AdvancedScenarios/01-ProxyUsage.cs",
    ["GitHub API Integration"] = "examples/RealWorld/01-GitHubApi.cs",
    ["Web Scraping"] = "examples/RealWorld/02-WebScraping.cs"
};

AnsiConsole.Write(
    new FigletText("Gist Publisher")
        .Centered()
        .Color(Color.Blue));

AnsiConsole.WriteLine();

// Check if we have a GitHub token
if (string.IsNullOrEmpty(token))
{
    AnsiConsole.MarkupLine("[yellow]Warning: No GITHUB_TOKEN found. Gists will be created anonymously.[/]");
    AnsiConsole.WriteLine();
}

// Create a table to track results
var table = new Table();
table.AddColumn("Example");
table.AddColumn("Status");
table.AddColumn("Gist URL");

// Process each example
await AnsiConsole.Progress()
    .Columns(
        new TaskDescriptionColumn(),
        new ProgressBarColumn(),
        new PercentageColumn(),
        new SpinnerColumn())
    .StartAsync(async ctx =>
    {
        var task = ctx.AddTask("[green]Publishing examples to Gists[/]", maxValue: examples.Count);

        foreach (var example in examples)
        {
            task.Description = $"Publishing [cyan]{example.Key}[/]";

            try
            {
                var filePath = Path.Combine("..", example.Value);
                if (!File.Exists(filePath))
                {
                    table.AddRow(
                        $"[red]{example.Key}[/]",
                        "[red]✗ File not found[/]",
                        "-"
                    );
                    task.Increment(1);
                    continue;
                }

                var content = await File.ReadAllTextAsync(filePath);

                // Create gist payload
                var gistData = new
                {
                    description = $"CurlDotNet Example: {example.Key}",
                    @public = true,
                    files = new Dictionary<string, object>
                    {
                        [Path.GetFileName(example.Value)] = new { content }
                    }
                };

                var json = JsonSerializer.Serialize(gistData);
                var response = await client.PostAsync(
                    "https://api.github.com/gists",
                    new StringContent(json, Encoding.UTF8, "application/json")
                );

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonDocument.Parse(responseContent);
                    var htmlUrl = responseJson.RootElement.GetProperty("html_url").GetString();

                    table.AddRow(
                        $"[green]{example.Key}[/]",
                        "[green]✓ Published[/]",
                        $"[link]{htmlUrl}[/]"
                    );
                }
                else
                {
                    table.AddRow(
                        $"[yellow]{example.Key}[/]",
                        $"[yellow]✗ Failed ({response.StatusCode})[/]",
                        "-"
                    );
                }
            }
            catch (Exception ex)
            {
                table.AddRow(
                    $"[red]{example.Key}[/]",
                    $"[red]✗ Error[/]",
                    ex.Message.Substring(0, Math.Min(30, ex.Message.Length)) + "..."
                );
            }

            task.Increment(1);
            await Task.Delay(500); // Rate limiting
        }
    });

AnsiConsole.WriteLine();
AnsiConsole.Write(table);

// Generate markdown for README
AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule("[yellow]Markdown for README[/]"));
AnsiConsole.WriteLine();

var markdown = new StringBuilder();
markdown.AppendLine("## 📝 Example Gists");
markdown.AppendLine();
markdown.AppendLine("Quick access to example code via GitHub Gists:");
markdown.AppendLine();

foreach (var row in table.Rows)
{
    if (row[1].ToString().Contains("✓"))
    {
        var name = row[0].ToString().Replace("[green]", "").Replace("[/]", "");
        var url = row[2].ToString().Replace("[link]", "").Replace("[/]", "");
        markdown.AppendLine($"- [{name}]({url})");
    }
}

AnsiConsole.WriteLine(markdown.ToString());

// Save to file
var gistsFile = Path.Combine("..", "examples", "GISTS.md");
await File.WriteAllTextAsync(gistsFile, markdown.ToString());
AnsiConsole.MarkupLine($"[green]Gist links saved to {gistsFile}[/]");

AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule());
AnsiConsole.MarkupLine("[green]✓[/] Gist publishing complete!");