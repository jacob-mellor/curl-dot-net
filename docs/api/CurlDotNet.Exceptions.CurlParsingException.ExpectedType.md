#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')

## CurlParsingException\.ExpectedType Property

The \.NET type the body was supposed to become, such as `List<User>`; `object` for
`AsJsonDynamic()`\.

```csharp
public System.Type ExpectedType { get; }
```

#### Property Value
[System\.Type](https://learn.microsoft.com/en-us/dotnet/api/system.type 'System\.Type')  
The requested type, or null when not known\.

### Remarks

Compare its properties with the JSON in `result.Body` when the shapes do not match.

### See Also
- [ContentType](CurlDotNet.Exceptions.CurlParsingException.ContentType.md 'CurlDotNet\.Exceptions\.CurlParsingException\.ContentType')