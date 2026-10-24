#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithBufferSize\(int\) Method

Sets [BufferSize](CurlDotNet.Core.CurlSettings.BufferSize.md 'CurlDotNet\.Core\.CurlSettings\.BufferSize'), the size of each response\-body read, and returns these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithBufferSize(int size);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithBufferSize(int).size'></a>

`size` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Bytes per read; 0 or less keeps the 80 KB default\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Example: `settings.WithBufferSize(64 * 1024)` reads (and reports progress) every 64 KB.

### See Also
- [BufferSize](CurlDotNet.Core.CurlSettings.BufferSize.md 'CurlDotNet\.Core\.CurlSettings\.BufferSize')