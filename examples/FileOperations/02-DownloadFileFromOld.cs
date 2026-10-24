using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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

            // Example 1: Download a small text file and save Body yourself
            Console.WriteLine("1. Downloading a text file...");
            var textResult = await Curl.GetAsync("https://raw.githubusercontent.com/curl/curl/master/README.md");

            if (textResult.IsSuccess)
            {
                var textPath = Path.Combine(downloadDir, "curl-readme.md");
                await File.WriteAllTextAsync(textPath, textResult.Body);
                Console.WriteLine($"Downloaded text file: {textPath}");
                Console.WriteLine($"   Size: {textResult.Body.Length:N0} characters\n");
            }

            // Example 2: Download with curl -o flag (output straight to file)
            Console.WriteLine("2. Using curl -o flag to save directly...");
            var imagePath = Path.Combine(downloadDir, "sample.png");

            // Quote the path in case the directory contains spaces
            var imageResult = await Curl.ExecuteAsync($@"curl -o ""{imagePath}"" https://httpbin.org/image/png");

            if (imageResult.IsSuccess)
            {
                Console.WriteLine($"Saved image to: {imagePath}");
                Console.WriteLine($"   Size on disk: {new FileInfo(imagePath).Length:N0} bytes\n");
            }

            // Example 3: Download several binary payloads into memory
            Console.WriteLine("3. Downloading binary data into memory...");
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

                // Binary responses are in BinaryData; Body is reserved for text
                if (result.IsSuccess && result.IsBinary)
                {
                    Console.WriteLine($"{result.BinaryData.Length:N0} bytes");
                }
            }
            Console.WriteLine();

            // Example 4: Check headers before downloading (curl -I sends a HEAD request)
            Console.WriteLine("4. Download with header inspection...");
            var headerResult = await Curl.ExecuteAsync(
                "curl -I https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf");

            if (headerResult.IsSuccess)
            {
                Console.WriteLine("File headers retrieved:");
                Console.WriteLine($"   Content-Type: {headerResult.GetHeader("Content-Type")}");
                Console.WriteLine($"   Content-Length: {headerResult.GetHeader("Content-Length")}");
                foreach (var header in headerResult.Headers.Take(3))
                {
                    Console.WriteLine($"   {header.Key}: {header.Value}");
                }
                Console.WriteLine();
            }

            // Example 5: Download JSON, save it and parse it
            Console.WriteLine("5. Downloading and parsing JSON data...");
            var jsonResult = await Curl.GetAsync("https://api.github.com/repos/curl/curl");

            if (jsonResult.IsSuccess)
            {
                var jsonPath = Path.Combine(downloadDir, "curl-repo.json");
                await File.WriteAllTextAsync(jsonPath, jsonResult.Body);

                using var doc = JsonDocument.Parse(jsonResult.Body);
                var root = doc.RootElement;

                Console.WriteLine("Downloaded curl repository info:");
                Console.WriteLine($"   Name: {root.GetProperty("name").GetString()}");
                Console.WriteLine($"   Description: {root.GetProperty("description").GetString()}");
                Console.WriteLine($"   Stars: {root.GetProperty("stargazers_count").GetInt32():N0}");
                Console.WriteLine($"   Saved to: {jsonPath}\n");
            }

            // Summary
            Console.WriteLine("=== Download Tips ===");
            Console.WriteLine("- Use -o to save directly to a file");
            Console.WriteLine("- Use -I to check headers before downloading");
            Console.WriteLine("- Read BinaryData (not Body) for binary content");
            Console.WriteLine($"\nFiles downloaded to: {downloadDir}");
        }
    }
}
