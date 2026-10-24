#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslPinnedPublicKeyMismatchException](CurlDotNet.Exceptions.CurlSslPinnedPublicKeyMismatchException.md 'CurlDotNet\.Exceptions\.CurlSslPinnedPublicKeyMismatchException')

## CurlSslPinnedPublicKeyMismatchException\(string, string\) Constructor

Creates the pinned\-key error \(exit code 90\)\.

```csharp
public CurlSslPinnedPublicKeyMismatchException(string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslPinnedPublicKeyMismatchException.CurlSslPinnedPublicKeyMismatchException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What did not match \(or could not be read\) and how to fix it, newline\-separated\.

<a name='CurlDotNet.Exceptions.CurlSslPinnedPublicKeyMismatchException.CurlSslPinnedPublicKeyMismatchException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

Example: `throw new CurlSslPinnedPublicKeyMismatchException("the key of api.example.com matches no --pinnedpubkey pin", command);`

### See Also
- [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')