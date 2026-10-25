#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.FailOnError Property

Turns an HTTP status of 400 or above into a [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')
\(exit 22\) and discards the error body, like curl's `-f`/`--fail`\.

```csharp
public bool FailOnError { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to fail on HTTP errors\. Default: false, which returns 4xx/5xx responses as a normal
            [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') set\.

### Remarks

The exception carries `StatusCode`, the URL and the response headers; no [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile') body is
            written. To keep the body (for API error details) use [FailWithBody](CurlDotNet.Core.CurlOptions.FailWithBody.md 'CurlDotNet\.Core\.CurlOptions\.FailWithBody') instead. A `416` while
            resuming ([ResumeFrom](CurlDotNet.Core.CurlOptions.ResumeFrom.md 'CurlDotNet\.Core\.CurlOptions\.ResumeFrom')) is not treated as a failure.

Example: `curl -f https://api.example.com/missing` is `new CurlOptions { Url = url, FailOnError = true }`;
            catch `CurlHttpReturnedErrorException ex` and read `ex.StatusCode`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [FailWithBody](CurlDotNet.Core.CurlOptions.FailWithBody.md 'CurlDotNet\.Core\.CurlOptions\.FailWithBody')
- [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode')
- [FailOnError\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FailOnError(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FailOnError\(bool\)')