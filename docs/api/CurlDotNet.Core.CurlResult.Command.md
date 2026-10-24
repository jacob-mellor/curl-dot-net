#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Command Property

The curl command that produced this result, as you passed it \(or as [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)')
rendered it\); used by [Retry\(\)](CurlDotNet.Core.CurlResult.Retry().md 'CurlDotNet\.Core\.CurlResult\.Retry\(\)') and in error messages\.

```csharp
public string Command { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The command text, or null for a result you created yourself\. It can contain secrets you put in the
            command \(`-u user:password`, `-H "Authorization: Bearer ..."`\), so redact it before logging; passing
            secrets through [Headers](CurlDotNet.Core.CurlSettings.Headers.md 'CurlDotNet\.Core\.CurlSettings\.Headers') keeps them out of this string\.

### Remarks

Example: `logger.LogInformation("{Status} for {Command}", result.StatusCode, result.Command);`

### See Also
- [Retry\(\)](CurlDotNet.Core.CurlResult.Retry().md 'CurlDotNet\.Core\.CurlResult\.Retry\(\)')
- [RetryWith\(Action&lt;CurlSettings&gt;\)](CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).md 'CurlDotNet\.Core\.CurlResult\.RetryWith\(System\.Action\<CurlDotNet\.Core\.CurlSettings\>\)')