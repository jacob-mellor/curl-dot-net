using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

namespace CurlDotNet.Examples.FileOperations
{
    /// <summary>
    /// Example: Download files with progress tracking
    /// </summary>
    public class DownloadFileExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== File Download Example ===\n");

            // Example 1: Simple file download using curl -o
            Console.WriteLine("Example 1: Download file using curl -o");
            var result1 = await Curl.ExecuteAsync(@"
                curl -o example-download.html https://example.com
            ");
            Console.WriteLine($"Download status: {result1.StatusCode}");
            if (File.Exists("example-download.html"))
            {
                var fileInfo = new FileInfo("example-download.html");
                Console.WriteLine($"File saved: {fileInfo.Length} bytes\n");
            }

            // Example 2: Download with progress callback
            Console.WriteLine("Example 2: Download with progress tracking");
            await Curl.DownloadFileAsync(
                "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                "downloads/sample.pdf",
                progress: (percent) => {
                    Console.Write($"\rDownloading: {percent:F1}%");
                    if (percent >= 100)
                        Console.WriteLine(" - Complete!");
                }
            );

            // Example 3: Download with custom headers
            Console.WriteLine("\nExample 3: Download with authentication");
            var result3 = await Curl.GetAsync("https://api.github.com/repos/jacob-mellor/curl-dot-net/zipball/master")
                .WithHeader("Authorization", "token github_pat_fake_token")
                .WithHeader("Accept", "application/vnd.github.v3+json")
                .ExecuteAsync();

            if (result3.BinaryData != null)
            {
                await File.WriteAllBytesAsync("downloads/repo.zip", result3.BinaryData);
                Console.WriteLine($"Repository archive saved: {result3.BinaryData.Length} bytes");
            }

            // Example 4: Resume partial download
            Console.WriteLine("\nExample 4: Resume partial download");
            var partialFile = "downloads/large-file.bin";
            long existingBytes = 0;

            if (File.Exists(partialFile))
            {
                existingBytes = new FileInfo(partialFile).Length;
                Console.WriteLine($"Resuming from byte {existingBytes}");
            }

            var result4 = await Curl.ExecuteAsync($@"
                curl -C {existingBytes} -o {partialFile} https://example.com/large-file.bin
            ");
            Console.WriteLine($"Resume status: {(result4.IsSuccess ? "Success" : "Failed")}");

            // Example 5: Download multiple files
            Console.WriteLine("\nExample 5: Batch download");
            string[] urls = {
                "https://example.com/file1.txt",
                "https://example.com/file2.txt",
                "https://example.com/file3.txt"
            };

            var downloadTasks = new Task[urls.Length];
            for (int i = 0; i < urls.Length; i++)
            {
                int index = i; // Capture for closure
                downloadTasks[i] = Task.Run(async () =>
                {
                    var fileName = $"downloads/file{index + 1}.txt";
                    var result = await Curl.ExecuteAsync($"curl -o {fileName} {urls[index]}");
                    Console.WriteLine($"Downloaded {fileName}: {result.StatusCode}");
                });
            }
            await Task.WhenAll(downloadTasks);
            Console.WriteLine("All downloads complete!");

            // Example 6: Save response to file with metadata
            Console.WriteLine("\nExample 6: Save with metadata");
            var result6 = await Curl.GetAsync("https://api.github.com/users/octocat")
                .WithHeader("Accept", "application/json")
                .ExecuteAsync();

            if (result6.IsSuccess)
            {
                // Save response body
                await File.WriteAllTextAsync("downloads/response.json", result6.Body);

                // Save metadata
                var metadata = new
                {
                    url = "https://api.github.com/users/octocat",
                    statusCode = result6.StatusCode,
                    headers = result6.Headers,
                    downloadTime = DateTime.Now,
                    contentLength = result6.Body.Length
                };

                var metadataJson = System.Text.Json.JsonSerializer.Serialize(metadata, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync("downloads/response.meta.json", metadataJson);
                Console.WriteLine("Response and metadata saved");
            }
        }
    }
}