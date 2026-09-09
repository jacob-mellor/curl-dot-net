#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.Curl Method

| Overloads | |
| :--- | :--- |
| [Curl\(string\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string) 'CurlDotNet\.DotNetCurl\.Curl\(string\)') | Execute any curl command synchronously \- the main API\. Just paste your curl command\! This method auto\-waits for async operations\. |
| [Curl\(string, int\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string,int) 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)') | Execute any curl command synchronously with timeout\. |

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