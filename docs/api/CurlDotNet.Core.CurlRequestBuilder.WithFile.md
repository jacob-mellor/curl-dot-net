#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithFile Method

| Overloads | |
| :--- | :--- |
| [WithFile\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string\)') | Uploads one file in a `multipart/form-data` body under the field name `file` \(curl's `-F 'file=@path'`\); switches a GET to POST\. |
| [WithFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)') | Adds a file upload field to a `multipart/form-data` body \(curl's `-F 'field=@path'`\) after checking the file exists; switches a GET to POST\. |

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string)'></a>

## CurlRequestBuilder\.WithFile\(string\) Method

Uploads one file in a `multipart/form-data` body under the field name `file` \(curl's
`-F 'file=@path'`\); switches a GET to POST\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path of the file to upload, absolute or relative to the current directory\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Shorthand for `WithUploadFile("file", filePath)`; use [WithFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)') when the
            server expects another field name. The file is read when the request runs; a missing file fails with exit code 26.

Example: `await CurlRequestBuilder.Post("https://httpbin.org/post").WithFile("report.pdf").ExecuteAsync();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [WithUploadFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUploadFile\(string, string\)')
- [WithFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string)'></a>

## CurlRequestBuilder\.WithFile\(string, string\) Method

Adds a file upload field to a `multipart/form-data` body \(curl's `-F 'field=@path'`\) after
checking the file exists; switches a GET to POST\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithFile(string fieldName, string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string).fieldName'></a>

`fieldName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The form field name the server expects, such as `"file"` or `"avatar"`\.

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path of the file to upload, absolute or relative to the current directory\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

#### Exceptions

[CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')  
The file does not exist \(exit code 26, curl's
            "Failed to open/read local data"\)\. The message names the path, the field and the current directory;
            fix the path or use an absolute one\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-file\-upload\-csharp/README\.md

### Remarks

The file is read when the request runs and sent with its file name and a Content-Type guessed
            from the extension. Call it several times to upload several files; a later call with the same
            [fieldName](CurlDotNet.Core.CurlRequestBuilder.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string).fieldName 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)\.fieldName') replaces the earlier path.

Unlike [WithUploadFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUploadFile\(string, string\)'), this method checks immediately that the file exists, so a
            typo fails at the line that caused it.

Example: `await CurlRequestBuilder.Post("https://api.example.com/upload").WithFile("avatar", "me.jpg").ExecuteAsync();`
            is `curl -F 'avatar=@me.jpg' https://api.example.com/upload`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [WithUploadFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUploadFile\(string, string\)')
- [WithMultipartForm\(Dictionary&lt;string,string&gt;, Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)')
- [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files')