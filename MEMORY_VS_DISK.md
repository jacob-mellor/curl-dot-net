# Memory vs Disk Output in CurlDotNet

Understanding how CurlDotNet handles output is crucial for building efficient applications. This document explains the different ways responses can be stored and accessed.

## Overview

CurlDotNet provides flexibility in how responses are handled. Responses can be:
1. **In memory only** (default) - Fast access, uses RAM
2. **On disk only** - Memory efficient for large files
3. **Both** - Available in memory and saved to disk

## Default Behavior: In Memory

By default, all responses are stored in memory:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");

// Response is immediately available in memory
Console.WriteLine(result.Body);          // Text response
Console.WriteLine(result.BinaryData);    // Binary response
```

This works great for:
- Small to medium responses
- When you need to process data immediately
- API responses (JSON, XML, etc.)

## Saving to Disk with `-o` Flag

When you use the `-o` flag in your curl command, CurlDotNet saves the response to disk **and** keeps it in memory:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -o output.json https://api.example.com/data"
);

// File is saved to disk automatically
Console.WriteLine($"Saved to: {result.OutputFiles[0]}");

// BUT the response is ALSO available in memory
Console.WriteLine(result.Body);  // Still works!
```

**Important:** Using `-o` does NOT remove the response from memory. Both are available.

## Saving to Disk with API

You can also save responses to disk using the `SaveToFile()` method:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");

// Save to disk
result.SaveToFile("output.json");

// Or save with async
await result.SaveToFileAsync("output.json");
```

Again, this saves to disk **and** keeps the data in memory.

## Memory-Only Workflow

For pure memory-based processing:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");

// Process in memory
var data = result.ParseJson<DataModel>();
ProcessData(data);

// Response stays in memory until result object is garbage collected
```

## Disk-Only Workflow

To minimize memory usage for large files:

```csharp
// Download large file to disk
var result = await Curl.ExecuteAsync(
    "curl -o large-file.zip https://example.com/downloads/large-file.zip"
);

// File is on disk at result.OutputFiles[0]
// If you need to process it, read it yourself to control memory usage
using var file = File.OpenRead(result.OutputFiles[0]);
// Process file in chunks to keep memory usage low
```

**Note:** Even with `-o`, the response data is still loaded into memory first, then written to disk. For truly memory-efficient downloads of very large files, you might want to process the stream directly.

## Both Memory and Disk

This is useful when you want to:
1. Save a backup copy
2. Process data now but keep a copy for later
3. Analyze data while preserving the original

```csharp
var result = await Curl.ExecuteAsync(
    "curl -o backup.json https://api.example.com/data"
);

// Process immediately
var data = result.ParseJson<DataModel>();
ProcessData(data);

// But also have a file saved for later
Console.WriteLine($"Backup saved: {result.OutputFiles[0]}");
```

## Large File Handling

For very large files, consider streaming:

```csharp
// Download to disk
var result = await Curl.ExecuteAsync(
    "curl -o large-file.bin https://example.com/large-file.bin"
);

// Process file in chunks to avoid loading entire file into memory
const int bufferSize = 8192;
using var file = File.OpenRead(result.OutputFiles[0]);
var buffer = new byte[bufferSize];
int bytesRead;

while ((bytesRead = await file.ReadAsync(buffer, 0, bufferSize)) > 0)
{
    ProcessChunk(buffer, bytesRead);
}
```

## Binary vs Text

CurlDotNet automatically detects binary vs text content:

```csharp
var textResult = await Curl.ExecuteAsync("curl https://api.example.com/data.json");
// textResult.Body is available
// textResult.BinaryData is null

var binaryResult = await Curl.ExecuteAsync("curl https://example.com/image.png");
// binaryResult.BinaryData is available
// binaryResult.Body is null

// Save appropriately
if (textResult.IsBinary)
{
    File.WriteAllBytes("output.bin", textResult.BinaryData);
}
else
{
    File.WriteAllText("output.txt", textResult.Body);
}

// Or use SaveToFile() which handles both automatically
textResult.SaveToFile("output");  // Automatically determines text vs binary
```

## Best Practices

### For API Responses (Small-Medium)

Use in-memory processing:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");
var data = result.ParseJson<DataModel>();
```

### For Large Downloads

Use `-o` flag and process file separately if needed:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -o download.zip https://example.com/large-file.zip"
);
// File is saved, use FileStream for processing
```

### For Immediate Processing + Backup

Save and process:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -o backup.json https://api.example.com/data"
);
var data = result.ParseJson<DataModel>();  // Process now
// File is already saved for later
```

### For Memory-Constrained Environments

Download to disk only, don't access memory properties:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -o output.bin https://example.com/large-file.bin"
);
// Don't access result.Body or result.BinaryData
// Process result.OutputFiles[0] with FileStream instead
```

## Summary

- **Default**: All responses are in memory (fast access)
- **With `-o` flag**: Saved to disk AND available in memory (both)
- **With `SaveToFile()`**: Saved to disk AND available in memory (both)
- **Memory efficiency**: Use `-o` for large files, process with FileStream for very large files
- **Flexibility**: You can always save, always process in memory, or do both

Choose the approach that best fits your use case. For most API integrations, in-memory processing is fine. For large file downloads, use the `-o` flag and process files directly from disk when needed.

