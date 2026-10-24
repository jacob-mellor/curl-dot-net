#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')

## CurlTimings\.PreTransfer Property

Milliseconds from the start until the request was about to be sent; curl's `-w '%{time_pretransfer}'`\.

```csharp
public double PreTransfer { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')  
Milliseconds since the start of the transfer, measured just before the request is handed
            to the connection\.

### Remarks

On CurlDotNet the connection is opened while the request is being sent, so this value can be
            smaller than [Connect](CurlDotNet.Core.CurlTimings.Connect.md 'CurlDotNet\.Core\.CurlTimings\.Connect'); use [StartTransfer](CurlDotNet.Core.CurlTimings.StartTransfer.md 'CurlDotNet\.Core\.CurlTimings\.StartTransfer') for "time to first byte".

### See Also
- [StartTransfer](CurlDotNet.Core.CurlTimings.StartTransfer.md 'CurlDotNet\.Core\.CurlTimings\.StartTransfer')