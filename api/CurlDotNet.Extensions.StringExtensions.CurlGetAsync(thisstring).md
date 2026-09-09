#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

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