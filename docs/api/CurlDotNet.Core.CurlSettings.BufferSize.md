#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.BufferSize Property

The size, in bytes, of each read of an http/https response body \(and so of each [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress')
report\); applied only when you set it\.

```csharp
public int BufferSize { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Reads 8192 by default, but until you assign it \(or call [WithBufferSize\(int\)](CurlDotNet.Core.CurlSettings.WithBufferSize(int).md 'CurlDotNet\.Core\.CurlSettings\.WithBufferSize\(int\)')\) the engine reads
            in 80 KB chunks\. Values of 0 or less are ignored\.

### Remarks

A smaller buffer gives finer-grained progress reports and `--limit-rate` pacing; a larger one fewer
            reads for big downloads. The body is still collected in memory: to bound memory for very large downloads, write
            to a file with `-o file` and use [MaxFileSize](CurlDotNet.Core.CurlOptions.MaxFileSize.md 'CurlDotNet\.Core\.CurlOptions\.MaxFileSize') (`--max-filesize`) as a guard.

Example: `settings.WithBufferSize(16 * 1024).WithProgress(...)` reports every 16 KB.

### See Also
- [WithBufferSize\(int\)](CurlDotNet.Core.CurlSettings.WithBufferSize(int).md 'CurlDotNet\.Core\.CurlSettings\.WithBufferSize\(int\)')
- [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress')