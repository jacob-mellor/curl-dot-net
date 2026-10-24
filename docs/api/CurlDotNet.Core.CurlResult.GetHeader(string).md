#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.GetHeader\(string\) Method

Returns the value of one response header by name, ignoring case, or null when the response did not have it \-
safer than indexing [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers'), which throws for a missing name\.

```csharp
public string GetHeader(string headerName);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.GetHeader(string).headerName'></a>

`headerName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The header name without the colon, in any case, such as `Content-Type`,
            `location` or `ETAG`\. Null or empty returns null\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The header value as sent by the server \(text after the colon, trimmed\), or null when absent\.

### Remarks

Repeated headers come back joined with `", "` (for example two `Set-Cookie` lines). The lookup is
            case-insensitive even when you replaced [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers') with a case-sensitive dictionary.

Example: `var left = result.GetHeader("X-RateLimit-Remaining"); if (left != null && int.Parse(left) < 10) await Task.Delay(60_000);`
            or `if (result.GetHeader("Content-Type")?.Contains("json") == true) data = result.ParseJson<Data>();`

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/08-headers-explained.md

### See Also
- [HasHeader\(string\)](CurlDotNet.Core.CurlResult.HasHeader(string).md 'CurlDotNet\.Core\.CurlResult\.HasHeader\(string\)')
- [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')