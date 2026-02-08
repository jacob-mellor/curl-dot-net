# Recipe: Download Files

## 🎯 What You'll Build

Programs that download files from the internet - documents, images, videos, archives, and more.

## 🥘 Ingredients

- CurlDotNet package
- Internet connection
- 10 minutes

## 📖 The Recipe

### Step 1: Simple Download

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Downloading file...");

        // Download and save directly to file
        var result = await Curl.ExecuteAsync(
            "curl -o image.jpg https://picsum.photos/400/300"
        );

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ File downloaded successfully!");
            Console.WriteLine("✓ Saved as: image.jpg");
        }
    }
}
```

### Step 2: Download with Progress

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class DownloadWithProgress
{
    static async Task Main()
    {
        Console.WriteLine("Downloading large file with progress...\n");

        var result = await Curl.ExecuteAsync(
            "curl -o file.zip https://example.com/largefile.zip --progress-bar"
        );

        if (result.IsSuccess)
        {
            Console.WriteLine($"\n✓ Downloaded: {result.Body.Length} bytes");
        }
    }
}
```

## 🍳 Complete Examples

### Example 1: Download Image

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class ImageDownloader
{
    static async Task Main()
    {
        string imageUrl = "https://picsum.photos/800/600";
        string outputPath = "downloaded-image.jpg";

        Console.WriteLine($"Downloading from {imageUrl}...");

        try
        {
            var result = await Curl.ExecuteAsync($"curl -o {outputPath} {imageUrl}");

            if (result.IsSuccess)
            {
                var fileInfo = new FileInfo(outputPath);
                Console.WriteLine($"✓ Image downloaded!");
                Console.WriteLine($"✓ Size: {fileInfo.Length:N0} bytes");
                Console.WriteLine($"✓ Location: {fileInfo.FullName}");
            }
            else
            {
                Console.WriteLine($"✗ Download failed: {result.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error: {ex.Message}");
        }
    }
}
```

### Example 2: Download PDF Document

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class PdfDownloader
{
    static async Task Main()
    {
        string pdfUrl = "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf";
        string filename = "document.pdf";

        Console.WriteLine("Downloading PDF document...");

        var result = await Curl.ExecuteAsync($@"
            curl -o {filename} \
                 -L \
                 {pdfUrl}
        ");

        if (result.IsSuccess)
        {
            var fileInfo = new FileInfo(filename);

            Console.WriteLine("✓ PDF downloaded successfully!");
            Console.WriteLine($"  File: {fileInfo.Name}");
            Console.WriteLine($"  Size: {fileInfo.Length:N0} bytes");
            Console.WriteLine($"  Path: {fileInfo.FullName}");

            // Verify it's a PDF
            if (fileInfo.Extension.ToLower() == ".pdf")
            {
                Console.WriteLine("  Type: ✓ Valid PDF file");
            }
        }
    }
}
```

### Example 3: Download to Memory (Then Save)

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class DownloadToMemory
{
    static async Task Main()
    {
        string url = "https://httpbin.org/image/jpeg";

        Console.WriteLine("Downloading to memory...");

        // Download to memory (don't use -o flag)
        var result = await Curl.ExecuteAsync($"curl {url}");

        if (result.IsSuccess)
        {
            Console.WriteLine($"✓ Downloaded {result.Body.Length} bytes");

            // Now save to file
            string filename = "image-from-memory.jpg";
            result.SaveToFile(filename);

            Console.WriteLine($"✓ Saved to {filename}");

            // Or save with custom logic
            await File.WriteAllTextAsync("custom-save.jpg", result.Body);
            Console.WriteLine("✓ Also saved with custom logic");
        }
    }
}
```

### Example 4: Download Multiple Files

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class MultipleDownloader
{
    static async Task Main()
    {
        var downloads = new Dictionary<string, string>
        {
            ["image1.jpg"] = "https://picsum.photos/400/300?random=1",
            ["image2.jpg"] = "https://picsum.photos/400/300?random=2",
            ["image3.jpg"] = "https://picsum.photos/400/300?random=3",
            ["document.pdf"] = "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf"
        };

        Console.WriteLine($"Downloading {downloads.Count} files...\n");

        int success = 0;
        int failed = 0;

        foreach (var download in downloads)
        {
            string filename = download.Key;
            string url = download.Value;

            Console.Write($"Downloading {filename}... ");

            try
            {
                var result = await Curl.ExecuteAsync($"curl -o {filename} {url}");

                if (result.IsSuccess)
                {
                    var fileInfo = new FileInfo(filename);
                    Console.WriteLine($"✓ ({fileInfo.Length:N0} bytes)");
                    success++;
                }
                else
                {
                    Console.WriteLine($"✗ (Status: {result.StatusCode})");
                    failed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ (Error: {ex.Message})");
                failed++;
            }
        }

        Console.WriteLine($"\nResults: {success} succeeded, {failed} failed");
    }
}
```

### Example 5: Resume Interrupted Download

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class ResumableDownload
{
    static async Task DownloadWithResume(string url, string filename)
    {
        var fileInfo = new FileInfo(filename);

        if (fileInfo.Exists)
        {
            Console.WriteLine($"Found partial file ({fileInfo.Length} bytes)");
            Console.WriteLine("Attempting to resume...");

            // Resume download from where it left off
            var result = await Curl.ExecuteAsync($@"
                curl -o {filename} \
                     -C - \
                     {url}
            ");

            if (result.IsSuccess)
            {
                Console.WriteLine("✓ Download resumed and completed!");
            }
        }
        else
        {
            Console.WriteLine("Starting fresh download...");

            var result = await Curl.ExecuteAsync($"curl -o {filename} {url}");

            if (result.IsSuccess)
            {
                Console.WriteLine("✓ Download completed!");
            }
        }
    }

    static async Task Main()
    {
        string url = "https://speed.hetzner.de/100MB.bin"; // Large test file
        string filename = "large-file.bin";

        await DownloadWithResume(url, filename);

        var fileInfo = new FileInfo(filename);
        Console.WriteLine($"Final size: {fileInfo.Length:N0} bytes");
    }
}
```

### Example 6: Download with Authentication

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class AuthenticatedDownload
{
    static async Task Main()
    {
        string apiToken = Environment.GetEnvironmentVariable("API_TOKEN");
        string fileUrl = "https://api.example.com/files/report.pdf";
        string outputFile = "report.pdf";

        Console.WriteLine("Downloading protected file...");

        var result = await Curl.ExecuteAsync($@"
            curl -o {outputFile} \
                 -H 'Authorization: Bearer {apiToken}' \
                 {fileUrl}
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Protected file downloaded successfully!");
        }
        else if (result.StatusCode == 401)
        {
            Console.WriteLine("✗ Authentication failed. Check your token.");
        }
        else if (result.StatusCode == 403)
        {
            Console.WriteLine("✗ Access forbidden. You don't have permission.");
        }
        else
        {
            Console.WriteLine($"✗ Download failed: {result.StatusCode}");
        }
    }
}
```

### Example 7: Download with Custom Headers

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class CustomHeaderDownload
{
    static async Task Main()
    {
        string url = "https://api.example.com/files/data.csv";
        string filename = "data.csv";

        var result = await Curl.ExecuteAsync($@"
            curl -o {filename} \
                 -H 'Accept: text/csv' \
                 -H 'User-Agent: MyApp/1.0' \
                 -H 'X-API-Key: your-api-key' \
                 {url}
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ File downloaded with custom headers!");
        }
    }
}
```

## 🎨 Variations

### Using Different Output Options

```csharp
// Save to specific file
var result = await Curl.ExecuteAsync("curl -o output.jpg https://example.com/image.jpg");

// Save with remote filename
var result = await Curl.ExecuteAsync("curl -O https://example.com/image.jpg");
// Creates: image.jpg

// Save to directory
var result = await Curl.ExecuteAsync("curl -o downloads/file.zip https://example.com/file.zip");
```

### Following Redirects

```csharp
// Many download links redirect - use -L to follow
var result = await Curl.ExecuteAsync(@"
    curl -L -o file.zip https://bit.ly/shortened-link
");
```

### Setting Timeout for Large Files

```csharp
// Increase timeout for large downloads
var result = await Curl.ExecuteAsync(@"
    curl -o large-file.iso \
         --connect-timeout 30 \
         --max-time 3600 \
         https://example.com/large-file.iso
");
```

### Download with Retry

```csharp
// Retry on failure
var result = await Curl.ExecuteAsync(@"
    curl -o file.zip \
         --retry 3 \
         --retry-delay 5 \
         https://example.com/file.zip
");
```

### Limit Download Speed

```csharp
// Limit to 1MB/s
var result = await Curl.ExecuteAsync(@"
    curl -o file.zip \
         --limit-rate 1M \
         https://example.com/file.zip
");
```

## 🔧 Checking File Properties

### Verify Downloaded File

```csharp
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CurlDotNet;

class VerifyDownload
{
    static async Task Main()
    {
        string url = "https://example.com/file.zip";
        string filename = "file.zip";
        string expectedMd5 = "5d41402abc4b2a76b9719d911017c592";

        // Download
        var result = await Curl.ExecuteAsync($"curl -o {filename} {url}");

        if (result.IsSuccess)
        {
            var fileInfo = new FileInfo(filename);

            // Check if file exists
            if (fileInfo.Exists)
            {
                Console.WriteLine($"✓ File exists: {fileInfo.Name}");
                Console.WriteLine($"✓ Size: {fileInfo.Length:N0} bytes");

                // Verify MD5 checksum
                using var md5 = MD5.Create();
                using var stream = File.OpenRead(filename);
                var hash = md5.ComputeHash(stream);
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                if (hashString == expectedMd5)
                {
                    Console.WriteLine("✓ Checksum verified!");
                }
                else
                {
                    Console.WriteLine("✗ Checksum mismatch!");
                    Console.WriteLine($"  Expected: {expectedMd5}");
                    Console.WriteLine($"  Got: {hashString}");
                }
            }
        }
    }
}
```

## 🐛 Troubleshooting

### Problem: File Not Downloaded

**Solution:**
```csharp
var result = await Curl.ExecuteAsync("curl -o file.jpg https://example.com/file.jpg");

// Check result
if (!result.IsSuccess)
{
    Console.WriteLine($"Download failed: {result.StatusCode}");
    Console.WriteLine($"Response: {result.Body}");
}

// Check if file exists
if (File.Exists("file.jpg"))
{
    Console.WriteLine("✓ File created");
}
else
{
    Console.WriteLine("✗ File not created");
}
```

For more details, see our [HTTP error troubleshooting guide](../../troubleshooting/common-issues.md#http-errors).

### Problem: Partial Download

**Solution:**
```csharp
// Check file size
var fileInfo = new FileInfo("file.zip");
Console.WriteLine($"Downloaded: {fileInfo.Length:N0} bytes");

// Resume if incomplete
var result = await Curl.ExecuteAsync(@"
    curl -o file.zip \
         -C - \
         https://example.com/file.zip
");
```

### Problem: Timeout on Large Files

**Solution:**
```csharp
// Increase timeout
var result = await Curl.ExecuteAsync(@"
    curl -o large-file.iso \
         --max-time 7200 \
         https://example.com/large-file.iso
");
```

For more details, see our [timeout troubleshooting guide](../../troubleshooting/common-issues.md#timeout-errors).

### Problem: Permission Denied

**Solution:**
```csharp
// Make sure you have write permission in the directory
string downloadPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "Downloads",
    "file.zip"
);

// Ensure directory exists
Directory.CreateDirectory(Path.GetDirectoryName(downloadPath));

var result = await Curl.ExecuteAsync($"curl -o {downloadPath} https://example.com/file.zip");
```

### Problem: Corrupted Download

**Solution:**
```csharp
// Always verify downloads
string url = "https://example.com/file.zip";
string filename = "file.zip";
long expectedSize = 1024000; // Expected size in bytes

var result = await Curl.ExecuteAsync($"curl -o {filename} {url}");

if (result.IsSuccess)
{
    var fileInfo = new FileInfo(filename);

    if (fileInfo.Length == expectedSize)
    {
        Console.WriteLine("✓ File size correct");
    }
    else
    {
        Console.WriteLine($"✗ Size mismatch! Expected {expectedSize}, got {fileInfo.Length}");
        Console.WriteLine("Retrying download...");

        // Delete and retry
        File.Delete(filename);
        await Curl.ExecuteAsync($"curl -o {filename} {url}");
    }
}
```

## 📊 Download Progress Tracking

### Basic Progress

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class ProgressDownload
{
    static async Task DownloadWithProgress(string url, string filename)
    {
        Console.WriteLine("Starting download...");

        var result = await Curl.ExecuteAsync($@"
            curl -o {filename} \
                 --progress-bar \
                 {url}
        ");

        if (result.IsSuccess)
        {
            var fileInfo = new FileInfo(filename);
            Console.WriteLine($"\n✓ Downloaded {fileInfo.Length:N0} bytes");
        }
    }

    static async Task Main()
    {
        await DownloadWithProgress(
            "https://speed.hetzner.de/10MB.bin",
            "test-file.bin"
        );
    }
}
```

## 🎓 Best Practices

### 1. Always Check If Download Succeeded

```csharp
var result = await Curl.ExecuteAsync("curl -o file.jpg https://example.com/file.jpg");

if (result.IsSuccess)
{
    // Verify file exists and has content
    var fileInfo = new FileInfo("file.jpg");
    if (fileInfo.Exists && fileInfo.Length > 0)
    {
        Console.WriteLine("✓ Download successful");
    }
    else
    {
        Console.WriteLine("✗ File is empty or missing");
    }
}
```

### 2. Handle Network Errors

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -o file.zip https://example.com/file.zip");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine("Download timed out - try again later");
    // Documentation: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#timeout-errors
}
catch (CurlException ex)
{
    Console.WriteLine($"Download failed: {ex.Message}");
    // Documentation: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/
}
```

### 3. Use Appropriate Timeouts

```csharp
// Small files: 30 seconds
var result = await Curl.ExecuteAsync(@"
    curl -o small.txt \
         --max-time 30 \
         https://example.com/small.txt
");

// Large files: 1 hour
var result = await Curl.ExecuteAsync(@"
    curl -o large.iso \
         --max-time 3600 \
         https://example.com/large.iso
");
```

### 4. Clean Up on Failure

```csharp
string filename = "file.zip";

try
{
    var result = await Curl.ExecuteAsync($"curl -o {filename} https://example.com/file.zip");

    if (!result.IsSuccess)
    {
        // Delete partial file
        if (File.Exists(filename))
        {
            File.Delete(filename);
        }
    }
}
catch
{
    // Clean up on exception
    if (File.Exists(filename))
    {
        File.Delete(filename);
    }
    throw;
}
```

### 5. Use Descriptive Filenames

```csharp
// Bad: generic name
var result = await Curl.ExecuteAsync("curl -o file.jpg https://example.com/image.jpg");

// Good: descriptive name with timestamp
string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
string filename = $"report-{timestamp}.pdf";
var result = await Curl.ExecuteAsync($"curl -o {filename} https://example.com/report.pdf");
```

## 🚀 Next Steps

Now that you can download files:

1. Learn to [Upload Files](upload-file.html)
2. Try [POST Form Data](post-form.html)
3. Explore [Error Handling](handle-errors.html)
4. Build [API Client](call-api.html)

## 📚 Related Recipes

- [Simple GET Request](simple-get.html) - Basic HTTP requests
- [Upload Files](upload-file.html) - Sending files to servers
- [Handle Errors](handle-errors.html) - Robust error handling
- [POST Form Data](post-form.html) - Submitting forms

## 🎓 Key Takeaways

- Use `-o filename` to save to a specific file
- Use `-O` to save with remote filename
- Use `-L` to follow redirects
- Use `-C -` to resume interrupted downloads
- Always verify downloaded files
- Set appropriate timeouts for large files
- Handle network errors gracefully
- Clean up partial downloads on failure

## 📖 Quick Reference

```csharp
// Simple download
await Curl.ExecuteAsync("curl -o file.jpg https://example.com/file.jpg");

// Download with redirect
await Curl.ExecuteAsync("curl -L -o file.zip https://example.com/file.zip");

// Resume download
await Curl.ExecuteAsync("curl -C - -o file.zip https://example.com/file.zip");

// Download with timeout
await Curl.ExecuteAsync("curl -o file.pdf --max-time 300 https://example.com/file.pdf");

// Save to memory then file
var result = await Curl.ExecuteAsync("curl https://example.com/file.jpg");
result.SaveToFile("file.jpg");
```

---

**Need help?** Check [Troubleshooting](../../troubleshooting/common-issues.html) | **Have questions?** Ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
