#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.AutomaticDecompression Property

Requests and decodes compressed responses \(true\) or turns that off \(false\), like curl's `--compressed`
and `--no-compressed`; applied only when you set it\.

```csharp
public bool AutomaticDecompression { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
Reads true by default, but until you assign it \(or call [WithAutoDecompression\(bool\)](CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithAutoDecompression\(bool\)')\) the
            command decides, so an untouched value never adds `--compressed` to a command that does not have it\.

### Remarks

Once set, it is copied to [Compressed](CurlDotNet.Core.CurlOptions.Compressed.md 'CurlDotNet\.Core\.CurlOptions\.Compressed'): true sends
            `Accept-Encoding: gzip, deflate, br` (gzip and deflate on .NET Standard 2.0/.NET Framework) and decodes
            the body; false sends the request without it and returns the bytes as received.

Example: `new CurlSettings().WithAutoDecompression()` is `curl --compressed`.

### See Also
- [WithAutoDecompression\(bool\)](CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithAutoDecompression\(bool\)')
- [Compressed](CurlDotNet.Core.CurlOptions.Compressed.md 'CurlDotNet\.Core\.CurlOptions\.Compressed')