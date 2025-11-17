#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.HasHeader\(string\) Method


<b>Check if a header exists.</b>

Test for header presence before accessing. This is case-insensitive, matching curl's behavior.

```csharp
public bool HasHeader(string headerName);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.HasHeader(string).headerName'></a>

`headerName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Name of the header to check \(case\-insensitive\)

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true if the header exists, false otherwise

### Example

```csharp
// Check for cookies
if (result.HasHeader("Set-Cookie"))
{
    var cookie = result.GetHeader("Set-Cookie");
    Console.WriteLine($"Cookie received: {cookie}");
}

// Check for authentication requirements
if (result.HasHeader("WWW-Authenticate"))
{
    Console.WriteLine("Authentication required");
}

// Check for custom headers
if (result.HasHeader("X-Custom-Header"))
{
    var value = result.GetHeader("X-Custom-Header");
    ProcessCustomValue(value);
}

// Conditional logic based on headers
if (result.HasHeader("Content-Encoding") && 
    result.GetHeader("Content-Encoding").Contains("gzip"))
{
    Console.WriteLine("Response is gzip compressed");
}
```

### See Also
- [Get header value](CurlDotNet.Core.CurlResult.GetHeader(string).md 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)')
- [Access all headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')