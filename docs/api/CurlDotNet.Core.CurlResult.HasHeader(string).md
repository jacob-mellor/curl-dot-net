#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.HasHeader\(string\) Method

True when the response contains the header [headerName](CurlDotNet.Core.CurlResult.HasHeader(string).md#CurlDotNet.Core.CurlResult.HasHeader(string).headerName 'CurlDotNet\.Core\.CurlResult\.HasHeader\(string\)\.headerName') \(ignoring case\), so you can test before
reading it\.

```csharp
public bool HasHeader(string headerName);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.HasHeader(string).headerName'></a>

`headerName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The header name without the colon, in any case, such as `Set-Cookie`\. Null or empty
            returns false\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true when the header is present \(even with an empty value\); otherwise false\.

### Remarks

Uses the same case-insensitive lookup as [GetHeader\(string\)](CurlDotNet.Core.CurlResult.GetHeader(string).md 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)'); `HasHeader(x)` is equivalent to
            `GetHeader(x) != null`.

Example: `if (result.HasHeader("WWW-Authenticate")) Console.WriteLine("Server wants credentials: " + result.GetHeader("WWW-Authenticate"));`

### See Also
- [GetHeader\(string\)](CurlDotNet.Core.CurlResult.GetHeader(string).md 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)')
- [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')