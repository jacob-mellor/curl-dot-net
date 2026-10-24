#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.FailWithBody Property

Like [FailOnError](CurlDotNet.Core.CurlOptions.FailOnError.md 'CurlDotNet\.Core\.CurlOptions\.FailOnError') \(exit code 22 for HTTP status 400 and above\) but the response body is still
written to the output and kept on the exception; curl's `--fail-with-body`\.

```csharp
public bool FailWithBody { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to fail on HTTP errors and keep the body\. Default: false\.

### Remarks

The thrown [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException') carries the body in `ResponseBody`
            (handy for API error JSON), and an [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile') is written before it is thrown.

Example: `curl --fail-with-body https://api.example.com/orders/9` is
            `new CurlOptions { Url = url, FailWithBody = true }`; in the catch block read `ex.ResponseBody`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [FailOnError](CurlDotNet.Core.CurlOptions.FailOnError.md 'CurlDotNet\.Core\.CurlOptions\.FailOnError')
- [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')