#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[IProtocolHandler](CurlDotNet.Core.IProtocolHandler.md 'CurlDotNet\.Core\.IProtocolHandler')

## IProtocolHandler\.ExecuteAsync\(CurlOptions, CancellationToken\) Method

Execute a request with the given options\.

```csharp
System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Core.CurlOptions options, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Core.IProtocolHandler.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

Parsed curl options

<a name='CurlDotNet.Core.IProtocolHandler.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancellation token

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Result of the operation