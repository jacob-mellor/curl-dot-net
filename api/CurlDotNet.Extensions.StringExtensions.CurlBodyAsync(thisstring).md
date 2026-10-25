#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

## StringExtensions\.CurlBodyAsync\(this string\) Method

Fetches this URL with GET and returns only the response text \- the shortest way to read a page or a JSON
document into a string\.

```csharp
public static System.Threading.Tasks.Task<string> CurlBodyAsync(this string url);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlBodyAsync(thisstring).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL such as `https://example.com/data.json`, or a full `curl ...` command\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Extensions.StringExtensions.CurlBodyAsync(thisstring).md#CurlDotNet.Extensions.StringExtensions.CurlBodyAsync(thisstring).url 'CurlDotNet\.Extensions\.StringExtensions\.CurlBodyAsync\(this string\)\.url') is null\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The URL is malformed or the transfer failed; see the message for the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Equivalent to `(await url.CurlGetAsync()).Body`, with the same quoting rules as
            [CurlGetAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlGetAsync\(this string\)'). The status is not checked: a 404 returns the error page text. Binary responses
            return an empty string (use [CurlGetAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlGetAsync\(this string\)') and [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')).

Example: `string json = await "https://api.github.com/users/octocat".CurlBodyAsync();`

### See Also
- [CurlGetAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlGetAsync(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.CurlGetAsync\(this string\)')
- [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')