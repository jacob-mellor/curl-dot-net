#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.IsRetryable\(\) Method

Tells whether running the same command again might succeed: true for transient network failures \(DNS, refused
or reset connections, timeouts, empty replies\), false for problems that need a change first \(bad URL, TLS, auth\)\.

```csharp
public virtual bool IsRetryable();
```

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when a retry may help; false when the command or the environment must change first\.

### Remarks

Retryable exit codes: 6, 7, 28, 52, 55, 56. [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') also treats 429 and 5xx as
            retryable. Use it to decide on a retry loop with backoff, or use curl's own `--retry 3` in the command.

Example: `catch (CurlException ex) when (ex.IsRetryable() && attempt < 3) { await Task.Delay(1000 * attempt); }`

### See Also
- [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')