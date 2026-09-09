#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlEngine](CurlDotNet.Core.CurlEngine.md 'CurlDotNet\.Core\.CurlEngine')

## CurlEngine\.ExecuteAsync Method

| Overloads | |
| :--- | :--- |
| [ExecuteAsync\(CurlOptions\)](CurlDotNet.Core.CurlEngine.ExecuteAsync.md#CurlDotNet.Core.CurlEngine.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Core\.CurlEngine\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)') | Execute with parsed options\. |
| [ExecuteAsync\(CurlOptions, CancellationToken\)](CurlDotNet.Core.CurlEngine.ExecuteAsync.md#CurlDotNet.Core.CurlEngine.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlEngine\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)') | Execute with parsed options and cancellation\. |
| [ExecuteAsync\(string\)](CurlDotNet.Core.CurlEngine.ExecuteAsync.md#CurlDotNet.Core.CurlEngine.ExecuteAsync(string) 'CurlDotNet\.Core\.CurlEngine\.ExecuteAsync\(string\)') | Execute a curl command string\. |
| [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Core.CurlEngine.ExecuteAsync.md#CurlDotNet.Core.CurlEngine.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Core\.CurlEngine\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)') | Execute a curl command with custom settings\. |
| [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Core.CurlEngine.ExecuteAsync.md#CurlDotNet.Core.CurlEngine.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlEngine\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)') | Execute a curl command with cancellation support\. |

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(CurlDotNet.Core.CurlOptions)'></a>

## CurlEngine\.ExecuteAsync\(CurlOptions\) Method

Execute with parsed options\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Core.CurlOptions options);
```
#### Parameters

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(CurlDotNet.Core.CurlOptions).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken)'></a>

## CurlEngine\.ExecuteAsync\(CurlOptions, CancellationToken\) Method

Execute with parsed options and cancellation\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Core.CurlOptions options, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(string)'></a>

## CurlEngine\.ExecuteAsync\(string\) Method

Execute a curl command string\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command);
```
#### Parameters

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to execute

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Result of the curl operation

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(string,CurlDotNet.Core.CurlSettings)'></a>

## CurlEngine\.ExecuteAsync\(string, CurlSettings\) Method

Execute a curl command with custom settings\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command, CurlDotNet.Core.CurlSettings settings);
```
#### Parameters

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(string,CurlDotNet.Core.CurlSettings).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(string,CurlDotNet.Core.CurlSettings).settings'></a>

`settings` [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(string,System.Threading.CancellationToken)'></a>

## CurlEngine\.ExecuteAsync\(string, CancellationToken\) Method

Execute a curl command with cancellation support\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(string,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlEngine.ExecuteAsync(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')