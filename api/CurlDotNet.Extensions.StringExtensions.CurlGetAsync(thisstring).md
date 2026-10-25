#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

## StringExtensions\.CurlGetAsync\(this string\) Method

Sends a GET request to this URL \(`curl URL`\) and returns the result; a string that already starts with
`"curl "` is run as a command\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlGetAsync(this string url);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL such as `https://api.example.com/items`, or a full `curl ...` command\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with the [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).md#CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).url 'CurlDotNet\.Extensions\.StringExtensions\.CurlGetAsync\(this string\)\.url') is null\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The URL is malformed or the transfer failed; see the message for the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

The URL is placed after `curl ` and read as a command line, so quote a URL that contains `&`,
            `;`, `|`, spaces or quotes (`"'https://x.example/?a=1&b=2'".CurlGetAsync()`), or use
            [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)'), which takes the URL literally. Redirects are not followed unless
            you write `curl -L URL`.

Example: `var result = await "https://api.github.com/users/octocat".CurlGetAsync(); var user = result.ParseJson<User>();`

### See Also
- [CurlBodyAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlBodyAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlBodyAsync\(this string\)')
- [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)')