#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')

## CurlFormPart\.Headers Property

Extra headers written inside this part, from curl's `;headers=Header: value` modifier \(for example
`X-Checksum: abc`\), one `"Name: value"` string per entry\.

```csharp
public System.Collections.Generic.List<string> Headers { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
Header lines\. Default: empty\.

### Remarks

Example: `-F "file=@a.bin;headers=X-Checksum: abc"` adds `"X-Checksum: abc"` here.

### See Also
- [ContentType](CurlDotNet.Core.CurlFormPart.ContentType.md 'CurlDotNet\.Core\.CurlFormPart\.ContentType')