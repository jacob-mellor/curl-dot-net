#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.Command Property

Gets the curl command that was being executed when the exception occurred\.

```csharp
public string Command { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The full curl command string, or null if not applicable\.

### Remarks

This property contains the exact command that was passed to the Execute method.

AI-Usage: Use this for logging and debugging to understand what command failed.