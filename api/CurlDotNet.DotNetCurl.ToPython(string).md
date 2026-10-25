#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.ToPython\(string\) Method

Translates a curl command into Python `requests` code without sending it; same as
[ToPythonRequests\(string\)](CurlDotNet.Curl.ToPythonRequests(string).md 'CurlDotNet\.Curl\.ToPythonRequests\(string\)')\.

```csharp
public static string ToPython(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.ToPython(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line to translate\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Python source text using `requests`\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[command](CurlDotNet.DotNetCurl.ToPython(string).md#CurlDotNet.DotNetCurl.ToPython(string).command 'CurlDotNet\.DotNetCurl\.ToPython\(string\)\.command') is null, empty or whitespace\.

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
The command text cannot be read\.

### Remarks

Covers method, URL, headers, body and `-u` credentials. Credentials appear in the output.

Example: `string py = DotNetCurl.ToPython("curl -u user:pass https://api.example.com/data");`

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md

### See Also
- [ToPythonRequests\(string\)](CurlDotNet.Curl.ToPythonRequests(string).md 'CurlDotNet\.Curl\.ToPythonRequests\(string\)')
- [ToHttpClient\(string\)](CurlDotNet.DotNetCurl.ToHttpClient(string).md 'CurlDotNet\.DotNetCurl\.ToHttpClient\(string\)')