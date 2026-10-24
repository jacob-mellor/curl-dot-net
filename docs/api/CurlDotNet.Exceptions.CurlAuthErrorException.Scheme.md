#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlAuthErrorException](CurlDotNet.Exceptions.CurlAuthErrorException.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException')

## CurlAuthErrorException\.Scheme Property

The scheme whose negotiation failed: `digest`, `ntlm`, `negotiate` or `anyauth`\.

```csharp
public string Scheme { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The scheme name in lower case\.

### Remarks

Switching to another scheme the server offers (see its WWW-Authenticate header) often works around a buggy challenge.

### See Also
- [Proxy](CurlDotNet.Exceptions.CurlAuthErrorException.Proxy.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException\.Proxy')