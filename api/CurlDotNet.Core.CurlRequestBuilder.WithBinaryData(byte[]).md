#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithBinaryData\(byte\[\]\) Method

Sends raw bytes as the request body exactly as given \(curl's `--data-binary`\), switching a GET
to POST; use it for images, protobuf, gzip payloads or a file's contents\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithBinaryData(byte[] data);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBinaryData(byte[]).data'></a>

`data` [System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

The exact body bytes; null sends no body\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

The bytes are sent without a `Content-Type` header. A multipart form ([WithMultipartForm\(Dictionary&lt;string,string&gt;, Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)')) takes precedence over these
            bytes, and these bytes take precedence over [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)') text. PUT and PATCH keep their method,
            so `Put(url).WithBinaryData(File.ReadAllBytes("disk.img"))` is the builder form of `curl -T disk.img URL`.

[ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') can only show bytes that are plain UTF-8 text: they are printed as
            `--data-binary 'TEXT'`. Real binary data cannot be written on a command line and is left out of
            the printed command (save it to a file and use `--data-binary @file` if you need to reproduce it in a shell).

Example: `await CurlRequestBuilder.Put("https://storage.example.com/bucket/cat.png").WithBinaryData(File.ReadAllBytes("cat.png")).ExecuteAsync();`
            uploads the image bytes unchanged.

### See Also
- [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)')
- [BinaryData](CurlDotNet.Core.CurlOptions.BinaryData.md 'CurlDotNet\.Core\.CurlOptions\.BinaryData')