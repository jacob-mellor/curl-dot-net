using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;

namespace CurlDotNet.Examples.FileOperations
{
    /// <summary>
    /// Example: Download files to disk - with curl -o, Curl.DownloadAsync,
    /// the fluent builder, byte ranges and parallel downloads.
    /// </summary>
    public class DownloadFileExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== File Download Example ===\n");
            Directory.CreateDirectory("downloads");

            // Example 1: Simple file download using curl -o
            Console.WriteLine("Example 1: Download file using curl -o");
            var result1 = await Curl.ExecuteAsync("curl -o downloads/example.html https://httpbin.org/html");
            Console.WriteLine($"Download status: {result1.StatusCode}");
            Console.WriteLine($"File saved: {new FileInfo("downloads/example.html").Length} bytes\n");

            // Example 2: Download with the Curl.DownloadAsync helper (no command syntax needed)
            Console.WriteLine("Example 2: Download with Curl.DownloadAsync");
            var result2 = await Curl.DownloadAsync(
                "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                "downloads/sample.pdf");
            Console.WriteLine($"Status {result2.StatusCode}, wrote: {string.Join(", ", result2.OutputFiles)}");

            // Example 3: Download binary content with custom headers via the fluent builder
            Console.WriteLine("\nExample 3: Download a repository archive");
            var builder = CurlRequestBuilder
                .Get("https://api.github.com/repos/jacob-mellor/curl-dot-net/zipball/master")
                .WithHeader("Accept", "application/vnd.github+json")
                .FollowRedirects(); // GitHub answers with a redirect to the archive

            var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            if (!string.IsNullOrEmpty(githubToken))
            {
                builder = builder.WithBearerToken(githubToken); // optional: higher rate limit
            }

            var result3 = await builder.ExecuteAsync();
            if (result3.IsBinary) // binary responses land in BinaryData, not Body
            {
                await File.WriteAllBytesAsync("downloads/repo.zip", result3.BinaryData);
                Console.WriteLine($"Repository archive saved: {result3.BinaryData.Length:N0} bytes");
            }

            // Example 4: Partial download - just the first 100 bytes with a byte range (-r)
            Console.WriteLine("\nExample 4: Byte-range download");
            var result4 = await Curl.ExecuteAsync(
                "curl -r 0-99 -o downloads/first-100-bytes.bin https://httpbin.org/range/1024");
            Console.WriteLine($"Status {result4.StatusCode} (206 = Partial Content), " +
                              $"{new FileInfo("downloads/first-100-bytes.bin").Length} bytes on disk");

            // Example 5: Download multiple files in parallel
            Console.WriteLine("\nExample 5: Batch download");
            int[] sizes = { 1024, 2048, 4096 };

            var downloads = sizes.Select(size =>
                Curl.DownloadAsync($"https://httpbin.org/bytes/{size}", $"downloads/file-{size}.bin"));
            var results = await Task.WhenAll(downloads);

            foreach (var result in results)
            {
                Console.WriteLine($"Downloaded {result.OutputFiles.FirstOrDefault()}: {result.StatusCode}");
            }

            // Example 6: Save a response to file, plus metadata about it
            Console.WriteLine("\nExample 6: Save with metadata");
            var result6 = await Curl.GetAsync("https://api.github.com/users/octocat");

            if (result6.IsSuccess)
            {
                // SaveToFile writes Body (or BinaryData for binary responses)
                result6.SaveToFile("downloads/response.json");

                var metadata = new
                {
                    url = "https://api.github.com/users/octocat",
                    statusCode = result6.StatusCode,
                    headers = result6.Headers,
                    downloadTime = DateTime.Now,
                    totalMs = result6.Timings?.Total,
                    contentLength = result6.Body.Length
                };

                var metadataJson = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync("downloads/response.meta.json", metadataJson);
                Console.WriteLine("Response and metadata saved");
            }
        }
    }
}
