#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions')

## StringExtensions Class

Run curl straight from a string: `await "curl https://api.github.com".CurlAsync()`,
`await "https://example.com".CurlBodyAsync()`, `"curl ...".Curl()` for synchronous code\.

```csharp
public static class StringExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; StringExtensions

### Remarks

Add `using CurlDotNet.Extensions;` to see these on every [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String'). They are thin wrappers over
            [Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl'): same parsing, same results, same exceptions. The command methods ([CurlAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string\)'),
            [Curl\(this string\)](CurlDotNet.Extensions.StringExtensions.Curl(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.Curl\(this string\)')) take a full command line (the leading `curl` is optional); the URL methods
            ([CurlGetAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlGetAsync\(this string\)'), [CurlPostJsonAsync\(this string, string\)](CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlPostJsonAsync\(this string, string\)'), [CurlDownloadAsync\(this string, string\)](CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlDownloadAsync\(this string, string\)'),
            [CurlBodyAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlBodyAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlBodyAsync\(this string\)')) take a URL.

Example: `string zen = await "https://api.github.com/zen".CurlBodyAsync();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md

### See Also
- [Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')
- [FluentExtensions](CurlDotNet.Extensions.FluentExtensions.md 'CurlDotNet\.Extensions\.FluentExtensions')

| Methods | |
| :--- | :--- |
| [Curl\(this string\)](CurlDotNet.Extensions.StringExtensions.Curl(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.Curl\(this string\)') | Runs this string as a curl command line and blocks until it finishes \- the synchronous form of [CurlAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string\)'), for console tools and scripts\. |
| [CurlAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string\)') | Runs this string as a curl command line \(with or without the leading `curl`\) and returns the result \- the same as [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. |
| [CurlAsync\(this string, CancellationToken\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string, System\.Threading\.CancellationToken\)') | Runs this string as a curl command line and stops the transfer when [cancellationToken](CurlDotNet.Extensions.StringExtensions.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled \- the same as [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')\. |
| [CurlBodyAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlBodyAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlBodyAsync\(this string\)') | Fetches this URL with GET and returns only the response text \- the shortest way to read a page or a JSON document into a string\. |
| [CurlDownloadAsync\(this string, string\)](CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlDownloadAsync\(this string, string\)') | Downloads this URL to [outputFile](CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).md#CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).outputFile 'CurlDotNet\.Extensions\.StringExtensions\.CurlDownloadAsync\(this string, string\)\.outputFile') \(like `curl -o FILE URL`\) and returns the result \- the same as [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)')\. |
| [CurlGetAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlGetAsync\(this string\)') | Sends a GET request to this URL \(`curl URL`\) and returns the result; a string that already starts with `"curl "` is run as a command\. |
| [CurlPostJsonAsync\(this string, string\)](CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlPostJsonAsync\(this string, string\)') | POSTs [json](CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).md#CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).json 'CurlDotNet\.Extensions\.StringExtensions\.CurlPostJsonAsync\(this string, string\)\.json') to this URL with `Content-Type: application/json` and returns the result \- like `curl -X POST -H 'Content-Type: application/json' -d JSON URL`, without any shell quoting\. |
