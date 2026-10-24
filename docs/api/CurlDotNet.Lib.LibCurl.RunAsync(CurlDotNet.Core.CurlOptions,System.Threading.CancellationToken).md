#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.RunAsync\(CurlOptions, CancellationToken\) Method

Runs the request and, like libcurl's `CURLcode` model, reports transfer failures
through [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception') instead of throwing\. Caller cancellation is
the one exception: it surfaces as [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException') so it composes
with normal \.NET cancellation handling\.

```csharp
private System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> RunAsync(CurlDotNet.Core.CurlOptions options, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.RunAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

<a name='CurlDotNet.Lib.LibCurl.RunAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')