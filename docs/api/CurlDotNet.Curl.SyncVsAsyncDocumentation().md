#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.SyncVsAsyncDocumentation\(\) Method


<b>📚 UNDERSTANDING SYNC vs ASYNC in CurlDotNet</b>

<b>ASYNC Methods (end with "Async"):</b>
- ✅ Return Task<CurlResult>
- ✅ Don't block your thread
- ✅ UI stays responsive
- ✅ Server can handle more requests
- ✅ Can run multiple in parallel
- ✅ THIS IS WHAT YOU SHOULD USE

<b>SYNC Methods (no "Async" suffix):</b>
- ⚠️ Return CurlResult directly
- ⚠️ BLOCK your thread completely
- ⚠️ UI will FREEZE
- ⚠️ Server throughput reduced
- ⚠️ Run one at a time only
- ⚠️ Only for legacy/simple scenarios

<b>Examples showing the difference:</b>

```csharp
// ASYNC - Doesn't block ✅
public async Task MyMethodAsync()
{
    // Thread is FREE to do other work while waiting
    var result = await Curl.ExecuteAsync("curl https://api.example.com");

    // Can run multiple in parallel (very fast!)
    var task1 = Curl.ExecuteAsync("curl https://api1.example.com");
    var task2 = Curl.ExecuteAsync("curl https://api2.example.com");
    await Task.WhenAll(task1, task2); // Both run at same time
}

// SYNC - Blocks thread ⚠️
public void MyMethod()
{
    // Thread is BLOCKED here, can't do anything else
    var result = Curl.Execute("curl https://api.example.com");

    // These run one after another (slow!)
    var result1 = Curl.Execute("curl https://api1.example.com");
    var result2 = Curl.Execute("curl https://api2.example.com");
}
```

<b>CancellationToken doesn't determine sync/async!</b>

```csharp
// ASYNC with cancellation - Still async! ✅
await Curl.ExecuteAsync(command, cancellationToken);

// SYNC with cancellation - Still blocks! ⚠️
Curl.Execute(command, cancellationToken);
```

```csharp
private static void SyncVsAsyncDocumentation();
```