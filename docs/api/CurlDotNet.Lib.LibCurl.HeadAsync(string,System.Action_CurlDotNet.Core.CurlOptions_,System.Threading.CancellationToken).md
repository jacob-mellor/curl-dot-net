#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.HeadAsync\(string, Action\<CurlOptions\>, CancellationToken\) Method

Sends a HEAD request: only the status and headers come back \(in [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')\), so you
can check existence, size or `Last-Modified` without downloading the body\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> HeadAsync(string url, System.Action<CurlDotNet.Core.CurlOptions> configure=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute URL to probe\.

<a name='CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Optional callback that adjusts this request's [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions'); null for none\.

<a name='CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Token that cancels the request; `default` for none\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with status code and headers and an empty body; for a transfer
            failure `StatusCode` 0 and the error in [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception')\.

#### Exceptions

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
[cancellationToken](CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md#CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Lib\.LibCurl\.HeadAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)\.cancellationToken') was cancelled\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/tutorials/12\-cancellation\-tokens\.md

### Remarks

Unlike curl's `-I`, the headers are not copied into [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'); read them with
            `result.GetHeader("Content-Length")`.

Example: `var head = await curl.HeadAsync("https://example.com/big.iso");` then
            `Console.WriteLine(head.GetHeader("Content-Length"));`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-head-request-csharp/README.md

### See Also
- [Head\(string\)](CurlDotNet.Core.CurlRequestBuilder.Head(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Head\(string\)')
- [GetAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.GetAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')