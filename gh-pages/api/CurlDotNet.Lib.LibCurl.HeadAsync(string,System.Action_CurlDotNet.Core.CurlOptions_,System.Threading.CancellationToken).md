#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.HeadAsync\(string, Action\<CurlOptions\>, CancellationToken\) Method

Perform a HEAD request\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> HeadAsync(string url, System.Action<CurlDotNet.Core.CurlOptions> configure=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to HEAD

<a name='CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Optional configuration action for this request

<a name='CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancellation token

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the request