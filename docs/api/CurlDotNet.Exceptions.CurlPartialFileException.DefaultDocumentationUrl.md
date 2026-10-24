#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlPartialFileException](CurlDotNet.Exceptions.CurlPartialFileException.md 'CurlDotNet\.Exceptions\.CurlPartialFileException')

## CurlPartialFileException\.DefaultDocumentationUrl Property

The documentation page for this exception type, used when the message does not end with its own
`See https://...` link\.

```csharp
protected override string? DefaultDocumentationUrl { protected get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Null in [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') itself \(the page for [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') is used
            instead, when there is a code\)\. Derived types return the page that explains their error, for example
            the cookie page for [CurlCookieException](CurlDotNet.Exceptions.CurlCookieException.md 'CurlDotNet\.Exceptions\.CurlCookieException')\.

### Remarks

Override this in your own exception types that derive from [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') so that
            [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message') and [DocumentationUrl](CurlDotNet.Exceptions.CurlException.DocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlException\.DocumentationUrl') always point somewhere useful. It is read
            lazily, the first time [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message') is used, so overrides may use the derived type's
            properties.

Example: `protected override string? DefaultDocumentationUrl => "https://wiki.example.com/payments-api-errors";`

### See Also
- [DefaultSuggestion](CurlDotNet.Exceptions.CurlException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlException\.DefaultSuggestion')
- [DocumentationUrl](CurlDotNet.Exceptions.CurlException.DocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlException\.DocumentationUrl')