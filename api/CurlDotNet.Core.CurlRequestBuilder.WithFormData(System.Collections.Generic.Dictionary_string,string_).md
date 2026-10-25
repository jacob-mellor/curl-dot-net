#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithFormData\(Dictionary\<string,string\>\) Method

Sends an HTML\-form body \(`application/x-www-form-urlencoded`\), URL\-encoding every name and value
for you; switches a GET to POST\. Equivalent to one `--data-urlencode name=value` per pair\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithFormData(System.Collections.Generic.Dictionary<string,string> formData);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).formData'></a>

`formData` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

Field names mapped to plain \(unencoded\) values; must not be null\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[formData](CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).md#CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).formData 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFormData\(System\.Collections\.Generic\.Dictionary\<string,string\>\)\.formData') is null\. Pass the fields to send; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/tutorials/11\-forms\-and\-data\.md

### Remarks

Each entry becomes `name=value` joined with `&`, both sides escaped with
            [System\.Uri\.EscapeDataString\(System\.String\)](https://learn.microsoft.com/en-us/dotnet/api/system.uri.escapedatastring#system-uri-escapedatastring(system-string) 'System\.Uri\.EscapeDataString\(System\.String\)') (space becomes `%20`, `&` becomes `%26`).
            The body replaces anything set earlier with [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)'), and the Content-Type header is set to
            `application/x-www-form-urlencoded`. For file fields use [WithMultipartForm\(Dictionary&lt;string,string&gt;, Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)') instead.

[ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') prints the encoded body as `-d 'user=ada&pass=p%40ss'` plus the
            `-H 'Content-Type: ...'` header.

Example: `.WithFormData(new Dictionary<string, string> { ["username"] = "ada", ["password"] = "p@ss word" })`
            sends `username=ada&password=p%40ss%20word`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/11-forms-and-data.md

### See Also
- [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)')
- [WithMultipartForm\(Dictionary&lt;string,string&gt;, Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)')
- [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data')