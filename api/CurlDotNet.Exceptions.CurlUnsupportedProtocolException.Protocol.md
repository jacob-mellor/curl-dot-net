#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException')

## CurlUnsupportedProtocolException\.Protocol Property

The scheme or protocol version that is not supported, such as `gopher`, `htps` or `HTTP/1.2`\.

```csharp
public string Protocol { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The scheme without `://`, or the status\-line version for a server reply\.

### Remarks

Example: `if (ex.Protocol == "htps") Console.WriteLine("Typo: use https://");`

### See Also
- [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')