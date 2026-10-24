#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')

## CurlHttpException\.ResponseHeaders Property

The response headers \([Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')\) of the failed request, such as `Retry-After` or
`WWW-Authenticate`\.

```csharp
public System.Collections.Generic.Dictionary<string,string> ResponseHeaders { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
The result's case\-insensitive header dictionary, or null when the exception was created without it\.

### Remarks

Example: `if (ex.ResponseHeaders?.TryGetValue("Retry-After", out var wait) == true) await Task.Delay(TimeSpan.FromSeconds(int.Parse(wait)));`

### See Also
- [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')
- [ResponseBody](CurlDotNet.Core.CurlHttpException.ResponseBody.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseBody')