#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException')

## CurlMalformedUrlException\.MalformedUrl Property

The URL that could not be parsed, exactly as it appeared in the command or in the redirect's `Location`
header\.

```csharp
public string MalformedUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The raw URL text; it may contain credentials, so mask it before logging\.

### Remarks

Example: `Console.WriteLine(Uri.EscapeUriString(ex.MalformedUrl));` shows what an encoded version would look like.

### See Also
- [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command')