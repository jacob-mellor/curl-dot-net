#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFtpException](CurlDotNet.Exceptions.CurlFtpException.md 'CurlDotNet\.Exceptions\.CurlFtpException')

## CurlFtpException\.FtpCode Property

The three\-digit FTP reply code the server answered with, for example 530 \(not logged in\), 550 \(file not
found or no permission\) or 421 \(service not available\)\.

```csharp
public int FtpCode { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
An FTP reply code from 100 to 599\.

### Remarks

4xx codes are temporary (retry), 5xx codes are permanent (change the request).

### See Also
- [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')