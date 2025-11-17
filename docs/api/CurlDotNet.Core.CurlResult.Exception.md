#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Exception Property


<b>Any exception if the request failed completely.</b>

Only set for network failures, not HTTP errors:

```csharp
if (result.Exception != null)
{
    // Network/DNS/Timeout failure
    Console.WriteLine($"Failed: {result.Exception.Message}");
}
else if (!result.IsSuccess)
{
    // HTTP error (404, 500, etc)
    Console.WriteLine($"HTTP {result.StatusCode}");
}
```

```csharp
public System.Exception Exception { get; set; }
```

#### Property Value
[System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')