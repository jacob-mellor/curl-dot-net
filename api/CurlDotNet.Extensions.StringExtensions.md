#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](index.md#CurlDotNet.Extensions 'CurlDotNet\.Extensions')

## StringExtensions Class

Extension methods for string to provide syntactic sugar for curl operations\.

```csharp
public static class StringExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; StringExtensions
### Methods

<a name='CurlDotNet.Extensions.StringExtensions.Curl(thisstring)'></a>

## StringExtensions\.Curl\(this string\) Method

Executes curl synchronously \(blocking\)\.

```csharp
public static CurlDotNet.Core.CurlResult Curl(this string command);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.Curl(thisstring).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command string

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The curl result

### Example
var result = "curl https://api\.github\.com"\.Curl\(\);

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

<a name='CurlDotNet.Extensions.StringExtensions.CurlBodyAsync(thisstring)'></a>

## StringExtensions\.CurlBodyAsync\(this string\) Method

Quick one\-liner to get response body as string\.

```csharp
public static System.Threading.Tasks.Task<string> CurlBodyAsync(this string url);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlBodyAsync(thisstring).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to fetch

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The response body

### Example
string json = await "https://api\.github\.com/users/octocat"\.CurlBodyAsync\(\);

<a name='CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string)'></a>

## StringExtensions\.CurlDownloadAsync\(this string, string\) Method

Downloads a file from the URL\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlDownloadAsync(this string url, string outputFile);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to download from

<a name='CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).outputFile'></a>

`outputFile` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path to save to

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The curl result

### Example
await "https://example\.com/file\.pdf"\.CurlDownloadAsync\("local\.pdf"\);

<a name='CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring)'></a>

## StringExtensions\.CurlGetAsync\(this string\) Method

Performs a GET request on the URL\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlGetAsync(this string url);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to GET

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The curl result

### Example
var result = await "https://api\.github\.com/users/octocat"\.CurlGetAsync\(\);

<a name='CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string)'></a>

## StringExtensions\.CurlPostJsonAsync\(this string, string\) Method

Performs a POST request with JSON data\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlPostJsonAsync(this string url, string json);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to POST to

<a name='CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).json'></a>

`json` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The JSON data to send

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The curl result

### Example
var result = await "https://api\.example\.com/users"\.CurlPostJsonAsync\(@"\{""name"":""John""\}"\);