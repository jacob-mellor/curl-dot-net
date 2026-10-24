#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.Dispose\(\) Method

Releases this client's connections and handlers; call it \(or use `using`\) when the client is no
longer needed, after its requests have finished\.

```csharp
public void Dispose();
```

Implements [Dispose\(\)](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable.dispose 'System\.IDisposable\.Dispose')

### Remarks

Requests started after [Dispose\(\)](CurlDotNet.Lib.LibCurl.Dispose().md 'CurlDotNet\.Lib\.LibCurl\.Dispose\(\)') fail with [System\.ObjectDisposedException](https://learn.microsoft.com/en-us/dotnet/api/system.objectdisposedexception 'System\.ObjectDisposedException'). Calling it
            more than once is harmless.

Example: `using (var curl = new LibCurl()) { var r = await curl.GetAsync(url); }` disposes the client
            at the end of the block.

### See Also
- [LibCurl\(\)](CurlDotNet.Lib.LibCurl.LibCurl().md 'CurlDotNet\.Lib\.LibCurl\.LibCurl\(\)')