#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet')

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

| Properties | |
| :--- | :--- |
| [DefaultConnectTimeoutSeconds](CurlDotNet.DotNetCurl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultConnectTimeoutSeconds') | Get or set global connection timeout \(like \-\-connect\-timeout\)\. |
| [DefaultFollowRedirects](CurlDotNet.DotNetCurl.DefaultFollowRedirects.md 'CurlDotNet\.DotNetCurl\.DefaultFollowRedirects') | Get or set whether to follow redirects by default \(like \-L\)\. |
| [DefaultInsecure](CurlDotNet.DotNetCurl.DefaultInsecure.md 'CurlDotNet\.DotNetCurl\.DefaultInsecure') | Get or set whether to ignore SSL errors by default \(like \-k\)\. WARNING: Only use this for development/testing\! |
| [DefaultMaxTimeSeconds](CurlDotNet.DotNetCurl.DefaultMaxTimeSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultMaxTimeSeconds') | Get or set global maximum time for all curl operations \(like \-\-max\-time\)\. |

| Methods | |
| :--- | :--- |
| [Curl\(string\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string) 'CurlDotNet\.DotNetCurl\.Curl\(string\)') | Execute any curl command synchronously \- the main API\. Just paste your curl command\! This method auto\-waits for async operations\. |
| [Curl\(string, int\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string,int) 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)') | Execute any curl command synchronously with timeout\. |
| [CurlAsync\(string\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)') | Execute any curl command asynchronously \- the main async API\. Just paste your curl command\! |
| [CurlAsync\(string, CurlSettings\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, CurlDotNet\.Core\.CurlSettings\)') | Execute curl command asynchronously with settings\. |
| [CurlAsync\(string, CancellationToken\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, System\.Threading\.CancellationToken\)') | Execute curl command asynchronously with cancellation support\. |
| [CurlMany\(string\[\]\)](CurlDotNet.DotNetCurl.CurlMany(string[]).md 'CurlDotNet\.DotNetCurl\.CurlMany\(string\[\]\)') | Execute multiple curl commands in parallel synchronously\. |
| [CurlManyAsync\(string\[\]\)](CurlDotNet.DotNetCurl.CurlManyAsync(string[]).md 'CurlDotNet\.DotNetCurl\.CurlManyAsync\(string\[\]\)') | Execute multiple curl commands in parallel asynchronously\. |
| [Download\(string, string\)](CurlDotNet.DotNetCurl.Download(string,string).md 'CurlDotNet\.DotNetCurl\.Download\(string, string\)') | Download a file synchronously\. |
| [DownloadAsync\(string, string\)](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)') | Download a file asynchronously\. |
| [Get\(string\)](CurlDotNet.DotNetCurl.Get(string).md 'CurlDotNet\.DotNetCurl\.Get\(string\)') | Quick GET request synchronously\. |
| [GetAsync\(string\)](CurlDotNet.DotNetCurl.GetAsync(string).md 'CurlDotNet\.DotNetCurl\.GetAsync\(string\)') | Quick GET request asynchronously\. |
| [Post\(string, string\)](CurlDotNet.DotNetCurl.Post(string,string).md 'CurlDotNet\.DotNetCurl\.Post\(string, string\)') | Quick POST request synchronously\. |
| [PostAsync\(string, string\)](CurlDotNet.DotNetCurl.PostAsync(string,string).md 'CurlDotNet\.DotNetCurl\.PostAsync\(string, string\)') | Quick POST request asynchronously\. |
| [PostJson\(string, object\)](CurlDotNet.DotNetCurl.PostJson(string,object).md 'CurlDotNet\.DotNetCurl\.PostJson\(string, object\)') | Quick POST with JSON synchronously\. |
| [PostJsonAsync\(string, object\)](CurlDotNet.DotNetCurl.PostJsonAsync(string,object).md 'CurlDotNet\.DotNetCurl\.PostJsonAsync\(string, object\)') | Quick POST with JSON asynchronously\. |
| [ToFetch\(string\)](CurlDotNet.DotNetCurl.ToFetch(string).md 'CurlDotNet\.DotNetCurl\.ToFetch\(string\)') | Convert curl command to JavaScript fetch code\. |
| [ToHttpClient\(string\)](CurlDotNet.DotNetCurl.ToHttpClient(string).md 'CurlDotNet\.DotNetCurl\.ToHttpClient\(string\)') | Convert curl command to equivalent HttpClient C\# code\. |
| [ToPython\(string\)](CurlDotNet.DotNetCurl.ToPython(string).md 'CurlDotNet\.DotNetCurl\.ToPython\(string\)') | Convert curl command to Python requests code\. |
| [Validate\(string\)](CurlDotNet.DotNetCurl.Validate(string).md 'CurlDotNet\.DotNetCurl\.Validate\(string\)') | Validate a curl command without executing\. |
