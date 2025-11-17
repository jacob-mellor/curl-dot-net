---
layout: default
title: Progress Tracking for File Uploads and Downloads in C# with curl
description: Implement real-time progress monitoring, speed calculation, and ETA estimation for file transfers using CurlDotNet
keywords: C# progress tracking, file upload progress, download progress bar, curl progress callback, .NET file transfer
---

# Progress Tracking for Uploads and Downloads

## Monitor File Transfer Progress with curl for C# and .NET

Learn how to implement professional progress tracking with real-time updates, speed calculation, ETA estimation, and progress bars for file transfers using CurlDotNet.

## Why Progress Tracking Matters

- **User Experience**: Show users that something is happening
- **Time Estimates**: Help users plan when large transfers will complete
- **Cancel Options**: Allow users to cancel slow transfers
- **Debugging**: Identify network bottlenecks and performance issues
- **Retry Decisions**: Detect stalled transfers automatically

## Basic Progress Tracking

```csharp
using CurlDotNet;
using System;
using System.IO;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class BasicProgressTracker
{
    private readonly Curl _curl;

    public BasicProgressTracker()
    {
        _curl = new Curl();
    }

    public async Task DownloadWithProgressAsync(string url, string outputPath)
    {
        long totalBytes = 0;
        long downloadedBytes = 0;
        var startTime = DateTime.UtcNow;

        var result = await _curl.GetAsync(url)
            .WithProgressCallback((downloaded, total) =>
            {
                totalBytes = total;
                downloadedBytes = downloaded;

                if (total > 0)
                {
                    var percentage = (downloaded * 100.0) / total;
                    var elapsed = DateTime.UtcNow - startTime;
                    var speed = downloaded / elapsed.TotalSeconds;

                    Console.Write($"\rProgress: {percentage:F1}% " +
                        $"({FormatBytes(downloaded)}/{FormatBytes(total)}) " +
                        $"Speed: {FormatBytes((long)speed)}/s");
                }

                return true; // Continue download
            })
            .DownloadFileAsync(outputPath);

        Console.WriteLine("\nDownload complete!");
    }

    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:F2} {sizes[order]}";
    }
}
```

## Advanced Progress Tracker with ETA

```csharp
using CurlDotNet;
using System.Diagnostics;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class AdvancedProgressTracker
{
    public class ProgressInfo
    {
        public long TotalBytes { get; set; }
        public long TransferredBytes { get; set; }
        public double Percentage { get; set; }
        public double SpeedBytesPerSecond { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public TimeSpan EstimatedTimeRemaining { get; set; }
        public bool IsComplete { get; set; }
        public string Operation { get; set; }
    }

    public class TransferStatistics
    {
        private readonly Queue<(DateTime time, long bytes)> _samples = new();
        private readonly int _sampleWindowSeconds;
        private long _lastBytes;
        private DateTime _startTime;

        public TransferStatistics(int sampleWindowSeconds = 5)
        {
            _sampleWindowSeconds = sampleWindowSeconds;
            _startTime = DateTime.UtcNow;
        }

        public double CalculateSpeed(long currentBytes)
        {
            var now = DateTime.UtcNow;
            _samples.Enqueue((now, currentBytes));

            // Remove old samples outside the window
            while (_samples.Count > 0 &&
                   (now - _samples.Peek().time).TotalSeconds > _sampleWindowSeconds)
            {
                _samples.Dequeue();
            }

            if (_samples.Count < 2)
            {
                return 0;
            }

            var oldest = _samples.First();
            var newest = _samples.Last();
            var timeDiff = (newest.time - oldest.time).TotalSeconds;

            if (timeDiff <= 0)
            {
                return 0;
            }

            var bytesDiff = newest.bytes - oldest.bytes;
            return bytesDiff / timeDiff;
        }

        public TimeSpan EstimateTimeRemaining(long currentBytes, long totalBytes, double currentSpeed)
        {
            if (currentSpeed <= 0 || currentBytes >= totalBytes)
            {
                return TimeSpan.Zero;
            }

            var remainingBytes = totalBytes - currentBytes;
            var secondsRemaining = remainingBytes / currentSpeed;

            return TimeSpan.FromSeconds(secondsRemaining);
        }
    }

    private readonly Curl _curl;
    private readonly IProgress<ProgressInfo> _progressReporter;

    public AdvancedProgressTracker(IProgress<ProgressInfo> progressReporter = null)
    {
        _curl = new Curl();
        _progressReporter = progressReporter;
    }

    public async Task<string> DownloadWithDetailedProgressAsync(
        string url,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        var stats = new TransferStatistics();
        var startTime = DateTime.UtcNow;
        var progressInfo = new ProgressInfo { Operation = "Download" };

        try
        {
            var result = await _curl.GetAsync(url)
                .WithProgressCallback((downloaded, total) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return false; // Cancel the transfer
                    }

                    progressInfo.TransferredBytes = downloaded;
                    progressInfo.TotalBytes = total;
                    progressInfo.Percentage = total > 0 ? (downloaded * 100.0) / total : 0;
                    progressInfo.ElapsedTime = DateTime.UtcNow - startTime;
                    progressInfo.SpeedBytesPerSecond = stats.CalculateSpeed(downloaded);
                    progressInfo.EstimatedTimeRemaining = stats.EstimateTimeRemaining(
                        downloaded, total, progressInfo.SpeedBytesPerSecond);

                    _progressReporter?.Report(progressInfo);

                    return true; // Continue
                })
                .WithCancellationToken(cancellationToken)
                .DownloadFileAsync(outputPath);

            progressInfo.IsComplete = true;
            progressInfo.Percentage = 100;
            _progressReporter?.Report(progressInfo);

            return outputPath;
        }
        catch (OperationCanceledException)
        {
            // Clean up partial file
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            throw;
        }
    }

    public async Task<CurlResult> UploadWithProgressAsync(
        string url,
        string filePath,
        IProgress<ProgressInfo> progress = null)
    {
        var fileInfo = new FileInfo(filePath);
        var totalBytes = fileInfo.Length;
        var stats = new TransferStatistics();
        var startTime = DateTime.UtcNow;

        return await _curl.PostAsync(url)
            .WithFile("file", filePath)
            .WithProgressCallback((uploaded, total) =>
            {
                var progressInfo = new ProgressInfo
                {
                    Operation = "Upload",
                    TransferredBytes = uploaded,
                    TotalBytes = totalBytes,
                    Percentage = (uploaded * 100.0) / totalBytes,
                    ElapsedTime = DateTime.UtcNow - startTime,
                    SpeedBytesPerSecond = stats.CalculateSpeed(uploaded)
                };

                progressInfo.EstimatedTimeRemaining = stats.EstimateTimeRemaining(
                    uploaded, totalBytes, progressInfo.SpeedBytesPerSecond);

                progress?.Report(progressInfo);
                return true;
            })
            .ExecuteAsync();
    }
}
```

## Console Progress Bar Implementation

```csharp
using CurlDotNet;
using System;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class ConsoleProgressBar : IProgress<AdvancedProgressTracker.ProgressInfo>, IDisposable
{
    private readonly int _barWidth;
    private readonly char _progressChar;
    private readonly char _backgroundChar;
    private readonly object _lock = new();
    private int _lastPercent = -1;
    private readonly Timer _spinnerTimer;
    private readonly string[] _spinner = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
    private int _spinnerIndex = 0;

    public ConsoleProgressBar(int barWidth = 40, char progressChar = '█', char backgroundChar = '░')
    {
        _barWidth = barWidth;
        _progressChar = progressChar;
        _backgroundChar = backgroundChar;

        // Animate spinner for indeterminate progress
        _spinnerTimer = new Timer(_ => UpdateSpinner(), null, 0, 100);
    }

    public void Report(AdvancedProgressTracker.ProgressInfo value)
    {
        lock (_lock)
        {
            var percent = (int)value.Percentage;

            // Only update if percentage changed to reduce flicker
            if (percent == _lastPercent && !value.IsComplete)
            {
                return;
            }

            _lastPercent = percent;

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, Console.CursorTop);

            // Draw progress bar
            var filledWidth = (int)(_barWidth * value.Percentage / 100);
            var emptyWidth = _barWidth - filledWidth;

            Console.Write($"{value.Operation}: [");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string(_progressChar, filledWidth));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string(_backgroundChar, emptyWidth));
            Console.ResetColor();
            Console.Write("] ");

            // Show percentage
            Console.Write($"{value.Percentage,5:F1}% ");

            // Show speed
            if (value.SpeedBytesPerSecond > 0)
            {
                Console.Write($"@ {FormatSpeed(value.SpeedBytesPerSecond)} ");
            }

            // Show ETA
            if (value.EstimatedTimeRemaining.TotalSeconds > 0)
            {
                Console.Write($"ETA: {FormatTimeSpan(value.EstimatedTimeRemaining)} ");
            }

            // Show totals
            Console.Write($"({FormatBytes(value.TransferredBytes)}/{FormatBytes(value.TotalBytes)})");

            // Clear rest of line
            Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft - 1));

            if (value.IsComplete)
            {
                Console.WriteLine("\n✅ Complete!");
                Console.CursorVisible = true;
            }
        }
    }

    private void UpdateSpinner()
    {
        if (_lastPercent < 0) // Only show spinner when no progress
        {
            lock (_lock)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"{_spinner[_spinnerIndex]} Waiting for response...");
                _spinnerIndex = (_spinnerIndex + 1) % _spinner.Length;
            }
        }
    }

    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:F2} {sizes[order]}";
    }

    private string FormatSpeed(double bytesPerSecond)
    {
        return $"{FormatBytes((long)bytesPerSecond)}/s";
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalHours >= 1)
        {
            return $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    public void Dispose()
    {
        _spinnerTimer?.Dispose();
        Console.CursorVisible = true;
    }
}
```

## Multi-File Progress Tracker

```csharp
using CurlDotNet;
using System.Collections.Concurrent;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class MultiFileProgressTracker
{
    public class MultiFileProgress
    {
        public int TotalFiles { get; set; }
        public int CompletedFiles { get; set; }
        public int FailedFiles { get; set; }
        public double OverallPercentage { get; set; }
        public ConcurrentDictionary<string, FileProgress> Files { get; set; } = new();
        public TimeSpan TotalElapsed { get; set; }
        public double AverageSpeed { get; set; }
    }

    public class FileProgress
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public long TransferredBytes { get; set; }
        public double Percentage { get; set; }
        public FileStatus Status { get; set; }
        public string Error { get; set; }
        public double Speed { get; set; }
    }

    public enum FileStatus
    {
        Pending, InProgress, Completed, Failed, Cancelled
    }

    private readonly Curl _curl;
    private readonly IProgress<MultiFileProgress> _progressReporter;

    public MultiFileProgressTracker(IProgress<MultiFileProgress> progressReporter = null)
    {
        _curl = new Curl();
        _progressReporter = progressReporter;
    }

    public async Task DownloadMultipleFilesAsync(
        Dictionary<string, string> urlToPathMap,
        int maxParallel = 3,
        CancellationToken cancellationToken = default)
    {
        var multiProgress = new MultiFileProgress
        {
            TotalFiles = urlToPathMap.Count
        };

        var startTime = DateTime.UtcNow;
        var semaphore = new SemaphoreSlim(maxParallel, maxParallel);

        var tasks = urlToPathMap.Select(async kvp =>
        {
            var url = kvp.Key;
            var path = kvp.Value;
            var fileName = Path.GetFileName(path);

            var fileProgress = new FileProgress
            {
                FileName = fileName,
                Status = FileStatus.Pending
            };

            multiProgress.Files[fileName] = fileProgress;

            await semaphore.WaitAsync(cancellationToken);

            try
            {
                fileProgress.Status = FileStatus.InProgress;

                await _curl.GetAsync(url)
                    .WithProgressCallback((downloaded, total) =>
                    {
                        fileProgress.TransferredBytes = downloaded;
                        fileProgress.FileSize = total;
                        fileProgress.Percentage = total > 0 ? (downloaded * 100.0) / total : 0;

                        // Calculate overall progress
                        UpdateOverallProgress(multiProgress, startTime);
                        _progressReporter?.Report(multiProgress);

                        return !cancellationToken.IsCancellationRequested;
                    })
                    .DownloadFileAsync(path);

                fileProgress.Status = FileStatus.Completed;
                multiProgress.CompletedFiles++;
            }
            catch (Exception ex)
            {
                fileProgress.Status = FileStatus.Failed;
                fileProgress.Error = ex.Message;
                multiProgress.FailedFiles++;
            }
            finally
            {
                semaphore.Release();
                UpdateOverallProgress(multiProgress, startTime);
                _progressReporter?.Report(multiProgress);
            }
        });

        await Task.WhenAll(tasks);
    }

    private void UpdateOverallProgress(MultiFileProgress progress, DateTime startTime)
    {
        var totalBytes = progress.Files.Values.Sum(f => f.FileSize);
        var transferredBytes = progress.Files.Values.Sum(f => f.TransferredBytes);

        if (totalBytes > 0)
        {
            progress.OverallPercentage = (transferredBytes * 100.0) / totalBytes;
        }

        progress.TotalElapsed = DateTime.UtcNow - startTime;

        if (progress.TotalElapsed.TotalSeconds > 0)
        {
            progress.AverageSpeed = transferredBytes / progress.TotalElapsed.TotalSeconds;
        }
    }
}
```

## Real-World Example: YouTube-like Video Upload

```csharp
using CurlDotNet;
using System.Security.Cryptography;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class VideoUploadService
{
    private readonly Curl _curl;
    private readonly string _apiEndpoint;

    public class VideoUploadProgress
    {
        public string VideoId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public UploadStage Stage { get; set; }
        public double Percentage { get; set; }
        public string Status { get; set; }
        public TimeSpan EstimatedTime { get; set; }
        public double UploadSpeed { get; set; }
        public bool CanPause { get; set; }
        public bool CanResume { get; set; }
    }

    public enum UploadStage
    {
        Preparing,
        Hashing,
        Uploading,
        Processing,
        Completed,
        Failed
    }

    public VideoUploadService(string apiEndpoint)
    {
        _apiEndpoint = apiEndpoint;
        _curl = new Curl(apiEndpoint);
    }

    public async Task<string> UploadVideoAsync(
        string videoPath,
        string title,
        string description,
        IProgress<VideoUploadProgress> progress = null,
        CancellationToken cancellationToken = default)
    {
        var fileInfo = new FileInfo(videoPath);
        var videoId = Guid.NewGuid().ToString();
        var uploadProgress = new VideoUploadProgress
        {
            VideoId = videoId,
            FileName = fileInfo.Name,
            FileSize = fileInfo.Length,
            Stage = UploadStage.Preparing
        };

        try
        {
            // Stage 1: Calculate file hash for deduplication
            uploadProgress.Stage = UploadStage.Hashing;
            uploadProgress.Status = "Calculating file hash...";
            progress?.Report(uploadProgress);

            var fileHash = await CalculateFileHashAsync(videoPath, (percent) =>
            {
                uploadProgress.Percentage = percent * 0.1; // 0-10%
                progress?.Report(uploadProgress);
            });

            // Check if file already exists
            var existsResponse = await _curl.GetAsync($"/videos/exists/{fileHash}");
            if (existsResponse.StatusCode == 200)
            {
                uploadProgress.Stage = UploadStage.Completed;
                uploadProgress.Status = "Video already uploaded";
                uploadProgress.Percentage = 100;
                progress?.Report(uploadProgress);
                return existsResponse.Body; // Return existing video ID
            }

            // Stage 2: Upload video file
            uploadProgress.Stage = UploadStage.Uploading;
            uploadProgress.CanPause = true;
            var stats = new AdvancedProgressTracker.TransferStatistics();
            var uploadStartTime = DateTime.UtcNow;

            var uploadResult = await _curl.PostAsync($"/videos/upload")
                .WithFormData(new Dictionary<string, string>
                {
                    ["title"] = title,
                    ["description"] = description,
                    ["hash"] = fileHash,
                    ["size"] = fileInfo.Length.ToString()
                })
                .WithFile("video", videoPath)
                .WithProgressCallback((uploaded, total) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        uploadProgress.Status = "Upload cancelled";
                        return false;
                    }

                    // Upload is 10-90% of total progress
                    uploadProgress.Percentage = 10 + (uploaded * 80.0 / total);
                    uploadProgress.UploadSpeed = stats.CalculateSpeed(uploaded);
                    uploadProgress.EstimatedTime = stats.EstimateTimeRemaining(
                        uploaded, total, uploadProgress.UploadSpeed);
                    uploadProgress.Status = $"Uploading... {FormatBytes(uploaded)}/{FormatBytes(total)}";

                    progress?.Report(uploadProgress);
                    return true;
                })
                .ExecuteAsync();

            if (uploadResult.StatusCode != 200)
            {
                throw new Exception($"Upload failed: {uploadResult.Body}");
            }

            var uploadResponse = System.Text.Json.JsonSerializer.Deserialize<UploadResponse>(
                uploadResult.Body);

            // Stage 3: Server-side processing
            uploadProgress.Stage = UploadStage.Processing;
            uploadProgress.CanPause = false;
            uploadProgress.Status = "Processing video...";

            await PollProcessingStatusAsync(uploadResponse.VideoId, (processingPercent) =>
            {
                // Processing is 90-100% of total progress
                uploadProgress.Percentage = 90 + (processingPercent * 0.1);
                uploadProgress.Status = GetProcessingStatus(processingPercent);
                progress?.Report(uploadProgress);
            });

            uploadProgress.Stage = UploadStage.Completed;
            uploadProgress.Percentage = 100;
            uploadProgress.Status = "Upload complete!";
            progress?.Report(uploadProgress);

            return uploadResponse.VideoId;
        }
        catch (Exception ex)
        {
            uploadProgress.Stage = UploadStage.Failed;
            uploadProgress.Status = $"Upload failed: {ex.Message}";
            progress?.Report(uploadProgress);
            throw;
        }
    }

    private async Task<string> CalculateFileHashAsync(
        string filePath,
        Action<double> progressCallback)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);

        var buffer = new byte[8192];
        var totalBytes = stream.Length;
        var processedBytes = 0L;
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            md5.TransformBlock(buffer, 0, bytesRead, null, 0);
            processedBytes += bytesRead;
            progressCallback(processedBytes / (double)totalBytes);
        }

        md5.TransformFinalBlock(buffer, 0, 0);
        return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
    }

    private async Task PollProcessingStatusAsync(
        string videoId,
        Action<double> progressCallback)
    {
        var pollInterval = TimeSpan.FromSeconds(2);
        var maxPolls = 300; // 10 minutes max
        var polls = 0;

        while (polls < maxPolls)
        {
            var statusResponse = await _curl.GetAsync($"/videos/status/{videoId}");
            var status = System.Text.Json.JsonSerializer.Deserialize<ProcessingStatus>(
                statusResponse.Body);

            if (status.IsComplete)
            {
                progressCallback(100);
                break;
            }

            progressCallback(status.Percentage);
            await Task.Delay(pollInterval);
            polls++;
        }
    }

    private string GetProcessingStatus(double percent)
    {
        return percent switch
        {
            < 25 => "Analyzing video...",
            < 50 => "Generating thumbnails...",
            < 75 => "Encoding for streaming...",
            < 100 => "Finalizing...",
            _ => "Processing complete"
        };
    }

    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:F1} {sizes[order]}";
    }

    private class UploadResponse
    {
        public string VideoId { get; set; }
        public string Status { get; set; }
    }

    private class ProcessingStatus
    {
        public double Percentage { get; set; }
        public bool IsComplete { get; set; }
        public string Stage { get; set; }
    }
}
```

## Usage Examples

### Basic Console Progress

```csharp
using CurlDotNet;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

var downloader = new BasicProgressTracker();
await downloader.DownloadWithProgressAsync(
    "https://example.com/bigfile.zip",
    "downloads/bigfile.zip");
```

### WPF/WinForms Progress

```csharp
// In your form or window
private async void DownloadButton_Click(object sender, EventArgs e)
{
    var progress = new Progress<AdvancedProgressTracker.ProgressInfo>(info =>
    {
        // Update UI on UI thread
        progressBar.Value = (int)info.Percentage;
        statusLabel.Text = $"{info.Percentage:F1}% - {info.EstimatedTimeRemaining}";
        speedLabel.Text = $"Speed: {FormatBytes((long)info.SpeedBytesPerSecond)}/s";
    });

    var tracker = new AdvancedProgressTracker(progress);
    await tracker.DownloadWithDetailedProgressAsync(
        "https://example.com/file.zip",
        "downloads/file.zip");
}
```

### ASP.NET Core SignalR Progress

```csharp
public class DownloadHub : Hub
{
    private readonly AdvancedProgressTracker _tracker;

    public async Task DownloadFile(string url, string fileName)
    {
        var progress = new Progress<AdvancedProgressTracker.ProgressInfo>(async info =>
        {
            await Clients.Caller.SendAsync("DownloadProgress", new
            {
                fileName,
                percentage = info.Percentage,
                speed = info.SpeedBytesPerSecond,
                eta = info.EstimatedTimeRemaining.ToString()
            });
        });

        await _tracker.DownloadWithDetailedProgressAsync(
            url,
            Path.Combine("downloads", fileName),
            Context.ConnectionAborted);
    }
}
```

## Best Practices

### 1. Don't Update Too Frequently
```csharp
// Throttle updates to prevent UI freezing
private DateTime _lastUpdate = DateTime.MinValue;

if ((DateTime.Now - _lastUpdate).TotalMilliseconds > 100)
{
    _lastUpdate = DateTime.Now;
    UpdateProgress();
}
```

### 2. Handle Cancellation Properly
```csharp
// Always clean up partial files on cancellation
try
{
    await DownloadAsync(url, path, cancellationToken);
}
catch (OperationCanceledException)
{
    if (File.Exists(path))
        File.Delete(path);
    throw;
}
```

### 3. Provide Meaningful Information
```csharp
// Good: Specific, actionable information
"Downloading update: 45.2 MB of 120.5 MB (2.3 MB/s) - 32 seconds remaining"

// Bad: Generic progress
"Downloading... 45%"
```

## Troubleshooting

### Progress Not Updating
- Check if callback is registered
- Verify server sends Content-Length header
- Ensure UI updates on correct thread

### Inaccurate Speed Calculations
- Use rolling window for averaging
- Filter out outlier samples
- Account for connection establishment time

### ETA Fluctuating Wildly
- Use exponential smoothing
- Increase sample window size
- Cap maximum ETA display

## Key Takeaways

- ✅ Always show progress for long-running operations
- ✅ Calculate accurate speed and ETA
- ✅ Allow cancellation of transfers
- ✅ Handle partial downloads gracefully
- ✅ Update UI on appropriate thread
- ✅ Provide meaningful status messages
- ✅ Clean up resources on cancellation

## Related Examples

- [File Downloads](../beginner/download-file)
- [File Uploads](../beginner/upload-file)
- [Streaming Data](../advanced/streaming-data)
- [Cancellation Tokens](../../tutorials/cancellation-tokens)

---

*Part of the CurlDotNet Cookbook - Professional patterns for C# and .NET developers*