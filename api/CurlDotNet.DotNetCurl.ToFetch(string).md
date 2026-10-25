#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.ToFetch\(string\) Method

Translates a curl command into JavaScript `fetch()` code without sending it; same as
[ToFetch\(string\)](CurlDotNet.Curl.ToFetch(string).md 'CurlDotNet\.Curl\.ToFetch\(string\)')\.

```csharp
public static string ToFetch(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.ToFetch(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line to translate\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
JavaScript source text using `fetch()`\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[command](CurlDotNet.DotNetCurl.ToFetch(string).md#CurlDotNet.DotNetCurl.ToFetch(string).command 'CurlDotNet\.DotNetCurl\.ToFetch\(string\)\.command') is null, empty or whitespace\.

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
The command text cannot be read\.

### Remarks

Covers URL, method, headers and body. Credentials from the command appear in the output.

Example: `string js = DotNetCurl.ToFetch("curl -X POST -d 'a=1' https://api.example.com");`

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md

### See Also
- [ToFetch\(string\)](CurlDotNet.Curl.ToFetch(string).md 'CurlDotNet\.Curl\.ToFetch\(string\)')
- [ToHttpClient\(string\)](CurlDotNet.DotNetCurl.ToHttpClient(string).md 'CurlDotNet\.DotNetCurl\.ToHttpClient\(string\)')