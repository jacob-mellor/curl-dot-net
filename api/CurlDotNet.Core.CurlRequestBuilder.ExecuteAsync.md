#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.ExecuteAsync Method

| Overloads | |
| :--- | :--- |
| [ExecuteAsync\(CurlSettings, CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(CurlDotNet\.Core\.CurlSettings, System\.Threading\.CancellationToken\)') | Execute the request with custom settings\. |
| [ExecuteAsync\(CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(System\.Threading\.CancellationToken\)') | Execute the request asynchronously\. |

<a name='CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken)'></a>

## CurlRequestBuilder\.ExecuteAsync\(CurlSettings, CancellationToken\) Method

Execute the request with custom settings\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Core.CurlSettings settings, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken).settings'></a>

`settings` [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken)'></a>

## CurlRequestBuilder\.ExecuteAsync\(CancellationToken\) Method

Execute the request asynchronously\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')