#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.UseRemoteFileName Property

Saves the body under the file name taken from the URL's last path segment \(`/files/a.zip` becomes
`a.zip`\), like curl's `-O`/`--remote-name`\.

```csharp
public bool UseRemoteFileName { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to name the output file after the URL\. Default: false\.

### Remarks

A URL without a file name in its path is saved as `curl_response`. The file goes into
            [OutputDir](CurlDotNet.Core.CurlOptions.OutputDir.md 'CurlDotNet\.Core\.CurlOptions\.OutputDir') when set, otherwise the current directory. With [RemoteHeaderName](CurlDotNet.Core.CurlOptions.RemoteHeaderName.md 'CurlDotNet\.Core\.CurlOptions\.RemoteHeaderName')
            (`-J`) the server's `Content-Disposition` file name is used instead. Ignored when
            [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile') is set.

Example: `curl -O https://example.com/downloads/tool-1.2.zip` is
            `new CurlOptions { Url = "https://example.com/downloads/tool-1.2.zip", UseRemoteFileName = true }`, which writes
            `tool-1.2.zip`.

### See Also
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')
- [RemoteHeaderName](CurlDotNet.Core.CurlOptions.RemoteHeaderName.md 'CurlDotNet\.Core\.CurlOptions\.RemoteHeaderName')
- [OutputDir](CurlDotNet.Core.CurlOptions.OutputDir.md 'CurlDotNet\.Core\.CurlOptions\.OutputDir')
- [SaveWithRemoteName\(\)](CurlDotNet.Core.CurlRequestBuilder.SaveWithRemoteName().md 'CurlDotNet\.Core\.CurlRequestBuilder\.SaveWithRemoteName\(\)')