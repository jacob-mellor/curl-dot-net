#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

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