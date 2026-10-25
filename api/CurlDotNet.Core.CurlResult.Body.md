#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Body Property

The response as text \- exactly what the curl tool would print to standard output \(the body, plus the
headers with `-i`, the `-v` log, and the `-w` output\)\.

```csharp
public string Body { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The decoded text \(charset from `Content-Type`, UTF\-8 when none\)\. Empty \(not null\) when the body was
            written to a file with `-o`/`-O`, when the response is binary \(the bytes are in [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')\),
            for HEAD requests without `-i`, and for 204 responses\. Null only on a CurlResult you created yourself\.

### Remarks

With a plain `curl URL` this is just the response body, ready for [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)'). Flags
            that make curl print more also change Body: `-i` puts the status line and headers first, `-w '%{http_code}'`
            appends the expanded text, `-v` prepends the request/response log. Use [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers') and
            [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') rather than parsing them out of Body.

Example: `var html = (await Curl.ExecuteAsync("curl -L https://example.com")).Body;`

### See Also
- [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')
- [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles')
- [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
- [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)')