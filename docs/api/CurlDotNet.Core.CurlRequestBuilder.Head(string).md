#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Head\(string\) Method

Starts a HEAD request: the server sends only the status line and headers, so you can check
existence, size \(`Content-Length`\) or `Last-Modified` without downloading the body\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Head(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Head(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute URL to probe\. Must not be null\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
A new builder whose method is HEAD\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Core.CurlRequestBuilder.Head(string).md#CurlDotNet.Core.CurlRequestBuilder.Head(string).url 'CurlDotNet\.Core\.CurlRequestBuilder\.Head\(string\)\.url') is null\. Pass the URL string; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

### Remarks

The result has [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') and [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers') filled and
            an empty [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'). curl's `-I` also prints the headers as the output; add
            [IncludeHeaders\(bool\)](CurlDotNet.Core.CurlRequestBuilder.IncludeHeaders(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.IncludeHeaders\(bool\)') if you want them in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') too.
            [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') prints this request as `curl -X HEAD URL`.

Example: `var head = await CurlRequestBuilder.Head("https://example.com/big.iso").ExecuteAsync();`
            then `long size = long.Parse(head.GetHeader("Content-Length"));`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-head-request-csharp/README.md

### See Also
- [HeadAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.HeadAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')
- [Get\(string\)](CurlDotNet.Core.CurlRequestBuilder.Get(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)')