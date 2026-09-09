#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

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