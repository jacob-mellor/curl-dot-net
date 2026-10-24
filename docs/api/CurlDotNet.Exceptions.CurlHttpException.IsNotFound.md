#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.IsNotFound Property

True for 404 Not Found: the URL path \(or the resource id in it\) does not exist on this server\.

```csharp
public bool IsNotFound { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode') is 404\.

### Remarks

Check the path, the API version prefix and the id; many APIs also answer 404 for resources you may not see.

### See Also
- [IsStatus\(int\)](CurlDotNet.Exceptions.CurlHttpException.IsStatus(int).md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsStatus\(int\)')