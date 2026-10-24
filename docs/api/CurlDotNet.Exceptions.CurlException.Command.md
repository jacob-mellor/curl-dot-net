#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.Command Property

The curl command \(or a description of the API call\) that was running when the error happened, exactly
as it was passed in, so you can log it or re\-run it in a terminal\.

```csharp
public string Command { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The full command string such as `curl -sS -H 'Accept: application/json' https://api.example.com`;
            for [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') calls, the equivalent produced by
            `ToCurlCommand()`; null when the error did not come from a command\.

### Remarks

The command may contain secrets (`-u user:password`, `-H 'Authorization: Bearer ...'`). Mask it
            before writing it to shared logs. [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message') itself never includes the whole command.

Example: `catch (CurlException ex) { logger.LogWarning("curl failed: {Cmd}", Mask(ex.Command)); }`

### See Also
- [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')
- [StandardOutput](CurlDotNet.Exceptions.CurlException.StandardOutput.md 'CurlDotNet\.Exceptions\.CurlException\.StandardOutput')