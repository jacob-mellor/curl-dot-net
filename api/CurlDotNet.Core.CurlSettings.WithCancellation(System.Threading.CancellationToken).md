#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithCancellation\(CancellationToken\) Method

Sets [CancellationToken](CurlDotNet.Core.CurlSettings.CancellationToken.md 'CurlDotNet\.Core\.CurlSettings\.CancellationToken') so that cancelling [token](CurlDotNet.Core.CurlSettings.WithCancellation(System.Threading.CancellationToken).md#CurlDotNet.Core.CurlSettings.WithCancellation(System.Threading.CancellationToken).token 'CurlDotNet\.Core\.CurlSettings\.WithCancellation\(System\.Threading\.CancellationToken\)\.token') stops the transfer, and
returns these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithCancellation(System.Threading.CancellationToken token);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithCancellation(System.Threading.CancellationToken).token'></a>

`token` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Any token; [System\.Threading\.CancellationToken\.None](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken.none 'System\.Threading\.CancellationToken\.None') removes cancellation\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

A cancelled transfer throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException') - the standard .NET contract, so
            `catch (OperationCanceledException)` works - with a [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException') (exit code 42)
            as its InnerException.

Example: `new CurlSettings().WithCancellation(HttpContext.RequestAborted)` stops the outgoing call
            when the browser that asked for it disconnects.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md

### See Also
- [CancellationToken](CurlDotNet.Core.CurlSettings.CancellationToken.md 'CurlDotNet\.Core\.CurlSettings\.CancellationToken')
- [WithTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithTimeout\(int\)')