# Recipe: Upload Files

## 🎯 What You'll Build

Programs that upload files to servers - images, documents, videos, and any file type.

## 🥘 Ingredients

- CurlDotNet package
- A file to upload
- An API endpoint that accepts file uploads
- 10 minutes

## 📖 The Recipe

### Step 1: Simple File Upload

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Uploading file...");

        // Upload a file using multipart form data
        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://httpbin.org/post \
              -F 'file=@document.pdf'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ File uploaded successfully!");
            Console.WriteLine(result.Body);
        }
    }
}
```

### Step 2: Upload with Additional Fields

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class UploadWithMetadata
{
    static async Task Main()
    {
        Console.WriteLine("Uploading file with metadata...");

        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://httpbin.org/post \
              -F 'file=@document.pdf' \
              -F 'title=My Document' \
              -F 'description=Important document' \
              -F 'category=reports'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ File and metadata uploaded!");
        }
    }
}
```

## 🍳 Complete Examples

### Example 1: Upload Image

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class ImageUpload
{
    static async Task Main()
    {
        string imagePath = "photo.jpg";
        string uploadUrl = "https://httpbin.org/post";

        // Verify file exists
        if (!File.Exists(imagePath))
        {
            Console.WriteLine($"✗ File not found: {imagePath}");
            return;
        }

        var fileInfo = new FileInfo(imagePath);
        Console.WriteLine($"Uploading {fileInfo.Name} ({fileInfo.Length:N0} bytes)...");

        var result = await Curl.ExecuteAsync($@"
            curl -X POST {uploadUrl} \
              -F 'file=@{imagePath}' \
              -F 'filename={fileInfo.Name}'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Image uploaded successfully!");
            dynamic response = result.AsJsonDynamic();
            Console.WriteLine($"Server response: {response.files.file}");
        }
        else
        {
            Console.WriteLine($"✗ Upload failed: {result.StatusCode}");
        }
    }
}
```

### Example 2: Upload with Authentication

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class AuthenticatedUpload
{
    static async Task Main()
    {
        string apiToken = Environment.GetEnvironmentVariable("API_TOKEN");
        string filePath = "report.pdf";
        string uploadUrl = "https://api.example.com/upload";

        Console.WriteLine("Uploading file with authentication...");

        var result = await Curl.ExecuteAsync($@"
            curl -X POST {uploadUrl} \
              -H 'Authorization: Bearer {apiToken}' \
              -F 'file=@{filePath}' \
              -F 'access=private'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ File uploaded to protected endpoint!");
            var response = result.ParseJson<UploadResponse>();
            Console.WriteLine($"File ID: {response.FileId}");
            Console.WriteLine($"URL: {response.Url}");
        }
        else if (result.StatusCode == 401)
        {
            Console.WriteLine("✗ Authentication failed. Check your token.");
        }
    }

    public class UploadResponse
    {
        public string FileId { get; set; }
        public string Url { get; set; }
        public long Size { get; set; }
    }
}
```

### Example 3: Upload Multiple Files

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class MultipleFileUpload
{
    static async Task Main()
    {
        string[] files = {
            "document1.pdf",
            "document2.pdf",
            "image.jpg"
        };

        string uploadUrl = "https://httpbin.org/post";

        Console.WriteLine($"Uploading {files.Length} files...\n");

        // Build command with multiple files
        string command = $"curl -X POST {uploadUrl}";
        foreach (var file in files)
        {
            if (File.Exists(file))
            {
                command += $" -F 'files[]=@{file}'";
            }
            else
            {
                Console.WriteLine($"Warning: {file} not found, skipping");
            }
        }

        var result = await Curl.ExecuteAsync(command);

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ All files uploaded successfully!");
            dynamic response = result.AsJsonDynamic();
            Console.WriteLine($"Uploaded files: {response.files}");
        }
    }
}
```

### Example 4: Upload with Progress Tracking

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class UploadWithProgress
{
    static async Task UploadFile(string filePath, string uploadUrl)
    {
        var fileInfo = new FileInfo(filePath);

        Console.WriteLine($"Uploading {fileInfo.Name}...");
        Console.WriteLine($"Size: {fileInfo.Length:N0} bytes");

        var startTime = DateTime.Now;

        var result = await Curl.ExecuteAsync($@"
            curl -X POST {uploadUrl} \
              -F 'file=@{filePath}' \
              --progress-bar
        ");

        var duration = DateTime.Now - startTime;

        if (result.IsSuccess)
        {
            Console.WriteLine($"\n✓ Upload completed in {duration.TotalSeconds:F1}s");

            // Calculate upload speed
            double mbps = (fileInfo.Length / 1024.0 / 1024.0) / duration.TotalSeconds;
            Console.WriteLine($"Average speed: {mbps:F2} MB/s");
        }
    }

    static async Task Main()
    {
        await UploadFile("large-file.zip", "https://httpbin.org/post");
    }
}
```

### Example 5: Upload Binary Data

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class BinaryUpload
{
    static async Task Main()
    {
        string filePath = "data.bin";
        string uploadUrl = "https://httpbin.org/post";

        Console.WriteLine("Uploading binary data...");

        // Upload as binary data (not multipart)
        var result = await Curl.ExecuteAsync($@"
            curl -X POST {uploadUrl} \
              -H 'Content-Type: application/octet-stream' \
              --data-binary '@{filePath}'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Binary data uploaded!");
        }
    }
}
```

### Example 6: Upload JSON with File Reference

```csharp
using System;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

class JsonFileUpload
{
    public class UploadRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsPublic { get; set; }
    }

    static async Task Main()
    {
        string filePath = "document.pdf";
        string uploadUrl = "https://httpbin.org/post";

        var metadata = new UploadRequest
        {
            Title = "Important Document",
            Description = "Quarterly report",
            Category = "Reports",
            IsPublic = false
        };

        string json = JsonSerializer.Serialize(metadata);

        Console.WriteLine("Uploading file with JSON metadata...");

        var result = await Curl.ExecuteAsync($@"
            curl -X POST {uploadUrl} \
              -F 'file=@{filePath}' \
              -F 'metadata={json}'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ File uploaded with metadata!");
        }
    }
}
```

### Example 7: Chunked Upload (Large Files)

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class ChunkedUpload
{
    static async Task UploadInChunks(string filePath, string uploadUrl, int chunkSize = 1024 * 1024)
    {
        var fileInfo = new FileInfo(filePath);
        int totalChunks = (int)Math.Ceiling((double)fileInfo.Length / chunkSize);

        Console.WriteLine($"Uploading {fileInfo.Name} in {totalChunks} chunks...");

        using var fileStream = File.OpenRead(filePath);
        byte[] buffer = new byte[chunkSize];
        int chunkNumber = 0;

        while (true)
        {
            int bytesRead = await fileStream.ReadAsync(buffer, 0, chunkSize);
            if (bytesRead == 0) break;

            chunkNumber++;
            Console.Write($"\rUploading chunk {chunkNumber}/{totalChunks}...");

            // Create temporary chunk file
            string chunkPath = $"chunk_{chunkNumber}.tmp";
            await File.WriteAllBytesAsync(chunkPath, buffer[..bytesRead]);

            // Upload chunk
            var result = await Curl.ExecuteAsync($@"
                curl -X POST {uploadUrl} \
                  -F 'chunk=@{chunkPath}' \
                  -F 'chunkNumber={chunkNumber}' \
                  -F 'totalChunks={totalChunks}' \
                  -F 'filename={fileInfo.Name}'
            ");

            // Clean up chunk file
            File.Delete(chunkPath);

            if (!result.IsSuccess)
            {
                Console.WriteLine($"\n✗ Chunk {chunkNumber} failed: {result.StatusCode}");
                return;
            }
        }

        Console.WriteLine("\n✓ All chunks uploaded successfully!");
    }

    static async Task Main()
    {
        await UploadInChunks("large-video.mp4", "https://httpbin.org/post");
    }
}
```

## 🎨 Variations

### Upload with Custom Content Type

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/upload \
      -F 'file=@image.jpg;type=image/jpeg'
");
```

### Upload with Custom Filename

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/upload \
      -F 'file=@local-file.pdf;filename=remote-name.pdf'
");
```

### Upload from Memory (Data URI)

```csharp
string base64Data = Convert.ToBase64String(fileBytes);
var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/upload \
      -F 'file=data:image/png;base64,{base64Data};filename=image.png'
");
```

## 🐛 Troubleshooting

### Problem: File Not Found

**Solution:**
```csharp
string filePath = "document.pdf";

if (!File.Exists(filePath))
{
    Console.WriteLine($"✗ File not found: {Path.GetFullPath(filePath)}");
    return;
}

var result = await Curl.ExecuteAsync($"curl -X POST https://api.example.com/upload -F 'file=@{filePath}'");
```

### Problem: File Too Large

**Solution:**
```csharp
var fileInfo = new FileInfo("large-file.zip");
long maxSize = 50 * 1024 * 1024; // 50 MB

if (fileInfo.Length > maxSize)
{
    Console.WriteLine($"✗ File too large: {fileInfo.Length:N0} bytes (max: {maxSize:N0})");
    Console.WriteLine("Consider using chunked upload");
    return;
}
```

For more details, see our [file size troubleshooting guide](../../troubleshooting/common-issues.md#file-size-errors).

### Problem: Upload Timeout

**Solution:**
```csharp
// Increase timeout for large files
var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/upload \
      -F 'file=@large-file.mp4' \
      --max-time 3600
");
```

For more details, see our [timeout troubleshooting guide](../../troubleshooting/common-issues.md#timeout-errors).

### Problem: Authentication Required

**Solution:**
```csharp
string apiKey = Environment.GetEnvironmentVariable("API_KEY");

var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/upload \
      -H 'Authorization: Bearer {apiKey}' \
      -F 'file=@document.pdf'
");

if (result.StatusCode == 401)
{
    Console.WriteLine("✗ Authentication failed");
    // Documentation: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#authentication-errors
}
```

## 🎓 Best Practices

### 1. Always Verify File Exists

```csharp
string filePath = "document.pdf";

if (!File.Exists(filePath))
{
    Console.WriteLine($"Error: File not found at {Path.GetFullPath(filePath)}");
    return;
}

// Proceed with upload
```

### 2. Check File Size Before Upload

```csharp
var fileInfo = new FileInfo(filePath);

if (fileInfo.Length == 0)
{
    Console.WriteLine("Error: File is empty");
    return;
}

Console.WriteLine($"Uploading {fileInfo.Length:N0} bytes...");
```

### 3. Handle Upload Errors Gracefully

```csharp
try
{
    var result = await Curl.ExecuteAsync($"curl -X POST {url} -F 'file=@{file}'");

    if (result.IsSuccess)
    {
        Console.WriteLine("✓ Upload successful");
    }
    else
    {
        Console.WriteLine($"✗ Upload failed: {result.StatusCode}");
        Console.WriteLine($"Response: {result.Body}");
    }
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Upload timed out: {ex.Message}");
}
catch (CurlException ex)
{
    Console.WriteLine($"Upload error: {ex.Message}");
}
```

### 4. Use Environment Variables for Credentials

```csharp
// Never hardcode API keys!
string apiKey = Environment.GetEnvironmentVariable("API_KEY")
    ?? throw new Exception("API_KEY environment variable not set");

var result = await Curl.ExecuteAsync($@"
    curl -X POST {url} \
      -H 'Authorization: Bearer {apiKey}' \
      -F 'file=@{file}'
");
```

### 5. Validate Response

```csharp
var result = await Curl.ExecuteAsync($"curl -X POST {url} -F 'file=@{file}'");

if (result.IsSuccess)
{
    try
    {
        var response = result.ParseJson<UploadResponse>();

        if (!string.IsNullOrEmpty(response.FileUrl))
        {
            Console.WriteLine($"✓ File uploaded: {response.FileUrl}");
        }
        else
        {
            Console.WriteLine("Warning: Upload succeeded but no URL returned");
        }
    }
    catch
    {
        Console.WriteLine("Warning: Could not parse response");
    }
}
```

## 🚀 Next Steps

Now that you can upload files:

1. Learn to [Download Files](download-file.md)
2. Try [POST Form Data](post-form.md)
3. Explore [Error Handling](handle-errors.md)
4. Build [API Client](call-api.md)

## 📚 Related Recipes

- [Download Files](download-file.md) - Getting files from servers
- [POST Form Data](post-form.md) - Submitting forms
- [Send JSON](send-json.md) - Sending structured data
- [Handle Errors](handle-errors.md) - Robust error handling

## 🎓 Key Takeaways

- Use `-F 'file=@filepath'` for multipart uploads
- Use `--data-binary '@filepath'` for raw binary uploads
- Always check if file exists before uploading
- Set appropriate timeouts for large files
- Handle authentication with environment variables
- Validate responses after upload
- Consider chunked uploads for very large files
- Clean up temporary files

## 📖 Quick Reference

```csharp
// Simple file upload
await Curl.ExecuteAsync("curl -X POST {url} -F 'file=@document.pdf'");

// With metadata
await Curl.ExecuteAsync(@"
    curl -X POST {url} \
      -F 'file=@document.pdf' \
      -F 'title=My Document'
");

// With authentication
await Curl.ExecuteAsync($@"
    curl -X POST {url} \
      -H 'Authorization: Bearer {token}' \
      -F 'file=@document.pdf'
");

// Binary upload
await Curl.ExecuteAsync(@"
    curl -X POST {url} \
      --data-binary '@file.bin'
");

// Multiple files
await Curl.ExecuteAsync(@"
    curl -X POST {url} \
      -F 'file1=@doc1.pdf' \
      -F 'file2=@doc2.pdf'
");
```

---

**Need help?** Check [Troubleshooting](../../troubleshooting/common-issues.md) | **Have questions?** Ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
