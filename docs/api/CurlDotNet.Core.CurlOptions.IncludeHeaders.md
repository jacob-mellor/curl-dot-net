#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.IncludeHeaders Property

Puts the response status line and headers in front of the body in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') \(or the
[OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')\), exactly as curl's `-i`/`--include` prints them\.

```csharp
public bool IncludeHeaders { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to include the header block\. Default: false \(the headers are still in [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')\)\.

### Remarks

Every header block of the transfer is included (redirect hops with [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation'), interim
            `100 Continue`), each ending with an empty line. To keep the body clean and write the headers elsewhere use
            [DumpHeaderFile](CurlDotNet.Core.CurlOptions.DumpHeaderFile.md 'CurlDotNet\.Core\.CurlOptions\.DumpHeaderFile') (`-D`).

Example: `curl -i https://example.com/` is `new CurlOptions { Url = "https://example.com/", IncludeHeaders = true }`;
            `result.Body` then starts with `HTTP/1.1 200 OK`.

### See Also
- [HeadOnly](CurlDotNet.Core.CurlOptions.HeadOnly.md 'CurlDotNet\.Core\.CurlOptions\.HeadOnly')
- [DumpHeaderFile](CurlDotNet.Core.CurlOptions.DumpHeaderFile.md 'CurlDotNet\.Core\.CurlOptions\.DumpHeaderFile')
- [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')
- [IncludeHeaders\(bool\)](CurlDotNet.Core.CurlRequestBuilder.IncludeHeaders(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.IncludeHeaders\(bool\)')