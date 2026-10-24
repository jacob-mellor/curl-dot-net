#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')

## CurlFormPart\.MixedFiles Property

The files of `-F "name=@a.txt,b.txt"`: when more than one file is named, each one is an entry here and they
are sent together as a nested `multipart/mixed` part, like curl\.

```csharp
public System.Collections.Generic.List<CurlDotNet.Core.CurlFormPart> MixedFiles { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
File parts; empty for single\-file and text parts\.

### Remarks

Example: `curl -F "docs=@a.txt,b.txt" URL` produces one part named `docs` with two entries here.

### See Also
- [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath')