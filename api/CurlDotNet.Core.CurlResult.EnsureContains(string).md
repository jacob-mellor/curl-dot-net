#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.EnsureContains\(string\) Method

Throws [System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException') unless [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') contains [expectedText](CurlDotNet.Core.CurlResult.EnsureContains(string).md#CurlDotNet.Core.CurlResult.EnsureContains(string).expectedText 'CurlDotNet\.Core\.CurlResult\.EnsureContains\(string\)\.expectedText')
\(exact, case\-sensitive\), otherwise returns this result \- a quick content check for smoke tests and health checks\.

```csharp
public CurlDotNet.Core.CurlResult EnsureContains(string expectedText);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.EnsureContains(string).expectedText'></a>

`expectedText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The text that must appear somewhere in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'), such as `"success"`\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result when the text is found, for chaining\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[expectedText](CurlDotNet.Core.CurlResult.EnsureContains(string).md#CurlDotNet.Core.CurlResult.EnsureContains(string).expectedText 'CurlDotNet\.Core\.CurlResult\.EnsureContains\(string\)\.expectedText') is null; pass the text to look for\.

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
The body does not contain the text\. The message names the URL, the
            status, the body length and its first characters\. Fix: print [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'), check case and spelling\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

The comparison is ordinal: `"OK"` does not match `"ok"`. Binary responses have an empty
            [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') and never match.

Example: `(await Curl.ExecuteAsync("curl https://status.example.com/health")).EnsureSuccess().EnsureContains("\"status\":\"up\"");`

### See Also
- [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)')
- [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')