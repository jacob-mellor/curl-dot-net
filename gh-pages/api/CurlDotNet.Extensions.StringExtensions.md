#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions')

## StringExtensions Class

Extension methods for string to provide syntactic sugar for curl operations\.

```csharp
public static class StringExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; StringExtensions

| Methods | |
| :--- | :--- |
| [Curl\(this string\)](CurlDotNet.Extensions.StringExtensions.Curl(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.Curl\(this string\)') | Executes curl synchronously \(blocking\)\. |
| [CurlAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string\)') | Executes a curl command directly from a string\. |
| [CurlAsync\(this string, CancellationToken\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string, System\.Threading\.CancellationToken\)') | Executes a curl command with cancellation support\. |
| [CurlBodyAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlBodyAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlBodyAsync\(this string\)') | Quick one\-liner to get response body as string\. |
| [CurlDownloadAsync\(this string, string\)](CurlDotNet.Extensions.StringExtensions.CurlDownloadAsync(thisstring,string).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlDownloadAsync\(this string, string\)') | Downloads a file from the URL\. |
| [CurlGetAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlGetAsync\(this string\)') | Performs a GET request on the URL\. |
| [CurlPostJsonAsync\(this string, string\)](CurlDotNet.Extensions.StringExtensions.CurlPostJsonAsync(thisstring,string).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlPostJsonAsync\(this string, string\)') | Performs a POST request with JSON data\. |
