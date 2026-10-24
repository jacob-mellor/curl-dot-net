#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException')

## CurlAuthenticationException\.AuthMethod Property

The authentication scheme that was rejected, such as `Basic`, `Digest`, `NTLM` or an FTP
`USER/PASS` login, when known\.

```csharp
public string AuthMethod { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The scheme name as reported by the code that threw, or null when it is not known\.

### Remarks

Example: `if (ex.AuthMethod == "Basic") Console.WriteLine("Try --digest; the server may not accept Basic.");`

### See Also
- [Scheme](CurlDotNet.Exceptions.CurlAuthErrorException.Scheme.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException\.Scheme')