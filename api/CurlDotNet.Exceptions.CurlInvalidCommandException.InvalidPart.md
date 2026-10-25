#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')

## CurlInvalidCommandException\.InvalidPart Property

The piece of the command that could not be read: an option name such as `--frobnicate`, an option
whose value is missing, or the unclosed quoted text\.

```csharp
public string InvalidPart { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The offending token exactly as it appeared in the command, or null when the problem is the
            command as a whole \(for example "no URL specified"\)\.

### Remarks

Use it to highlight the error in an editor or form: `commandBox.Select(cmd.IndexOf(ex.InvalidPart), ex.InvalidPart.Length);`

### See Also
- [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command')