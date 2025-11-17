#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ToPythonRequests\(string\) Method


<b>Convert curl command to Python requests code.</b>

Generates Python code using the popular 'requests' library.
             Great for Python developers or data scientists.

<b>Example:</b>

```python
var curlCommand = "curl -u user:pass https://api.example.com/data";

string pythonCode = Curl.ToPythonRequests(curlCommand);
Console.WriteLine(pythonCode);

// Output:
// import requests
//
// response = requests.get(
//     'https://api.example.com/data',
//     auth=('user', 'pass')
// )
// print(response.json())
```

```csharp
public static string ToPythonRequests(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToPythonRequests(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to convert\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Python code using requests library\.