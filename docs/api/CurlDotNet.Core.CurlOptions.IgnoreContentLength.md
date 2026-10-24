#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.IgnoreContentLength Property

Reads the body until the server closes the connection and accepts it even when it is shorter than the announced
`Content-Length`, like curl's `--ignore-content-length`\.

```csharp
public bool IgnoreContentLength { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to ignore a wrong Content\-Length\. Default: false: a short body fails with
            [CurlPartialFileException](CurlDotNet.Exceptions.CurlPartialFileException.md 'CurlDotNet\.Exceptions\.CurlPartialFileException') \(exit 18\)\.

### Remarks

Use it for broken servers that announce the wrong length. Example: `curl --ignore-content-length URL` is
            `new CurlOptions { Url = url, IgnoreContentLength = true }`.

### See Also
- [Raw](CurlDotNet.Core.CurlOptions.Raw.md 'CurlDotNet\.Core\.CurlOptions\.Raw')