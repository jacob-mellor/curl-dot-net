#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ToFetch\(string\) Method

Translates a curl command into JavaScript `fetch()` code that sends the same request, for web front ends
and Node\.js\.

```csharp
public static string ToFetch(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToFetch(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line to translate, read with [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
JavaScript source text using `fetch()`\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[command](CurlDotNet.Curl.ToFetch(string).md#CurlDotNet.Curl.ToFetch(string).command 'CurlDotNet\.Curl\.ToFetch\(string\)\.command') is null, empty or whitespace\.

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
The command text cannot be read; the message names the problem\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Example

```csharp
string jsCode = Curl.ToFetch("curl -X GET https://api.example.com/data -H 'Authorization: Bearer token'");
Console.WriteLine(jsCode);
```

### Remarks

The generated code covers the URL, method, headers and body. Nothing is sent. Credentials from the
            command appear in the output.

Example: `string js = Curl.ToFetch("curl -H 'Authorization: Bearer token' https://api.example.com/data");`
            returns `const response = await fetch('https://api.example.com/data', { ... headers: { 'Authorization': 'Bearer token' } ... });`.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md

### See Also
- [ToHttpClient\(string\)](CurlDotNet.Curl.ToHttpClient(string).md 'CurlDotNet\.Curl\.ToHttpClient\(string\)')
- [ToPythonRequests\(string\)](CurlDotNet.Curl.ToPythonRequests(string).md 'CurlDotNet\.Curl\.ToPythonRequests\(string\)')
- [ToFetch\(string\)](CurlDotNet.DotNetCurl.ToFetch(string).md 'CurlDotNet\.DotNetCurl\.ToFetch\(string\)')