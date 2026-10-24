#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.Headers Property

Extra request headers added to \(or replacing headers of the same name in\) the command, like extra
`-H "Name: value"` flags\.

```csharp
public System.Collections.Generic.Dictionary<string,string> Headers { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
Header name to value; empty by default\. A name already set by the command is overwritten
            \(names compare case\-insensitively when applied\)\.

### Remarks

Copied into [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers'). Do not include the colon in the name.

Example: `settings.Headers["Authorization"] = "Bearer " + token;` keeps the token out of the
            command string (and out of [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command') logs).

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md

### See Also
- [WithHeader\(string, string\)](CurlDotNet.Core.CurlSettings.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlSettings\.WithHeader\(string, string\)')
- [WithHeaders\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlSettings.WithHeaders(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlSettings\.WithHeaders\(System\.Collections\.Generic\.Dictionary\<string,string\>\)')
- [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers')