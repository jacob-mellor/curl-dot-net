#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ToFetch\(string\) Method


<b>Convert curl command to JavaScript fetch() code.</b>

Generates JavaScript code that does the same thing as your curl command.
             Useful for web developers who need the same request in JavaScript.

<b>Example:</b>

```javascript
var curlCommand = "curl -X GET https://api.example.com/data -H 'Authorization: Bearer token'";

string jsCode = Curl.ToFetch(curlCommand);
Console.WriteLine(jsCode);

// Output:
// fetch('https://api.example.com/data', {
//     method: 'GET',
//     headers: {
//         'Authorization': 'Bearer token'
//     }
// })
// .then(response => response.json())
// .then(data => console.log(data));
```

```csharp
public static string ToFetch(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToFetch(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to convert\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
JavaScript fetch\(\) code that does the same thing\.