#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.CancellationToken Property

The token that cancels the transfer; when it fires, the call throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')
\(its InnerException is a [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException'), curl exit code 42\)\.

```csharp
public System.Threading.CancellationToken CancellationToken { get; set; }
```

#### Property Value
[System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')  
Defaults to [System\.Threading\.CancellationToken\.None](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken.none 'System\.Threading\.CancellationToken\.None') \(never cancelled\)\.

### Remarks

Used when the method you call does not take its own token. [ExecuteAsync\(CurlSettings, CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(CurlDotNet\.Core\.CurlSettings, System\.Threading\.CancellationToken\)')
            prefers the token passed directly when it can be cancelled.

Example: `using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); settings.CancellationToken = cts.Token;`
            For a time limit that behaves like curl (exit code 28), prefer [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds').

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md

### See Also
- [WithCancellation\(CancellationToken\)](CurlDotNet.Core.CurlSettings.WithCancellation(System.Threading.CancellationToken).md 'CurlDotNet\.Core\.CurlSettings\.WithCancellation\(System\.Threading\.CancellationToken\)')
- [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds')