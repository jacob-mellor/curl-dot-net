#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')

## CurlParsingException\.ContentType Property

The Content\-Type of the response that could not be parsed, such as `text/html; charset=utf-8`, which
usually explains the failure \(an HTML error page instead of JSON\)\.

```csharp
public string ContentType { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The header value, or null when the response had none\.

### Remarks

Example: `if (ex.ContentType?.StartsWith("text/html") == true) Console.WriteLine("The server sent an error page");`

### See Also
- [ExpectedType](CurlDotNet.Exceptions.CurlParsingException.ExpectedType.md 'CurlDotNet\.Exceptions\.CurlParsingException\.ExpectedType')