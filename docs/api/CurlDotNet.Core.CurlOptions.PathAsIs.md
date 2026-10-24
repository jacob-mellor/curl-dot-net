#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.PathAsIs Property

Sends the URL path exactly as written, without removing `/./` and `/../` segments, like curl's
`--path-as-is`; used to test path\-traversal handling on servers\.

```csharp
public bool PathAsIs { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to keep dot segments\. Default: false \(dot segments are resolved, as browsers do\)\.

### Remarks

Honoured on .NET 6 and later, where the request URI is created without path canonicalisation; .NET Standard
            2.0 and .NET Framework always normalise the path.

Example: `curl --path-as-is http://localhost:8080/../../etc/passwd` is `new CurlOptions { Url = url, PathAsIs = true }`.

### See Also
- [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url')