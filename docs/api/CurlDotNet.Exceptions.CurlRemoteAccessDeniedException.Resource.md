#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRemoteAccessDeniedException](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException')

## CurlRemoteAccessDeniedException\.Resource Property

The remote resource \(path, directory or URL\) the server refused access to, as requested\.

```csharp
public string Resource { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A remote path such as `/pub/private/report.csv`\.

### Remarks

Compare it with what the account may see, for example by listing the parent directory.

### See Also
- [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command')