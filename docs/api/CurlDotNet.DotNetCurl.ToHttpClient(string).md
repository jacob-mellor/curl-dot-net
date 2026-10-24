#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.ToHttpClient\(string\) Method

Translates a curl command into equivalent C\# `HttpClient` code without sending it; same as
[ToHttpClient\(string\)](CurlDotNet.Curl.ToHttpClient(string).md 'CurlDotNet\.Curl\.ToHttpClient\(string\)')\.

```csharp
public static string ToHttpClient(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.ToHttpClient(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line to translate\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
C\# source text using `HttpClient`\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[command](CurlDotNet.DotNetCurl.ToHttpClient(string).md#CurlDotNet.DotNetCurl.ToHttpClient(string).command 'CurlDotNet\.DotNetCurl\.ToHttpClient\(string\)\.command') is null, empty or whitespace\.

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
The command text cannot be read; the message names the problem\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Remarks

Covers method, URL, headers, body, `-u`, `-x`, `-k` and `-L`. Credentials from the
            command appear in the output in clear text.

Example: `Console.WriteLine(DotNetCurl.ToHttpClient("curl -H 'Accept: application/json' https://api.example.com"));`

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md

### See Also
- [ToHttpClient\(string\)](CurlDotNet.Curl.ToHttpClient(string).md 'CurlDotNet\.Curl\.ToHttpClient\(string\)')
- [ToFetch\(string\)](CurlDotNet.DotNetCurl.ToFetch(string).md 'CurlDotNet\.DotNetCurl\.ToFetch\(string\)')
- [ToPython\(string\)](CurlDotNet.DotNetCurl.ToPython(string).md 'CurlDotNet\.DotNetCurl\.ToPython\(string\)')