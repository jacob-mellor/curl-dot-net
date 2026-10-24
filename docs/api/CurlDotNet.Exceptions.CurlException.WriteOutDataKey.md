#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.WriteOutDataKey Field

Key in [System\.Exception\.Data](https://learn.microsoft.com/en-us/dotnet/api/system.exception.data 'System\.Exception\.Data') holding the standard\-output part of `-w`/`--write-out`
for a failed transfer\.

```csharp
public const string WriteOutDataKey = "CurlDotNet.WriteOut";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

### Remarks

curl expands `-w` even when a transfer fails, so `%{exitcode}`, `%{errormsg}`
            and `%{onerror}` are useful. The expanded text (without `%{stderr}` parts) is stored
            under this key as a [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String'); it is also included in [StandardOutput](CurlDotNet.Exceptions.CurlException.StandardOutput.md 'CurlDotNet\.Exceptions\.CurlException\.StandardOutput').

Usage: `curl -w '%{exitcode} %{errormsg}' URL`, then read
            `ex.Data[CurlException.WriteOutDataKey]` in the `catch` block.

### See Also
- [StandardOutputDataKey](CurlDotNet.Exceptions.CurlException.StandardOutputDataKey.md 'CurlDotNet\.Exceptions\.CurlException\.StandardOutputDataKey')