#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException')

## CurlNotBuiltInException\.Feature Property

The feature or option that is not available in this build or platform, such as `--http3` on \.NET
Standard 2\.0\.

```csharp
public string Feature { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The feature name as passed to the constructor\.

### Remarks

Use it to decide whether to fall back to another option.

### See Also
- [Feature](CurlDotNet.Exceptions.CurlNotSupportedException.Feature.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException\.Feature')