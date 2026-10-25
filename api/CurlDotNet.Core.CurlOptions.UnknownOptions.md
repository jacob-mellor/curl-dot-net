#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.UnknownOptions Property

Options in a parsed command that are not curl options at all \(typos such as `--hedaer`\); they are
ignored when the request runs, so check this list to catch mistakes before sending\.

```csharp
public System.Collections.Generic.List<string> UnknownOptions { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
The options exactly as typed, in command\-line order; empty for hand\-built options\.

### Remarks

curl itself would stop with exit code 2 on these. CurlDotNet still sends the request, and
            [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)') reports them so that a pasted command with a typo is caught early.
            Real curl options that CurlDotNet merely does not implement go to [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions') instead.

Example: `var v = Curl.Validate("curl --hedaer 'X: 1' https://example.com");` gives
            `v.IsValid == false` and `v.ParsedOptions.UnknownOptions` contains `--hedaer`.

### See Also
- [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions')
- [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)')