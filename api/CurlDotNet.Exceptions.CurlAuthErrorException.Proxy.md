#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlAuthErrorException](CurlDotNet.Exceptions.CurlAuthErrorException.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException')

## CurlAuthErrorException\.Proxy Property

True when the failing negotiation was with the proxy \(`--proxy-digest`, `--proxy-ntlm`\.\.\.\) rather than
with the server\.

```csharp
public bool Proxy { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for proxy authentication, false for server authentication\.

### Remarks

Use it to decide whether the proxy settings or the server credentials need attention.

### See Also
- [Scheme](CurlDotNet.Exceptions.CurlAuthErrorException.Scheme.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException\.Scheme')