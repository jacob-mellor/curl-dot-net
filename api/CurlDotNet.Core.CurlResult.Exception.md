#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Exception Property

The failure behind a result that has no HTTP response, set only by the non\-throwing
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl') API; null for results from `Curl.ExecuteAsync`, which throws instead\.

```csharp
public System.Exception Exception { get; set; }
```

#### Property Value
[System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')  
A [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') subclass \(DNS, connect, timeout, TLS\.\.\.\) with [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') 0, or
            null\. HTTP errors such as 404 never set it \- check [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') for those\.

### Remarks

Example: `var r = await libCurl.GetAsync(url); if (r.Exception is CurlException ce) Console.WriteLine($"{ce.WhatHappened} - {ce.DocumentationUrl}");`

### See Also
- [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')
- [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess')