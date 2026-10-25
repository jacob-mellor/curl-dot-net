#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithHeaders\(Dictionary\<string,string\>\) Method

Adds several request headers at once \(one `-H` per entry\); existing headers with the same names
are replaced\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithHeaders(System.Collections.Generic.Dictionary<string,string> headers);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHeaders(System.Collections.Generic.Dictionary_string,string_).headers'></a>

`headers` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

Header names mapped to values; must not be null \(an empty dictionary adds nothing\)\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[headers](CurlDotNet.Core.CurlRequestBuilder.WithHeaders(System.Collections.Generic.Dictionary_string,string_).md#CurlDotNet.Core.CurlRequestBuilder.WithHeaders(System.Collections.Generic.Dictionary_string,string_).headers 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeaders\(System\.Collections\.Generic\.Dictionary\<string,string\>\)\.headers') is null\. Pass an empty dictionary
            to add nothing; see https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-custom\-headers\-user\-agent\-csharp/README\.md

### Remarks

Each key is a header name (`"Accept"`) and each value the text after the colon. Handy for
            headers kept in configuration: `.WithHeaders(config.GetSection("ApiHeaders").Get<Dictionary<string, string>>())`.

Example: `.WithHeaders(new Dictionary<string, string> { ["Accept"] = "application/json", ["X-Tenant"] = "acme" })`
            is `-H 'Accept: application/json' -H 'X-Tenant: acme'`.

### See Also
- [WithHeader\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)')
- [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers')