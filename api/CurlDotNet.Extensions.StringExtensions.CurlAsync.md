#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

## StringExtensions\.CurlAsync Method

| Overloads | |
| :--- | :--- |
| [CurlAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string\)') | Executes a curl command directly from a string\. |
| [CurlAsync\(this string, CancellationToken\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string, System\.Threading\.CancellationToken\)') | Executes a curl command with cancellation support\. |

<a name='CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring)'></a>

## StringExtensions\.CurlAsync\(this string\) Method

Executes a curl command directly from a string\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlAsync(this string command);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command string

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The curl result

### Example
var result = await "curl https://api\.github\.com"\.CurlAsync\(\);
var json = await "https://api\.example\.com/data"\.CurlGetAsync\(\);

<a name='CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken)'></a>

## StringExtensions\.CurlAsync\(this string, CancellationToken\) Method

Executes a curl command with cancellation support\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlAsync(this string command, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')