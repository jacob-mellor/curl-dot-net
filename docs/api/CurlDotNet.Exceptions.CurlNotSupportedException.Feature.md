#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlNotSupportedException](CurlDotNet.Exceptions.CurlNotSupportedException.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException')

## CurlNotSupportedException\.Feature Property

The curl feature or option that is not available, such as `--libcurl` or a specific protocol, so you can
remove it from the command\.

```csharp
public string Feature { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The feature name as given to the constructor\.

### Remarks

Check the compatibility list linked from the type before relying on an option.

### See Also
- [Feature](CurlDotNet.Exceptions.CurlNotBuiltInException.Feature.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException\.Feature')