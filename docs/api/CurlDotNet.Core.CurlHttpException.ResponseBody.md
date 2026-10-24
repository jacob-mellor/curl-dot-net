#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')

## CurlHttpException\.ResponseBody Property

The response text \([Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\) of the failed request \- APIs usually explain the error here\.

```csharp
public string ResponseBody { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The body text, empty for binary or empty responses, or null when the exception was created without it\.

### Remarks

Example: `catch (CurlHttpException ex) { logger.LogError("{Status}: {Body}", ex.StatusCode, ex.ResponseBody); }`

### See Also
- [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')
- [ResponseHeaders](CurlDotNet.Core.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseHeaders')