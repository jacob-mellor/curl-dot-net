#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Print\(\) Method

Writes `Status: {StatusCode}` and then [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') to the console, and returns this result\.

```csharp
public CurlDotNet.Core.CurlResult Print();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result, for chaining\.

### Remarks

Example: `result.Print();` prints `Status: 200` followed by the body text.

### See Also
- [PrintBody\(\)](CurlDotNet.Core.CurlResult.PrintBody().md 'CurlDotNet\.Core\.CurlResult\.PrintBody\(\)')
- [PrintVerbose\(\)](CurlDotNet.Core.CurlResult.PrintVerbose().md 'CurlDotNet\.Core\.CurlResult\.PrintVerbose\(\)')