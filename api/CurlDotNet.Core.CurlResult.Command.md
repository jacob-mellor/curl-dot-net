#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Command Property


<b>The original curl command that was executed.</b>

Useful for debugging or retrying:

```csharp
Console.WriteLine($"Executed: {result.Command}");

// Retry the same command
var retry = await Curl.Execute(result.Command);
```

```csharp
public string Command { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')