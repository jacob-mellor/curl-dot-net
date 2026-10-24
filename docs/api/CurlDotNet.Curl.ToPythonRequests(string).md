#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ToPythonRequests\(string\) Method

Translates a curl command into Python code using the `requests` library, for sharing a request with
Python scripts and notebooks\.

```csharp
public static string ToPythonRequests(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToPythonRequests(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line to translate, read with [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Python source text using `requests`\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[command](CurlDotNet.Curl.ToPythonRequests(string).md#CurlDotNet.Curl.ToPythonRequests(string).command 'CurlDotNet\.Curl\.ToPythonRequests\(string\)\.command') is null, empty or whitespace\.

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
The command text cannot be read; the message names the problem\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Remarks

The generated code covers the method, URL, headers, body and `-u` credentials (as
            `auth=('user', 'pass')`). Nothing is sent. Credentials appear in the output.

Example: `string py = Curl.ToPythonRequests("curl -u user:pass https://api.example.com/data");`
            returns code starting with `import requests` that calls `requests.get(...)`.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md

### See Also
- [ToHttpClient\(string\)](CurlDotNet.Curl.ToHttpClient(string).md 'CurlDotNet\.Curl\.ToHttpClient\(string\)')
- [ToFetch\(string\)](CurlDotNet.Curl.ToFetch(string).md 'CurlDotNet\.Curl\.ToFetch\(string\)')
- [ToPython\(string\)](CurlDotNet.DotNetCurl.ToPython(string).md 'CurlDotNet\.DotNetCurl\.ToPython\(string\)')