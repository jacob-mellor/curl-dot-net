#### [CurlDotNet](index.md 'index')
### [CurlDotNet](index.md#CurlDotNet 'CurlDotNet')

## DotNetCurl Class

DotNetCurl \- Alternative API entry point for curl commands\.
The killer feature: Copy and paste any curl command and it works\!

```csharp
public static class DotNetCurl
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; DotNetCurl

### Example

```csharp
// 🔥 Synchronous execution (auto-waits)
var response = DotNetCurl.Curl("curl https://api.github.com/users/octocat");
Console.WriteLine(response.Body);

// 🚀 Async execution
var response = await DotNetCurl.CurlAsync("curl https://api.github.com/users/octocat");
Console.WriteLine(response.Body);

// 📮 POST request
var result = DotNetCurl.Curl(@"
    curl -X POST https://api.example.com/users
    -H 'Content-Type: application/json'
    -d '{""name"":""John"",""email"":""john@example.com""}'
");

// 🔄 Multiple commands
var results = await DotNetCurl.CurlManyAsync(new[] {
    "curl https://api.example.com/users",
    "curl https://api.example.com/posts"
});
```

### Remarks

🚀 <b>THE KILLER FEATURE:</b> Copy any curl command from anywhere - it just works!

DotNetCurl provides an alternative naming to the Curl class.

Use DotNetCurl.Curl() for synchronous or DotNetCurl.CurlAsync() for async operations.

Sponsored by IronSoftware - creators of IronPDF, IronOCR, IronXL, and more.
### Properties

<a name='CurlDotNet.DotNetCurl.DefaultConnectTimeoutSeconds'></a>

## DotNetCurl\.DefaultConnectTimeoutSeconds Property

Get or set global connection timeout \(like \-\-connect\-timeout\)\.

```csharp
public static int DefaultConnectTimeoutSeconds { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.DotNetCurl.DefaultFollowRedirects'></a>

## DotNetCurl\.DefaultFollowRedirects Property

Get or set whether to follow redirects by default \(like \-L\)\.

```csharp
public static bool DefaultFollowRedirects { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.DotNetCurl.DefaultInsecure'></a>

## DotNetCurl\.DefaultInsecure Property

Get or set whether to ignore SSL errors by default \(like \-k\)\.
WARNING: Only use this for development/testing\!

```csharp
public static bool DefaultInsecure { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.DotNetCurl.DefaultMaxTimeSeconds'></a>

## DotNetCurl\.DefaultMaxTimeSeconds Property

Get or set global maximum time for all curl operations \(like \-\-max\-time\)\.

```csharp
public static int DefaultMaxTimeSeconds { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')
### Methods

<a name='CurlDotNet.DotNetCurl.Curl(string)'></a>

## DotNetCurl\.Curl\(string\) Method

Execute any curl command synchronously \- the main API\.
Just paste your curl command\! This method auto\-waits for async operations\.

```csharp
public static CurlDotNet.Core.CurlResult Curl(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Curl(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Any curl command \- with or without "curl" prefix

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
Result object with response data, headers, and status

### Example

```csharp
// Simple GET - synchronous
var response = DotNetCurl.Curl("curl https://api.example.com/data");
Console.WriteLine(response.Body);

// Works without "curl" prefix
var data = DotNetCurl.Curl("https://api.example.com/data");
```

<a name='CurlDotNet.DotNetCurl.Curl(string,int)'></a>

## DotNetCurl\.Curl\(string, int\) Method

Execute any curl command synchronously with timeout\.

```csharp
public static CurlDotNet.Core.CurlResult Curl(string command, int timeoutSeconds);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Curl(string,int).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Any curl command string

<a name='CurlDotNet.DotNetCurl.Curl(string,int).timeoutSeconds'></a>

`timeoutSeconds` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Timeout in seconds

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
Result object with response data

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

<a name='CurlDotNet.DotNetCurl.CurlMany(string[])'></a>

## DotNetCurl\.CurlMany\(string\[\]\) Method

Execute multiple curl commands in parallel synchronously\.

```csharp
public static CurlDotNet.Core.CurlResult[] CurlMany(params string[] commands);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlMany(string[]).commands'></a>

`commands` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

Array of curl commands

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')  
Array of results

### Example

```csharp
var results = DotNetCurl.CurlMany(new[] {
    "curl https://api.example.com/users",
    "curl https://api.example.com/posts"
});
foreach (var result in results)
{
    Console.WriteLine(result.Body);
}
```

<a name='CurlDotNet.DotNetCurl.CurlManyAsync(string[])'></a>

## DotNetCurl\.CurlManyAsync\(string\[\]\) Method

Execute multiple curl commands in parallel asynchronously\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult[]> CurlManyAsync(params string[] commands);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlManyAsync(string[]).commands'></a>

`commands` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

Array of curl commands

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Task with array of results

### Example

```csharp
var results = await DotNetCurl.CurlManyAsync(new[] {
    "curl https://api.example.com/users",
    "curl https://api.example.com/posts",
    "curl https://api.example.com/comments"
});
```

<a name='CurlDotNet.DotNetCurl.Download(string,string)'></a>

## DotNetCurl\.Download\(string, string\) Method

Download a file synchronously\.

```csharp
public static CurlDotNet.Core.CurlResult Download(string url, string outputPath);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Download(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

URL to download from

<a name='CurlDotNet.DotNetCurl.Download(string,string).outputPath'></a>

`outputPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path to save the file

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
Result object

<a name='CurlDotNet.DotNetCurl.DownloadAsync(string,string)'></a>

## DotNetCurl\.DownloadAsync\(string, string\) Method

Download a file asynchronously\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> DownloadAsync(string url, string outputPath);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.DownloadAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

URL to download from

<a name='CurlDotNet.DotNetCurl.DownloadAsync(string,string).outputPath'></a>

`outputPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path to save the file

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Task with result object

<a name='CurlDotNet.DotNetCurl.Get(string)'></a>

## DotNetCurl\.Get\(string\) Method

Quick GET request synchronously\.

```csharp
public static CurlDotNet.Core.CurlResult Get(string url);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Get(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

URL to GET

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
Result object

<a name='CurlDotNet.DotNetCurl.GetAsync(string)'></a>

## DotNetCurl\.GetAsync\(string\) Method

Quick GET request asynchronously\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> GetAsync(string url);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.GetAsync(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

URL to GET

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Task with result object

<a name='CurlDotNet.DotNetCurl.Post(string,string)'></a>

## DotNetCurl\.Post\(string, string\) Method

Quick POST request synchronously\.

```csharp
public static CurlDotNet.Core.CurlResult Post(string url, string data);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Post(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

URL to POST to

<a name='CurlDotNet.DotNetCurl.Post(string,string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Data to POST

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
Result object

<a name='CurlDotNet.DotNetCurl.PostAsync(string,string)'></a>

## DotNetCurl\.PostAsync\(string, string\) Method

Quick POST request asynchronously\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostAsync(string url, string data);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.PostAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

URL to POST to

<a name='CurlDotNet.DotNetCurl.PostAsync(string,string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Data to POST

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Task with result object

<a name='CurlDotNet.DotNetCurl.PostJson(string,object)'></a>

## DotNetCurl\.PostJson\(string, object\) Method

Quick POST with JSON synchronously\.

```csharp
public static CurlDotNet.Core.CurlResult PostJson(string url, object data);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.PostJson(string,object).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

URL to POST to

<a name='CurlDotNet.DotNetCurl.PostJson(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Object to serialize as JSON

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
Result object

<a name='CurlDotNet.DotNetCurl.PostJsonAsync(string,object)'></a>

## DotNetCurl\.PostJsonAsync\(string, object\) Method

Quick POST with JSON asynchronously\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostJsonAsync(string url, object data);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.PostJsonAsync(string,object).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

URL to POST to

<a name='CurlDotNet.DotNetCurl.PostJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Object to serialize as JSON

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Task with result object

<a name='CurlDotNet.DotNetCurl.ToFetch(string)'></a>

## DotNetCurl\.ToFetch\(string\) Method

Convert curl command to JavaScript fetch code\.

```csharp
public static string ToFetch(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.ToFetch(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Curl command to convert

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
JavaScript fetch code

<a name='CurlDotNet.DotNetCurl.ToHttpClient(string)'></a>

## DotNetCurl\.ToHttpClient\(string\) Method

Convert curl command to equivalent HttpClient C\# code\.

```csharp
public static string ToHttpClient(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.ToHttpClient(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Curl command to convert

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
C\# HttpClient code

<a name='CurlDotNet.DotNetCurl.ToPython(string)'></a>

## DotNetCurl\.ToPython\(string\) Method

Convert curl command to Python requests code\.

```csharp
public static string ToPython(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.ToPython(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Curl command to convert

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Python requests code

<a name='CurlDotNet.DotNetCurl.Validate(string)'></a>

## DotNetCurl\.Validate\(string\) Method

Validate a curl command without executing\.

```csharp
public static CurlDotNet.Core.ValidationResult Validate(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Validate(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Curl command to validate

#### Returns
[ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult')  
Validation result