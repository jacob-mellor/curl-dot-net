#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.CurlAsync Method

| Overloads | |
| :--- | :--- |
| [CurlAsync\(string\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)') | Execute any curl command asynchronously \- the main async API\. Just paste your curl command\! |
| [CurlAsync\(string, CurlSettings\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, CurlDotNet\.Core\.CurlSettings\)') | Execute curl command asynchronously with settings\. |
| [CurlAsync\(string, CancellationToken\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, System\.Threading\.CancellationToken\)') | Execute curl command asynchronously with cancellation support\. |

<a name='CurlDotNet.DotNetCurl.CurlAsync(string)'></a>

## DotNetCurl\.CurlAsync\(string\) Method

Execute any curl command asynchronously \- the main async API\.
Just paste your curl command\!

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlAsync(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlAsync(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Any curl command \- with or without "curl" prefix

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Task with result object containing response data

### Example

```csharp
// Simple GET - async
var response = await DotNetCurl.CurlAsync("curl https://api.example.com/data");
Console.WriteLine(response.Body);

// POST with JSON
var result = await DotNetCurl.CurlAsync(@"
    curl -X POST https://api.example.com/users
    -H 'Content-Type: application/json'
    -d '{""name"":""John""}'
");
```

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings)'></a>

## DotNetCurl\.CurlAsync\(string, CurlSettings\) Method

Execute curl command asynchronously with settings\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlAsync(string command, CurlDotNet.Core.CurlSettings settings);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Any curl command string

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings).settings'></a>

`settings` [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

Curl settings for the operation

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Task with result object

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken)'></a>

## DotNetCurl\.CurlAsync\(string, CancellationToken\) Method

Execute curl command asynchronously with cancellation support\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlAsync(string command, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Any curl command string

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Token to cancel the operation

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Task with result object

### Example

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var response = await DotNetCurl.CurlAsync("curl https://slow-api.com", cts.Token);
```