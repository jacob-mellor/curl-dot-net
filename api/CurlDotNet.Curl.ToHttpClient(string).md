#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ToHttpClient\(string\) Method


<b>Convert curl command to C# HttpClient code - great for learning!</b>

Shows you exactly how to write the same request using HttpClient.
             Perfect for understanding what curl is doing or migrating to pure HttpClient.

<b>Example:</b>

```csharp
var curlCommand = @"
    curl -X POST https://api.example.com/users \
      -H 'Content-Type: application/json' \
      -H 'Authorization: Bearer token123' \
      -d '{""name"":""John"",""age"":30}'
";

string code = Curl.ToHttpClient(curlCommand);
Console.WriteLine(code);

// Output:
// using var client = new HttpClient();
// var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com/users");
// request.Headers.Add("Authorization", "Bearer token123");
// request.Content = new StringContent("{\"name\":\"John\",\"age\":30}",
//     Encoding.UTF8, "application/json");
// var response = await client.SendAsync(request);
```

```csharp
public static string ToHttpClient(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToHttpClient(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to convert\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
C\# code using HttpClient that does the same thing\.

### Remarks

Great for:
- Learning how HttpClient works
- Migrating from CurlDotNet to pure HttpClient
- Understanding what curl commands actually do
- Code generation for your projects