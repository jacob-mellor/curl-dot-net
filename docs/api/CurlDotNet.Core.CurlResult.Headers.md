#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Headers Property

The headers of the final response, by name, case\-insensitive \(`Headers["content-type"]` works\); repeated
headers such as `Set-Cookie` are joined with `", "`\.

```csharp
public System.Collections.Generic.Dictionary<string,string> Headers { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
A dictionary with [System\.StringComparer\.OrdinalIgnoreCase](https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparer.ordinalignorecase 'System\.StringComparer\.OrdinalIgnoreCase'); values are exactly as the server sent them
            \(after the colon, trimmed\)\. Empty when there was no HTTP response\. After `-L` only the last hop's headers are
            here; use `-i` and [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') to see every hop\.

### Remarks

Indexing a missing name throws [System\.Collections\.Generic\.KeyNotFoundException](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keynotfoundexception 'System\.Collections\.Generic\.KeyNotFoundException'); [GetHeader\(string\)](CurlDotNet.Core.CurlResult.GetHeader(string).md 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)') returns
            null instead and also works on a dictionary you assigned yourself with a case-sensitive comparer.

Example: `if (result.Headers.TryGetValue("X-RateLimit-Remaining", out var left) && int.Parse(left) < 10) await Task.Delay(60_000);`

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/08-headers-explained.md

### See Also
- [GetHeader\(string\)](CurlDotNet.Core.CurlResult.GetHeader(string).md 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)')
- [HasHeader\(string\)](CurlDotNet.Core.CurlResult.HasHeader(string).md 'CurlDotNet\.Core\.CurlResult\.HasHeader\(string\)')