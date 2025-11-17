#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.GetAsync\(string\) Method


<b>Quick GET request - simpler syntax for basic GET operations.</b>

When you just need to GET a URL without any options, use this shortcut method.

<b>Example:</b>

```csharp
// Instead of:
await Curl.Execute("curl https://api.github.com/users/octocat");

// You can use:
var response = await Curl.Get("https://api.github.com/users/octocat");

// Work with response
var user = response.ParseJson<GitHubUser>();
Console.WriteLine($"Followers: {user.Followers}");
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> GetAsync(string url);
```
#### Parameters

<a name='CurlDotNet.Curl.GetAsync(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to GET\. Can be HTTP or HTTPS\. Query parameters can be included\.

```csharp
await Curl.Get("https://api.example.com/users?page=1&limit=10");
```

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the response\.

### Remarks

This is equivalent to: `Curl.Execute($"curl {url}")`

For GET requests with headers or auth, use the full [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') method.