#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFtpException](CurlDotNet.Core.CurlFtpException.md 'CurlDotNet\.Core\.CurlFtpException')

## CurlFtpException\.FtpStatusCode Property

The FTP reply code the server sent, for example 530 \(not logged in\), 550 \(file unavailable\)
or 552 \(storage allocation exceeded\)\.

```csharp
public int FtpStatusCode { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
A three\-digit FTP reply code from RFC 959; 4xx codes are temporary, 5xx permanent\.

### Remarks

Use it to decide what to do: retry later on 4xx, fix credentials on 530, fix the path on 550.

Example: `catch (CurlFtpException ex) when (ex.FtpStatusCode / 100 == 4) { await Task.Delay(5000); /* retry */ }`

### See Also
- [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')