#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.StatusCode Property

The HTTP status code of the final response \(after any redirects followed with `-L`\), such as 200, 201,
304, 404 or 500 \- curl's `%{http_code}`\.

```csharp
public int StatusCode { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
100\-599 for HTTP; the FTP reply code \(such as 226\) for `ftp://`; 200, or 404 for a missing file, for
            `file://`; 0 when there was no response \(`--skip-existing` skipped the request, or a
            [LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl') call failed \- see [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception')\); 304 when a `-z` time
            condition was not met, like curl\.

### Remarks

An HTTP error status is not an exception: 404 and 500 come back here unless the command used
            `-f`/`--fail` (then [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException'), exit code 22, is thrown). Use
            [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess') for "2xx", or [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)')/[EnsureStatus\(int\)](CurlDotNet.Core.CurlResult.EnsureStatus(int).md 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)') to throw.

Example: `switch (result.StatusCode) { case 200: Use(result.Body); break; case 404: return null; case 401: await RefreshTokenAsync(); break; }`.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess')
- [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)')
- [EnsureStatus\(int\)](CurlDotNet.Core.CurlResult.EnsureStatus(int).md 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)')