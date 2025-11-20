# Tutorial 10: Files and Downloads

Learn how to download files, upload documents, and handle binary data with CurlDotNet.

## Downloading Files

### Simple File Download
```csharp
var curl = new Curl();

// Download to file
await curl.DownloadFileAsync(
    "https://example.com/document.pdf",
    "/path/to/save/document.pdf"
);

Console.WriteLine("File downloaded successfully!");
```

### Download with Progress
```csharp
public async Task DownloadWithProgress(string url, string filePath)
{
    var curl = new Curl();

    // Set up progress tracking
    curl.OnProgress = (downloaded, total) =>
    {
        if (total > 0)
        {
            var percentage = (downloaded * 100.0) / total;
            Console.Write($"\rDownloading: {percentage:F1}% ({downloaded}/{total} bytes)");
        }
    };

    await curl.DownloadFileAsync(url, filePath);
    Console.WriteLine("\nDownload complete!");
}
```

### Download to Memory
```csharp
// For small files - load into memory
var curl = new Curl();
var bytes = await curl.GetBytesAsync("https://example.com/image.jpg");

// Process in memory
using var stream = new MemoryStream(bytes);
using var image = Image.FromStream(stream);
Console.WriteLine($"Image size: {image.Width}x{image.Height}");

// Save if needed
File.WriteAllBytes("image.jpg", bytes);
```

## Uploading Files

### Simple File Upload
```csharp
var curl = new Curl();

// Upload a single file
var result = await curl.UploadFileAsync(
    "https://api.example.com/upload",
    "/path/to/file.pdf",
    "file"  // Form field name
);

if (result.IsSuccess)
{
    Console.WriteLine("Upload successful!");
    var response = JsonSerializer.Deserialize<UploadResponse>(result.Data);
    Console.WriteLine($"File ID: {response.FileId}");
}
```

### Multi-Part Form Upload
```csharp
// Upload file with additional form data
var additionalData = new Dictionary<string, string>
{
    ["description"] = "Monthly report",
    ["category"] = "reports",
    ["tags"] = "finance,2024"
};

var result = await curl.UploadFileAsync(
    "https://api.example.com/documents",
    "/path/to/report.pdf",
    "document",
    additionalData
);
```

### Multiple File Upload
```csharp
public async Task UploadMultipleFiles(List<string> filePaths)
{
    var curl = new Curl();
    var formData = new MultipartFormDataContent();

    foreach (var filePath in filePaths)
    {
        var fileBytes = File.ReadAllBytes(filePath);
        var fileName = Path.GetFileName(filePath);

        var byteContent = new ByteArrayContent(fileBytes);
        byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

        formData.Add(byteContent, "files", fileName);
    }

    // Add other form fields
    formData.Add(new StringContent("batch-upload"), "upload-type");

    var result = await curl.PostAsync("https://api.example.com/batch-upload", formData);
}
```

## Working with Streams

### Streaming Download
```csharp
public async Task StreamDownload(string url, string outputPath)
{
    var curl = new Curl();

    using var response = await curl.GetStreamAsync(url);
    using var fileStream = File.Create(outputPath);

    // Copy in chunks to avoid loading entire file in memory
    var buffer = new byte[8192];
    int bytesRead;
    long totalBytes = 0;

    while ((bytesRead = await response.ReadAsync(buffer, 0, buffer.Length)) > 0)
    {
        await fileStream.WriteAsync(buffer, 0, bytesRead);
        totalBytes += bytesRead;

        Console.Write($"\rDownloaded: {totalBytes:N0} bytes");
    }

    Console.WriteLine("\nDownload complete!");
}
```

### Streaming Upload
```csharp
public async Task StreamUpload(string filePath, string url)
{
    using var fileStream = File.OpenRead(filePath);
    var fileInfo = new FileInfo(filePath);

    var curl = new Curl();
    curl.Headers.Add("Content-Length", fileInfo.Length.ToString());
    curl.Headers.Add("Content-Type", "application/octet-stream");

    var streamContent = new StreamContent(fileStream);
    var result = await curl.PostAsync(url, streamContent);

    if (result.IsSuccess)
    {
        Console.WriteLine("Stream upload successful!");
    }
}
```

## File Type Detection

### Content-Type Detection
```csharp
public string GetContentType(string fileName)
{
    var extension = Path.GetExtension(fileName).ToLowerInvariant();

    return extension switch
    {
        ".pdf" => "application/pdf",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".txt" => "text/plain",
        ".html" => "text/html",
        ".json" => "application/json",
        ".xml" => "application/xml",
        ".zip" => "application/zip",
        ".doc" => "application/msword",
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        ".xls" => "application/vnd.ms-excel",
        ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        _ => "application/octet-stream"
    };
}

// Use in upload
var contentType = GetContentType(filePath);
curl.Headers.Add("Content-Type", contentType);
```

## Resume Downloads

### Partial Content / Range Requests
```csharp
public async Task ResumeDownload(string url, string filePath)
{
    var curl = new Curl();
    long existingLength = 0;

    // Check if partial file exists
    if (File.Exists(filePath))
    {
        var fileInfo = new FileInfo(filePath);
        existingLength = fileInfo.Length;

        // Request remaining bytes
        curl.Headers.Add("Range", $"bytes={existingLength}-");
    }

    var result = await curl.GetAsync(url);

    if (result.StatusCode == HttpStatusCode.PartialContent ||
        result.StatusCode == HttpStatusCode.OK)
    {
        // Append to existing file or create new
        using var fileStream = new FileStream(
            filePath,
            existingLength > 0 ? FileMode.Append : FileMode.Create
        );

        var bytes = Encoding.UTF8.GetBytes(result.Data);
        await fileStream.WriteAsync(bytes, 0, bytes.Length);

        Console.WriteLine("Download completed!");
    }
    else if (result.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
    {
        Console.WriteLine("File already complete!");
    }
}
```

## Binary Data Handling

### Working with Images
```csharp
public async Task<byte[]> ResizeImage(string imageUrl, int maxWidth, int maxHeight)
{
    var curl = new Curl();
    var imageBytes = await curl.GetBytesAsync(imageUrl);

    using var inputStream = new MemoryStream(imageBytes);
    using var image = Image.FromStream(inputStream);

    // Calculate new dimensions
    var ratioX = (double)maxWidth / image.Width;
    var ratioY = (double)maxHeight / image.Height;
    var ratio = Math.Min(ratioX, ratioY);

    var newWidth = (int)(image.Width * ratio);
    var newHeight = (int)(image.Height * ratio);

    // Resize image
    using var resized = new Bitmap(newWidth, newHeight);
    using var graphics = Graphics.FromImage(resized);
    graphics.DrawImage(image, 0, 0, newWidth, newHeight);

    // Convert back to bytes
    using var outputStream = new MemoryStream();
    resized.Save(outputStream, ImageFormat.Jpeg);
    return outputStream.ToArray();
}
```

### Working with ZIP Files
```csharp
public async Task DownloadAndExtractZip(string zipUrl, string extractPath)
{
    var curl = new Curl();

    // Download ZIP to memory
    var zipBytes = await curl.GetBytesAsync(zipUrl);

    // Extract from memory stream
    using var zipStream = new MemoryStream(zipBytes);
    using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

    Directory.CreateDirectory(extractPath);

    foreach (var entry in archive.Entries)
    {
        var destinationPath = Path.Combine(extractPath, entry.FullName);

        // Create directory if needed
        var directory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Extract file
        if (!string.IsNullOrEmpty(entry.Name))
        {
            entry.ExtractToFile(destinationPath, overwrite: true);
            Console.WriteLine($"Extracted: {entry.FullName}");
        }
    }
}
```

## Large File Handling

### Chunked Upload
```csharp
public async Task UploadLargeFile(string filePath, string uploadUrl)
{
    const int chunkSize = 1024 * 1024 * 5; // 5MB chunks
    var fileInfo = new FileInfo(filePath);
    var totalChunks = (int)Math.Ceiling((double)fileInfo.Length / chunkSize);

    using var fileStream = File.OpenRead(filePath);
    var buffer = new byte[chunkSize];

    for (int i = 0; i < totalChunks; i++)
    {
        var bytesRead = await fileStream.ReadAsync(buffer, 0, chunkSize);
        var chunk = new byte[bytesRead];
        Array.Copy(buffer, chunk, bytesRead);

        var curl = new Curl();
        curl.Headers.Add("X-Chunk-Number", i.ToString());
        curl.Headers.Add("X-Total-Chunks", totalChunks.ToString());
        curl.Headers.Add("X-File-Name", Path.GetFileName(filePath));

        var result = await curl.PostAsync(
            $"{uploadUrl}/chunk",
            chunk,
            "application/octet-stream"
        );

        if (!result.IsSuccess)
        {
            throw new Exception($"Failed to upload chunk {i}: {result.Error}");
        }

        var progress = ((i + 1) * 100.0) / totalChunks;
        Console.Write($"\rUploading: {progress:F1}%");
    }

    Console.WriteLine("\nUpload complete!");
}
```

## File Validation

### Checksum Verification
```csharp
public async Task<bool> DownloadWithVerification(string url, string filePath, string expectedMd5)
{
    var curl = new Curl();

    // Download file
    var fileBytes = await curl.GetBytesAsync(url);

    // Calculate MD5
    using var md5 = MD5.Create();
    var hashBytes = md5.ComputeHash(fileBytes);
    var actualMd5 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

    if (actualMd5 == expectedMd5.ToLowerInvariant())
    {
        File.WriteAllBytes(filePath, fileBytes);
        Console.WriteLine("File verified and saved!");
        return true;
    }
    else
    {
        Console.WriteLine($"Checksum mismatch! Expected: {expectedMd5}, Got: {actualMd5}");
        return false;
    }
}
```

## Temporary Files

### Safe Temporary File Handling
```csharp
public async Task ProcessTemporaryDownload(string url)
{
    string tempFile = null;

    try
    {
        // Create unique temp file
        tempFile = Path.GetTempFileName();

        var curl = new Curl();
        await curl.DownloadFileAsync(url, tempFile);

        // Process the file
        var content = File.ReadAllText(tempFile);
        Console.WriteLine($"File contains {content.Length} characters");

        // Do something with the content...
    }
    finally
    {
        // Always clean up temp file
        if (tempFile != null && File.Exists(tempFile))
        {
            File.Delete(tempFile);
        }
    }
}
```

## Error Handling for File Operations

### Robust Download with Retry
```csharp
public async Task<bool> RobustDownload(string url, string filePath, int maxRetries = 3)
{
    var curl = new Curl();

    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        try
        {
            Console.WriteLine($"Download attempt {attempt + 1}...");

            var result = await curl.GetBytesAsync(url);

            // Verify download succeeded
            if (result != null && result.Length > 0)
            {
                File.WriteAllBytes(filePath, result);
                Console.WriteLine("Download successful!");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Attempt {attempt + 1} failed: {ex.Message}");

            if (attempt < maxRetries - 1)
            {
                // Wait before retry with exponential backoff
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            }
        }
    }

    Console.WriteLine("Download failed after all retries");
    return false;
}
```

## Best Practices

1. **Use streams for large files** - Avoid loading entire file in memory
2. **Implement progress tracking** - Show download/upload progress
3. **Verify file integrity** - Use checksums when available
4. **Handle partial downloads** - Support resume for large files
5. **Clean up temp files** - Always delete temporary files
6. **Set appropriate timeouts** - Large files need longer timeouts
7. **Validate file types** - Check extensions and content types
8. **Use chunking for huge files** - Split uploads/downloads
9. **Handle errors gracefully** - Retry on network failures
10. **Respect rate limits** - Don't overwhelm servers

## Summary

File operations with CurlDotNet:
- Simple methods for common download/upload scenarios
- Stream support for large files
- Progress tracking capabilities
- Resume support for interrupted transfers
- Binary data handling for various file types

## What's Next?

Learn about [working with forms and data](11-forms-and-data.md) in the next tutorial.

---

[← Previous: Authentication Basics](09-authentication-basics.md) | [Next: Forms and Data →](11-forms-and-data.md)