#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithMultipartForm\(Dictionary\<string,string\>, Dictionary\<string,string\>\) Method

Sends a `multipart/form-data` body with text fields and file uploads \(one curl `-F` per
entry\), switching a GET to POST; this is how browsers upload files\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithMultipartForm(System.Collections.Generic.Dictionary<string,string> fields, System.Collections.Generic.Dictionary<string,string> files=null);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).fields'></a>

`fields` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

Text fields \(name to value\); null means none\.

<a name='CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).files'></a>

`files` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

File fields \(name to local path, absolute or relative to the current directory\); null means none\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

[fields](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md#CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).fields 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)\.fields') maps form field names to text values (`-F 'name=value'`).
            [files](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md#CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).files 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)\.files') maps field names to local file paths (`-F 'name=@path'`); each file is read
            when the request runs and sent with its file name and a Content-Type guessed from the extension.
            This call replaces fields and files set earlier by this method, [WithFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)')
            or [WithUploadFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUploadFile\(string, string\)'); call it first and add more files afterwards if needed.

A missing or unreadable file is reported when the request runs, as a
            [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') with exit code 26 naming the path.

Example: `.WithMultipartForm(new Dictionary<string, string> { ["title"] = "Q3 report" }, new Dictionary<string, string> { ["document"] = @"C:\reports\q3.pdf" })`
            is `curl -F 'title=Q3 report' -F 'document=@C:\reports\q3.pdf' URL`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [WithFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)')
- [WithUploadFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUploadFile\(string, string\)')
- [WithFormData\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFormData\(System\.Collections\.Generic\.Dictionary\<string,string\>\)')
- [FormData](CurlDotNet.Core.CurlOptions.FormData.md 'CurlDotNet\.Core\.CurlOptions\.FormData')
- [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files')