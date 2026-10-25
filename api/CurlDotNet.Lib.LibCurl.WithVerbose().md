#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithVerbose\(\) Method

Records curl's `-v` verbose log \(request and response headers, connection notes\) at the start of
every result's [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'), for debugging\.

```csharp
public CurlDotNet.Lib.LibCurl WithVerbose();
```

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

### Remarks

Turn it off before parsing bodies: the log precedes the response text. For a log written to a file use
            `opts => opts.TraceAsciiFile = "trace.txt"` (curl's `--trace-ascii`).

Example: `using var debug = new LibCurl().WithVerbose();` then `Console.WriteLine((await debug.GetAsync(url)).Body);`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md

### See Also
- [Verbose\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Verbose(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Verbose\(bool\)')
- [Verbose](CurlDotNet.Core.CurlOptions.Verbose.md 'CurlDotNet\.Core\.CurlOptions\.Verbose')