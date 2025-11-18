using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

namespace DownloadFile
{
    /// <summary>
    /// Demonstrates downloading files with CurlDotNet
    /// Based on cookbook/beginner/download-file.md
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CurlDotNet: File Download Example ===\n");

            // Create downloads directory
            var downloadDir = Path.Combine(Directory.GetCurrentDirectory(), "downloads");
            Directory.CreateDirectory(downloadDir);
            Console.WriteLine($"Download directory: {downloadDir}\n");

            // Example 1: Download a small text file
            Console.WriteLine("1. Downloading a text file...");

            var textUrl = "https://raw.githubusercontent.com/curl/curl/master/README.md";
            var textResult = await Curl.GetAsync(textUrl);

            if (textResult.IsSuccess)
            {
                var textPath = Path.Combine(downloadDir, "curl-readme.md");
                await File.WriteAllTextAsync(textPath, textResult.Body);
                Console.WriteLine($"✅ Downloaded text file: {textPath}");
                Console.WriteLine($"   Size: {textResult.Body.Length:N0} bytes\n");
            }

            // Example 2: Download with curl -o flag (output to file)
            Console.WriteLine("2. Using curl -o flag to save directly...");

            var imagePath = Path.Combine(downloadDir, "placeholder.png");
            var curlCommand = $@"curl -o ""{imagePath}"" https://via.placeholder.com/150";

            // Note: In real CurlDotNet, this would be handled differently
            // For demo, we'll download and save manually
            var imageResult = await Curl.GetAsync("https://via.placeholder.com/150");

            if (imageResult.IsSuccess)
            {
                // For binary data, we'd need to handle bytes properly
                // This is a simplified example
                Console.WriteLine($"✅ Would save image to: {imagePath}");
                Console.WriteLine($"   Response size: {imageResult.Body.Length:N0} bytes\n");
            }

            // Example 3: Download with progress tracking
            Console.WriteLine("3. Downloading with progress simulation...");

            var urls = new[]
            {
                ("File 1", "https://httpbin.org/bytes/1024"),
                ("File 2", "https://httpbin.org/bytes/2048"),
                ("File 3", "https://httpbin.org/bytes/4096")
            };

            foreach (var (name, url) in urls)
            {
                Console.Write($"   Downloading {name}... ");
                var result = await Curl.GetAsync(url);

                if (result.IsSuccess)
                {
                    Console.WriteLine($"✅ {result.Body.Length:N0} bytes");
                }
            }
            Console.WriteLine();

            // Example 4: Download and check headers
            Console.WriteLine("4. Download with header inspection...");

            var headerResult = await Curl.ExecuteAsync(@"
                curl -I https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf
            ");

            if (headerResult.IsSuccess)
            {
                Console.WriteLine("✅ File headers retrieved:");
                var headers = headerResult.Body.Split('\n');
                foreach (var header in headers.Take(5))
                {
                    if (!string.IsNullOrWhiteSpace(header))
                        Console.WriteLine($"   {header.Trim()}");
                }
                Console.WriteLine();
            }

            // Example 5: Download JSON and parse
            Console.WriteLine("5. Downloading and parsing JSON data...");

            var jsonResult = await Curl.GetAsync("https://api.github.com/repos/curl/curl");

            if (jsonResult.IsSuccess)
            {
                var jsonPath = Path.Combine(downloadDir, "curl-repo.json");
                await File.WriteAllTextAsync(jsonPath, jsonResult.Body);

                // Parse some basic info
                using var doc = System.Text.Json.JsonDocument.Parse(jsonResult.Body);
                var root = doc.RootElement;

                Console.WriteLine("✅ Downloaded curl repository info:");
                Console.WriteLine($"   Name: {root.GetProperty("name").GetString()}");
                Console.WriteLine($"   Description: {root.GetProperty("description").GetString()}");
                Console.WriteLine($"   Stars: {root.GetProperty("stargazers_count").GetInt32():N0}");
                Console.WriteLine($"   Saved to: {jsonPath}\n");
            }

            // Example 6: Resume download simulation
            Console.WriteLine("6. Resume download capability (curl -C -)...");

            var resumeCommand = @"curl -C - -o largefile.zip https://httpbin.org/bytes/10240";
            Console.WriteLine($"   Command: {resumeCommand}");
            Console.WriteLine("   This would resume a partial download if supported by server");
            Console.WriteLine("   Useful for large files or unreliable connections\n");

            // Summary
            Console.WriteLine("=== Download Tips ===");
            Console.WriteLine("• Use -o flag to save directly to file");
            Console.WriteLine("• Use -I flag to check headers before downloading");
            Console.WriteLine("• Use -C - to resume interrupted downloads");
            Console.WriteLine("• Check Content-Type header for file type");
            Console.WriteLine("• Handle binary vs text data appropriately");

            Console.WriteLine($"\n📁 Files downloaded to: {downloadDir}");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}