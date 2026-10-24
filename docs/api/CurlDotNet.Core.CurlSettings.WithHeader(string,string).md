#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithHeader\(string, string\) Method

Adds \(or replaces\) one request header, like an extra `-H "key: value"`, and returns these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithHeader(string key, string value);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithHeader(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The header name without the colon, such as `Accept` or `X-Api-Key`\.

<a name='CurlDotNet.Core.CurlSettings.WithHeader(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The header value, such as `application/json`\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[key](CurlDotNet.Core.CurlSettings.WithHeader(string,string).md#CurlDotNet.Core.CurlSettings.WithHeader(string,string).key 'CurlDotNet\.Core\.CurlSettings\.WithHeader\(string, string\)\.key') is null; pass a header name\.

### Remarks

Stored in [Headers](CurlDotNet.Core.CurlSettings.Headers.md 'CurlDotNet\.Core\.CurlSettings\.Headers') and applied over the command's headers. Calling it again with the same
            [key](CurlDotNet.Core.CurlSettings.WithHeader(string,string).md#CurlDotNet.Core.CurlSettings.WithHeader(string,string).key 'CurlDotNet\.Core\.CurlSettings\.WithHeader\(string, string\)\.key') replaces the value.

Example: `new CurlSettings().WithHeader("Authorization", "Bearer " + token).WithHeader("Accept", "application/json")`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md

### See Also
- [Headers](CurlDotNet.Core.CurlSettings.Headers.md 'CurlDotNet\.Core\.CurlSettings\.Headers')
- [WithHeaders\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlSettings.WithHeaders(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlSettings\.WithHeaders\(System\.Collections\.Generic\.Dictionary\<string,string\>\)')
- [WithHeader\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)')